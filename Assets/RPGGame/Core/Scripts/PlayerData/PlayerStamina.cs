using System.Collections.Generic;

[System.Serializable]
public class PlayerStamina : BasePlayerData, IPlayerStamina
{
    public static readonly Dictionary<string, PlayerStamina> DataMap = new Dictionary<string, PlayerStamina>();
    public static PlayerStamina StageStamina
    {
        get
        {
            PlayerStamina result;
            TryGetData(GameDatabase.stageStamina.id, out result);
            return result;
        }
    }
    public static PlayerStamina ArenaStamina
    {
        get
        {
            PlayerStamina result;
            TryGetData(GameDatabase.arenaStamina.id, out result);
            return result;
        }
    }
    public static PlayerStamina GetStamina(string id)
    {
        PlayerStamina result;
        TryGetData(id, out result);
        return result;
    }
    public static bool HasStamina(string id)
    {
        PlayerStamina result;
        return TryGetData(id, out result);
    }
    public string Id { get { return GetId(PlayerId, DataId); } set { } }
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

    public PlayerStamina Clone()
    {
        return CloneTo(this, new PlayerStamina());
    }

    public static T CloneTo<T>(IPlayerStamina from, T to) where T : IPlayerStamina
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Amount = from.Amount;
        to.RecoveredTime = from.RecoveredTime;
        to.RefillCount = from.RefillCount;
        to.LastRefillTime = from.LastRefillTime;
        return to;
    }

    public PlayerStamina SetAmount(int amount, long recoveredTime)
    {
        Amount = amount;
        RecoveredTime = recoveredTime;
        return this;
    }

    #region Non Serialize Fields
    public Stamina StaminaData
    {
        get
        {
            Stamina stamina;
            if (GameDatabase != null && !string.IsNullOrEmpty(DataId) && GameDatabase.Staminas.TryGetValue(DataId, out stamina))
                return stamina;
            return null;
        }
    }

    public long RecoveryingTime
    {
        get { return 0; }
    }
    #endregion

    public static bool HaveEnoughStageStamina(int amount)
    {
        var data = StageStamina;
        if (data != null)
            return data.Amount >= amount;
        return false;
    }

    public static bool HaveEnoughArenaStamina(int amount)
    {
        var data = ArenaStamina;
        if (data != null)
            return data.Amount >= amount;
        return false;
    }

    public static string GetId(string playerId, string dataId)
    {
        return playerId + "_" + dataId;
    }

    public static void SetData(PlayerStamina data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, out PlayerStamina data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId), out data);
    }

    public static bool TryGetData(string dataId, out PlayerStamina data)
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

    public static void SetDataRange(IEnumerable<PlayerStamina> list)
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
