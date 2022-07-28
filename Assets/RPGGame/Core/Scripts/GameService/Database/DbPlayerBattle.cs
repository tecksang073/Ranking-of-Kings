using System.Collections.Generic;

[System.Serializable]
public class DbPlayerBattle : IPlayerBattle
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public string session;
    public string Session { get { return session; } set { session = value; } }
    public byte battleResult;
    public byte BattleResult { get { return battleResult; } set { battleResult = value; } }
    public int rating;
    public int Rating { get { return rating; } set { rating = value; } }
    public byte battleType;
    public byte BattleType { get { return battleType; } set { battleType = value; } }

    public static List<PlayerBattle> CloneList(IEnumerable<DbPlayerBattle> list)
    {
        var result = new List<PlayerBattle>();
        foreach (var entry in list)
        {
            result.Add(PlayerBattle.CloneTo(entry, new PlayerBattle()));
        }
        return result;
    }
}
