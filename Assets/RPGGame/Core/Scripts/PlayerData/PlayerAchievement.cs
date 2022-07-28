using System.Collections.Generic;

[System.Serializable]
public class PlayerAchievement : BasePlayerData, IPlayerAchievement
{
    public static readonly Dictionary<string, PlayerAchievement> DataMap = new Dictionary<string, PlayerAchievement>();
    public string Id { get { return GetId(PlayerId, DataId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int progress;
    public int Progress { get { return progress; } set { progress = value; } }
    public bool earned;
    public bool Earned { get { return earned; } set { earned = value; } }
    public Achievement Achievement
    {
        get
        {
            if (GameDatabase != null && GameDatabase.Achievements.ContainsKey(DataId))
                return GameDatabase.Achievements[DataId];
            return null;
        }
    }

    public PlayerAchievement Clone()
    {
        return CloneTo(this, new PlayerAchievement());
    }

    public static T CloneTo<T>(IPlayerAchievement from, T to) where T : IPlayerAchievement
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Progress = from.Progress;
        to.Earned = from.Earned;
        return to;
    }

    public static string GetId(string playerId, string dataId)
    {
        return playerId + "_" + dataId;
    }

    public static void SetData(PlayerAchievement data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, out PlayerAchievement data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId), out data);
    }

    public static bool TryGetData(string dataId, out PlayerAchievement data)
    {
        return TryGetData(Player.CurrentPlayerId, dataId, out data);
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerAchievement> list)
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
