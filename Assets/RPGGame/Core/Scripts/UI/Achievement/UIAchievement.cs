using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievement : UIDataItem<PlayerAchievement>
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
    public Text textProgress;
    public Image imageProgressGage;
    public Button buttonEarn;
    public UIAchievementManager uiAchievementManager;

    protected override void Update()
    {
        base.Update();

        if (textProgress != null)
            textProgress.text = data.Progress.ToString("N0") + "/" + data.Achievement.targetAmount.ToString("N0");

        if (imageProgressGage != null)
            imageProgressGage.fillAmount = (float)data.Progress / (float)data.Achievement.targetAmount;

        if (buttonEarn != null)
            buttonEarn.interactable = data.Progress >= data.Achievement.targetAmount && !data.Earned;
    }

    public override void Clear()
    {
        if (textTitle != null)
            textTitle.text = string.Empty;

        if (textDescription != null)
            textDescription.text = string.Empty;

        if (imageIcon != null)
            imageIcon.sprite = null;
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.DataId);
    }

    public override void UpdateData()
    {
        if (textTitle != null)
            textTitle.text = data == null ? "" : data.Achievement.Title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.Achievement.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.Achievement.icon;
    }

    public void OnClickEarn()
    {
        GameInstance.GameService.EarnAchievementReward(data.DataId, OnClickEarnSuccess, OnClickEarnFail);
    }

    private void OnClickEarnSuccess(EarnAchievementResult result)
    {
        GameInstance.Singleton.OnGameServiceEarnAchievementResult(result);
        data.Earned = true;
        PlayerAchievement.SetData(data);
        uiAchievementManager.ReloadList();
    }

    private void OnClickEarnFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
