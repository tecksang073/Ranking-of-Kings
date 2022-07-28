using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OfflineAchievementHelpers
{
    public static Dictionary<string, Achievement> FilterAchievements(AchievementType type)
    {
        Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
        foreach (var achievement in GameInstance.GameDatabase.Achievements.Values)
        {
            if (achievement != null && achievement.type == type)
                achievements.Add(achievement.Id, achievement);
        }
        return achievements;
    }

    public static Dictionary<string, PlayerAchievement> FilterPlayerAchievements(Dictionary<string, Achievement> achievements, List<PlayerAchievement> playerAchievements)
    {
        Dictionary<string, PlayerAchievement> result = new Dictionary<string, PlayerAchievement>();
        foreach (var playerAchievement in playerAchievements)
        {
            if (achievements.ContainsKey(playerAchievement.DataId))
                result.Add(playerAchievement.DataId, playerAchievement);
        }
        return result;
    }

    public static void UpdateTotalClearStage(string playerId, List<PlayerAchievement> playerAchievements, List<PlayerClearStage> playerClearStages, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        createPlayerAchievements = new List<PlayerAchievement>();
        updatePlayerAchievements = new List<PlayerAchievement>();
        var achievements = FilterAchievements(AchievementType.TotalClearStage);
        var playerAchievementDict = FilterPlayerAchievements(achievements, playerAchievements);
        foreach (var achievement in achievements.Values)
        {
            if (!playerAchievementDict.ContainsKey(achievement.Id))
            {
                var newPlayerAchievement = new PlayerAchievement();
                newPlayerAchievement.PlayerId = playerId;
                newPlayerAchievement.DataId = achievement.Id;
                newPlayerAchievement.Progress = playerClearStages.Count;
                createPlayerAchievements.Add(newPlayerAchievement);
            }
            else
            {
                var oldPlayerAchievement = playerAchievementDict[achievement.Id];
                oldPlayerAchievement.Progress = playerClearStages.Count;
                updatePlayerAchievements.Add(oldPlayerAchievement);
            }
        }
    }

    public static void UpdateTotalClearStageRating(string playerId, List<PlayerAchievement> playerAchievements, List<PlayerClearStage> playerClearStages, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        createPlayerAchievements = new List<PlayerAchievement>();
        updatePlayerAchievements = new List<PlayerAchievement>();
        var achievements = FilterAchievements(AchievementType.TotalClearStageRating);
        var playerAchievementDict = FilterPlayerAchievements(achievements, playerAchievements);
        var countRating = 0;
        foreach (var playerClearStage in playerClearStages)
        {
            countRating += playerClearStage.BestRating;
        }
        foreach (var achievement in achievements.Values)
        {
            if (!playerAchievementDict.ContainsKey(achievement.Id))
            {
                var newPlayerAchievement = new PlayerAchievement();
                newPlayerAchievement.PlayerId = playerId;
                newPlayerAchievement.DataId = achievement.Id;
                newPlayerAchievement.Progress = countRating;
                createPlayerAchievements.Add(newPlayerAchievement);
            }
            else
            {
                var oldPlayerAchievement = playerAchievementDict[achievement.Id];
                oldPlayerAchievement.Progress = countRating;
                updatePlayerAchievements.Add(oldPlayerAchievement);
            }
        }
    }

    public static void UpdateCountLevelUpCharacter(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountLevelUpCharacter, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountLevelUpEquipment(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountLevelUpEquipment, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountEvolveCharacter(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountEvolveCharacter, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountEvolveEquipment(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountEvolveEquipment, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountRevive(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountRevive, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountUseHelper(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountUseHelper, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountWinStage(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountWinStage, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountWinDuel(string playerId, List<PlayerAchievement> playerAchievements, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        UpdateCountingProgress(playerId, playerAchievements, AchievementType.CountWinDuel, out createPlayerAchievements, out updatePlayerAchievements);
    }

    public static void UpdateCountingProgress(string playerId, List<PlayerAchievement> playerAchievements, AchievementType type, out List<PlayerAchievement> createPlayerAchievements, out List<PlayerAchievement> updatePlayerAchievements)
    {
        createPlayerAchievements = new List<PlayerAchievement>();
        updatePlayerAchievements = new List<PlayerAchievement>();
        var achievements = FilterAchievements(type);
        var playerAchievementDict = FilterPlayerAchievements(achievements, playerAchievements);
        foreach (var achievement in achievements.Values)
        {
            if (!playerAchievementDict.ContainsKey(achievement.Id))
            {
                var newPlayerAchievement = new PlayerAchievement();
                newPlayerAchievement.PlayerId = playerId;
                newPlayerAchievement.DataId = achievement.Id;
                newPlayerAchievement.Progress = 1;
                createPlayerAchievements.Add(newPlayerAchievement);
            }
            else
            {
                var oldPlayerAchievement = playerAchievementDict[achievement.Id];
                ++oldPlayerAchievement.Progress;
                updatePlayerAchievements.Add(oldPlayerAchievement);
            }
        }
    }
}
