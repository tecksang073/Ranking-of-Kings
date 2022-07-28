public class UIClanJoinPendingRequestList : UIClanList
{
    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetClanJoinPendingRequestList(OnRefreshListSuccess, OnRefreshListFail);
    }

    private void OnRefreshListSuccess(ClanListResult result)
    {
        SetListItems(result.list);
    }

    private void OnRefreshListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
