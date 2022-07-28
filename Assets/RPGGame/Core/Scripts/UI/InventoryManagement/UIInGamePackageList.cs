using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInGamePackageList : UIDataItemList<UIInGamePackage, InGamePackage>
{
    public AnimItemsRewarding animItemsRewarding;

    public void SetListItems(List<InGamePackage> list, UnityAction<UIInGamePackage> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIInGamePackage SetListItem(InGamePackage data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.SetData(data);
        return item;
    }
}
