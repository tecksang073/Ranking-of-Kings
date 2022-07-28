﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UILootBoxList : UIDataItemList<UILootBox, LootBox>
{
    public AnimItemsRewarding animItemsRewarding;

    public void SetListItems(List<LootBox> list, UnityAction<UILootBox> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UILootBox SetListItem(LootBox data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.SetData(data);
        return item;
    }
}
