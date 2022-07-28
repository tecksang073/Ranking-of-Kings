using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICreateClan : UIBase
{
    public InputField inputClanName;
    public UICurrency uiRequireCurrency;
    public UIClanManager manager;

    private void OnEnable()
    {
        inputClanName.text = "";

        PlayerCurrency currencyData = null;
        switch (GameInstance.GameDatabase.createClanCurrencyType)
        {
            case CreateClanRequirementType.RequireSoftCurrency:
                currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(GameInstance.GameDatabase.createClanCurrencyAmount, 0);
                break;
            case CreateClanRequirementType.RequireHardCurrency:
                currencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(GameInstance.GameDatabase.createClanCurrencyAmount, 0);
                break;
        }
        uiRequireCurrency.SetData(currencyData);
    }

    public void OnClickCreate()
    {
        GameInstance.GameService.CreateClan(inputClanName.text, OnCreateClanSuccess, OnCreateClanFail);
    }

    private void OnCreateClanSuccess(CreateClanResult result)
    {
        GameInstance.Singleton.OnGameServiceCreateClanResult(result);
        manager.RefreshData();
    }

    private void OnCreateClanFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
