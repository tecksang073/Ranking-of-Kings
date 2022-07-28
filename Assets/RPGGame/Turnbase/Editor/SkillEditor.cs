using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Skill))]
[CanEditMultipleObjects]
public class SkillEditor : BaseCustomEditor
{
    private Skill cacheSkill;
    protected override void SetFieldCondition()
    {
        if (cacheSkill == null)
            cacheSkill = CreateInstance<Skill>();
        ShowOnBool(cacheSkill.GetMemberName(a => a.isPassive), false, cacheSkill.GetMemberName(a => a.castAnimation));
        ShowOnBool(cacheSkill.GetMemberName(a => a.isPassive), false, cacheSkill.GetMemberName(a => a.usageScope));
        ShowOnBool(cacheSkill.GetMemberName(a => a.isPassive), false, cacheSkill.GetMemberName(a => a.coolDownTurns));
        ShowOnBool(cacheSkill.GetMemberName(a => a.isPassive), false, cacheSkill.GetMemberName(a => a.coolDownIncreaseEachLevel));
        ShowOnBool(cacheSkill.GetMemberName(a => a.isPassive), false, cacheSkill.GetMemberName(a => a.attacks));
    }
}
