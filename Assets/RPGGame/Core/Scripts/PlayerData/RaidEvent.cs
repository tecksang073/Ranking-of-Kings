[System.Serializable]
public partial class RaidEvent : IRaidEvent
{
    public string id;
    public string Id { get { return id; } set { id = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int remainingHp;
    public int RemainingHp { get { return remainingHp; } set { remainingHp = value; } }
    public long startTime;
    public long StartTime { get { return startTime; } set { startTime = value; } }
    public long endTime;
    public long EndTime { get { return endTime; } set { endTime = value; } }

    public static GameDatabase GameDatabase
    {
        get { return GameInstance.GameDatabase; }
    }

    public BaseRaidBossStage RaidBossStage
    {
        get
        {
            if (GameDatabase != null && GameDatabase.RaidBossStages.ContainsKey(DataId))
                return GameDatabase.RaidBossStages[DataId];
            return null;
        }
    }

    public RaidEvent Clone()
    {
        return CloneTo(this, new RaidEvent());
    }

    public static T CloneTo<T>(IRaidEvent from, T to) where T : IRaidEvent
    {
        to.Id = from.Id;
        to.DataId = from.DataId;
        to.RemainingHp = from.RemainingHp;
        to.StartTime = from.StartTime;
        to.EndTime = from.EndTime;
        return to;
    }
}
