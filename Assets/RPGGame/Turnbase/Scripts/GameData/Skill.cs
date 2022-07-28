using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillUsageScope
{
    Self,
    Ally,
    Enemy,
    All,
    DeadAlly,
}

public enum AttackScope
{
    SelectedTarget,
    SelectedAndOneRandomTargets,
    SelectedAndTwoRandomTargets,
    SelectedAndThreeRandomTargets,
    OneRandomEnemy,
    TwoRandomEnemies,
    ThreeRandomEnemies,
    FourRandomEnemies,
    AllEnemies,
}

public enum BuffScope
{
    Self,
    SelectedTarget,
    SelectedAndOneRandomTargets,
    SelectedAndTwoRandomTargets,
    SelectedAndThreeRandomTargets,
    OneRandomAlly,
    TwoRandomAllies,
    ThreeRandomAllies,
    FourRandomAllies,
    AllAllies,
    OneRandomEnemy,
    TwoRandomEnemies,
    ThreeRandomEnemies,
    FourRandomEnemies,
    AllEnemies,
    All,
}

[System.Serializable]
public struct SkillAttackDamage
{
    public float fixDamage;
    public float fixDamageIncreaseEachLevel;
    public float pAtkDamageRate;
    public float pAtkDamageRateIncreaseEachLevel;
    public float mAtkDamageRate;
    public float mAtkDamageRateIncreaseEachLevel;
    [Tooltip("This will devide with calculated damage to show damage number text")]
    public int hitCount;

    public float GetFixDamage(int level = 1)
    {
        return fixDamage + (fixDamageIncreaseEachLevel * level);
    }

    public float GetPAtkDamageRate(int level = 1)
    {
        return pAtkDamageRate + (pAtkDamageRateIncreaseEachLevel * level);
    }

    public float GetMAtkDamageRate(int level = 1)
    {
        return mAtkDamageRate + (mAtkDamageRateIncreaseEachLevel * level);
    }
}

[System.Serializable]
public struct SkillAttack
{
    public AttackScope attackScope;
    public BaseAttackAnimationData attackAnimation;
    [Tooltip("Skill damage formula = `a.fixDamage` + ((`a.pAtkDamageRate` * `a.pAtk`) - `b.pDef`) + ((`a.mAtkDamageRate` * `a.mAtk`) - `b.mDef`)")]
    public SkillAttackDamage attackDamage;
}

[System.Serializable]
public class SkillBuff : BaseSkillBuff
{
    public BuffScope buffScope;

    [Header("Apply turns, Amount of turns that buff will be applied")]
    [Range(1, 10)]
    public int applyTurns = 1;
    [Range(-10, 10)]
    public float applyTurnsIncreaseEachLevel = 0;

    public int GetApplyTurns(int level = 1)
    {
        return applyTurns + (int)(applyTurnsIncreaseEachLevel * level);
    }
}

public class Skill : BaseSkill
{
    public SkillUsageScope usageScope;
    public int coolDownTurns;
    [Range(-10, 10)]
    public float coolDownIncreaseEachLevel = 0;
    [Tooltip("Attack each hits, leave its length to 0 to not attack")]
    public SkillAttack[] attacks;
    [Tooltip("Buffs, leave its length to 0 to not apply buffs")]
    public SkillBuff[] buffs;
    [Tooltip("Revive dead character (Character HP will be 1, use buffs to increase HP)")]
    public bool reviveCharacter;

    public override List<BaseSkillBuff> GetBuffs()
    {
        return new List<BaseSkillBuff>(buffs);
    }

    public int GetCoolDownTurns(int level = 1)
    {
        return coolDownTurns + (int)(coolDownIncreaseEachLevel * level);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (attacks.Length > 0)
            usageScope = SkillUsageScope.Enemy;
    }
#endif
}
