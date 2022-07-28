using System.Collections.Generic;

[System.Serializable]
public class PlayerBattle : BasePlayerData, IPlayerBattle
{
    public static readonly Dictionary<string, PlayerBattle> DataMap = new Dictionary<string, PlayerBattle>();
    public string id;
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

    public PlayerBattle Clone()
    {
        return CloneTo(this, new PlayerBattle());
    }

    public static T CloneTo<T>(IPlayerBattle from, T to) where T : IPlayerBattle
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Session = from.Session;
        to.BattleResult = from.BattleResult;
        to.Rating = from.Rating;
        to.BattleType = from.BattleType;
        return to;
    }

    public static void SetData(PlayerBattle data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerBattle> list)
    {
        foreach (var data in list)
        {
            SetData(data);
        }
    }

    public static void RemoveDataRange(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            RemoveData(id);
        }
    }

    public static void RemoveDataRange(string playerId)
    {
        var values = DataMap.Values;
        foreach (var value in values)
        {
            if (value.PlayerId == playerId)
                RemoveData(value.Id);
        }
    }

    public static void RemoveDataRange()
    {
        RemoveDataRange(Player.CurrentPlayerId);
    }
}
