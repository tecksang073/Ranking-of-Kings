using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RandomStoreItem
{
    public ItemAmount rewardItem;
    public string requireCurrencyId;
    public int requireCurrencyAmount;
    public int randomWeight;

    public string Id
    {
        get { return rewardItem == null ? "" : rewardItem.Id; }
        set
        {
            if (rewardItem == null)
                rewardItem = new ItemAmount();
            rewardItem.Id = value;
        }
    }

    public int Amount
    {
        get { return rewardItem == null ? 0 : rewardItem.amount; }
        set
        {
            if (rewardItem == null)
                rewardItem = new ItemAmount();
            rewardItem.amount = value;
        }
    }

    public string ToJson()
    {
        if (string.IsNullOrEmpty(Id) || Amount <= 0)
            return "";
        return "{\"id\":\"" + Id + "\"," +
            "\"amount\":" + Amount + "," +
            "\"requireCurrencyId\":\"" + requireCurrencyId + "\"," +
            "\"requireCurrencyAmount\":" + requireCurrencyAmount + "," +
            "\"randomWeight\":" + randomWeight + "}";
    }
}

public class RandomStore : BaseGameData
{
    public RandomStoreItem[] items;
    [Min(1)]
    [Tooltip("Amount of items which will be randomed and shows in store")]
    public int itemsAmount;
    [Min(30)]
    [Tooltip("Automatically in-store item list refresh duration in seconds")]
    public float refreshDuration;
    [Tooltip("Currency Id which will be used to refresh selling item list")]
    public string refreshCurrencyId;
    [Tooltip("Currency amount which will be used to refresh selling item list")]
    public int refreshCurrencyAmount;

    public RandomStoreItem RandomItem()
    {
        var weight = new Dictionary<RandomStoreItem, int>();
        foreach (var item in items)
        {
            weight.Add(item, item.randomWeight);
        }
        return WeightedRandomizer.From(weight).TakeOne();
    }

    public virtual string ToJson()
    {
        // Random store items
        var jsonItems = "";
        foreach (var entry in items)
        {
            if (!string.IsNullOrEmpty(jsonItems))
                jsonItems += ",";
            jsonItems += entry.ToJson();
        }
        jsonItems = "[" + jsonItems + "]";
        // Combine
        return "{\"id\":\"" + Id + "\"," +
            "\"itemsAmount\":" + itemsAmount + "," +
            "\"refreshDuration\":" + refreshDuration + "," +
            "\"refreshCurrencyId\":\"" + refreshCurrencyId + "\"," +
            "\"refreshCurrencyAmount\":" + refreshCurrencyAmount + "," +
            "\"items\":" + jsonItems + "}";
    }
}
