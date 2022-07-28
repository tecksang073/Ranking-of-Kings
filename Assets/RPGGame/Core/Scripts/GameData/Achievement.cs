using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType : byte
{
    TotalClearStage,
    TotalClearStageRating,
    CountLevelUpCharacter,
    CountLevelUpEquipment,
    CountEvolveCharacter,
    CountEvolveEquipment,
    CountRevive,
    CountUseHelper,
    CountWinStage,
    CountWinDuel
}

public class Achievement : BaseGameData
{
    public AchievementType type;
    public int targetAmount;
    [Header("Rewards")]
    public int rewardSoftCurrency;
    public int rewardHardCurrency;
    public int rewardPlayerExp;
    public ItemAmount[] rewardItems;

    public virtual string ToJson()
    {
        // Reward Items
        var jsonRewardItems = "";
        foreach (var entry in rewardItems)
        {
            if (!string.IsNullOrEmpty(jsonRewardItems))
                jsonRewardItems += ",";
            jsonRewardItems += entry.ToJson();
        }
        jsonRewardItems = "[" + jsonRewardItems + "]";
        return "{\"type\":" + (byte)type + "," +
            "\"targetAmount\":" + targetAmount + "," +
            "\"rewardSoftCurrency\":" + rewardSoftCurrency + "," +
            "\"rewardHardCurrency\":" + rewardHardCurrency + "," +
            "\"rewardPlayerExp\":" + rewardPlayerExp + "," +
            "\"rewardItems\":" + jsonRewardItems + "}";
    }
}
