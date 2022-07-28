using UnityEngine;

public class ArenaRank : BaseGameData
{
    public Sprite highlight;
    public int scoreToRankUp;
    [Tooltip("Reward when rank up")]
    public int rewardSoftCurrency;
    [Tooltip("Reward when rank up")]
    public int rewardHardCurrency;
    [Tooltip("Reward when rank up")]
    public ItemAmount[] rewardItems;

    public virtual string ToJson()
    {
        // Items
        var jsonRewardItems = "";
        foreach (var entry in rewardItems)
        {
            if (!string.IsNullOrEmpty(jsonRewardItems))
                jsonRewardItems += ",";
            jsonRewardItems += entry.ToJson();
        }
        jsonRewardItems = "[" + jsonRewardItems + "]";
        // Combine
        return "{\"id\":\"" + Id + "\"," +
            "\"scoreToRankUp\":" + scoreToRankUp + "," +
            "\"rewardSoftCurrency\":" + rewardSoftCurrency + "," +
            "\"rewardHardCurrency\":" + rewardHardCurrency + "," +
            "\"rewardItems\":" + jsonRewardItems + "}";
    }
}
