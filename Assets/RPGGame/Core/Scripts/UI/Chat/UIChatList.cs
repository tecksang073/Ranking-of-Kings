using System.Collections.Generic;
using UnityEngine.Events;

public class UIChatList : UIDataItemList<UIChat, ChatMessage>
{
    public void SetListItems(List<ChatMessage> list, UnityAction<UIChat> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIChat SetListItem(ChatMessage data)
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
