using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InGamePackageRequirementType : byte
{
    RequireSoftCurrency = 0,
    RequireHardCurrency = 1,
}

public class InGamePackage : BaseGameData
{
    public Sprite highlight;
    public InGamePackageRequirementType requirementType;
    public int price = 0;
    public int rewardSoftCurrency;
    public int rewardHardCurrency;
    public ItemAmount[] rewardItems;

    public virtual string ToJson()
    {
        // Rewards
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
            "\"requirementType\":" + (byte)requirementType + "," +
            "\"price\":" + price + "," +
            "\"rewardSoftCurrency\":" + rewardSoftCurrency + "," +
            "\"rewardHardCurrency\":" + rewardHardCurrency + "," +
            "\"rewardItems\":" + jsonRewardItems + "}";
    }
}
