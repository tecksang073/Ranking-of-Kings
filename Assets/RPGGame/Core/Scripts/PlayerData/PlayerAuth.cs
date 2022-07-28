using System.Collections.Generic;

[System.Serializable]
public class PlayerAuth : BasePlayerData, IPlayerAuth
{
    public static readonly Dictionary<string, PlayerAuth> DataMap = new Dictionary<string, PlayerAuth>();
    public string Id { get { return GetId(PlayerId, Type); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string type;
    public string Type { get { return type; } set { type = value; } }
    public string username;
    public string Username { get { return username; } set { username = value; } }
    public string password;
    public string Password { get { return password; } set { password = value; } }

    public PlayerAuth Clone()
    {
        return CloneTo(this, new PlayerAuth());
    }

    public static T CloneTo<T>(IPlayerAuth from, T to) where T : IPlayerAuth
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.Type = from.Type;
        to.Username = from.Username;
        to.Password = from.Password;
        return to;
    }

    public static string GetId(string playerId, string type)
    {
        return playerId + "_" + type;
    }

    public static void SetData(PlayerAuth data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string type, out PlayerAuth data)
    {
        return DataMap.TryGetValue(GetId(playerId, type), out data);
    }

    public static bool TryGetData(string type, out PlayerAuth data)
    {
        return TryGetData(Player.CurrentPlayerId, type, out data);
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerAuth> list)
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
