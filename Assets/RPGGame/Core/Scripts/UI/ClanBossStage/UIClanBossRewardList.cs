using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIClanBossRewardList : UIDataItemList<UIClanBossReward, ClanBossReward>
{
    public void SetListItems(List<ClanBossReward> list, UnityAction<UIClanBossReward> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIClanBossReward SetListItem(ClanBossReward data)
    {
        string id = System.Guid.NewGuid().ToString();
        var item = SetListItem(id);
        item.SetData(data);
        return item;
    }
}
