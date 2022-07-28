using System.Collections.Generic;

[System.Serializable]
public class PlayerClearStage : BasePlayerData, IPlayerClearStage
{
    public static readonly Dictionary<string, PlayerClearStage> DataMap = new Dictionary<string, PlayerClearStage>();
    public string Id { get { return GetId(PlayerId, DataId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int bestRating;
    public int BestRating { get { return bestRating; } set { bestRating = value; } }

    public PlayerClearStage Clone()
    {
        return CloneTo(this, new PlayerClearStage());
    }

    public static T CloneTo<T>(IPlayerClearStage from, T to) where T : IPlayerClearStage
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.BestRating = from.BestRating;
        return to;
    }

    public static string GetId(string playerId, string dataId)
    {
        return playerId + "_" + dataId;
    }

    public static void SetData(PlayerClearStage data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, out PlayerClearStage data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId), out data);
    }

    public static bool TryGetData(string dataId, out PlayerClearStage data)
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

    public static void SetDataRange(IEnumerable<PlayerClearStage> list)
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

    public static bool IsUnlock(string playerId, BaseStage checkStage)
    {
        var unlockStages = GameDatabase.unlockStages;
        foreach (var unlockStage in unlockStages)
        {
            if (unlockStage.Id == checkStage.Id)
                return true;
        }
        var stages = GameDatabase.Stages.Values;
        foreach (var stage in stages)
        {
            if (!DataMap.ContainsKey(GetId(playerId, stage.Id)))
                continue;

            var stageUnlockStages = new List<BaseStage>(stage.unlockStages);
            foreach (var unlockStage in stageUnlockStages)
            {
                if (unlockStage.Id == checkStage.Id)
                    return true;
            }
        }
        return false;
    }

    public static bool IsUnlock(BaseStage checkStage)
    {
        return IsUnlock(Player.CurrentPlayerId, checkStage);
    }
}
