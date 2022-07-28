using System.Collections.Generic;

public abstract class BaseCharacterBuff
{
    public BaseGamePlayManager Manager { get { return BaseGamePlayManager.Singleton; } }
    public BaseCharacterEntity Giver { get; protected set; }
    public BaseCharacterEntity Receiver { get; protected set; }
    public int Level { get; protected set; }
    public BaseSkill Skill { get; protected set; }
    public int BuffIndex { get; protected set; }
    public string Id { get { return Skill.Id + "_" + BuffIndex; } }
    private List<BaseSkillBuff> buffs;
    public List<BaseSkillBuff> Buffs
    {
        get
        {
            if (buffs == null)
                buffs = Skill.GetBuffs();
            return buffs;
        }
    }
    public BaseSkillBuff Buff { get { return Buffs[BuffIndex]; } }
    public CalculatedAttributes Attributes { get { return Buff.GetAttributes(Level); } }
    public float PAtkHealRate { get { return Buff.GetPAtkHealRate(Level); } }
#if !NO_MAGIC_STATS
    public float MAtkHealRate { get { return Buff.GetMAtkHealRate(Level); } }
#endif
    protected readonly List<GameEffect> effects = new List<GameEffect>();

    public BaseCharacterBuff(int level, BaseSkill skill, int buffIndex, BaseCharacterEntity giver, BaseCharacterEntity receiver)
    {
        Level = level;
        Skill = skill;
        BuffIndex = buffIndex;
        Giver = giver;
        Receiver = receiver;

        if (Buff.buffEffects != null)
            effects.AddRange(Buff.buffEffects.InstantiatesTo(receiver));
    }

    public void BuffRemove()
    {
        foreach (var effect in effects)
        {
            if (effect != null)
                effect.DestroyEffect();
        }
        effects.Clear();
    }

    public abstract float GetRemainsDurationRate();
    public abstract float GetRemainsDuration();
}
