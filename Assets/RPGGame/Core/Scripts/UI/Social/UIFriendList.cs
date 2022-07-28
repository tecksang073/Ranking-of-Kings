using UnityEngine;
using System.Collections;

public class UIFriendList : UIPlayerList
{
    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetFriendList(OnRefreshListSuccess, OnRefreshListFail);
    }

    private void OnRefreshListSuccess(PlayerListResult result)
    {
        SetListItems(result.list);
    }

    private void OnRefreshListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
