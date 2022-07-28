using UnityEngine;

public class UIItemSkillList : MonoBehaviour
{
    public UIItem uiItem;
    public UISkillList uiSkillList;

    private void Awake()
    {
        if (uiItem != null)
        {
            uiItem.eventUpdate.RemoveListener(OnItemDataUpdate);
            uiItem.eventUpdate.AddListener(OnItemDataUpdate);
        }
    }

    private void OnEnable()
    {
        if (uiSkillList == null)
            return;

        if (uiItem == null)
        {
            if (UIGlobalData.SelectedItem == null)
                uiSkillList.Hide();
            else
                UpdateList(UIGlobalData.SelectedItem);
        }
    }

    private void OnDestroy()
    {
        if (uiItem != null)
            uiItem.eventUpdate.RemoveListener(OnItemDataUpdate);
    }

    private void OnItemDataUpdate(UIDataItem ui)
    {
        var uiItem = ui as UIItem;
        UpdateList(uiItem.data);
    }

    private void UpdateList(PlayerItem data)
    {
        if (data.CharacterData != null)
        {
            uiSkillList.SetListItems(data.CharacterData.skills);
            uiSkillList.Show();
        }
        else if (data.EquipmentData != null)
        {
            uiSkillList.SetListItems(data.EquipmentData.skills);
            uiSkillList.Show();
        }
        else
        {
            uiSkillList.Hide();
        }
    }
}
