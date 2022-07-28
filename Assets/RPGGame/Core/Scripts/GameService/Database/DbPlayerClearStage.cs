using System.Collections.Generic;

[System.Serializable]
public class DbPlayerClearStage : IPlayerClearStage
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int bestRating;
    public int BestRating { get { return bestRating; } set { bestRating = value; } }

    public static List<PlayerClearStage> CloneList(IEnumerable<DbPlayerClearStage> list)
    {
        var result = new List<PlayerClearStage>();
        foreach (var entry in list)
        {
            result.Add(PlayerClearStage.CloneTo(entry, new PlayerClearStage()));
        }
        return result;
    }
}
