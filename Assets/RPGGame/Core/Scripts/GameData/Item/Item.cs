using UnityEngine;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseItemAmount<T> where T : BaseItem
{
    public T item;
    public int amount;

    public string Id
    {
        get { return item == null ? "" : item.Id; }
        set { item = GameInstance.GameDatabase.Items[value] as T; }
    }

    public virtual string ToJson()
    {
        if (string.IsNullOrEmpty(Id) || amount <= 0)
            return "";
        return "{\"id\":\"" + Id + "\"," +
            "\"amount\":" + amount + "}";
    }
}

public abstract class BaseItemDrop<T> : BaseItemAmount<T> where T : BaseItem
{
    [Range(0, 1)]
    public float randomRate;

    public override string ToJson()
    {
        if (string.IsNullOrEmpty(Id) || amount <= 0)
            return "";
        return "{\"id\":\"" + Id + "\"," +
            "\"amount\":" + amount + "," +
            "\"randomRate\":" + randomRate.ToString(new CultureInfo("en-US", false)) + "}";
    }
}

[System.Serializable]
public class ItemAmount : BaseItemAmount<BaseItem> { }

[System.Serializable]
public class ItemDrop : BaseItemDrop<BaseItem> { }

[System.Serializable]
public class GenericItemEvolve
{
    [Tooltip("Price to evolve to next level")]
    public int price;

    [Tooltip("This will be multiplied by `GameDatabase.itemExpTable` to calculate item level up exp at current evolve level")]
    [Range(1f, 10f)]
    public float expRate = 1;

    [Tooltip("This will be multiplied by `GameDatabase.itemSellPriceTable` to calculate item sell price at current evolve level")]
    [Range(1f, 10f)]
    public float sellPriceRate = 1;

    [Tooltip("This will be multiplied by `GameDatabase.itemLevelUpPriceTable` to calculate item level up price at current evolve level")]
    [Range(1f, 10f)]
    public float levelUpPriceRate = 1;

    [Tooltip("This will be multiplied by `GameDatabase.itemRewardExpTable` to calculate item reward exp at current evolve level")]
    [Range(1f, 10f)]
    public float rewardExpRate = 1;
}

public abstract class SpecificItemEvolve
{
    [Tooltip("Required items for evolving")]
    public ItemAmount[] requiredMaterials;

    public abstract BaseItem GetEvolveItem();

    public virtual string ToJson()
    {
        // Lootbox rewards
        var jsonRequiredMaterials = "";
        foreach (var entry in requiredMaterials)
        {
            if (!string.IsNullOrEmpty(jsonRequiredMaterials))
                jsonRequiredMaterials += ",";
            jsonRequiredMaterials += entry.ToJson();
        }
        jsonRequiredMaterials = "[" + jsonRequiredMaterials + "]";

        var evolveItem = GetEvolveItem();
        var evolveItemId = "";
        if (evolveItem != null)
            evolveItemId = evolveItem.Id;

        return "{\"requiredMaterials\":" + jsonRequiredMaterials + "," +
            "\"evolveItem\":\"" + evolveItemId + "\"}";
    }
}

public abstract class SpecificItemEvolve<T> : SpecificItemEvolve where T : BaseItem
{
    public T evolveItem;

    public override BaseItem GetEvolveItem()
    {
        return evolveItem;
    }

    public SpecificItemEvolve Clone()
    {
        var result = Create();
        result.requiredMaterials = new ItemAmount[requiredMaterials.Length];
        for (var i = 0; i < result.requiredMaterials.Length; ++i)
        {
            var cloningData = requiredMaterials[i];
            var newData = new ItemAmount();
            if (cloningData != null)
            {
                newData.item = cloningData.item;
                newData.amount = cloningData.amount;
            }
            result.requiredMaterials[i] = newData;
        }
        result.evolveItem = evolveItem;
        return result;
    }
    public abstract SpecificItemEvolve<T> Create();
}

[System.Serializable]
public class CreateEvolveItemData
{
    public string title;
    public string description;
    public Sprite icon;
    public ItemTier itemTier;
    [Range(1f, 10f)]
    public float attributesRate;
}

public abstract class BaseItem : BaseGameData
{
    [Tooltip("This data MUST be set")]
    public ItemTier itemTier;

    [Tooltip("0 is not limit stack"), Range(0, 1000)]
    public int maxStack = 1;

    private string type;
    public string Type
    {
        get
        {
            if (string.IsNullOrEmpty(type))
                type = GetType().ToString();
            return type;
        }
    }

    public virtual int MaxStack
    {
        get { return maxStack; }
    }

    public virtual string ToJson()
    {
        return "{\"id\":\"" + Id + "\"," +
            "\"category\":\"" + category + "\"," +
            "\"itemTier\":" + itemTier.ToJson() + "," +
            "\"type\":\"" + Type + "\"," +
            "\"maxStack\":" + MaxStack + "}";
    }
}

public abstract class BaseActorItem : MaterialItem
{
    public bool useFixLevelUpPrice = false;
    public int fixLevelUpPrice = 0;

    [Tooltip("Max values of these attributes are max values of `GameDatabase.itemMaxLevel` level")]
    public Attributes attributes;

