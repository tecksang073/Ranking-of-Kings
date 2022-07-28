using UnityEngine.UI;

public class UIStamina : UIDataItem<PlayerStamina>
{
    public Image imageIcon;
    public Text textAmount;
    public Text recoveryingTime;
    public bool isCurrentPlayerStamina;

    private GameDatabase gameDatabase { get { return GameInstance.GameDatabase; } }
    private int tempMaxStamina;
    public override void UpdateData()
    {
        SetupInfo(data);
    }

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(PlayerStamina data)
    {
        if (data == null)
            data = new PlayerStamina();

        var staminaData = data.StaminaData;

        if (imageIcon != null)
            imageIcon.sprite = staminaData == null ? null : staminaData.icon;

        if (textAmount != null)
            textAmount.text = data.Amount.ToString("N0");
    }

    protected override void Update()
    {
        base.Update();

        if (Player.CurrentPlayer == null)
            return;

        if (isCurrentPlayerStamina)
        {
            tempMaxStamina = 0;
            Stamina staminaTable = null;
            if (gameDatabase != null && data != null && gameDatabase.Staminas.TryGetValue(data.dataId, out staminaTable))
            {
                tempMaxStamina = staminaTable.maxAmountTable.Calculate(Player.CurrentPlayer.Level, gameDatabase.playerMaxLevel);
                if (data.Amount < tempMaxStamina)
                {
                    var currentTimeInSeconds = GameInstance.GameService.Timestamp + GameInstance.GameService.ServiceTimeOffset;
                    var diffTimeInSeconds = currentTimeInSeconds - data.RecoveredTime;

                    var devideAmount = 1;
                    switch (staminaTable.recoverUnit)
                    {
                        case StaminaUnit.Days:
                            devideAmount = 60 * 60 * 24;
                            break;
                        case StaminaUnit.Hours:
                            devideAmount = 60 * 60;
                            break;
                        case StaminaUnit.Minutes:
                            devideAmount = 60;
                            break;
                        case StaminaUnit.Seconds:
                            devideAmount = 1;
                            break;
                    }
                    var countDownInSeconds = (staminaTable.recoverDuration * devideAmount) - diffTimeInSeconds;
                    var recoveryAmount = (int)(diffTimeInSeconds / devideAmount) / staminaTable.recoverDuration;
                    if (recoveryAmount > 0)
                    {
                        data.Amount += recoveryAmount;
                        if (data.Amount > tempMaxStamina)
                            data.Amount = tempMaxStamina;
                        data.RecoveredTime = currentTimeInSeconds;

                        if (textAmount != null)
                            textAmount.text = data.Amount.ToString("N0");
                    }

                    if (recoveryingTime != null)
                    {
                        System.TimeSpan time = System.TimeSpan.FromSeconds(countDownInSeconds);
                        recoveryingTime.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                        recoveryingTime.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (recoveryingTime != null)
                        recoveryingTime.gameObject.SetActive(false);
                }
            }
            else
            {
                if (recoveryingTime != null)
                    recoveryingTime.gameObject.SetActive(false);
            }
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.DataId);
    }
}

[System.Serializable]
public struct UIStaninaPairing
{
    public string id;
    public UIStamina ui;
}