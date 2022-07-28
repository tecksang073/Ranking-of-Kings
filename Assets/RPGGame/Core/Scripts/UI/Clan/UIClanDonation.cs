using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIClanDonation : UIDataItem<ClanDonation>
{
    public Image imageIcon;
    public UICurrency uiCurrency;
    public UICurrencyPairing[] uiRewardCurrencies;
    public Text textRewardClanExp;
    public UIClanManager uiClanManager;
    [Header("Buttons")]
    public Button buttonDonate;
    [Header("Events")]
    public UnityEvent eventDonationSuccess;
    public UnityEvent eventDonationFail;

    private Dictionary<string, UICurrency> cacheUiRewardCurrencies;
    public Dictionary<string, UICurrency> CacheUiRewardCurrencies
    {
        get
        {
            if (cacheUiRewardCurrencies == null)
            {
                cacheUiRewardCurrencies = new Dictionary<string, UICurrency>();
                if (uiRewardCurrencies != null)
                {
                    foreach (var uiRewardCurrency in uiRewardCurrencies)
                    {
                        cacheUiRewardCurrencies[uiRewardCurrency.id] = uiRewardCurrency.ui;
                    }
                }
            }
            return cacheUiRewardCurrencies;
        }
    }

    public override void Clear()
    {
        if (imageIcon != null)
            imageIcon.sprite = null;

        if (uiCurrency != null)
            uiCurrency.data = new PlayerCurrency();

        if (textRewardClanExp != null)
            textRewardClanExp.text = "0";

        foreach (var kv in CacheUiRewardCurrencies)
        {
            kv.Value.data = new PlayerCurrency()
            {
                DataId = kv.Key,
                Amount = 0,
            };
        }
    }

    public override bool IsEmpty()
    {
        return string.IsNullOrEmpty(data.id);
    }

    public override void UpdateData()
    {
        if (imageIcon != null)
            imageIcon.sprite = data.icon;

        if (uiCurrency != null)
        {
            uiCurrency.SetData(new PlayerCurrency()
            {
                DataId = data.requireCurrencyId,
                Amount = data.requireCurrencyAmount,
            });
        }

        if (textRewardClanExp != null)
            textRewardClanExp.text = data.rewardClanExp.ToString("N0");

        foreach (var kv in CacheUiRewardCurrencies)
        {
            kv.Value.data = new PlayerCurrency()
            {
                DataId = kv.Key,
                Amount = 0,
            };
        }

        if (data.rewardCurrencies != null)
        {
            foreach (var rewardCurrency in data.rewardCurrencies)
            {
                if (CacheUiRewardCurrencies.ContainsKey(rewardCurrency.id))
                {
                    CacheUiRewardCurrencies[rewardCurrency.id].data = new PlayerCurrency()
                    {
                        DataId = rewardCurrency.id,
                        Amount = rewardCurrency.amount,
                    };
                }
            }
        }

        if (buttonDonate != null)
        {
            buttonDonate.onClick.RemoveListener(OnClickDonation);
            buttonDonate.onClick.AddListener(OnClickDonation);
            buttonDonate.interactable = !IsEmpty() && uiClanManager != null && !uiClanManager.DonatedReachedLimit;
        }
    }

    public void OnClickDonation()
    {
        GameInstance.GameService.ClanDonation(data.id, OnDonationSuccess, OnDonationFail);
    }

    private void OnDonationSuccess(ClanDonationResult result)
    {
        if (eventDonationSuccess != null)
            eventDonationSuccess.Invoke();
        if (uiClanManager != null)
            uiClanManager.SetData(result.clan);
        PlayerCurrency.SetDataRange(result.updateCurrencies);
        if (uiClanManager != null)
            uiClanManager.RefreshDonationStatus();
    }

    private void OnDonationFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventDonationFail != null)
            eventDonationFail.Invoke();
    }
}
