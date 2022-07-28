using System.Collections.Generic;

[System.Serializable]
public class DbPlayerFormation : IPlayerFormation
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int position;
    public int Position { get { return position; } set { position = value; } }
    public string itemId;
    public string ItemId { get { return itemId; } set { itemId = value; } }

    public static List<PlayerFormation> CloneList(IEnumerable<DbPlayerFormation> list)
    {
        var result = new List<PlayerFormation>();
        foreach (var entry in list)
        {
            result.Add(PlayerFormation.CloneTo(entry, new PlayerFormation()));
        }
        return result;
    }
}
