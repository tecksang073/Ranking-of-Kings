using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseStage : BaseMission
{
    [Header("Stage Info")]
    public string stageNumber;
    public StageType stageType;
    [Header("Rewards")]
    public CurrencyRandomAmount[] randomCustomCurrencies;
    public int randomSoftCurrencyMinAmount;
    public int randomSoftCurrencyMaxAmount;
    public int rewardPlayerExp;
    public int rewardClanExp;
    public int rewardCharacterExp;
    public ItemDrop[] rewardItems;
    [Header("First Clear Rewards")]
    public CurrencyAmount[] firstClearRewardCustomCurrencies;
    public int firstClearRewardSoftCurrency;
    public int firstClearRewardHardCurrency;
    public int firstClearRewardPlayerExp;
    public ItemAmount[] firstClearRewardItems;
    [Header("Unlock")]
    public bool unlocked;
    public BaseStage[] unlockStages;

    protected override void OnValidate()
    {
        base.OnValidate();
        bool hasChanges = false;
        bool entryHasChanges = false;
        if (availabilities != null)
        {
            for (int i = 0; i < availabilities.Length; ++i)
            {
                availabilities[i] = availabilities[i].ValidateSetting(out entryHasChanges);
                hasChanges = hasChanges || entryHasChanges;
            }
        }
        int daysInMonth = System.DateTime.DaysInMonth(startYear, (int)startMonth);
        if (startDay > daysInMonth)
        {
            startDay = daysInMonth;
            hasChanges = true;
        }
#if UNITY_EDITOR
        if (hasChanges)
            EditorUtility.SetDirty(this);
#endif
    }

    public abstract List<PlayerItem> GetCharacters();

    public virtual List<PlayerItem> GetRewardItems()
    {
        var dict = new Dictionary<string, PlayerItem>();
        foreach (var rewardItem in rewardItems)
        {
            var item = rewardItem.item;
            var newEntry = new PlayerItem();
            newEntry.Id = item.Id;
            newEntry.DataId = item.Id;
            newEntry.Amount = 1;
            dict[item.Id] = newEntry;
        }
        return new List<PlayerItem>(dict.Values);
    }

    public virtual List<PlayerItem> GetFirstClearRewardItems()
    {
        var dict = new Dictionary<string, PlayerItem>();
        foreach (var rewardItem in firstClearRewardItems)
        {
            var item = rewardItem.item;
            var newEntry = new PlayerItem();
            newEntry.Id = item.Id;
            newEntry.DataId = item.Id;
            newEntry.Amount = 1;
            dict[item.Id] = newEntry;
        }
        return new List<PlayerItem>(dict.Values);
    }

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
        // Reward Custom Currencies
        var jsonRandomCustomCurrencies = "";
        if (randomCustomCurrencies != null)
        {
            foreach (var entry in randomCustomCurrencies)
            {
                if (!string.IsNullOrEmpty(jsonRandomCustomCurrencies))
                    jsonRandomCustomCurrencies += ",";
                jsonRandomCustomCurrencies += entry.ToJson();
            }
        }
        jsonRandomCustomCurrencies = "[" + jsonRandomCustomCurrencies + "]";
        // Reward Items
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
        // First Clear Custom Currencies
        var jsonFirstClearRewardCustomCurrencies = "";
        if (firstClearRewardCustomCurrencies != null)
        {
            foreach (var entry in firstClearRewardCustomCurrencies)
            {
                if (!string.IsNullOrEmpty(jsonFirstClearRewardCustomCurrencies))
                    jsonFirstClearRewardCustomCurrencies += ",";
                jsonFirstClearRewardCustomCurrencies += entry.ToJson();
            }
        }
        jsonFirstClearRewardCustomCurrencies = "[" + jsonFirstClearRewardCustomCurrencies + "]";
        // First Clear Reward Items
        var jsonFirstClearRewardItems = "";
        if (firstClearRewardItems != null)
        {
            foreach (var entry in firstClearRewardItems)
            {
                if (!string.IsNullOrEmpty(jsonFirstClearRewardItems))
                    jsonFirstClearRewardItems += ",";
                jsonFirstClearRewardItems += entry.ToJson();
            }
        }
        jsonFirstClearRewardItems = "[" + jsonFirstClearRewardItems + "]";
        // Unlock Stages
        var jsonUnlockStages = "";
        if (unlockStages != null)
        {
            foreach (var entry in unlockStages)
            {
                if (!string.IsNullOrEmpty(jsonUnlockStages))
                    jsonUnlockStages += ",";
                jsonUnlockStages += "\"" + entry.Id + "\"";
            }
        }
        jsonUnlockStages = "[" + jsonUnlockStages + "]";
        return "{\"id\":\"" + Id + "\"," +
            "\"stageType\":" + (int)stageType + "," +
            "\"recommendBattlePoint\":" + recommendBattlePoint + "," +
            "\"requireStamina\":" + requireStamina + "," +
            "\"requireCustomStamina\":\"" + requireCustomStamina + "\"," +
            "\"randomCustomCurrencies\":" + jsonRandomCustomCurrencies + "," +
            "\"randomSoftCurrencyMinAmount\":" + randomSoftCurrencyMinAmount + "," +
            "\"randomSoftCurrencyMaxAmount\":" + randomSoftCurrencyMaxAmount + "," +
            "\"rewardPlayerExp\":" + rewardPlayerExp + "," +
            "\"rewardClanExp\":" + rewardClanExp + "," +
            "\"rewardCharacterExp\":" + rewardCharacterExp + "," +
            "\"availabilities\":" + jsonAvailabilities + "," +
            "\"hasAvailableDate\":" + (hasAvailableDate ? 1 : 0) + "," +
            "\"startYear\":" + startYear + "," +
            "\"startMonth\":" + (int)startMonth + "," +
            "\"startDay\":" + startDay + "," +
            "\"durationDays\":" + durationDays + "," +
            "\"rewardItems\":" + jsonRewardItems + "," +
            "\"firstClearRewardCustomCurrencies\":" + jsonFirstClearRewardCustomCurrencies + "," +
            "\"firstClearRewardSoftCurrency\":" + firstClearRewardSoftCurrency + "," +
            "\"firstClearRewardHardCurrency\":" + firstClearRewardHardCurrency + "," +
            "\"firstClearRewardPlayerExp\":" + firstClearRewardPlayerExp + "," +
            "\"firstClearRewardItems\":" + jsonFirstClearRewardItems + "," +
            "\"unlocked\":" + (unlocked ? 1 : 0) + "," +
            "\"unlockStages\":" + jsonUnlockStages + "}";
    }
}
