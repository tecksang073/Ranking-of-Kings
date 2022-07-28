using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : BaseActorItem
{
    [Header("Equipment Data")]
    public CalculatedAttributes extraAttributes;
    public List<BaseSkill> skills;
    public List<string> equippablePositions;
    public EquipmentItemEvolve evolveInfo;
    public EquipItemModelPrefab[] equipModelPrefabs;

    public override SpecificItemEvolve GetSpecificItemEvolve()
    {
        return evolveInfo;
    }

#if UNITY_EDITOR
    public override BaseActorItem CreateEvolveItemAsset(CreateEvolveItemData createEvolveItemData)
    {
        var newItem = ScriptableObjectUtility.CreateAsset<EquipmentItem>(name);
        newItem.extraAttributes = extraAttributes.Clone();
        newItem.skills = new List<BaseSkill>(skills);
        newItem.equippablePositions = new List<string>(equippablePositions);
        newItem.evolveInfo = (EquipmentItemEvolve)evolveInfo.Clone();
        return newItem;
    }
#endif

    public override string ToJson()
    {
        var jsonEquippablePositions = "";
        foreach (var entry in equippablePositions)
        {
            if (!string.IsNullOrEmpty(jsonEquippablePositions))
                jsonEquippablePositions += ",";
            jsonEquippablePositions += "\"" + entry + "\"";
        }
        jsonEquippablePositions = "[" + jsonEquippablePositions + "]";
        return "{\"id\":\"" + Id + "\"," +
            "\"category\":\"" + category + "\"," +
            "\"itemTier\":" + itemTier.ToJson() + "," +
            "\"type\":\"" + Type + "\"," +
            "\"maxStack\":" + MaxStack + "," +
            "\"useFixSellPrice\":" + (useFixSellPrice ? 1 : 0) + "," +
            "\"fixSellPrice\":" + fixSellPrice + "," +
            "\"useFixLevelUpPrice\":" + (useFixLevelUpPrice ? 1 : 0) + "," +
            "\"fixLevelUpPrice\":" + fixLevelUpPrice + "," +
            "\"useFixRewardExp\":" + (useFixRewardExp ? 1 : 0) + "," +
            "\"fixRewardExp\":" + fixRewardExp + "," +
            "\"evolveInfo\":" + GetSpecificItemEvolve().ToJson() + "," +
            "\"randomAttributes\":" + randomAttributes.ToJson() + "," +
            "\"equippablePositions\":" + jsonEquippablePositions + "}";
    }
}

[System.Serializable]
public class EquipmentItemAmount : BaseItemAmount<EquipmentItem> { }

[System.Serializable]
public class EquipmentItemDrop : BaseItemDrop<EquipmentItem> { }

[System.Serializable]
public class EquipmentItemEvolve : SpecificItemEvolve<EquipmentItem>
{
    public override SpecificItemEvolve<EquipmentItem> Create()
    {
        return new EquipmentItemEvolve();
    }
}

[System.Serializable]
public struct EquipItemModelPrefab
{
    public string slotId;
    public GameObject modelPrefab;
}