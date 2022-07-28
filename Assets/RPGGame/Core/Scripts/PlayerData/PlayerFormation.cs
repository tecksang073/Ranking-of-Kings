using System.Collections.Generic;

[System.Serializable]
public class PlayerFormation : BasePlayerData, IPlayerFormation
{
    public static readonly Dictionary<string, PlayerFormation> DataMap = new Dictionary<string, PlayerFormation>();
    public string Id { get { return GetId(PlayerId, DataId, Position); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int position;
    public int Position { get { return position; } set { position = value; } }
    public string itemId;
    public string ItemId { get { return itemId; } set { itemId = value; } }

    #region Non Serialize Fields
    public Formation FormationData
    {
        get
        {
            Formation formation;
            if (GameDatabase != null && !string.IsNullOrEmpty(DataId) && GameDatabase.Formations.TryGetValue(DataId, out formation))
                return formation;
            return null;
        }
    }
    #endregion

    public PlayerFormation Clone()
    {
        return CloneTo(this, new PlayerFormation());
    }

    public static T CloneTo<T>(IPlayerFormation from, T to) where T : IPlayerFormation
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Position = from.Position;
        to.ItemId = from.ItemId;
        return to;
    }

    public static string GetId(string playerId, string dataId, int position)
    {
        return playerId + "_" + dataId + "_" + position;
    }

    public static void SetData(PlayerFormation data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, int position, out PlayerFormation data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId, position), out data);
    }

    public static bool TryGetData(string dataId, int position, out PlayerFormation data)
    {
        return TryGetData(Player.CurrentPlayerId, dataId, position, out data);
    }

    public static List<PlayerFormation> GetList(string playerId, string dataId)
    {
        List<PlayerFormation> result = new List<PlayerFormation>();
        foreach (var playerFormation in DataMap.Values)
        {
            if (playerFormation.PlayerId.Equals(playerId) &&
                playerFormation.DataId.Equals(dataId))
                result.Add(playerFormation);
        }
        return result;
    }

    public static List<PlayerFormation> GetList(string dataId)
    {
        return GetList(Player.CurrentPlayerId, dataId);
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerFormation> list)
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

    public static bool ContainsDataWithItemId(string itemId)
    {
        var values = DataMap.Values;
        foreach (var value in values)
        {
            if (value.ItemId == itemId)
                return true;
        }
        return false;
    }
}
