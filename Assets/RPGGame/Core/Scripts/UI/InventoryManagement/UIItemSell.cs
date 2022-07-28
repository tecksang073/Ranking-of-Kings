using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class UIItemSell : UIItemSelection
{
    public UICurrency uiCurrency;
    public UIItemListFilterSetting filterSetting;
    // Events
    public UnityEvent eventSellSuccess;
    public UnityEvent eventSellFail;

    // Private
    private int totalSellPrice;

    public override void Show()
    {
        base.Show();

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
        SetSelectingItemIds();
    }

    public override void Hide()
    {
        base.Hide();

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
    }

    protected override List<PlayerItem> GetAvailableItemList()
    {
        var list = PlayerItem.DataMap.Values.Where(a => UIItemListFilter.Filter(a, filterSetting) && a.CanSell).ToList();
        list.SortSellPrice();
        return list;
    }

    protected override void OnSetListItem(UIItem ui)
    {
        base.OnSetListItem(ui);
        ui.displayStats = UIItem.DisplayStats.SellPrice;
    }

    protected override void SelectItem(UIDataItem ui)
    {
        if ((ui as UIItem).data.CanSell)
            base.SelectItem(ui);
        else
            ui.Selected = false;
        Calculate();
    }

    protected override void DeselectItem(UIDataItem ui)
    {
        base.DeselectItem(ui);
        Calculate();
    }

    public void Calculate()
    {
        var selectedItem = GetSelectedItemAmountPair();
        totalSellPrice = 0;
        foreach (var entry in selectedItem)
        {
            totalSellPrice += entry.Value * entry.Key.SellPrice;
        }

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(totalSellPrice, 0);
            uiCurrency.SetData(currencyData);
        }
    }

    public void SetSelectingItemIds()
    {
        var availableItems = GetAvailableItems();
        if (UIGlobalData.SelectingItemIds.Count > 0 && availableItems.Count > 0)
        {
            foreach (var selectingItemId in UIGlobalData.SelectingItemIds)
            {
                if (availableItems.ContainsKey(selectingItemId))
                    availableItems[selectingItemId].Select();
            }
        }
    }

    public void OnClickSell()
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        var idAmountPair = GetSelectedItemIdAmountPair();
        gameService.SellItems(idAmountPair, OnSellSuccess, OnSellFail);
    }

    private void OnSellSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        eventSellSuccess.Invoke();
        if (uiSelectedItemList != null)
            uiSelectedItemList.ClearListItems();
        var items = GetAvailableItems();
        var updateItem = result.updateItems;
        foreach (var entry in updateItem)
        {
            var id = entry.Id;
            if (items.ContainsKey(id))
                items[id].SetData(entry);
        }
        var deleteItemIds = result.deleteItemIds;
        foreach (var deleteItemId in deleteItemIds)
        {
            if (uiAvailableItemList != null)
                uiAvailableItemList.RemoveListItem(deleteItemId);
        }
        var updateCurrencies = result.updateCurrencies;
        foreach (var updateCurrency in updateCurrencies)
        {
            PlayerCurrency.SetData(updateCurrency);
        }
        Calculate();
    }

    private void OnSellFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        eventSellFail.Invoke();
    }

    protected override void OnUpdateItemAmount(PlayerItem item, int amount)
    {
        Calculate();
    }
}
