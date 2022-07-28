using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILootBox : UIDataItem<LootBox>
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
	public Image imageHighlight;
    [FormerlySerializedAs("uiCurrencies")]
    public UICurrency[] uiPrices;

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(LootBox data)
    {
        if (textTitle != null)
            textTitle.text = data == null ? "" : data.Title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.icon;

		if (imageHighlight != null)
			imageHighlight.sprite = data.highlight;

        if (uiPrices != null && uiPrices.Length > 0)
        {
            for (var i = 0; i < uiPrices.Length; ++i)
            {
                var uiPrice = uiPrices[i];
                uiPrice.Clear();
                if (data != null)
                {
                    var packIndex = i;
                    if (packIndex > data.lootboxPacks.Length - 1)
                        packIndex = 0;
                    var price = data.lootboxPacks[packIndex].price;
                    PlayerCurrency currencyData = null;
                    switch (data.requirementType)
                    {
                        case LootBoxRequirementType.RequireSoftCurrency:
                            currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(price, 0);
                            break;
                        case LootBoxRequirementType.RequireHardCurrency:
                            currencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(price, 0);
                            break;
                    }
                    uiPrice.SetData(currencyData);
                }
            }
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickOpen(int packIndex)
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        if (packIndex > data.lootboxPacks.Length - 1)
            packIndex = 0;
        var price = data.lootboxPacks[packIndex].price;
        switch (data.requirementType)
        {
            case LootBoxRequirementType.RequireSoftCurrency:
                if (!PlayerCurrency.HaveEnoughSoftCurrency(price))
                {
                    gameInstance.WarnNotEnoughSoftCurrency();
                    return;
                }
                break;
            case LootBoxRequirementType.RequireHardCurrency:
                if (!PlayerCurrency.HaveEnoughHardCurrency(price))
                {
                    gameInstance.WarnNotEnoughHardCurrency();
                    return;
                }
                break;
        }
        gameService.OpenLootBox(data.Id, packIndex, OnOpenLootBoxSuccess, OnOpenLootBoxFail);
    }

    private void OnOpenLootBoxSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        if (result.rewardItems.Count > 0)
        {
            var lootBoxList = list as UILootBoxList;
            if (lootBoxList != null && lootBoxList.animItemsRewarding != null)
                lootBoxList.animItemsRewarding.Play(result.rewardItems);
            else
                GameInstance.Singleton.ShowRewardItemsDialog(result.rewardItems);
        }
    }

    private void OnOpenLootBoxFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
