public class UIClanEventList : UIDataItemList<UIClanEvent, ClanEvent>
{
    public UIClanEventPreparation uiClanEventPreparation;

    private void OnEnable()
    {
        GetClanEventList();
    }

    private void GetClanEventList()
    {
        ClearListItems();
        GameInstance.GameService.GetClanEventList(OnGetClanEventListSuccess, OnGetClanEventListFail);
    }

    private void OnGetClanEventListSuccess(ClanEventListResult result)
    {
        foreach (var entry in result.list)
        {
            if (!GameInstance.GameDatabase.ClanBossStages.ContainsKey(entry.DataId))
                continue;
            var ui = SetListItem(entry.Id);
            ui.data = entry;
            ui.uiClanEventPreparation = uiClanEventPreparation;
            ui.Show();
        }
    }

    private void OnGetClanEventListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, GetClanEventList);
    }
}