using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIInventoryManager : UIBase
{
    public UIItem uiSelectedInfo;
    public UIItemList uiItemList;
    public UIItemListFilterSetting filterSetting;
    public bool dontSelectFirstEntryOnShow;
    public UIDataItemSelectionMode listSelectionMode = UIDataItemSelectionMode.Toggle;

    public override void Show()
    {
        base.Show();

        if (uiItemList != null)
        {
            uiItemList.selectionMode = listSelectionMode;
            uiItemList.eventSelect.RemoveListener(SelectItem);
            uiItemList.eventSelect.AddListener(SelectItem);
            uiItemList.eventDeselect.RemoveListener(DeselectItem);
            uiItemList.eventDeselect.AddListener(DeselectItem);
            uiItemList.ClearListItems();
            var list = PlayerItem.DataMap.Values.Where(a => UIItemListFilter.Filter(a, filterSetting)).ToList();
            list.SortLevel();
            uiItemList.SetListItems(list);

            if (dontSelectFirstEntryOnShow)
            {
                uiItemList.DeselectedItems(null);
            }
            else
            {
                if (uiItemList.UIEntries.Count > 0)
                {
                    if (listSelectionMode == UIDataItemSelectionMode.Toggle ||
                        listSelectionMode == UIDataItemSelectionMode.MultipleToggle)
                    {
                        var allUIs = uiItemList.UIEntries.Values.ToList();
                        allUIs[0].Selected = true;
                        SelectItem(allUIs[0]);
                    }
                }
                else
                {
                    if (uiSelectedInfo != null)
                    {
                        uiSelectedInfo.Clear();
                        uiSelectedInfo.Hide();
                    }
                }
            }
        }
    }

    public override void Hide()
    {
        base.Hide();

        if (uiSelectedInfo != null)
            uiSelectedInfo.Clear();

        if (uiItemList != null)
            uiItemList.ClearListItems();
    }

    protected virtual void SelectItem(UIDataItem ui)
    {
        UIGlobalData.SelectedItem = (ui as UIItem).data;
        if (uiSelectedInfo != null)
        {
            uiSelectedInfo.SetData((ui as UIItem).data);
            uiSelectedInfo.Show();
        }
    }

    protected virtual void DeselectItem(UIDataItem ui)
    {
        // Don't deselect
        ui.Selected = true;
    }
}
