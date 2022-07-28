using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIGamePlayGeneric : UIBase
{
    public Toggle toggleAutoPlay;
    public Toggle toggleSpeedMultiply;
    private bool isAutoPlayDirty;
    private bool isSpeedMultiplyDirty;

    private BaseGamePlayManager manager;
    public BaseGamePlayManager Manager
    {
        get
        {
            if (manager == null)
                manager = FindObjectOfType<BaseGamePlayManager>();
            return manager;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (toggleAutoPlay != null)
        {
            toggleAutoPlay.onValueChanged.RemoveListener(OnToggleAutoPlay);
            toggleAutoPlay.onValueChanged.AddListener(OnToggleAutoPlay);
        }

        if (toggleSpeedMultiply != null)
        {
            toggleSpeedMultiply.onValueChanged.RemoveListener(OnToggleSpeedMultiply);
            toggleSpeedMultiply.onValueChanged.AddListener(OnToggleSpeedMultiply);
        }
    }

    protected virtual void Update()
    {
        if (Manager.IsAutoPlay != isAutoPlayDirty)
        {
            if (toggleAutoPlay != null)
                toggleAutoPlay.isOn = Manager.IsAutoPlay;
            isAutoPlayDirty = Manager.IsAutoPlay;
        }

        if (Manager.IsSpeedMultiply != isSpeedMultiplyDirty)
        {
            if (toggleSpeedMultiply != null)
                toggleSpeedMultiply.isOn = Manager.IsSpeedMultiply;
            isSpeedMultiplyDirty = Manager.IsSpeedMultiply;
        }
    }

    public void OnToggleAutoPlay(bool isOn)
    {
        Manager.IsAutoPlay = isOn;
    }

    public void OnToggleSpeedMultiply(bool isOn)
    {
        Manager.IsSpeedMultiply = isOn;
    }
}
