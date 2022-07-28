using System.Collections.Generic;

[System.Serializable]
public class DbPlayerItem : IPlayerItem
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
    public int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    public string equipItemId;
    public string EquipItemId { get { return equipItemId; } set { equipItemId = value; } }
    public string equipPosition;
    public string EquipPosition { get { return equipPosition; } set { equipPosition = value; } }
    public CalculatedAttributes randomedAttributes;
    public CalculatedAttributes RandomedAttributes { get { return randomedAttributes; } set { randomedAttributes = value; } }

    public static List<PlayerItem> CloneList(IEnumerable<DbPlayerItem> list)
    {
        var result = new List<PlayerItem>();
        foreach (var entry in list)
        {
            result.Add(PlayerItem.CloneTo(entry, new PlayerItem()));
        }
        return result;
    }
}
