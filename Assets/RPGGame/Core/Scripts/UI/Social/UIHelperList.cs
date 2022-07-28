public class UIHelperList : UIPlayerList
{
    private void OnEnable()
    {
        selectionMode = UIDataItemSelectionMode.Toggle;
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetHelperList(OnRefreshListSuccess, OnRefreshListFail);
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
