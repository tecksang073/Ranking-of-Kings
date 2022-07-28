using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class UIUnlockItemEvent : UnityEvent<UIUnlockItem> { }

public class UIUnlockItem : UIDataItem<string>
{
    public UIItem uiItem;
    public GameObject lockObject;
    public GameObject unlockObject;

    protected override void Update()
    {
        base.Update();

        if (lockObject != null)
            lockObject.SetActive(!PlayerUnlockItem.IsUnlock(data));

        if (unlockObject != null)
            unlockObject.SetActive(PlayerUnlockItem.IsUnlock(data));
    }

    public override void Clear()
    {
        if (uiItem != null)
            uiItem.Clear();
    }

    public override bool IsEmpty()
    {
        return string.IsNullOrEmpty(data);
    }

    public override void UpdateData()
    {
        if (uiItem != null)
            uiItem.data = GetPlayerItem();
    }

    public PlayerItem GetPlayerItem()
    {
        return new PlayerItem()
        {
            Id = "",
            PlayerId = "",
            DataId = data,
            Amount = 1,
            Exp = 0,
            EquipItemId = "",
            EquipPosition = "",
        };
    }
}
