using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIapPackage : UIDataItem<IapPackage>
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
	public Image imageHighlight;
    public Text textPrice;

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(IapPackage data)
    {
        if (textTitle != null)
            textTitle.text = data == null ? "" : data.Title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.icon;

		if (imageHighlight != null)
			imageHighlight.sprite = data.highlight;

        if (textPrice != null)
            textPrice.text = data == null ? "N/A" : data.GetSellPrice();
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickOpen()
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        if (!gameInstance.gameDatabase.IapPackages.ContainsKey(data.Id))
            return;
        
        GameInstance.PurchaseCallback = ResponsePurchase;
        GameInstance.Singleton.Purchase(data.Id);
    }

    private void ResponsePurchase(bool success, string errorMessage)
    {
        if (!success)
        {
            // TODO: show error message
            return;
        }
    }
}
