using System.Collections.Generic;
using UnityEngine;

public enum CraftRequirementType : byte
{
    RequireSoftCurrency = 0,
    RequireHardCurrency = 1,
}

public class ItemCraftFormula : BaseGameData
{
    public CraftRequirementType requirementType;
    public int price = 0;
    [Tooltip("Amount must > 0")]
    public ItemAmount resultItem;
    public ItemAmount[] materials;

    private Dictionary<string, int> cacheMaterials = null;
    public Dictionary<string, int> CacheMaterials
    {
        get
        {
            if (cacheMaterials == null)
                MakeCacheMaterials();
            return cacheMaterials;
        }
    }

    private void MakeCacheMaterials()
    {
        cacheMaterials = new Dictionary<string, int>();
        foreach (var material in materials)
        {
            if (material.item == null || string.IsNullOrEmpty(material.item.name))
                continue;
            var dataId = material.item.name;
            if (!cacheMaterials.ContainsKey(dataId))
                cacheMaterials.Add(dataId, material.amount);
            else
                cacheMaterials[dataId] += material.amount;
        }
    }

    public string ToJson()
    {
        if (resultItem.item == null || resultItem.amount <= 0 || materials == null || materials.Length == 0)
            return "{}";
        string materialsJson = string.Empty;
        for (int i = 0; i < materials.Length; ++i)
        {
            if (!string.IsNullOrEmpty(materialsJson))
                materialsJson += ",";
            if (materials[i].item == null || materials[i].amount <= 0)
                continue;
            materialsJson += materials[i].ToJson();
        }
        materialsJson = "[" + materialsJson + "]";
        return "{\"id\":\"" + Id + "\"," +
            "\"requirementType\":" + (byte)requirementType + "," +
            "\"price\":" + price + "," +
            "\"resultItem\":" + resultItem.ToJson() + "," +
            "\"materials\":" + materialsJson + "}";
    }
}
