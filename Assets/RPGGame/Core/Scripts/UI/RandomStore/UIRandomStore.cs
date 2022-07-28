using UnityEngine;
using UnityEngine.UI;

public class UIRandomStore : UIDataItemList<UIRandomStoreItem, RandomStoreItem>
{
    public Text textEndsIn;
    public UICurrency uiRefreshCurrency;
    public RandomStore randomStore;
    public AnimItemsRewarding animItemsRewarding;
    public RandomStoreEvent StoreEvent { get; private set; }
    private float refreshCountDown;

    private void OnEnable()
    {
        GetRandomStore();
        if (uiRefreshCurrency != null)
        {
            uiRefreshCurrency.data = new PlayerCurrency()
            {
                DataId = randomStore.refreshCurrencyId,
                Amount = randomStore.refreshCurrencyAmount,
            };
            uiRefreshCurrency.Show();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (refreshCountDown > 0)
            refreshCountDown -= Time.unscaledDeltaTime;
        if (refreshCountDown < 0)
            refreshCountDown = 0;
        if (textEndsIn != null)
        {
            textEndsIn.text = refreshCountDown.ToString("N0");
        }
    }

    private void GetRandomStore()
    {
        ClearListItems();
        GameInstance.GameService.GetRandomStore(randomStore.Id, OnGetRandomStoreSuccess, OnGetRandomStoreFail);
    }

    private void OnGetRandomStoreSuccess(RandomStoreResult result)
    {
        StoreEvent = result.store;
        refreshCountDown = result.endsIn;
        for (var i = 0; i < result.store.RandomedItems.Count; ++i)
        {
            var entry = result.store.RandomedItems[i];
            var ui = SetListItem(i.ToString());
            ui.data = entry;
            ui.uiRandomStore = this;
            ui.index = i;
            ui.Show();
        }
    }

    private void OnGetRandomStoreFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, GetRandomStore);
    }

    private void RefreshRandomStore()
    {
        ClearListItems();
        GameInstance.GameService.RefreshRandomStore(randomStore.Id, OnRefreshRandomStoreSuccess, OnRefreshRandomStoreFail);
    }

    private void OnRefreshRandomStoreSuccess(RefreshRandomStoreResult result)
    {
        OnGetRandomStoreSuccess(result);
        PlayerCurrency.SetDataRange(result.updateCurrencies);
    }

    private void OnRefreshRandomStoreFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, RefreshRandomStore);
    }

    public void OnClickRefresh()
    {
        RefreshRandomStore();
    }
}
