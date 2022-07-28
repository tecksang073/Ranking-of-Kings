using UnityEngine;
using System.Collections;

public class MaterialItem : BaseItem
{
    [Header("Fix/Override variables")]
    public bool useFixSellPrice = false;
    public int fixSellPrice = 0;
    public bool useFixRewardExp = false;
    public int fixRewardExp = 0;

    public override string ToJson()
    {
        return "{\"id\":\"" + Id + "\"," +
            "\"category\":\"" + category + "\"," +
            "\"itemTier\":" + itemTier.ToJson() + "," +
            "\"type\":\"" + Type + "\"," +
            "\"maxStack\":" + MaxStack + "," +
            "\"useFixSellPrice\":" + (useFixSellPrice ? 1 : 0) + "," +
            "\"fixSellPrice\":" + fixSellPrice + "," +
            "\"useFixRewardExp\":" + (useFixRewardExp ? 1 : 0) + "," +
            "\"fixRewardExp\":" + fixRewardExp + "}";
    }
}
