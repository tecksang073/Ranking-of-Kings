using System.Collections.Generic;

[System.Serializable]
public class PlayerCurrency : BasePlayerData, IPlayerCurrency
{
    public static readonly Dictionary<string, PlayerCurrency> DataMap = new Dictionary<string, PlayerCurrency>();
    public static PlayerCurrency SoftCurrency
    {
        get
        {
            PlayerCurrency result = null;
            if (GameDatabase != null)
                TryGetData(GameDatabase.softCurrency.id, out result);
            return result;
        }
    }
    public static PlayerCurrency HardCurrency
    {
        get
        {
            PlayerCurrency result = null;
            if (GameDatabase != null)
                TryGetData(GameDatabase.hardCurrency.id, out result);
            return result;
        }
    }
    public string Id { get { return GetId(PlayerId, DataId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    public int purchasedAmount;
    public int PurchasedAmount { get { return purchasedAmount; } set { purchasedAmount = value; } }

    public PlayerCurrency Clone()
    {
        return CloneTo(this, new PlayerCurrency());
    }

    public static T CloneTo<T>(IPlayerCurrency from, T to) where T : IPlayerCurrency
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Amount = from.Amount;
        to.PurchasedAmount = from.PurchasedAmount;
        return to;
    }

    public PlayerCurrency SetAmount(int amount, int purchasedAmount)
    {
        Amount = amount;
        PurchasedAmount = purchasedAmount;
        return this;
    }

    #region Non Serialize Fields
    public Currency CurrencyData
    {
        get
        {
            Currency currency = default(Currency);
            if (GameDatabase != null && !string.IsNullOrEmpty(DataId) && GameDatabase.Currencies.TryGetValue(DataId, out currency))
                return currency;
            return currency;
        }
    }
    #endregion

    public static bool HaveEnoughSoftCurrency(int amount)
    {
        var data = SoftCurrency;
        if (data != null)
            return data.Amount >= amount;
        return false;
    }

    public static bool HaveEnoughHardCurrency(int amount)
    {
        var data = HardCurrency;
        if (data != null)
            return data.Amount >= amount;
        return false;
    }

    public static string GetId(string playerId, string dataId)
    {
        return playerId + "_" + dataId;
    }

    public static void SetData(PlayerCurrency data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, out PlayerCurrency data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId), out data);
    }

    public static bool TryGetData(string dataId, out PlayerCurrency data)
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

    public static void SetDataRange(IEnumerable<PlayerCurrency> list)
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
