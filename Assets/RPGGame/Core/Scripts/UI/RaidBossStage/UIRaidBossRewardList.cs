using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIRaidBossRewardList : UIDataItemList<UIRaidBossReward, RaidBossReward>
{
    public void SetListItems(List<RaidBossReward> list, UnityAction<UIRaidBossReward> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIRaidBossReward SetListItem(RaidBossReward data)
    {
        string id = System.Guid.NewGuid().ToString();
        var item = SetListItem(id);
        item.SetData(data);
        return item;
    }
}
