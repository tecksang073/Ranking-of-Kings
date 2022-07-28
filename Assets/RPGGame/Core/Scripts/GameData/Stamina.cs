using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StaminaUnit : short
{
    Seconds = 0,
    Minutes = 1,
    Hours = 2,
    Days = 3,
}

[System.Serializable]
public class Stamina
{
    public string id;
    public Sprite icon;
    public Sprite icon2;
    public Sprite icon3;
    public Int32Attribute maxAmountTable;
    [Tooltip("It will uses hard-currency to refill stamina")]
    public int[] refillPrices = new int[0];
    public StaminaUnit recoverUnit;
    [Tooltip("Recover Duration, maximum is 360 days")]
    [Range(0, 360)]
    public int recoverDuration;

    public string Id { get { return id; } }
    public Sprite Icon { get { return icon; } }
    public Sprite Icon2 { get { return icon2; } }
    public Sprite Icon3 { get { return icon3; } }

    public virtual string ToJson()
    {
        var refillPricesJson = "";
        foreach (var refillPrice in refillPrices)
        {
            if (!string.IsNullOrEmpty(refillPricesJson))
                refillPricesJson += ",";
            refillPricesJson += refillPrice;
        }
        refillPricesJson = "[" + refillPricesJson + "]";
        return "{\"id\":\"" + id + "\"," +
            "\"maxAmountTable\":" + maxAmountTable.ToJson() + "," +
            "\"refillPrices\":" + refillPricesJson + "," +
            "\"recoverUnit\":" + (short)recoverUnit + "," +
            "\"recoverDuration\":" + recoverDuration + "}";
    }
}
