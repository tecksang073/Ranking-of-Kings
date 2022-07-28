using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class BaseUISkillList<TUI, TSkill> : UIDataItemList<TUI, TSkill>
    where TUI : UIDataItem<TSkill>
    where TSkill : BaseSkill
{
    public void SetListItems(List<BaseSkill> list, UnityAction<TUI> onSetListItem = null)
    {
        ClearListItems();
        foreach (var entry in list)
        {
            var ui = SetListItem(entry as TSkill);
            if (ui != null && onSetListItem != null)
                onSetListItem(ui);
        }
    }

    public TUI SetListItem(TSkill data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return null;
        var item = SetListItem(data.Id);
        item.SetData(data);
        return item;
    }
}
