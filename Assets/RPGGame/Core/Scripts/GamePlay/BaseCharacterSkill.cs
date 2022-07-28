public abstract class BaseCharacterSkill
{
    public int Level { get; protected set; }
    public BaseSkill Skill { get; protected set; }
    public string Id { get { return Skill.Id; } }

    public BaseCharacterSkill(int level, BaseSkill skill)
    {
        Level = level;
        Skill = skill;
    }

    public abstract float GetCoolDownDurationRate();
    public abstract float GetCoolDownDuration();
}
