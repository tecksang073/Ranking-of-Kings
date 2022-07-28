using System.Collections.Generic;

public partial interface IRandomStoreEvent
{
    string DataId { get; set; }
    List<RandomStoreItem> RandomedItems { get; set; }
    List<int> PurchaseItems { get; set; }
}
