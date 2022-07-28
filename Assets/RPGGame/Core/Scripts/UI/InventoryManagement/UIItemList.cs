using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class UIItemList : UIDataItemList<UIItem, PlayerItem>
{
    // Private
    private readonly Dictionary<string, int> selectedItemIdAmountPair = new Dictionary<string, int>();
    private readonly Dictionary<PlayerItem, int> selectedItemAmountPair = new Dictionary<PlayerItem, int>();

    public void SetListItems(List<PlayerItem> list, UnityAction<UIItem> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIItem SetListItem(PlayerItem data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.SetData(data);
        return item;
    }

    protected override void MakeSelectedList(string id, UIItem uiEntry)
    {
        base.MakeSelectedList(id, uiEntry);
        selectedItemIdAmountPair.Add(id, uiEntry.SelectedAmount);
        selectedItemAmountPair.Add(uiEntry.data, uiEntry.SelectedAmount);
    }

    protected override void ClearSelectedLists()
    {
        base.ClearSelectedLists();
        selectedItemIdAmountPair.Clear();
        selectedItemAmountPair.Clear();
    }

    public List<UIItem> GetSelectedUIList(string dataId)
    {
        var valueList = GetSelectedUIList();
        var list = valueList.Where(entry =>
            entry != null &&
            entry.data != null &&
            entry.data.ItemData != null &&
            entry.data.DataId.Equals(dataId)).ToList();
        return list;
    }

    public List<PlayerItem> GetSelectedDataList(string dataId)
    {
        var valueList = GetSelectedDataList();
        var list = valueList.Where(entry =>
            entry != null &&
            entry.ItemData != null &&
            entry.DataId.Equals(dataId)).ToList();
        return list;
    }

    public Dictionary<string, int> GetSelectedItemIdAmountPair(bool forceRebuild = false)
    {
        MakeSelectedLists(forceRebuild);
        return selectedItemIdAmountPair;
    }

    public Dictionary<PlayerItem, int> GetSelectedItemAmountPair(bool forceRebuild = false)
    {
        MakeSelectedLists(forceRebuild);
        return selectedItemAmountPair;
    }

    public bool ContainsItemWithDataId(string dataId, bool forceRebuild = false)
    {
        MakeSelectedLists(forceRebuild);
        var list = GetSelectedDataList();
        foreach (var entry in list)
        {
            if (entry.DataId == dataId)
                return true;
        }
        return false;
    }
}
