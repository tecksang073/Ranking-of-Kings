using UnityEngine.Events;

public partial class SQLiteGameService
{
    protected override void DoGetRandomStore(string playerId, string loginToken, string id, UnityAction<RandomStoreResult> onFinish)
    {
        var result = new RandomStoreResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoPurchaseRandomStoreItem(string playerId, string loginToken, string id, int index, UnityAction<PurchaseRandomStoreItemResult> onFinish)
    {
        var result = new PurchaseRandomStoreItemResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoRefreshRandomStore(string playerId, string loginToken, string id, UnityAction<RefreshRandomStoreResult> onFinish)
    {
        var result = new RefreshRandomStoreResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }
}
