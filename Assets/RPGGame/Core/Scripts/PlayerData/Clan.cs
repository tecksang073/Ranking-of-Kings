[System.Serializable]
public partial class Clan : ILevel, IClan
{
    public string id;
    public string Id { get { return id; } set { id = value; } }
    public int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    public string name;
    public string Name { get { return name; } set { name = value; } }
    public string description;
    public string Description { get { return description; } set { description = value; } }
    public Player owner;
    public Player Owner { get { return owner; } set { owner = value; } }

    private int level = -1;
    private int collectExp = -1;
    private int dirtyExp = -1;  // Exp for dirty check to calculate `Level` and `CollectExp` fields

    public static GameDatabase GameDatabase
    {
        get { return GameInstance.GameDatabase; }
    }

    public int Level
    {
        get
        {
            CalculateLevelAndRemainExp();
            return level;
        }
    }

    public int CurrentExp
    {
        get
        {
            CalculateLevelAndRemainExp();
            return collectExp;
        }
    }

    public int MaxLevel
    {
        get { return GameDatabase == null ? 1 : GameDatabase.clanMaxLevel; }
    }

    public int RequiredExp
    {
        get { return GameDatabase == null ? 0 : GameDatabase.clanExpTable.Calculate(Level, GameDatabase.clanMaxLevel); }
    }

    private void CalculateLevelAndRemainExp()
    {
        if (GameDatabase == null)
        {
            level = 1;
            collectExp = 0;
            return;
        }
        if (dirtyExp == -1 || dirtyExp != Exp)
        {
            dirtyExp = Exp;
            var remainExp = Exp;
            var maxLevel = GameDatabase.clanMaxLevel;
            for (level = 1; level < maxLevel; ++level)
            {
                var nextExp = GameDatabase.clanExpTable.Calculate(level, maxLevel);
                if (remainExp - nextExp < 0)
                    break;
                remainExp -= nextExp;
            }
            collectExp = remainExp;
        }
    }

    public Clan Clone()
    {
        return CloneTo(this, new Clan());
    }

    public static T CloneTo<T>(IClan from, T to) where T : IClan
    {
        to.Id = from.Id;
        to.Name = from.Name;
        to.Exp = from.Exp;
        to.Description = from.Description;
        to.Owner = from.Owner;
        return to;
    }
}
