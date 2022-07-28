using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class UIRandomStoreItem : UIDataItem<RandomStoreItem>
{
    public UIItem uiItem;
    public UICurrency uiRequiredCurrency;
    public UIRandomStore uiRandomStore;
    public GameObject[] purchasedObjects;
    public GameObject[] unpurchasedObjects;
    public int index;

    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        return string.IsNullOrEmpty(data.Id);
    }

    protected override void Update()
    {
        base.Update();
        if (purchasedObjects != null && purchasedObjects.Length > 0)
        {
            for (int i = 0; i < purchasedObjects.Length; ++i)
            {
                purchasedObjects[i].SetActive(uiRandomStore.StoreEvent.PurchaseItems.Contains(index));
            }
        }
        if (unpurchasedObjects != null && unpurchasedObjects.Length > 0)
        {
            for (int i = 0; i < unpurchasedObjects.Length; ++i)
            {
                unpurchasedObjects[i].SetActive(!uiRandomStore.StoreEvent.PurchaseItems.Contains(index));
            }
        }
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(RandomStoreItem data)
    {
        if (uiItem != null)
        {
            uiItem.data = new PlayerItem()
            {
                Id = data.Id,
                DataId = data.Id,
                Amount = data.Amount,
            };
            uiItem.Show();
        }

        if (uiRequiredCurrency != null)
        {
            uiRequiredCurrency.data = new PlayerCurrency()
            {
                DataId = data.requireCurrencyId,
                Amount = data.requireCurrencyAmount,
            };
            uiRequiredCurrency.Show();
        }
    }

    public void OnClickPurchase()
    {
        GameInstance.GameService.PurchaseRandomStoreItem(uiRandomStore.StoreEvent.DataId, index, OnPurchaseRandomStoreItemSuccess, OnPurchaseRandomStoreItemFail);
    }

    private void OnPurchaseRandomStoreItemSuccess(PurchaseRandomStoreItemResult result)
    {
        if (!uiRandomStore.StoreEvent.PurchaseItems.Contains(index))
            uiRandomStore.StoreEvent.PurchaseItems.Add(index);
        GameInstance.Singleton.OnGameServiceItemResult(result);
        if (uiRandomStore != null && uiRandomStore.animItemsRewarding != null)
            uiRandomStore.animItemsRewarding.Play(result.rewardItems);
        else
            GameInstance.Singleton.ShowRewardItemsDialog(result.rewardItems);
    }

    private void OnPurchaseRandomStoreItemFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, OnClickPurchase);
    }
}
