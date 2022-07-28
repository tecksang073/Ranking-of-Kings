using System.Collections.Generic;

[System.Serializable]
public partial class RandomStoreEvent : IRandomStoreEvent
{
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public List<RandomStoreItem> randomedItems = new List<RandomStoreItem>();
    public List<RandomStoreItem> RandomedItems { get { return randomedItems; } set { randomedItems = value; } }
    public List<int> purchaseItems = new List<int>();
    public List<int> PurchaseItems { get { return purchaseItems; } set { purchaseItems = value; } }

    public static GameDatabase GameDatabase
    {
        get { return GameInstance.GameDatabase; }
    }

    public RandomStore RandomStore
    {
        get
        {
            if (GameDatabase != null && GameDatabase.RandomStores.ContainsKey(DataId))
                return GameDatabase.RandomStores[DataId];
            return null;
        }
    }

    public RandomStoreEvent Clone()
    {
        return CloneTo(this, new RandomStoreEvent());
    }

    public static T CloneTo<T>(IRandomStoreEvent from, T to) where T : IRandomStoreEvent
    {
        to.DataId = from.DataId;
        to.RandomedItems = from.RandomedItems;
        to.PurchaseItems = from.PurchaseItems;
        return to;
    }
}
