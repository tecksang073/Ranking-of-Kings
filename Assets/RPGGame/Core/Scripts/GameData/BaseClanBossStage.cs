using UnityEngine;

[System.Serializable]
public struct ClanBossReward
{
    public int damageDealtMin;
    public int damageDealtMax;
    public CurrencyAmount[] rewardCustomCurrencies;
    public int rewardSoftCurrency;
    public int rewardHardCurrency;
    public ItemAmount[] rewardItems;

    public string ToJson()
    {
        // Reward custom currencies
        var jsonRewardCustomCurrencies = "";
        if (rewardCustomCurrencies != null)
        {
            foreach (var entry in rewardCustomCurrencies)
            {
                if (!string.IsNullOrEmpty(jsonRewardCustomCurrencies))
                    jsonRewardCustomCurrencies += ",";
                jsonRewardCustomCurrencies += entry.ToJson();
            }
        }
        jsonRewardCustomCurrencies = "[" + jsonRewardCustomCurrencies + "]";
        // Reward items
        var jsonRewardItems = "";
        if (rewardItems != null)
        {
            foreach (var entry in rewardItems)
            {
                if (!string.IsNullOrEmpty(jsonRewardItems))
                    jsonRewardItems += ",";
                jsonRewardItems += entry.ToJson();
            }
        }
        jsonRewardItems = "[" + jsonRewardItems + "]";
        return "{\"damageDealtMin\":" + damageDealtMin + "," +
            "\"damageDealtMax\":" + damageDealtMax + "," +
            "\"rewardCustomCurrencies\":" + jsonRewardCustomCurrencies + "," +
            "\"rewardSoftCurrency\":" + rewardSoftCurrency + "," +
            "\"rewardHardCurrency\":" + rewardHardCurrency + "," +
            "\"rewardItems\":" + jsonRewardItems + "}";
    }
}

public abstract class BaseClanBossStage : BaseMission
{
    [Header("Rewards")]
    public ClanBossReward[] rewards;

    public abstract PlayerItem GetCharacter();

    public virtual string ToJson()
    {
        // Event Availibilities
        var jsonAvailabilities = "";
        if (availabilities != null)
        {
            foreach (var entry in availabilities)
            {
                if (!string.IsNullOrEmpty(jsonAvailabilities))
                    jsonAvailabilities += ",";
                jsonAvailabilities += entry.ToJson();
            }
        }
        jsonAvailabilities = "[" + jsonAvailabilities + "]";
        // Rewards
        var jsonRewards = "";
        if (rewards != null)
        {
            foreach (var entry in rewards)
            {
                if (!string.IsNullOrEmpty(jsonRewards))
                    jsonRewards += ",";
                jsonRewards += entry.ToJson();
            }
        }
        jsonRewards = "[" + jsonRewards + "]";
        return "{\"id\":\"" + Id + "\"," +
            "\"recommendBattlePoint\":" + recommendBattlePoint + "," +
            "\"requireStamina\":" + requireStamina + "," +
            "\"requireCustomStamina\":\"" + requireCustomStamina + "\"," +
            "\"availabilities\":" + jsonAvailabilities + "," +
            "\"hasAvailableDate\":" + (hasAvailableDate ? 1 : 0) + "," +
            "\"startYear\":" + startYear + "," +
            "\"startMonth\":" + (int)startMonth + "," +
            "\"startDay\":" + startDay + "," +
            "\"durationDays\":" + durationDays + "," +
            "\"rewards\":" + jsonRewards + "," +
            "\"maxHp\":" + GetCharacter().Attributes.hp + "}";
    }
}
