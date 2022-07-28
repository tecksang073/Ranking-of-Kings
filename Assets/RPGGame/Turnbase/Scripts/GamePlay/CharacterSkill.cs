public class CharacterSkill : BaseCharacterSkill
{
    public Skill CastedSkill { get { return Skill as Skill; } }
    public int CoolDownTurns { get { return CastedSkill.GetCoolDownTurns(Level); } }
    public int TurnsCount { get; set; }
    public int RemainsTurns { get { return CoolDownTurns - TurnsCount; } }

    public CharacterSkill(int level, BaseSkill skill) : base(level, skill)
    {
        TurnsCount = CoolDownTurns;
    }

    public void IncreaseTurnsCount()
    {
        if (IsReady())
            return;

        ++TurnsCount;
    }

    public bool IsReady()
    {
        return TurnsCount >= CoolDownTurns && !Skill.isPassive;
    }

    public void OnUseSkill()
    {
        TurnsCount = 0;
    }

    public override float GetCoolDownDurationRate()
    {
        return (float)TurnsCount / (float)CoolDownTurns;
    }

    public override float GetCoolDownDuration()
    {
        return RemainsTurns;
    }
}
