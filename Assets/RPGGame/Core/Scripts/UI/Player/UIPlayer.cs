using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class UIPlayer : UIDataItem<Player>
{
    public Text textProfileName;
    public UILevel uiLevel;
    public UIItem uiMainCharacter;
    public UIPlayerList uiPlayerList;
    [Header("Friend buttons")]
    public Button buttonFriendRequest;
    public Button buttonFriendAccept;
    public Button buttonFriendDecline;
    public Button buttonFriendDelete;
    [FormerlySerializedAs("buttonRequestDelete")]
    public Button buttonFriendRequestDelete;
    [Header("Clan buttons")]
    public Button buttonClanJoinAccept;
    public Button buttonClanJoinDecline;
    public Button buttonClanMemberDelete;
    public Button buttonClanOwnerTransfer;
    public Button buttonClanSetRoleToMember;
    public Button buttonClanSetRoleToManager;
    [Header("Arena UIs")]
    public Text textArenaScore;
    public UIArenaRank uiArenaRank;
    // Events
    [Header("Friend events")]
    public UnityEvent eventFriendRequestSuccess;
    public UnityEvent eventFriendRequestFail;
    public UnityEvent eventFriendAcceptSuccess;
    public UnityEvent eventFriendAcceptFail;
    public UnityEvent eventFriendDeclineSuccess;
    public UnityEvent eventFriendDeclineFail;
    public UnityEvent eventFriendDeleteSuccess;
    public UnityEvent eventFriendDeleteFail;
    public UnityEvent eventFriendRequestDeleteSuccess;
    public UnityEvent eventFriendRequestDeleteFail;
    [Header("Clan events")]
    public UnityEvent eventClanJoinAcceptSuccess;
    public UnityEvent eventClanJoinAcceptFail;
    public UnityEvent eventClanJoinDeclineSuccess;
    public UnityEvent eventClanJoinDeclineFail;
    public UnityEvent eventClanMemberDeleteSuccess;
    public UnityEvent eventClanMemberDeleteFail;
    public UnityEvent eventClanOwnerTransferSuccess;
    public UnityEvent eventClanOwnerTransferFail;
    public UnityEvent eventClanSetRoleToMemberSuccess;
    public UnityEvent eventClanSetRoleToMemberFail;
    public UnityEvent eventClanSetRoleToManagerSuccess;
    public UnityEvent eventClanSetRoleToManagerFail;
    [Header("State UIs")]
    public GameObject[] memberObjects;
    public GameObject[] managerObjects;
    public GameObject[] ownerObjects;

    private void UpdateState()
    {
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
        if (!IsEmpty())
        {
            switch (data.ClanRole)
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
        }
    }

    public override void UpdateData()
    {
        SetupInfo(data);
        // Friend buttons
        if (buttonFriendRequest != null)
        {
            buttonFriendRequest.onClick.RemoveListener(OnClickFriendRequest);
            buttonFriendRequest.onClick.AddListener(OnClickFriendRequest);
            buttonFriendRequest.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendAccept != null)
        {
            buttonFriendAccept.onClick.RemoveListener(OnClickFriendAccept);
            buttonFriendAccept.onClick.AddListener(OnClickFriendAccept);
            buttonFriendAccept.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendDecline != null)
        {
            buttonFriendDecline.onClick.RemoveListener(OnClickFriendDecline);
            buttonFriendDecline.onClick.AddListener(OnClickFriendDecline);
            buttonFriendDecline.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendDelete != null)
        {
            buttonFriendDelete.onClick.RemoveListener(OnClickFriendDelete);
            buttonFriendDelete.onClick.AddListener(OnClickFriendDelete);
            buttonFriendDelete.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendRequestDelete != null)
        {
            buttonFriendRequestDelete.onClick.RemoveListener(OnClickRequestDelete);
            buttonFriendRequestDelete.onClick.AddListener(OnClickRequestDelete);
            buttonFriendRequestDelete.gameObject.SetActive(!IsEmpty());
        }
        // Clan buttons
        if (buttonClanJoinAccept != null)
        {
            buttonClanJoinAccept.onClick.RemoveListener(OnClickClanJoinAccept);
            buttonClanJoinAccept.onClick.AddListener(OnClickClanJoinAccept);
            buttonClanJoinAccept.gameObject.SetActive(!IsEmpty());
        }
        if (buttonClanJoinDecline != null)
        {
            buttonClanJoinDecline.onClick.RemoveListener(OnClickClanJoinDecline);
            buttonClanJoinDecline.onClick.AddListener(OnClickClanJoinDecline);
            buttonClanJoinDecline.gameObject.SetActive(!IsEmpty());
        }
        if (buttonClanMemberDelete != null)
        {
            buttonClanMemberDelete.onClick.RemoveListener(OnClickClanMemberDelete);
            buttonClanMemberDelete.onClick.AddListener(OnClickClanMemberDelete);
            buttonClanMemberDelete.gameObject.SetActive(!IsEmpty());
        }
        if (buttonClanOwnerTransfer != null)
        {
            buttonClanOwnerTransfer.onClick.RemoveListener(OnClickClanOwnerTransfer);
            buttonClanOwnerTransfer.onClick.AddListener(OnClickClanOwnerTransfer);
            buttonClanOwnerTransfer.gameObject.SetActive(!IsEmpty());
        }
        if (buttonClanSetRoleToMember != null)
        {
            buttonClanSetRoleToMember.onClick.RemoveListener(OnClickClanSetRoleToMember);
            buttonClanSetRoleToMember.onClick.AddListener(OnClickClanSetRoleToMember);
            buttonClanSetRoleToMember.gameObject.SetActive(!IsEmpty());
        }
        if (buttonClanSetRoleToManager != null)
        {
            buttonClanSetRoleToManager.onClick.RemoveListener(OnClickClanSetRoleToManager);
            buttonClanSetRoleToManager.onClick.AddListener(OnClickClanSetRoleToManager);
            buttonClanSetRoleToManager.gameObject.SetActive(!IsEmpty());
        }
        UpdateState();
    }

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(Player data)
    {
        if (data == null)
            data = new Player();

        if (textProfileName != null)
            textProfileName.text = data.ProfileName;

        // Stats
        if (uiLevel != null)
        {
            uiLevel.level = data.Level;
            uiLevel.maxLevel = data.MaxLevel;
            uiLevel.currentExp = data.CurrentExp;
            uiLevel.requiredExp = data.RequiredExp;
        }

        if (uiMainCharacter != null)
        {
            if (string.IsNullOrEmpty(data.MainCharacter) || !GameInstance.GameDatabase.Items.ContainsKey(data.MainCharacter))
            {
                uiMainCharacter.data = null;
            }
            else
            {
                uiMainCharacter.data = new PlayerItem()
                {
                    DataId = data.MainCharacter,
                    Exp = data.MainCharacterExp,
                };
            }
        }

        if (textArenaScore != null)
            textArenaScore.text = data.ArenaScore.ToString("N0");

        if (uiArenaRank != null)
            uiArenaRank.SetData(data.ArenaRank);
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickFriendRequest()
    {
        GameInstance.GameService.FriendRequest(data.Id, OnFriendRequestSuccess, OnFriendRequestFail);
    }

    private void OnFriendRequestSuccess(GameServiceResult result)
    {
        if (eventFriendRequestSuccess != null)
            eventFriendRequestSuccess.Invoke();
    }

    private void OnFriendRequestFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendRequestFail != null)
            eventFriendRequestFail.Invoke();
    }

    public void OnClickFriendAccept()
    {
        GameInstance.GameService.FriendAccept(data.Id, OnFriendAcceptSuccess, OnFriendAcceptFail);
    }

    private void OnFriendAcceptSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendAcceptSuccess != null)
            eventFriendAcceptSuccess.Invoke();
    }

    private void OnFriendAcceptFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendAcceptFail != null)
            eventFriendAcceptFail.Invoke();
    }

    public void OnClickFriendDecline()
    {
        GameInstance.GameService.FriendDecline(data.Id, OnFriendDeclineSuccess, OnFriendDeclineFail);
    }

    private void OnFriendDeclineSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendDeclineSuccess != null)
            eventFriendDeclineSuccess.Invoke();
    }

    private void OnFriendDeclineFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendDeclineFail != null)
            eventFriendDeclineFail.Invoke();
    }

    public void OnClickFriendDelete()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_DELETE_FRIEND),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_DELETE_FRIEND),
            () =>
            {
                GameInstance.GameService.FriendDelete(data.Id, OnFriendDeleteSuccess, OnFriendDeleteFail);
            });
    }

    private void OnFriendDeleteSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendDeleteSuccess != null)
            eventFriendDeleteSuccess.Invoke();
    }

    private void OnFriendDeleteFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendDeleteFail != null)
            eventFriendDeleteFail.Invoke();
    }

    public void OnClickRequestDelete()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_DELETE_REQUEST),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_DELETE_REQUEST),
            () =>
            {
                GameInstance.GameService.FriendRequestDelete(data.Id, OnRequestDeleteSuccess, OnRequestDeleteFail);
            });
    }

    private void OnRequestDeleteSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendRequestDeleteSuccess != null)
            eventFriendRequestDeleteSuccess.Invoke();
    }

    private void OnRequestDeleteFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendRequestDeleteFail != null)
            eventFriendRequestDeleteFail.Invoke();
    }

    public void OnClickClanJoinAccept()
    {
        GameInstance.GameService.ClanJoinAccept(data.Id, ClanJoinAcceptSuccess, ClanJoinAcceptFail);
    }

    private void ClanJoinAcceptSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventClanJoinAcceptSuccess != null)
            eventClanJoinAcceptSuccess.Invoke();
    }

    private void ClanJoinAcceptFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanJoinAcceptFail != null)
            eventClanJoinAcceptFail.Invoke();
    }
    public void OnClickClanJoinDecline()
    {
        GameInstance.GameService.ClanJoinDecline(data.Id, ClanJoinDeclineSuccess, ClanJoinDeclineFail);
    }

    private void ClanJoinDeclineSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventClanJoinDeclineSuccess != null)
            eventClanJoinDeclineSuccess.Invoke();
    }

    private void ClanJoinDeclineFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanJoinDeclineFail != null)
            eventClanJoinDeclineFail.Invoke();
    }

    public void OnClickClanMemberDelete()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_DELETE_CLAN_MEMBER),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_DELETE_CLAN_MEMBER),
            () =>
            {
                GameInstance.GameService.ClanMemberDelete(data.Id, ClanMemberDeleteSuccess, ClanMemberDeleteFail);
            });
    }

    private void ClanMemberDeleteSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventClanMemberDeleteSuccess != null)
            eventClanMemberDeleteSuccess.Invoke();
    }

    private void ClanMemberDeleteFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanMemberDeleteFail != null)
            eventClanMemberDeleteFail.Invoke();
    }

    public void OnClickClanOwnerTransfer()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_CLAN_OWNER_TRANSFER),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_CLAN_OWNER_TRANSFER),
            () =>
            {
                GameInstance.GameService.ClanOwnerTransfer(data.Id, ClanOwnerTransferSuccess, ClanOwnerTransferFail);
            });
    }

    private void ClanOwnerTransferSuccess(GameServiceResult result)
    {
        data.ClanRole = 2;
        Player.CurrentPlayer.ClanRole = 0;
        UpdateData();
        if (eventClanOwnerTransferSuccess != null)
            eventClanOwnerTransferSuccess.Invoke();
    }

    private void ClanOwnerTransferFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanOwnerTransferFail != null)
            eventClanOwnerTransferFail.Invoke();
    }

    public void OnClickClanSetRoleToMember()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_CLAN_SET_ROLE_TO_MEMBER),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MEMBER),
            () =>
            {
                GameInstance.GameService.ClanSetRole(data.Id, 0, ClanSetRoleToMemberSuccess, ClanSetRoleToMemberFail);
            });
    }

    private void ClanSetRoleToMemberSuccess(GameServiceResult result)
    {
        data.ClanRole = 0;
        UpdateData();
        if (eventClanSetRoleToMemberSuccess != null)
            eventClanSetRoleToMemberSuccess.Invoke();
    }

    private void ClanSetRoleToMemberFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanSetRoleToMemberFail != null)
            eventClanSetRoleToMemberFail.Invoke();
    }

    public void OnClickClanSetRoleToManager()
    {
        GameInstance.Singleton.ShowConfirmDialog(
            LanguageManager.GetText(GameText.WARN_TITLE_CLAN_SET_ROLE_TO_MANAGER),
            LanguageManager.GetText(GameText.WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MANAGER),
            () =>
            {
                GameInstance.GameService.ClanSetRole(data.Id, 1, ClanSetRoleToManagerSuccess, ClanSetRoleToManagerFail);
            });
    }

    private void ClanSetRoleToManagerSuccess(GameServiceResult result)
    {
        data.ClanRole = 1;
        UpdateData();
        if (eventClanSetRoleToManagerSuccess != null)
            eventClanSetRoleToManagerSuccess.Invoke();
    }

    private void ClanSetRoleToManagerFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventClanSetRoleToManagerFail != null)
            eventClanSetRoleToManagerFail.Invoke();
    }
}
