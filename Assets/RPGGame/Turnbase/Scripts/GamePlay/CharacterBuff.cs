using UnityEngine;

public class CharacterBuff : BaseCharacterBuff
{
    private int healAmount;
    public SkillBuff CastedBuff { get { return Buff as SkillBuff; } }
    public int ApplyTurns { get { return CastedBuff.GetApplyTurns(Level); } }
    public int TurnsCount { get; set; }
    public int RemainsTurns { get { return ApplyTurns - TurnsCount; } }

    public CharacterBuff(int level, BaseSkill skill, int buffIndex, int seed, BaseCharacterEntity giver, BaseCharacterEntity receiver) : base(level, skill, buffIndex, giver, receiver)
    {
        healAmount = 0;
        var pAtkHealRate = PAtkHealRate;
        if (pAtkHealRate != 0)
            healAmount += Mathf.CeilToInt(giver.Item.Attributes.pAtk * pAtkHealRate);
#if !NO_MAGIC_STATS
        var mAtkHealRate = MAtkHealRate;
        if (mAtkHealRate != 0)
            healAmount += Mathf.CeilToInt(giver.Item.Attributes.mAtk * mAtkHealRate);
#endif
        ApplyHeal();
    }

    public void IncreaseTurnsCount()
    {
        if (IsEnd())
            return;

        ApplyHeal();
        ++TurnsCount;
    }

    public void ApplyHeal()
    {
        if (Mathf.Abs(healAmount) <= 0)
            return;

        Receiver.Hp += healAmount;
        if (healAmount > 0)
        {
            Manager.SpawnHealText(Mathf.Abs(healAmount), Receiver, 1);
        }
        else
        {
            Manager.SpawnPoisonText(Mathf.Abs(healAmount), Receiver, 1);
            if (!Receiver.IsPlayerCharacter)
                BaseGamePlayManager.IncreaseTotalDamage(Mathf.Abs(healAmount));
        }
    }

    public bool IsEnd()
    {
        return TurnsCount >= ApplyTurns;
    }

    public override float GetRemainsDurationRate()
    {
        return (float)TurnsCount / (float)ApplyTurns;
    }

    public override float GetRemainsDuration()
    {
        return RemainsTurns;
    }
}
