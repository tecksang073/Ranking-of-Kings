public abstract class UIItemWithMaterials : UIItemSelection
{
    public UIItem uiBeforeInfo;
    public UIItem uiAfterInfo;
    public UICurrency uiCurrency;

    private PlayerItem dirtyItem;

    protected virtual void Update()
    {
        if (dirtyItem != UIGlobalData.SelectedItem)
        {
            dirtyItem = UIGlobalData.SelectedItem;

            if (uiBeforeInfo != null)
                uiBeforeInfo.SetData(dirtyItem);

            if (uiAfterInfo != null)
                uiAfterInfo.SetData(dirtyItem);
        }
    }

    public override void Show()
    {
        base.Show();

        if (uiBeforeInfo != null)
            uiBeforeInfo.SetData(UIGlobalData.SelectedItem);

        if (uiAfterInfo != null)
            uiAfterInfo.SetData(UIGlobalData.SelectedItem);

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
    }

    public override void Hide()
    {
        base.Hide();

        if (uiBeforeInfo != null)
            uiBeforeInfo.Clear();

        if (uiAfterInfo != null)
            uiAfterInfo.Clear();

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
    }
}
