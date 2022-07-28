using System.Collections.Generic;

[System.Serializable]
public class DbPlayerAchievement : IPlayerAchievement
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int progress;
    public int Progress { get { return progress; } set { progress = value; } }
    public bool earned;
    public bool Earned { get { return earned; } set { earned = value; } }

    public static List<PlayerAchievement> CloneList(IEnumerable<DbPlayerAchievement> list)
    {
        var result = new List<PlayerAchievement>();
        foreach (var entry in list)
        {
            result.Add(PlayerAchievement.CloneTo(entry, new PlayerAchievement()));
        }
        return result;
    }
}
