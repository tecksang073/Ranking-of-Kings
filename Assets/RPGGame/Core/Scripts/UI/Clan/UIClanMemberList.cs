using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClanMemberList : UIPlayerList
{
    public UIPlayer ownerPrefab;
    public UIPlayer managerPrefab;
    public UIPlayer memberPrefab;
    public UIClanManager manager;

    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetClanMemberList(OnRefreshListSuccess, OnRefreshListFail);
    }

    private void OnRefreshListSuccess(PlayerListResult result)
    {
        itemPrefab = manager.IsOwner ? ownerPrefab : manager.IsManager ? managerPrefab : memberPrefab;
        SetListItems(result.list);
    }

    private void OnRefreshListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