    public RandomAttributes randomAttributes;

#if UNITY_EDITOR
    #region Helpers Variables
    [Header("Set Max Attributes Tool")]
    [Tooltip("This will be multiplied by min level Hp then set to max level Hp, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float hpRateToSetMax = 1f;

    [Tooltip("This will be multiplied by min level PAtk then set to max level PAtk, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float pAtkRateToSetMax = 1f;

    [Tooltip("This will be multiplied by min level PDef then set to max level PDef, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float pDefRateToSetMax = 1f;

#if !NO_MAGIC_STATS
    [Tooltip("This will be multiplied by min level MAtk then set to max level MAtk, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float mAtkRateToSetMax = 1f;

    [Tooltip("This will be multiplied by min level MDef then set to max level MDef, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float mDefRateToSetMax = 1f;
#endif

    [Tooltip("This will be multiplied by min level Spd then set to max level Spd, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float spdRateToSetMax = 1f;

#if !NO_EVADE_STATS
    [Tooltip("This will be multiplied by min level Eva then set to max level Eva, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float evaRateToSetMax = 1f;

    [Tooltip("This will be multiplied by min level Acc then set to max level Acc, when use using context menu `Set Max Attributes By Rates`")]
    [Range(1f, 100f)]
    public float accRateToSetMax = 1f;
#endif

    [Header("Create Evolve Item Tool")]
    [Tooltip("Create new evolve item based on this item with context menu `Create Evolve Item`")]
    public CreateEvolveItemData[] createEvolveItems;
    #endregion
#endif

    public override int MaxStack
    {
        get { return 1; }
    }

#if UNITY_EDITOR
    #region Helpers Functions
    [ContextMenu("Set Max Attributes By Rates")]
    public void SetMaxAttributesByRates()
    {
        SetMaxHpByRate();
        SetMaxPAtkByRate();
        SetMaxPDefByRate();
#if !NO_MAGIC_STATS
        SetMaxMAtkByRate();
        SetMaxMDefByRate();
#endif
        SetMaxSpdByRate();
#if !NO_EVADE_STATS
        SetMaxEvaByRate();
        SetMaxAccByRate();
#endif
    }

    [ContextMenu("Set Max Attributes By Rates (HP Only)")]
    public void SetMaxHpByRate()
    {
        attributes.hp.maxValue = Mathf.CeilToInt(attributes.hp.minValue * hpRateToSetMax);
    }

    [ContextMenu("Set Max Attributes By Rates (PATK Only)")]
    public void SetMaxPAtkByRate()
    {
        attributes.pAtk.maxValue = Mathf.CeilToInt(attributes.pAtk.minValue * pAtkRateToSetMax);
    }

    [ContextMenu("Set Max Attributes By Rates (PDEF Only)")]
    public void SetMaxPDefByRate()
    {
        attributes.pDef.maxValue = Mathf.CeilToInt(attributes.pDef.minValue * pDefRateToSetMax);
    }

#if !NO_MAGIC_STATS
    [ContextMenu("Set Max Attributes By Rates (MATK Only)")]
    public void SetMaxMAtkByRate()
    {
        attributes.mAtk.maxValue = Mathf.CeilToInt(attributes.mAtk.minValue * mAtkRateToSetMax);
    }

    [ContextMenu("Set Max Attributes By Rates (MDEF Only)")]
    public void SetMaxMDefByRate()
    {
        attributes.mDef.maxValue = Mathf.CeilToInt(attributes.mDef.minValue * mDefRateToSetMax);
    }
#endif

    [ContextMenu("Set Max Attributes By Rates (SPD Only)")]
    public void SetMaxSpdByRate()
    {
        attributes.spd.maxValue = Mathf.CeilToInt(attributes.spd.minValue * spdRateToSetMax);
    }

#if !NO_EVADE_STATS
    [ContextMenu("Set Max Attributes By Rates (EVA Only)")]
    public void SetMaxEvaByRate()
    {
        attributes.eva.maxValue = Mathf.CeilToInt(attributes.eva.minValue * evaRateToSetMax);
    }

    [ContextMenu("Set Max Attributes By Rates (ACC Only)")]
    public void SetMaxAccByRate()
    {
        attributes.acc.maxValue = Mathf.CeilToInt(attributes.acc.minValue * accRateToSetMax);
    }
#endif

    [ContextMenu("Create Evolve Item")]
    public void CreateEvolveItem()
    {
        for (var i = 0; i < createEvolveItems.Length; ++i)
        {
            var createEvolveItemData = createEvolveItems[i];
            SetEvolveItemData(createEvolveItemData, CreateEvolveItemAsset(createEvolveItemData));
        }
    }

    public virtual void SetEvolveItemData(CreateEvolveItemData createEvolveItemData, BaseActorItem newItem)
    {
        newItem.title = string.IsNullOrEmpty(createEvolveItemData.title) ? title : createEvolveItemData.title;
        newItem.description = string.IsNullOrEmpty(createEvolveItemData.description) ? description : createEvolveItemData.description;
        newItem.icon = createEvolveItemData.icon == null ? icon : createEvolveItemData.icon;

        newItem.category = category;
        newItem.maxStack = maxStack;
        newItem.useFixSellPrice = useFixSellPrice;
        newItem.fixSellPrice = fixSellPrice;
        newItem.useFixLevelUpPrice = useFixLevelUpPrice;
        newItem.fixLevelUpPrice = fixLevelUpPrice;
        newItem.useFixRewardExp = useFixRewardExp;
        newItem.fixRewardExp = fixRewardExp;

        newItem.itemTier = createEvolveItemData.itemTier == null ? itemTier : createEvolveItemData.itemTier;
        newItem.attributes = attributes.CreateOverrideMaxLevelAttributes(itemTier.maxLevel, newItem.itemTier.maxLevel) * createEvolveItemData.attributesRate;
        EditorUtility.SetDirty(newItem);
    }
    #endregion
#endif

    public abstract SpecificItemEvolve GetSpecificItemEvolve();
#if UNITY_EDITOR
    public abstract BaseActorItem CreateEvolveItemAsset(CreateEvolveItemData createEvolveItemData);
#endif

    public override string ToJson()
    {
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
            "\"randomAttributes\":" + randomAttributes.ToJson() + "}";
    }
}
