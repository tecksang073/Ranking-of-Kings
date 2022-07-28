using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class UIUnlockItemList : UIDataItemList<UIUnlockItem, string>
{
    public void SetListItems(List<string> list, UnityAction<UIUnlockItem> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public override UIUnlockItem SetListItem(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        UIUnlockItem ui = base.SetListItem(id);
        ui.SetData(id);
        return ui;
    }

    public bool ContainsItemWithDataId(string dataId)
    {
        MakeSelectedLists();
        var list = GetSelectedDataList();
        foreach (var entry in list)
        {
            if (entry == dataId)
                return true;
        }
        return false;
    }
}
