using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIClanDonationList : UIDataItemList<UIClanDonation, ClanDonation>
{
    public UIClanManager uiClanManager;

    public void SetListItems(List<ClanDonation> list, UnityAction<UIClanDonation> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public UIClanDonation SetListItem(ClanDonation data)
    {
        if (string.IsNullOrEmpty(data.id))
            return null;
        var item = SetListItem(data.id);
        item.uiClanManager = uiClanManager;
        item.SetData(data);
        return item;
    }
}
