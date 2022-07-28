using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(CharacterItem))]
[CanEditMultipleObjects]
public class CharacterItemEditor : BaseCustomEditor
{
    private CharacterItem cacheItem;
    protected override void SetFieldCondition()
    {
        if (cacheItem == null)
            cacheItem = CreateInstance<CharacterItem>();
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixSellPrice), true, cacheItem.GetMemberName(a => a.fixSellPrice));
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixLevelUpPrice), true, cacheItem.GetMemberName(a => a.fixLevelUpPrice));
        ShowOnBool(cacheItem.GetMemberName(a => a.useFixRewardExp), true, cacheItem.GetMemberName(a => a.fixRewardExp));
    }
}
