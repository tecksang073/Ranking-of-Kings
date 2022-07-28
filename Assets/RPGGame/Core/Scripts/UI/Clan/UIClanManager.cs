using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIClanManager : UIClan
{
    [Header("Manager UIs")]
    public UICurrencyPairing[] uiCheckinRewardCurrencies;
    public Text textClanDonateCount;
    public Text textMaxClanDonation;
    public UIClanDonationList uiUIClanDonationList;
    public GameObject[] notJoinedClanObjects;
    public GameObject[] joinedClanObjects;
    public GameObject[] memberObjects;
    public GameObject[] managerObjects;
    public GameObject[] ownerObjects;
    public GameObject[] notCheckedInObjects;
    public GameObject[] checkedInObjects;
    public GameObject[] notDonatedObjects;
    public GameObject[] donatedObjects;
    public Button buttonTerminate;
    public Button buttonExit;
    public Button buttonCheckin;

    [Header("Manager Events")]
    public UnityEvent eventTerminateSuccess;
    public UnityEvent eventTerminateFail;
    public UnityEvent eventExitSuccess;
    public UnityEvent eventExitFail;
    public UnityEvent eventCheckinSuccess;
    public UnityEvent eventCheckinFail;

    public bool IsManager { get { return !IsEmpty() && Player.CurrentPlayer.ClanId.Equals(data.Id) && Player.CurrentPlayer.ClanRole == 1; } }
    public bool IsOwner { get { return !IsEmpty() && Player.CurrentPlayer.ClanId.Equals(data.Id) && Player.CurrentPlayer.ClanRole == 2; } }

    private byte? previousClanRole;


    private Dictionary<string, UICurrency> cacheUiCheckinRewardCurrencies;
    public Dictionary<string, UICurrency> CacheUiCheckinRewardCurrencies
    {
        get
        {
            if (cacheUiCheckinRewardCurrencies == null)
            {
                cacheUiCheckinRewardCurrencies = new Dictionary<string, UICurrency>();
                if (uiCheckinRewardCurrencies != null)
                {
                    foreach (var uiRewardCurrency in uiCheckinRewardCurrencies)
                    {
                        cacheUiCheckinRewardCurrencies[uiRewardCurrency.id] = uiRewardCurrency.ui;
                    }
                }
            }
            return cacheUiCheckinRewardCurrencies;
        }
    }

    public bool CheckedIn { get; protected set; }
    public int ClanDonateCount { get; protected set; }
    public int MaxClanDonation { get; protected set; }
    public bool DonatedReachedLimit { get { return ClanDonateCount >= MaxClanDonation; } }

    private void OnEnable()
    {
        if (uiUIClanDonationList != null)
        {
            uiUIClanDonationList.uiClanManager = this;
            uiUIClanDonationList.SetListItems(new List<ClanDonation>(GameInstance.GameDatabase.clanDonations));
        }
        RefreshData();
        RefreshCheckinStatus();
        RefreshDonationStatus();
    }

    protected override void Update()
    {
        base.Update();
        if (!previousClanRole.HasValue || previousClanRole.Value != Player.CurrentPlayer.ClanRole)
        {
            previousClanRole = Player.CurrentPlayer.ClanRole;
            UpdateState();
        }
    }

    private void UpdateState()
    {
        if (notJoinedClanObjects != null)
        {
            foreach (var notJoinedClanObject in notJoinedClanObjects)
            {
                notJoinedClanObject.SetActive(IsEmpty());
            }
        }
        if (joinedClanObjects != null)
        {
            foreach (var joinedClanObject in joinedClanObjects)
            {
                joinedClanObject.SetActive(!IsEmpty());
            }
        }
        if (memberObjects != null)
        {
            foreach (var memberObject in memberObjects)
            {
                memberObject.SetActive(false);
            }
        }
        if (managerObjects != null)
        {
            foreach (var managerObject in managerObjects)
            {
                managerObject.SetActive(false);
            }
        }
        if (ownerObjects != null)
        {
            foreach (var ownerObject in ownerObjects)
            {
                ownerObject.SetActive(false);
            }
        }
        switch (Player.CurrentPlayer.ClanRole)
        {
            case 0:
                if (memberObjects != null)
                {
                    foreach (var memberObject in memberObjects)
                    {
                        memberObject.SetActive(true);
                    }
                }
                break;
            case 1:
                if (managerObjects != null)
                {
                    foreach (var managerObject in managerObjects)
                    {
                        managerObject.SetActive(true);
                    }
                }
                break;
            case 2:
                if (ownerObjects != null)
                {
                    foreach (var ownerObject in ownerObjects)
                    {
                        ownerObject.SetActive(true);
                    }
                }
                break;
        }
        if (notCheckedInObjects != null)
        {
            foreach (var notCheckedInObject in notCheckedInObjects)
            {
                notCheckedInObject.SetActive(IsEmpty() || !CheckedIn);
            }
        }
        if (checkedInObjects != null)
        {
            foreach (var checkedInObject in checkedInObjects)
            {
                checkedInObject.SetActive(!IsEmpty() && CheckedIn);
            }
        }
        if (notDonatedObjects != null)
        {
            foreach (var notDonatedObject in notDonatedObjects)
            {
                notDonatedObject.SetActive(IsEmpty() || !DonatedReachedLimit);
            }
        }
        if (donatedObjects != null)
        {
            foreach (var donatedObject in donatedObjects)
            {
                donatedObject.SetActive(!IsEmpty() && DonatedReachedLimit);
            }
        }
    }

    public override void UpdateData()
    {
        base.UpdateData();

        foreach (var kv in CacheUiCheckinRewardCurrencies)
        {
            kv.Value.data = new PlayerCurrency()
            {
                DataId = kv.Key,
                Amount = 0,
            };
        }

        if (GameInstance.GameDatabase.clanCheckinRewardCurrencies != null)
        {
            foreach (var rewardCurrency in GameInstance.GameDatabase.clanCheckinRewardCurrencies)
            {
                if (CacheUiCheckinRewardCurrencies.ContainsKey(rewardCurrency.id))
                {
                    CacheUiCheckinRewardCurrencies[rewardCurrency.id].data = new PlayerCurrency()
                    {
                        DataId = rewardCurrency.id,
                        Amount = rewardCurrency.amount,
                    };
                }
            }
        }

        if (buttonTerminate != null)
        {
            buttonTerminate.onClick.RemoveListener(OnClickTerminate);
            buttonTerminate.onClick.AddListener(OnClickTerminate);
            buttonTerminate.interactable = !IsEmpty() && Player.CurrentPlayer.IsClanLeader && Player.CurrentPlayer.ClanId.Equals(data.Id);
        }
        if (buttonExit != null)
        {
            buttonExit.onClick.RemoveListener(OnClickExit);
            buttonExit.onClick.AddListener(OnClickExit);
            buttonExit.interactable = !IsEmpty() && !Player.CurrentPlayer.IsClanLeader && Player.CurrentPlayer.ClanId.Equals(data.Id);
        }
        if (buttonCheckin != null)
        {
            buttonCheckin.onClick.RemoveListener(OnClickCheckin);
            buttonCheckin.onClick.AddListener(OnClickCheckin);
            buttonCheckin.interactable = !IsEmpty() && Player.CurrentPlayer.ClanId.Equals(data.Id) && !CheckedIn;
        }
        if (textClanDonateCount != null)
            textClanDonateCount.text = ClanDonateCount.ToString("N0");
        if (textMaxClanDonation != null)
            textMaxClanDonation.text = MaxClanDonation > 0 ? MaxClanDonation.ToString("N0") : GameInstance.GameDatabase.maxClanDonation.ToString("N0");
        UpdateState();
    }

    public void RefreshData()
    {
        GameInstance.GameService.GetClan(OnRefreshSuccess, OnRefreshFail);
    }

    private void OnRefreshSuccess(ClanResult result)
    {
        SetData(result.clan);
    }

    private void OnRefreshFail(string error)
    {
        SetData(null);
    }

    public void RefreshCheckinStatus()
    {
        GameInstance.GameService.GetClanCheckinStatus(OnRefreshCheckinStatusSuccess);
    }

    private void OnRefreshCheckinStatusSuccess(ClanCheckinStatusResult result)
    {
        CheckedIn = result.alreadyCheckin;
        UpdateState();
        ForceUpdate();
    }

    public void RefreshDonationStatus()
    {
        GameInstance.GameService.GetClanDonationStatus(OnRefreshDonationStatusSuccess);
    }

    private void OnRefreshDonationStatusSuccess(ClanDonationStatusResult result)
    {
        ClanDonateCount = result.clanDonateCount;
        MaxClanDonation = result.maxClanDonation;
        UpdateState();
        ForceUpdate();
        if (uiUIClanDonationList != null)
        {
            uiUIClanDonationList.ClearListItems();
            uiUIClanDonationList.SetListItems(new List<ClanDonation>(GameInstance.GameDatabase.clanDonations));
        }
    }

    public void OnClickTerminate()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_CLAN_TERMINATE),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_CLAN_TERMINATE),
            () =>
            {
                GameInstance.GameService.ClanTerminate(OnTerminateSuccess, OnTerminateFail);
            });
    }

    private void OnTerminateSuccess(GameServiceResult result)
    {
        if (eventTerminateSuccess != null)
            eventTerminateSuccess.Invoke();
    }

    private void OnTerminateFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventTerminateFail != null)
            eventTerminateFail.Invoke();
    }

    public void OnClickExit()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_CLAN_EXIT),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_CLAN_EXIT),
            () =>
            {
                GameInstance.GameService.ClanExit(OnExitSuccess, OnExitFail);
            });
    }

    private void OnExitSuccess(GameServiceResult result)
    {
        if (eventExitSuccess != null)
            eventExitSuccess.Invoke();
    }

    private void OnExitFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventExitFail != null)
            eventExitFail.Invoke();
    }

    public void OnClickCheckin()
    {
        GameInstance.GameService.ClanCheckin(OnCheckinSuccess, OnCheckinFail);
    }

    private void OnCheckinSuccess(ClanCheckinResult result)
    {
        if (eventCheckinSuccess != null)
            eventCheckinSuccess.Invoke();
        CheckedIn = true;
        SetData(result.clan);
        PlayerCurrency.SetDataRange(result.updateCurrencies);
    }

    private void OnCheckinFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventCheckinFail != null)
            eventCheckinFail.Invoke();
    }
}
