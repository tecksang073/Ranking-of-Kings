public class UIRaidEventList : UIDataItemList<UIRaidEvent, RaidEvent>
{
    public UIRaidEventPreparation uiRaidEventPreparation;

    private void OnEnable()
    {
        GetRaidEventList();
    }

    private void GetRaidEventList()
    {
        ClearListItems();
        GameInstance.GameService.GetRaidEventList(OnGetRaidEventListSuccess, OnGetRaidEventListFail);
    }

    private void OnGetRaidEventListSuccess(RaidEventListResult result)
    {
        foreach (var entry in result.list)
        {
            if (!GameInstance.GameDatabase.RaidBossStages.ContainsKey(entry.DataId))
                continue;
            var ui = SetListItem(entry.Id);
            ui.data = entry;
            ui.uiRaidEventPreparation = uiRaidEventPreparation;
            ui.Show();
        }
    }

    private void OnGetRaidEventListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, GetRaidEventList);
    }
}