using System.Collections.Generic;

[System.Serializable]
public class DbPlayerUnlockItem : IPlayerUnlockItem
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

    public static List<PlayerUnlockItem> CloneList(IEnumerable<DbPlayerUnlockItem> list)
    {
        var result = new List<PlayerUnlockItem>();
        foreach (var entry in list)
        {
            result.Add(PlayerUnlockItem.CloneTo(entry, new PlayerUnlockItem()));
        }
        return result;
    }
}
