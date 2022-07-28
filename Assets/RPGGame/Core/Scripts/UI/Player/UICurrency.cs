using UnityEngine.UI;

public class UICurrency : UIDataItem<PlayerCurrency>
{
    public Image imageIcon;
    public Text textAmount;
    public override void UpdateData()
    {
        SetupInfo(data);
    }

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(PlayerCurrency data)
    {
        if (data == null)
            data = new PlayerCurrency();

        var currencyData = data.CurrencyData;

        if (imageIcon != null)
            imageIcon.sprite = currencyData.icon;

        if (textAmount != null)
            textAmount.text = data.Amount.ToString("N0");
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.DataId);
    }
}

[System.Serializable]
public struct UICurrencyPairing
{
    public string id;
    public UICurrency ui;
}