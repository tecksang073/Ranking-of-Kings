using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mono.Data.Sqlite;

public partial class SQLiteGameService
{
    protected override void DoGetAvailableStageList(UnityAction<AvailableStageListResult> onFinish)
    {
        var result = new AvailableStageListResult();
        foreach (var key in GameInstance.GameDatabase.Stages.Keys)
        {
            if (IsStageAvailable(GameInstance.GameDatabase.Stages[key]))
                result.list.Add(key);
        }
        onFinish(result);
    }

    protected override void DoStartStage(string playerId, string loginToken, string stageDataId, string helperPlayerId, UnityAction<StartStageResult> onFinish)
    {
        var result = new StartStageResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Stages.ContainsKey(stageDataId))
            result.error = GameServiceErrorCode.INVALID_STAGE_DATA;
        else if (!IsStageAvailable(gameDb.Stages[stageDataId]))
            result.error = GameServiceErrorCode.INVALID_STAGE_NOT_AVAILABLE;
        else
        {
            ExecuteNonQuery(@"DELETE FROM playerBattle WHERE playerId=@playerId AND battleResult=@battleResult AND battleType=@battleType",
                new SqliteParameter("@playerId", playerId),
                new SqliteParameter("@battleResult", (byte)EBattleResult.None),
                new SqliteParameter("@battleType", (byte)EBattleType.Stage));
            var stage = gameDb.Stages[stageDataId];
            var stageStaminaTable = gameDb.stageStamina;
            if (string.IsNullOrEmpty(stage.requireCustomStamina) &&
                gameDb.Staminas.ContainsKey(stage.requireCustomStamina))
            {
                // Use custom stamina if `requireCustomStamina` not empty
                stageStaminaTable = gameDb.Staminas[stage.requireCustomStamina];
            }
            if (!DecreasePlayerStamina(player, stageStaminaTable, stage.requireStamina))
            {
                result.error = GameServiceErrorCode.NOT_ENOUGH_STAGE_STAMINA;
            }
            else
            {
                var playerBattle = new PlayerBattle();
                playerBattle.Id = System.Guid.NewGuid().ToString();
                playerBattle.PlayerId = playerId;
                playerBattle.DataId = stageDataId;
                playerBattle.Session = System.Guid.NewGuid().ToString();
                playerBattle.BattleResult = (byte)EBattleResult.None;
                playerBattle.BattleType = (byte)EBattleType.Stage;
                ExecuteNonQuery(@"INSERT INTO playerBattle (id, playerId, dataId, session, battleResult, rating, battleType) VALUES (@id, @playerId, @dataId, @session, @battleResult, @rating, @battleType)",
                    new SqliteParameter("@id", playerBattle.Id),
                    new SqliteParameter("@playerId", playerBattle.PlayerId),
                    new SqliteParameter("@dataId", playerBattle.DataId),
                    new SqliteParameter("@session", playerBattle.Session),
                    new SqliteParameter("@battleResult", playerBattle.BattleResult),
                    new SqliteParameter("@rating", playerBattle.Rating),
                    new SqliteParameter("@battleType", playerBattle.BattleType));

                var stamina = GetStamina(player.Id, stageStaminaTable.id);
                result.stamina = stamina;
                result.session = playerBattle.Session;

                // Update achievement
                if (!string.IsNullOrEmpty(helperPlayerId))
                {
                    List<PlayerAchievement> createAchievements;
                    List<PlayerAchievement> updateAchievements;
                    OfflineAchievementHelpers.UpdateCountUseHelper(playerId, GetPlayerAchievements(playerId), out createAchievements, out updateAchievements);
                    foreach (var createEntry in createAchievements)
                    {
                        QueryCreatePlayerAchievement(createEntry);
                    }
                    foreach (var updateEntry in updateAchievements)
                    {
                        QueryUpdatePlayerAchievement(updateEntry);
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoFinishStage(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishStageResult> onFinish)
    {
        var result = new FinishStageResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        var battle = GetPlayerBattleBySession(playerId, session);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (battle == null)
            result.error = GameServiceErrorCode.INVALID_BATTLE_SESSION;
        else
        {
            var rating = 0;
            battle.BattleResult = (byte)battleResult;
            if (battleResult == EBattleResult.Win)
            {
                rating = 3 - deadCharacters;
                if (rating <= 0)
                    rating = 1;
            }
            battle.Rating = rating;
            result.rating = rating;
            ExecuteNonQuery(@"UPDATE playerBattle SET battleResult=@battleResult, rating=@rating WHERE id=@id",
                new SqliteParameter("@battleResult", battle.BattleResult),
                new SqliteParameter("@rating", battle.Rating),
                new SqliteParameter("@id", battle.Id));
            if (battleResult == EBattleResult.Win)
            {
                var stage = gameDb.Stages[battle.DataId];
                // Player exp
                result.rewardPlayerExp = stage.rewardPlayerExp;
                player.Exp += stage.rewardPlayerExp;
                ExecuteNonQuery(@"UPDATE player SET exp=@exp WHERE id=@playerId",
                    new SqliteParameter("@exp", player.Exp),
                    new SqliteParameter("@playerId", playerId));
                result.player = player;
                // Character exp
                var countFormation = ExecuteScalar(@"SELECT COUNT(*) FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId",
                    new SqliteParameter("@playerId", playerId),
                    new SqliteParameter("@dataId", player.SelectedFormation));
                if (countFormation != null && (long)countFormation > 0)
                {
                    var devivedExp = (int)(stage.rewardCharacterExp / (long)countFormation);
                    result.rewardCharacterExp = devivedExp;

                    var formations = ExecuteReader(@"SELECT itemId FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId",
                        new SqliteParameter("@playerId", playerId),
                        new SqliteParameter("@dataId", player.SelectedFormation));
                    while (formations.Read())
                    {
                        var itemId = formations.GetString(0);
                        var character = GetPlayerItemById(itemId);
                        if (character != null)
                        {
                            character.Exp += devivedExp;
                            ExecuteNonQuery(@"UPDATE playerItem SET exp=@exp WHERE id=@id",
                                new SqliteParameter("@exp", character.Exp),
                                new SqliteParameter("@id", character.Id));
                            result.updateItems.Add(character);
                        }
                    }
                }
                // Soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                var rewardSoftCurrency = Random.Range(stage.randomSoftCurrencyMinAmount, stage.randomSoftCurrencyMaxAmount);
                result.rewardSoftCurrency = rewardSoftCurrency;
                softCurrency.Amount += rewardSoftCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                result.updateCurrencies.Add(softCurrency);
                // Items
                for (var i = 0; i < stage.rewardItems.Length; ++i)
                {
                    var rewardItem = stage.rewardItems[i];
                    if (rewardItem == null || rewardItem.item == null || Random.value > rewardItem.randomRate)
                        continue;
                    var createItems = new List<PlayerItem>();
                    var updateItems = new List<PlayerItem>();
                    if (AddItems(player.Id, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {
                        result.rewardItems.Add(new PlayerItem()
                        {
                            Id = i.ToString(),
                            PlayerId = player.Id,
                            DataId = rewardItem.Id,
                            Amount = rewardItem.amount
                        });
                        foreach (var createEntry in createItems)
                        {
                            QueryCreatePlayerItem(createEntry);
                            result.createItems.Add(createEntry);
                            HelperUnlockItem(player.Id, rewardItem.Id);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            QueryUpdatePlayerItem(updateEntry);
                            result.updateItems.Add(updateEntry);
                        }
                    }
                    // End add item condition
                }
                // End reward items loop
                result = HelperClearStage(result, player, stage, rating);
            }
        }
        onFinish(result);
    }

    protected override void DoReviveCharacters(string playerId, string loginToken, UnityAction<CurrencyResult> onFinish)
    {
        var result = new CurrencyResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var revivePrice = gameDb.revivePrice;
            if (revivePrice > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                hardCurrency.Amount -= revivePrice;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", hardCurrency.Amount),
                    new SqliteParameter("@id", hardCurrency.Id));
                result.updateCurrencies.Add(hardCurrency);
                // Update achievement
                List<PlayerAchievement> createAchievements;
                List<PlayerAchievement> updateAchievements;
                OfflineAchievementHelpers.UpdateCountRevive(playerId, GetPlayerAchievements(playerId), out createAchievements, out updateAchievements);
                foreach (var createEntry in createAchievements)
                {
                    QueryCreatePlayerAchievement(createEntry);
                }
                foreach (var updateEntry in updateAchievements)
                {
                    QueryUpdatePlayerAchievement(updateEntry);
                }
            }
        }
        onFinish(result);
    }

    protected override void DoSelectFormation(string playerId, string loginToken, string formationName, EFormationType formationType, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Formations.ContainsKey(formationName))
            result.error = GameServiceErrorCode.INVALID_FORMATION_DATA;
        else
        {
            if (formationType == EFormationType.Stage)
                player.SelectedFormation = formationName;
            else if (formationType == EFormationType.Arena)
                player.SelectedArenaFormation = formationName;
                
            ExecuteNonQuery(@"UPDATE player SET selectedFormation=@selectedFormation, selectedArenaFormation=@selectedArenaFormation WHERE id=@id",
                new SqliteParameter("@selectedFormation", player.SelectedFormation),
                new SqliteParameter("@selectedArenaFormation", player.SelectedArenaFormation),
                new SqliteParameter("@id", player.Id));
            result.player = player;
        }
        onFinish(result);
    }

    protected override void DoSetFormation(string playerId, string loginToken, string characterId, string formationName, int position, UnityAction<FormationListResult> onFinish)
    {
        var result = new FormationListResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        PlayerItem character = null;
        if (!string.IsNullOrEmpty(characterId))
            character = GetPlayerItemById(characterId);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (character != null && character.CharacterData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else
        {
            HelperSetFormation(playerId, characterId, formationName, position);
            var reader = ExecuteReader(@"SELECT * FROM playerFormation WHERE playerId=@playerId", new SqliteParameter("@playerId", playerId));
            var list = new List<PlayerFormation>();
            while (reader.Read())
            {
                var entry = new PlayerFormation();
                entry.Id = reader.GetString("id");
                entry.PlayerId = reader.GetString("playerId");
                entry.DataId = reader.GetString("dataId");
                entry.Position = reader.GetInt32("position");
                entry.ItemId = reader.GetString("itemId");
                list.Add(entry);
            }
            result.list = list;
        }
        onFinish(result);
    }
}
