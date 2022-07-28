﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIItemCraftList : UIDataItemList<UIItemCraft, ItemCraftFormula>
{
    public AnimItemsRewarding animItemsRewarding;

    public void SetListItems(List<ItemCraftFormula> list, UnityAction<UIItemCraft> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIItemCraft SetListItem(ItemCraftFormula data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.SetData(data);
        return item;
    }

    public bool ContainsItemWithDataId(string dataId)
    {
        MakeSelectedLists();
        var list = GetSelectedDataList();
        foreach (var entry in list)
        {
            if (entry.Id == dataId)
                return true;
        }
        return false;
    }
}
