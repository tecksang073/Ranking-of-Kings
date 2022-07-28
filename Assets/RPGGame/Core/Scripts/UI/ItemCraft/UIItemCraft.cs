using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemCraft : UIDataItem<ItemCraftFormula>
{
    public Text textTitle;
    public Text textDescription;
    public UIItem uiResultItem;
    public Text textAmount;
    public UICurrency uiPrice;
    public UIItemList uiMaterials;
    public Button buttonCraft;
    public UIItemCraftManager uiItemCraftManager;
    // Private
    private readonly Dictionary<string, int> countMaterials = new Dictionary<string, int>();
    private readonly Dictionary<string, int> selectedMaterials = new Dictionary<string, int>();

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(ItemCraftFormula data)
    {
        countMaterials.Clear();
        selectedMaterials.Clear();

        bool haveEnoughMaterials = false;
        if (data != null && data.materials != null && data.materials.Length > 0)
        {
            haveEnoughMaterials = true;
            for (var i = 0; i < data.materials.Length; ++i)
            {
                var material = data.materials[i];
                var count = 0;
                var values = PlayerItem.DataMap.Values;
                foreach (var value in values)
                {
                    if (value.PlayerId == Player.CurrentPlayerId && value.DataId == material.Id)
                    {
                        if (count < material.amount)
                            selectedMaterials[value.Id] = value.Amount;
                        count += value.Amount;
                    }
                }
                if (haveEnoughMaterials)
                    haveEnoughMaterials = count >= material.amount;
                countMaterials[material.Id] = count;
            }
        }

        if (textTitle != null)
            textTitle.text = data == null ? "" : data.Title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.Description;

        if (uiResultItem != null)
        {
            uiResultItem.data = data == null ? null : new PlayerItem()
            {
                DataId = data.resultItem.Id,
                Amount = data.resultItem.amount,
            };
        }

        if (textAmount != null)
            textAmount.text = data == null ? "0" : data.resultItem.amount.ToString("N0");

        if (uiPrice != null)
        {
            uiPrice.Clear();
            PlayerCurrency currencyData = null;
            if (data != null)
            {
                switch (data.requirementType)
                {
                    case CraftRequirementType.RequireSoftCurrency:
                        currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(data.price, 0);
                        break;
                    case CraftRequirementType.RequireHardCurrency:
                        currencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(data.price, 0);
                        break;
                }
            }
            uiPrice.SetData(currencyData);
        }

        if (uiMaterials != null)
        {
            uiMaterials.ClearListItems();
            if (data != null && data.materials != null && data.materials.Length > 0)
            {
                for (var i = 0; i < data.materials.Length; ++i)
                {
                    var material = data.materials[i];
                    var item = new PlayerItem()
                    {
                        Id = i.ToString("N0"),
                        DataId = material.Id,
                    };
                    var newUIMaterial = uiMaterials.SetListItem(item);
                    newUIMaterial.ForceUpdate();
                    newUIMaterial.SetupSelectedAmount(countMaterials[material.Id], material.amount);
                }
            }
        }

        if (buttonCraft != null)
            buttonCraft.interactable = haveEnoughMaterials;
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickCraft()
    {
        GameInstance.GameService.CraftItem(data.Id, selectedMaterials, OnClickCraftSuccess, OnClickCraftFail);
    }

    private void OnClickCraftSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        if (result.rewardItems.Count > 0)
        {
            var itemCraftList = list as UIItemCraftList;
            if (itemCraftList != null && itemCraftList.animItemsRewarding != null)
                itemCraftList.animItemsRewarding.Play(result.rewardItems);
            else
                GameInstance.Singleton.ShowRewardItemsDialog(result.rewardItems);
        }
        uiItemCraftManager.ReloadList();
    }

    private void OnClickCraftFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
