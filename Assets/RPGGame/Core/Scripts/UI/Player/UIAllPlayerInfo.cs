public class UIAllPlayerInfo : UIBase
{
    public UIPlayer uiPlayer;
    public UICurrency uiSoftCurrency;
    public UICurrency uiHardCurrency;
    public UIStamina uiStageStamina;
    public UIStamina uiArenaStamina;
    
	void Update ()
    {
        if (uiPlayer != null)
            uiPlayer.SetData(Player.CurrentPlayer);
        if (uiSoftCurrency != null)
            uiSoftCurrency.SetData(PlayerCurrency.SoftCurrency);
        if (uiHardCurrency != null)
            uiHardCurrency.SetData(PlayerCurrency.HardCurrency);
        if (uiStageStamina != null)
        {
            uiStageStamina.SetData(PlayerStamina.StageStamina);
            uiStageStamina.isCurrentPlayerStamina = true;
        }
        if (uiArenaStamina != null)
        {
            uiArenaStamina.SetData(PlayerStamina.ArenaStamina);
            uiArenaStamina.isCurrentPlayerStamina = true;
        }
    }
}
