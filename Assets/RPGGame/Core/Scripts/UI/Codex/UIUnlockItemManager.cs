using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIUnlockItemManager : UIBase
{
    public UIItem uiSelectedInfo;
    public UIUnlockItemList uiUnlockItemList;
    public bool showCharacter;
    public bool showEquipment;
    public bool dontSelectFirstEntryOnShow;
    public override void Show()
    {
        base.Show();

        if (uiUnlockItemList != null)
        {
            uiUnlockItemList.selectionMode = UIDataItemSelectionMode.Toggle;
            uiUnlockItemList.eventSelect.RemoveListener(SelectItem);
            uiUnlockItemList.eventSelect.AddListener(SelectItem);
            uiUnlockItemList.eventDeselect.RemoveListener(DeselectItem);
            uiUnlockItemList.eventDeselect.AddListener(DeselectItem);
            uiUnlockItemList.ClearListItems();

            var list = new List<string>();
            var listCharacter = new List<string>();
            var listEquipment = new List<string>();
            foreach (var item in GameInstance.GameDatabase.items)
            {
                if (item is CharacterItem)
                    listCharacter.Add(item.Id);

                if (item is EquipmentItem)
                    listEquipment.Add(item.Id);
            }

            if (showCharacter)
                list.AddRange(listCharacter);

            if (showEquipment)
                list.AddRange(listEquipment);
            uiUnlockItemList.SetListItems(list);

            if (dontSelectFirstEntryOnShow)
            {
                uiUnlockItemList.DeselectedItems(null);
            }
            else
            {
                if (uiUnlockItemList.UIEntries.Count > 0)
                {
                    var allUIs = uiUnlockItemList.UIEntries.Values.ToList();
                    allUIs[0].Selected = true;
                    SelectItem(allUIs[0]);
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

        if (uiUnlockItemList != null)
            uiUnlockItemList.ClearListItems();
    }

    protected virtual void SelectItem(UIDataItem ui)
    {
        if (uiSelectedInfo != null)
            uiSelectedInfo.SetData((ui as UIUnlockItem).GetPlayerItem());
    }

    protected virtual void DeselectItem(UIDataItem ui)
    {
        // Don't deselect
        ui.Selected = true;
    }
}
