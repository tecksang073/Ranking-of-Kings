using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameplayRule : ScriptableObject
{
    public virtual float GetDamage(
        int seed,
        Elemental attackerElemental,
        Elemental defenderElemental,
        CalculatedAttributes attackerAttributes,
        CalculatedAttributes defenderAttributes,
        out float stealHp,
        float pAtkRate = 1f,
        float mAtkRate = 1f,
        int hitCount = 1,
        int fixDamage = 0)
    {
        stealHp = 0;

        if (hitCount <= 0)
            hitCount = 1;

        // damage = attack * (100 / (100 + defense))
        var calcPAtk = attackerAttributes.pAtk * pAtkRate;
        var pDmg = calcPAtk * (100f / (100f + defenderAttributes.pDef));
        stealHp += pDmg * attackerAttributes.bloodStealRateByPAtk;
#if !NO_MAGIC_STATS
        var calcMAtk = attackerAttributes.mAtk * mAtkRate;
        var mDmg = calcMAtk * (100f / (100f + defenderAttributes.mDef));
        stealHp += mDmg * attackerAttributes.bloodStealRateByMAtk;
#endif
        if (pDmg < 0)
            pDmg = 0;
#if !NO_MAGIC_STATS
        if (mDmg < 0)
            mDmg = 0;
#endif
        var totalDmg = pDmg;
#if !NO_MAGIC_STATS
        totalDmg += mDmg;
#endif
        // Increase / Decrease damage by effectiveness
        var effectiveness = 1f;
        if (attackerElemental != null && attackerElemental.CacheElementEffectiveness.TryGetValue(defenderElemental, out effectiveness))
            totalDmg *= effectiveness;
        totalDmg += Mathf.CeilToInt(totalDmg * RandomNumberUtils.RandomFloat(seed, GameInstance.GameDatabase.minAtkVaryRate, GameInstance.GameDatabase.maxAtkVaryRate)) + fixDamage;
        totalDmg /= hitCount;
        if (totalDmg < 0)
            totalDmg = 0;
        return totalDmg;
    }

    public virtual bool IsCrit(int seed, CalculatedAttributes attackerAttributes, CalculatedAttributes defenderAttributes)
    {
        return RandomNumberUtils.RandomFloat(seed, 0, 1) <= attackerAttributes.critChance;
    }

    public virtual float GetCritDamage(int seed, CalculatedAttributes attackerAttributes, CalculatedAttributes defenderAttributes, float damage)
    {
        return damage * attackerAttributes.critDamageRate;
    }

    public virtual bool IsBlock(int seed, CalculatedAttributes attackerAttributes, CalculatedAttributes defenderAttributes)
    {
        return RandomNumberUtils.RandomFloat(seed, 0, 1) <= defenderAttributes.blockChance;
    }

    public virtual float GetBlockDamage(CalculatedAttributes attributes, CalculatedAttributes defenderAttributes, float damage)
    {
        return damage / defenderAttributes.blockDamageRate;
    }

    public virtual bool IsHit(int seed, CalculatedAttributes attackerAttributes, CalculatedAttributes defenderAttributes)
    {
#if !NO_EVADE_STATS
        var hitChance = 1f;
        if (attackerAttributes.acc > 0 && defenderAttributes.eva > 0)
            hitChance = attackerAttributes.acc / defenderAttributes.eva;
        return !(hitChance < 0 || RandomNumberUtils.RandomFloat(seed, 0, 1) > hitChance);
#else
        return true;
#endif
    }

    public virtual int GetBattlePoint(PlayerItem item)
    {
        float battlePoint = 0;
        battlePoint += item.Attributes.hp / 25f;
        battlePoint += item.Attributes.pAtk;
        battlePoint += item.Attributes.pDef;
#if !NO_MAGIC_STATS
        battlePoint += item.Attributes.mAtk;
        battlePoint += item.Attributes.mDef;
#endif
        battlePoint += item.Attributes.spd;
#if !NO_EVADE_STATS
        battlePoint += item.Attributes.acc;
        battlePoint += item.Attributes.eva;
#endif
        return (int)battlePoint;
    }
}
