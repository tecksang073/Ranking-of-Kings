using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class UIClanEvent : UIDataItem<ClanEvent>
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
    public Text textRecommendBattlePoint;
    public UIStamina uiRequireStamina;
    public Text textHpPerMaxHp;
    public Text textHpPercent;
    public Image imageHpGage;
    public Text textStartTime;
    public Text textEndTime;
    public UIClanBossRewardList rewardList;
    public UnityEvent eventSetClanEventPreparation;

    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(ClanEvent data)
    {
        if (textTitle != null)
            textTitle.text = data.ClanBossStage.Title;

        if (textDescription != null)
            textDescription.text = data.ClanBossStage.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.ClanBossStage.icon;

        if (textRecommendBattlePoint != null)
            textRecommendBattlePoint.text = data.ClanBossStage.recommendBattlePoint.ToString("N0");

        if (uiRequireStamina != null)
        {
            var staminaData = PlayerStamina.StageStamina.Clone();
            if (!string.IsNullOrEmpty(data.ClanBossStage.requireCustomStamina) && PlayerStamina.HasStamina(data.ClanBossStage.requireCustomStamina))
                staminaData = PlayerStamina.GetStamina(data.ClanBossStage.requireCustomStamina).Clone();
            staminaData.SetAmount(data.ClanBossStage.requireStamina, 0);
            uiRequireStamina.SetData(staminaData);
        }

        var maxHp = data.ClanBossStage.GetCharacter().Attributes.hp;
        var rate = (float)data.RemainingHp / (float)maxHp;

        if (textHpPerMaxHp != null)
            textHpPerMaxHp.text = data.RemainingHp.ToString("N0") + "/" + maxHp.ToString("N0");

        if (textHpPercent != null)
            textHpPercent.text = (rate * 100).ToString("N2") + "%";

        if (imageHpGage != null)
            imageHpGage.fillAmount = rate;

        if (textStartTime != null)
        {
            var d = new System.DateTime(1970, 1, 1);
            d = d.AddSeconds(data.StartTime);
            textStartTime.text = d.ToString();
        }

        if (textEndTime != null)
        {
            var d = new System.DateTime(1970, 1, 1);
            d = d.AddSeconds(data.EndTime);
            textEndTime.text = d.ToString();
        }

        if (rewardList != null)
        {
            rewardList.SetListItems(new List<ClanBossReward>(data.ClanBossStage.rewards));
        }
    }

    public void SetClanEventPreparationData()
    {
        if (ClanEventPreparation != null)
        {
            ClanEventPreparation.data = data;
            eventSetClanEventPreparation.Invoke();
        }
    }

    public void ShowClanEventPreparation()
    {
        if (ClanEventPreparation != null)
        {
            var historyManager = FindObjectOfType<UIHistoryManager>();
            if (historyManager)
                historyManager.Next(ClanEventPreparation);
            else
                ClanEventPreparation.Show();
        }
    }

    public UIClanEventPreparation uiClanEventPreparation;
    public UIClanEventPreparation ClanEventPreparation
    {
        get { return uiClanEventPreparation; }
    }
}
