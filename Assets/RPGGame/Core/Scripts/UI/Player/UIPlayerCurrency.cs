public class UIPlayerCurrency : UICurrency
{
    public string currencyId;

    protected override void Update()
    {
        PlayerCurrency data;
        if (!PlayerCurrency.TryGetData(currencyId, out data))
        {
            data = new PlayerCurrency()
            {
                PlayerId = Player.CurrentPlayerId,
                DataId = currencyId,
                Amount = 0,
            };
        }
        SetData(data);
        base.Update();
    }
}
