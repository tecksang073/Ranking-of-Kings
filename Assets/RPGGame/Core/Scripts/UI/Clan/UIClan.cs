using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIClan : UIDataItem<Clan>
{
    public Text textName;
    public UILevel uiLevel;
    public UIPlayer uiOwner;
    [FormerlySerializedAs("uiClanList")]
    public UIClanList uiClanJoinRequestList;
    [Header("Buttons")]
    public Button buttonJoinRequest;
    public Button buttonJoinRequestDelete;
    [Header("Events")]
    public UnityEvent eventJoinRequestSuccess;
    public UnityEvent eventJoinRequestFail;
    public UnityEvent eventJoinRequestDeleteSuccess;
    public UnityEvent eventJoinRequestDeleteFail;

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
        if (buttonJoinRequest != null)
        {
            buttonJoinRequest.onClick.RemoveListener(OnClickJoinRequest);
            buttonJoinRequest.onClick.AddListener(OnClickJoinRequest);
            buttonJoinRequest.interactable = !IsEmpty() && !Player.CurrentPlayer.JoinedClan;
        }
        if (buttonJoinRequestDelete != null)
        {
            buttonJoinRequestDelete.onClick.RemoveListener(OnClickJoinRequestDelete);
            buttonJoinRequestDelete.onClick.AddListener(OnClickJoinRequestDelete);
            buttonJoinRequestDelete.interactable = !IsEmpty() && !Player.CurrentPlayer.JoinedClan;
        }
    }

    private void SetupInfo(Clan data)
    {
        if (data == null)
            data = new Clan();

        if (textName != null)
            textName.text = data.Name;

        if (uiLevel != null)
        {
            uiLevel.level = data.Level;
            uiLevel.maxLevel = data.MaxLevel;
            uiLevel.currentExp = data.CurrentExp;
            uiLevel.requiredExp = data.RequiredExp;
        }

        if (uiOwner != null)
            uiOwner.data = data.Owner;
    }

    public void OnClickJoinRequest()
    {
        GameInstance.GameService.ClanJoinRequest(data.Id, OnJoinRequestSuccess, OnJoinRequestFail);
    }

    private void OnJoinRequestSuccess(GameServiceResult result)
    {
        if (uiClanJoinRequestList != null)
            uiClanJoinRequestList.RemoveListItem(data.Id);
        if (eventJoinRequestSuccess != null)
            eventJoinRequestSuccess.Invoke();
    }

    private void OnJoinRequestFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventJoinRequestFail != null)
            eventJoinRequestFail.Invoke();
    }

    public void OnClickJoinRequestDelete()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_DELETE_CLAN_JOIN_REQUEST),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_DELETE_CLAN_JOIN_REQUEST),
            () =>
            {
                GameInstance.GameService.ClanJoinRequestDelete(data.Id, OnJoinRequestDeleteSuccess, OnJoinRequestDeleteFail);
            });
    }

    private void OnJoinRequestDeleteSuccess(GameServiceResult result)
    {
        if (uiClanJoinRequestList != null)
            uiClanJoinRequestList.RemoveListItem(data.Id);
        if (eventJoinRequestDeleteSuccess != null)
            eventJoinRequestDeleteSuccess.Invoke();
    }

    private void OnJoinRequestDeleteFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventJoinRequestDeleteFail != null)
            eventJoinRequestDeleteFail.Invoke();
    }
}
