using UnityEngine;
using System.Collections;

public class UIPendingRequestList : UIPlayerList
{
    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetPendingRequestList(OnRefreshListSuccess, OnRefreshListFail);
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
