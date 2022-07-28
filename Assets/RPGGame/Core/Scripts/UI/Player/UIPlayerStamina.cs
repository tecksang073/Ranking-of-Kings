public class UIPlayerStamina : UIStamina
{
    public string staminaId;

    protected override void Update()
    {
        PlayerStamina data;
        if (!PlayerStamina.TryGetData(staminaId, out data))
        {
            data = new PlayerStamina()
            {
                PlayerId = Player.CurrentPlayerId,
                DataId = staminaId,
                Amount = 0,
            };
        }
        SetData(data);
        isCurrentPlayerStamina = true;
        base.Update();
    }
}
