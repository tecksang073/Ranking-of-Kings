public class UIClanJoinRequestList : UIPlayerList
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
        GameInstance.GameService.GetClanJoinRequestList(OnRefreshListSuccess, OnRefreshListFail);
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
