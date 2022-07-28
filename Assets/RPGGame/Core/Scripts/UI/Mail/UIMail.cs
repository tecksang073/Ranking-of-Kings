using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMail : UIDataItem<Mail>
{
    public Text textTitle;
    public Text textContent;
    public Text textSendTimestamp;
    public UIGameDataWithAmountList uiRewards;
    public GameObject[] readStateObjects;
    public GameObject[] unreadStateObjects;
    public GameObject[] claimStateObjects;
    public GameObject[] unclaimStateObjects;
    public GameObject[] claimableStateObjects;
    public GameObject[] unclaimableStateObjects;
    public UIMailList uiMailList;
    public UnityEvent onClaimMailRewardSuccess = new UnityEvent();

    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(Mail data)
    {
        if (textTitle != null)
            textTitle.text = data.Title;

        if (textContent != null)
            textContent.text = data.Content;

        if (textSendTimestamp != null)
        {
            var d = new System.DateTime(1970, 1, 1);
            d = d.AddSeconds(data.SentTimestamp);
            textSendTimestamp.text = d.GetPrettyDate();
        }

        if (uiRewards != null)
        {
            Dictionary<IGameData, int> dataAndAmounts = new Dictionary<IGameData, int>();
            if (!string.IsNullOrEmpty(data.Currencies))
            {
                List<IdAmountPair> list = JsonConvert.DeserializeObject<List<IdAmountPair>>(data.Currencies);

                foreach (var customCurrency in list)
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
            if (!string.IsNullOrEmpty(data.Items))
            {
                List<IdAmountPair> list = JsonConvert.DeserializeObject<List<IdAmountPair>>(data.Items);
                foreach (var customItem in list)
                {
                    if (!GameInstance.GameDatabase.Items.ContainsKey(customItem.id))
                        continue;
                    var item = GameInstance.GameDatabase.Items[customItem.id];
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

        if (readStateObjects != null)
        {
            foreach (var obj in readStateObjects)
                obj.SetActive(data.IsRead);
        }

        if (unreadStateObjects != null)
        {
            foreach (var obj in unreadStateObjects)
                obj.SetActive(!data.IsRead);
        }

        if (claimStateObjects != null)
        {
            foreach (var obj in claimStateObjects)
                obj.SetActive(data.IsClaim);
        }

        if (unclaimStateObjects != null)
        {
            foreach (var obj in unclaimStateObjects)
                obj.SetActive(!data.IsClaim);
        }

        if (claimableStateObjects != null)
        {
            foreach (var obj in claimableStateObjects)
                obj.SetActive(!data.IsClaim && data.HasReward);
        }

        if (unclaimableStateObjects != null)
        {
            foreach (var obj in unclaimableStateObjects)
                obj.SetActive(data.IsClaim || !data.HasReward);
        }
    }

    public void OnClickRead()
    {
        GameInstance.GameService.ReadMail(data.Id, OnReadMailSuccess, OnReadMailFail);
    }

    private void OnReadMailSuccess(ReadMailResult result)
    {
        uiMailList.ShowReadingMail(result.mail);
        data.IsRead = true;
        SetupInfo(data);
    }

    private void OnReadMailFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public void OnClickClaimMailRewards()
    {
        GameInstance.GameService.ClaimMailRewards(data.Id, OnClaimMailRewardsSuccess, OnClaimMailRewardsFail);
    }

    private void OnClaimMailRewardsSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        onClaimMailRewardSuccess.Invoke();
        data.isClaim = true;
        SetupInfo(data);
    }

    private void OnClaimMailRewardsFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public void OnClickDelete()
    {
        GameInstance.GameService.DeleteMail(data.Id, OnDeleteMailSuccess, OnDeleteMailFail);
    }

    private void OnDeleteMailSuccess(GameServiceResult result)
    {
        uiMailList.HideReadingMail();
        uiMailList.GetMailList();
    }

    private void OnDeleteMailFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
