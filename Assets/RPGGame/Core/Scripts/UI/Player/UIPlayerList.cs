using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIPlayerList : UIDataItemList<UIPlayer, Player>
{
    public void SetListItems(List<Player> list, UnityAction<UIPlayer> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry as Player);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIPlayer SetListItem(Player data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.uiPlayerList = this;
        item.SetData(data);
        return item;
    }
}
