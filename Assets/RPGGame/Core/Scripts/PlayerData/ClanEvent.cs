[System.Serializable]
public partial class ClanEvent : IClanEvent
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

    public BaseClanBossStage ClanBossStage
    {
        get
        {
            if (GameDatabase != null && GameDatabase.ClanBossStages.ContainsKey(DataId))
                return GameDatabase.ClanBossStages[DataId];
            return null;
        }
    }

    public ClanEvent Clone()
    {
        return CloneTo(this, new ClanEvent());
    }

    public static T CloneTo<T>(IClanEvent from, T to) where T : IClanEvent
    {
        to.Id = from.Id;
        to.DataId = from.DataId;
        to.RemainingHp = from.RemainingHp;
        to.StartTime = from.StartTime;
        to.EndTime = from.EndTime;
        return to;
    }
}
