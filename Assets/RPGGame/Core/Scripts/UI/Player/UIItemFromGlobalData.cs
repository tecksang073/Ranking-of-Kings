using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemFromGlobalData : UIBase
{
    public UIItem uiItem;

    private PlayerItem dirtyItem;
    protected virtual void Update()
    {
        if (dirtyItem != UIGlobalData.SelectedItem)
        {
            dirtyItem = UIGlobalData.SelectedItem;
            uiItem.data = UIGlobalData.SelectedItem;
        }
    }
}
