using System.Collections.Generic;

[System.Serializable]
public class DbPlayerStamina : IPlayerStamina
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    public long recoveredTime;
    public long RecoveredTime { get { return recoveredTime; } set { recoveredTime = value; } }
    public int refillCount;
    public int RefillCount { get { return refillCount; } set { refillCount = value; } }
    public long lastRefillTime;
    public long LastRefillTime { get { return lastRefillTime; } set { lastRefillTime = value; } }

    public static List<PlayerStamina> CloneList(IEnumerable<DbPlayerStamina> list)
    {
        var result = new List<PlayerStamina>();
        foreach (var entry in list)
        {
            result.Add(PlayerStamina.CloneTo(entry, new PlayerStamina()));
        }
        return result;
    }
}
