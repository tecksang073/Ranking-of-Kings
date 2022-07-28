using UnityEngine;

[System.Serializable]
public struct ClanDonation
{
    public string id;
    public Sprite icon;
    public string requireCurrencyId;
    public int requireCurrencyAmount;
    public CurrencyAmount[] rewardCurrencies;
    public int rewardClanExp;

    public string ToJson()
    {
        // Reward Custom Currencies
        var jsonRewardCurrencies = "";
        if (rewardCurrencies != null)
        {
            foreach (var entry in rewardCurrencies)
            {
                if (!string.IsNullOrEmpty(jsonRewardCurrencies))
                    jsonRewardCurrencies += ",";
                jsonRewardCurrencies += entry.ToJson();
            }
        }
        jsonRewardCurrencies = "[" + jsonRewardCurrencies + "]";
        return "{\"id\":\"" + id + "\"," +
            "\"requireCurrencyId\":\"" + requireCurrencyId + "\"," +
            "\"requireCurrencyAmount\":" + requireCurrencyAmount + "," +
            "\"rewardCurrencies\":" + jsonRewardCurrencies + "," +
            "\"rewardClanExp\":" + rewardClanExp + "}";
    }
}
