using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIClanList : UIDataItemList<UIClan, Clan>
{
    public void SetListItems(List<Clan> list, UnityAction<UIClan> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIClan SetListItem(Clan data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.uiClanJoinRequestList = this;
        item.SetData(data);
        return item;
    }
}
