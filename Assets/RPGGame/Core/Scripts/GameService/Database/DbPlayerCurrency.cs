using System.Collections.Generic;

[System.Serializable]
public class DbPlayerCurrency : IPlayerCurrency
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
    public int purchasedAmount;
    public int PurchasedAmount { get { return purchasedAmount; } set { purchasedAmount = value; } }

    public static List<PlayerCurrency> CloneList(IEnumerable<DbPlayerCurrency> list)
    {
        var result = new List<PlayerCurrency>();
        foreach (var entry in list)
        {
            result.Add(PlayerCurrency.CloneTo(entry, new PlayerCurrency()));
        }
        return result;
    }
}
