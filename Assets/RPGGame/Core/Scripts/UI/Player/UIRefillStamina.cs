using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIRefillStamina : UIBase
{
    public enum StaminaType
    {
        Stage,
        Arena,
        Custom
    }

    public StaminaType type;
    public string customStaminaDataId;
    public UICurrency uiCurrency;
    public UIStamina uiStamina;
    public UnityEvent eventGetRefillInfoSuccess;
    public UnityEvent eventGetRefillInfoFail;
    public UnityEvent eventRefillSuccess;
    public UnityEvent eventRefillFail;

    public string StaminaDataId
    {
        get
        {
            string staminaDataId = customStaminaDataId;
            switch (type)
            {
                case StaminaType.Stage:
                    staminaDataId = GameInstance.GameDatabase.stageStamina.id;
                    break;
                case StaminaType.Arena:
                    staminaDataId = GameInstance.GameDatabase.arenaStamina.id;
                    break;
            }
            return staminaDataId;
        }
    }

    public override void Show()
    {
        base.Show();
        if (uiCurrency != null)
        {
            uiCurrency.data = PlayerCurrency.HardCurrency.Clone().SetAmount(0, 0);
        }
        if (uiStamina != null)
        {
            var staminaDataId = StaminaDataId;
            if (!string.IsNullOrEmpty(staminaDataId) && PlayerStamina.HasStamina(staminaDataId))
                uiStamina.data = PlayerStamina.GetStamina(staminaDataId).Clone().SetAmount(0, 0);
        }
        GameInstance.GameService.GetRefillStaminaInfo(StaminaDataId, OnGetRefillInfoSuccess, OnGetRefillInfoFail);
    }

    private void OnGetRefillInfoSuccess(RefillStaminaInfoResult result)
    {
        if (eventGetRefillInfoSuccess != null)
            eventGetRefillInfoSuccess.Invoke();
        if (uiCurrency != null)
        {
            uiCurrency.data = PlayerCurrency.HardCurrency.Clone().SetAmount(result.requireHardCurrency, 0);
        }
        if (uiStamina != null)
        {
            var staminaDataId = StaminaDataId;
            if (!string.IsNullOrEmpty(staminaDataId) && PlayerStamina.HasStamina(staminaDataId))
                uiStamina.data = PlayerStamina.GetStamina(staminaDataId).Clone().SetAmount(result.refillAmount, 0);
        }
    }

    private void OnGetRefillInfoFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventGetRefillInfoFail != null)
            eventGetRefillInfoFail.Invoke();
    }

    public void OnClickRefill()
    {
        GameInstance.GameService.RefillStamina(StaminaDataId, OnRefillSuccess, OnRefillFail);
    }

    private void OnRefillSuccess(RefillStaminaResult result)
    {
        GameInstance.Singleton.OnGameServiceRefillStaminaResult(result);
        if (eventRefillSuccess != null)
            eventRefillSuccess.Invoke();
    }

    private void OnRefillFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventRefillFail != null)
            eventRefillFail.Invoke();
    }
}
