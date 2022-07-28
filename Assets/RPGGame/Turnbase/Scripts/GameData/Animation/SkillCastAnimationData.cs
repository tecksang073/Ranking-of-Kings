using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCastAnimationData : BaseSkillCastAnimationData
{
    public bool castAtMapCenter;
}

public static class SkillCastAnimationDataExtension
{
    public static bool GetCastAtMapCenter(this SkillCastAnimationData skillCastAnimation)
    {
        return skillCastAnimation == null ? false : skillCastAnimation.castAtMapCenter;
    }
}
