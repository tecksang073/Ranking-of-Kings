using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(EquipmentItem))]
[CanEditMultipleObjects]
public class EquipmentItemEditor : BaseCustomEditor
{
    private EquipmentItem cacheItem;
    protected override void SetFieldCondition()
    {
        if (cacheItem == null)
            cacheItem = CreateInstance<EquipmentItem>();
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixSellPrice), true, cacheItem.GetMemberName(a => a.fixSellPrice));
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixLevelUpPrice), true, cacheItem.GetMemberName(a => a.fixLevelUpPrice));
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixRewardExp), true, cacheItem.GetMemberName(a => a.fixRewardExp));
    }
}
