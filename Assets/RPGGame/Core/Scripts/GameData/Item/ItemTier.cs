using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTier : BaseGameData
{
    [Range(1, 1000)]
    public int maxLevel;
    [Tooltip("Requires Exp to levelup for each level")]
    public Int32Attribute expTable;
    [Tooltip("Sell price for each level")]
    public Int32Attribute sellPriceTable;
    [Tooltip("Levelup price for each level")]
    public Int32Attribute levelUpPriceTable;
    [Tooltip("Exp that other item gain when use this as material")]
    public Int32Attribute rewardExpTable;
    [Tooltip("Requires currency for evolve")]
    public int evolvePrice;
    public ushort sortWeight;

    public virtual string ToJson()
    {
        return "{\"id\":\"" + Id + "\"," +
            "\"maxLevel\":" + maxLevel + "," +
            "\"expTable\":" + expTable.ToJson() + "," +
            "\"sellPriceTable\":" + sellPriceTable.ToJson() + "," +
            "\"levelUpPriceTable\":" + levelUpPriceTable.ToJson() + "," +
            "\"rewardExpTable\":" + rewardExpTable.ToJson() + "," +
            "\"evolvePrice\":" + evolvePrice + "," +
            "\"sortWeight\":" + sortWeight + "}";
    }
}
