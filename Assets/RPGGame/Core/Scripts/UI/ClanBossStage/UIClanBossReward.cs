using System.Collections.Generic;
using UnityEngine.UI;

public class UIClanBossReward : UIDataItem<ClanBossReward>
{
    public Text textRank;
    public UIGameDataWithAmountList uiRewards;

    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        // Never empty
        return false;
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(ClanBossReward data)
    {
        if (textRank != null)
        {
            if (data.damageDealtMin == data.damageDealtMax)
                textRank.text = data.damageDealtMin.ToString("N0");
            else
                textRank.text = data.damageDealtMin + "-" + data.damageDealtMax;
        }

        if (uiRewards != null)
        {
            Dictionary<IGameData, int> dataAndAmounts = new Dictionary<IGameData, int>();
            if (data.rewardCustomCurrencies != null && data.rewardCustomCurrencies.Length> 0)
            {
                foreach (var customCurrency in data.rewardCustomCurrencies)
                {
                    if (!GameInstance.GameDatabase.Currencies.ContainsKey(customCurrency.id))
                        continue;
                    var currency = GameInstance.GameDatabase.Currencies[customCurrency.id];
                    if (!dataAndAmounts.ContainsKey(currency))
                        dataAndAmounts[currency] = customCurrency.amount;
                    else
                        dataAndAmounts[currency] += customCurrency.amount;
                }
            }
            if (data.rewardSoftCurrency > 0)
            {
                var currency = GameInstance.GameDatabase.Currencies[GameInstance.GameDatabase.softCurrency.Id];
                if (!dataAndAmounts.ContainsKey(currency))
                    dataAndAmounts[currency] = data.rewardSoftCurrency;
                else
                    dataAndAmounts[currency] += data.rewardSoftCurrency;
            }
            if (data.rewardHardCurrency > 0)
            {
                var currency = GameInstance.GameDatabase.Currencies[GameInstance.GameDatabase.hardCurrency.Id];
                if (!dataAndAmounts.ContainsKey(currency))
                    dataAndAmounts[currency] = data.rewardHardCurrency;
                else
                    dataAndAmounts[currency] += data.rewardHardCurrency;
            }
            if (data.rewardItems != null && data.rewardItems.Length > 0)
            {
                foreach (var customItem in data.rewardItems)
                {
                    if (!GameInstance.GameDatabase.Items.ContainsKey(customItem.Id))
                        continue;
                    var item = GameInstance.GameDatabase.Items[customItem.Id];
                    if (!dataAndAmounts.ContainsKey(item))
                        dataAndAmounts[item] = customItem.amount;
                    else
                        dataAndAmounts[item] += customItem.amount;
                }
            }
            uiRewards.ClearListItems();
            foreach (var kvPair in dataAndAmounts)
            {
                uiRewards.SetListItem(kvPair.Key, kvPair.Value);
            }
        }
    }
}
