using System.Collections.Generic;
using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoGetArenaOpponentList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        var gameDb = GameInstance.GameDatabase;
        foreach (var fakePlayer in gameDb.fakePlayers)
        {
            if (fakePlayer.level <= 0 || fakePlayer.mainCharacter == null || fakePlayer.mainCharacterLevel <= 0)
                continue;
            result.list.Add(fakePlayer.MakePlayer());
        }
        onFinish(result);
    }

    protected override void DoStartDuel(string playerId, string loginToken, string targetPlayerId, UnityAction<StartDuelResult> onFinish)
    {
        var result = new StartDuelResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.FakePlayers.ContainsKey(targetPlayerId))
            result.error = GameServiceErrorCode.INVALID_PLAYER_DATA;
        else
        {
            var fakePlayer = gameDb.FakePlayers[targetPlayerId];
            colPlayerBattle.Delete(a => a.PlayerId == playerId && a.BattleResult == (byte)EBattleResult.None && a.BattleType == (byte)EBattleType.Arena);
            var stage = gameDb.FakePlayers[targetPlayerId];
            var arenaStaminaTable = gameDb.arenaStamina;
            if (!DecreasePlayerStamina(player, arenaStaminaTable, 1))
                result.error = GameServiceErrorCode.NOT_ENOUGH_ARENA_STAMINA;
            else
            {
                var playerBattle = new DbPlayerBattle();
                playerBattle.Id = System.Guid.NewGuid().ToString();
                playerBattle.PlayerId = playerId;
                playerBattle.DataId = targetPlayerId;
                playerBattle.Session = System.Guid.NewGuid().ToString();
                playerBattle.BattleResult = (byte)EBattleResult.None;
                playerBattle.BattleType = (byte)EBattleType.Arena;
                colPlayerBattle.Insert(playerBattle);

                var stamina = GetStamina(player.Id, arenaStaminaTable.id);
                result.stamina = PlayerStamina.CloneTo(stamina, new PlayerStamina());
                result.session = playerBattle.Session;
                
                // Opponent characters
                foreach (var arenaCharacter in fakePlayer.arenaCharacters)
                {
                    result.opponentCharacters.Add(arenaCharacter.MakeAsItem());
                }
            }
        }
        onFinish(result);
    }

    protected override void DoFinishDuel(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishDuelResult> onFinish)
    {
        var result = new FinishDuelResult();
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
                // Increase arena score
                var resultPlayer = new Player();
                Player.CloneTo(player, resultPlayer);
                var oldArenaScore = resultPlayer.ArenaScore;
                var oldArenaLevel = resultPlayer.ArenaLevel;
                var arenaRank = resultPlayer.ArenaRank;
                result.updateScore = gameDb.arenaWinScoreIncrease;
                player.ArenaScore += gameDb.arenaWinScoreIncrease;
                colPlayer.Update(player);
                result.player = Player.CloneTo(player, resultPlayer);

                // Arena rank up, rewarding items
                if (arenaRank != null && resultPlayer.ArenaLevel > oldArenaLevel && player.HighestArenaRankCurrentSeason < resultPlayer.ArenaLevel)
                {
                    // Update highest rank
                    player.HighestArenaRankCurrentSeason = resultPlayer.ArenaLevel;
                    if (player.HighestArenaRank < resultPlayer.ArenaLevel)
                        player.HighestArenaRank = resultPlayer.ArenaLevel;
                    colPlayer.Update(player);

                    // Soft currency
                    var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                    var rewardSoftCurrency = arenaRank.rewardSoftCurrency;
                    result.rewardSoftCurrency = rewardSoftCurrency;
                    softCurrency.Amount += rewardSoftCurrency;
                    colPlayerCurrency.Update(softCurrency);
                    result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                    // Hard currency
                    var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
                    var rewardHardCurrency = arenaRank.rewardHardCurrency;
                    result.rewardHardCurrency = rewardHardCurrency;
                    hardCurrency.Amount += rewardHardCurrency;
                    colPlayerCurrency.Update(hardCurrency);
                    result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                    // Items
                    for (var i = 0; i < arenaRank.rewardItems.Length; ++i)
                    {
                        var rewardItem = arenaRank.rewardItems[i];
                        if (rewardItem == null || rewardItem.item == null)
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
                }
                // Update achievement
                List<PlayerAchievement> createAchievements;
                List<PlayerAchievement> updateAchievements;
                OfflineAchievementHelpers.UpdateCountWinDuel(playerId, DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == playerId)), out createAchievements, out updateAchievements);
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
            else
            {
                result.updateScore = -gameDb.arenaLoseScoreDecrease;
                player.ArenaScore -= gameDb.arenaLoseScoreDecrease;
                colPlayer.Update(player);
                result.player = Player.CloneTo(player, new Player());
            }
        }
        onFinish(result);
    }
}
