using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class LiteDbGameService
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Stages.ContainsKey(stageDataId))
            result.error = GameServiceErrorCode.INVALID_STAGE_DATA;
        else if (!IsStageAvailable(gameDb.Stages[stageDataId]))
            result.error = GameServiceErrorCode.INVALID_STAGE_NOT_AVAILABLE;
        else
        {
            colPlayerBattle.Delete(a => a.PlayerId == playerId && a.BattleResult == (byte)EBattleResult.None && a.BattleType == (byte)EBattleType.Stage);
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
                var playerBattle = new DbPlayerBattle();
                playerBattle.Id = System.Guid.NewGuid().ToString();
                playerBattle.PlayerId = playerId;
                playerBattle.DataId = stageDataId;
                playerBattle.Session = System.Guid.NewGuid().ToString();
                playerBattle.BattleResult = (byte)EBattleResult.None;
                playerBattle.BattleType = (byte)EBattleType.Stage;
                colPlayerBattle.Insert(playerBattle);

                var stamina = GetStamina(player.Id, stageStaminaTable.id);
                result.stamina = PlayerStamina.CloneTo(stamina, new PlayerStamina());
                result.session = playerBattle.Session;

                // Update achievement
                if (!string.IsNullOrEmpty(helperPlayerId))
                {
                    List<PlayerAchievement> createAchievements;
                    List<PlayerAchievement> updateAchievements;
                    OfflineAchievementHelpers.UpdateCountUseHelper(playerId, DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == playerId)), out createAchievements, out updateAchievements);
                    foreach (var createEntry in createAchievements)
                    {
                        createEntry.Id = System.Guid.NewGuid().ToString();
                        colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
                    }
                    foreach (var updateEntry in updateAchievements)
                    {
                        colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var battle = colPlayerBattle.FindOne(a => a.PlayerId == playerId && a.Session == session);
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
            colPlayerBattle.Update(battle);
            if (battleResult == EBattleResult.Win)
            {
                var stage = gameDb.Stages[battle.DataId];
                // Player exp
                result.rewardPlayerExp = stage.rewardPlayerExp;
                player.Exp += stage.rewardPlayerExp;
                colPlayer.Update(player);
                result.player = Player.CloneTo(player, new Player());
                // Character exp
                var formations = new List<DbPlayerFormation>(colPlayerFormation.Find(a => a.PlayerId == playerId && a.DataId == player.SelectedFormation));
                var countFormation = 0;
                foreach (var formation in formations)
                {
                    if (!string.IsNullOrEmpty(formation.ItemId))
                        ++countFormation;
                }
                if (countFormation > 0)
                {
                    var devivedExp = stage.rewardCharacterExp / countFormation;
                    result.rewardCharacterExp = devivedExp;
                    foreach (var formation in formations)
                    {
                        var character = colPlayerItem.FindById(formation.ItemId);
                        if (character != null)
                        {
                            character.Exp += devivedExp;
                            colPlayerItem.Update(character);
                            result.updateItems.Add(PlayerItem.CloneTo(character, new PlayerItem()));
                        }
                    }
                }
                // Soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                var rewardSoftCurrency = Random.Range(stage.randomSoftCurrencyMinAmount, stage.randomSoftCurrencyMaxAmount);
                result.rewardSoftCurrency = rewardSoftCurrency;
                softCurrency.Amount += rewardSoftCurrency;
                colPlayerCurrency.Update(softCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Items
                for (var i = 0; i < stage.rewardItems.Length; ++i)
                {
                    var rewardItem = stage.rewardItems[i];
                    if (rewardItem == null || rewardItem.item == null || Random.value > rewardItem.randomRate)
                        continue;
                    var createItems = new List<DbPlayerItem>();
                    var updateItems = new List<DbPlayerItem>();
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
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerItem.Insert(createEntry);
                            var resultItem = PlayerItem.CloneTo(createEntry, new PlayerItem());
                            result.createItems.Add(resultItem);
                            HelperUnlockItem(player.Id, rewardItem.Id);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            colPlayerItem.Update(updateEntry);
                            var resultItem = PlayerItem.CloneTo(updateEntry, new PlayerItem());
                            result.updateItems.Add(resultItem);
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                colPlayerCurrency.Update(hardCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                // Update achievement
                List<PlayerAchievement> createAchievements;
                List<PlayerAchievement> updateAchievements;
                OfflineAchievementHelpers.UpdateCountRevive(playerId, DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == playerId)), out createAchievements, out updateAchievements);
                foreach (var createEntry in createAchievements)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
                }
                foreach (var updateEntry in updateAchievements)
                {
                    colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
                }
            }
        }
        onFinish(result);
    }

    protected override void DoSelectFormation(string playerId, string loginToken, string formationName, EFormationType formationType, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                
            colPlayer.Update(player);
            result.player = Player.CloneTo(player, new Player());
        }
        onFinish(result);
    }

    protected override void DoSetFormation(string playerId, string loginToken, string characterId, string formationName, int position, UnityAction<FormationListResult> onFinish)
    {
        var result = new FormationListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        PlayerItem character = null;
        if (!string.IsNullOrEmpty(characterId))
        {
            character = PlayerItem.CloneTo(colPlayerItem.FindOne(a => a.Id == characterId && a.PlayerId == playerId), new PlayerItem());
        }

        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (character != null && character.CharacterData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else
        {
            HelperSetFormation(playerId, characterId, formationName, position);
            result.list.AddRange(DbPlayerFormation.CloneList(colPlayerFormation.Find(a => a.PlayerId == playerId)));
        }
        onFinish(result);
    }
}
