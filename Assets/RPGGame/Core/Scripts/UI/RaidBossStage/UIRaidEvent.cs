using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class UIRaidEvent : UIDataItem<RaidEvent>
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
    public UIRaidBossRewardList rewardList;
    public UnityEvent eventSetRaidEventPreparation;

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

    private void SetupInfo(RaidEvent data)
    {
        if (textTitle != null)
            textTitle.text = data.RaidBossStage.Title;

        if (textDescription != null)
            textDescription.text = data.RaidBossStage.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.RaidBossStage.icon;

        if (textRecommendBattlePoint != null)
            textRecommendBattlePoint.text = data.RaidBossStage.recommendBattlePoint.ToString("N0");

        if (uiRequireStamina != null)
        {
            var staminaData = PlayerStamina.StageStamina.Clone();
            if (!string.IsNullOrEmpty(data.RaidBossStage.requireCustomStamina) && PlayerStamina.HasStamina(data.RaidBossStage.requireCustomStamina))
                staminaData = PlayerStamina.GetStamina(data.RaidBossStage.requireCustomStamina).Clone();
            staminaData.SetAmount(data.RaidBossStage.requireStamina, 0);
            uiRequireStamina.SetData(staminaData);
        }

        var maxHp = data.RaidBossStage.GetCharacter().Attributes.hp;
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
            rewardList.SetListItems(new List<RaidBossReward>(data.RaidBossStage.rewards));
        }
    }

    public void SetRaidEventPreparationData()
    {
        if (RaidEventPreparation != null)
        {
            RaidEventPreparation.data = data;
            eventSetRaidEventPreparation.Invoke();
        }
    }

    public void ShowRaidEventPreparation()
    {
        if (RaidEventPreparation != null)
        {
            var historyManager = FindObjectOfType<UIHistoryManager>();
            if (historyManager)
                historyManager.Next(RaidEventPreparation);
            else
                RaidEventPreparation.Show();
        }
    }

    public UIRaidEventPreparation uiRaidEventPreparation;
    public UIRaidEventPreparation RaidEventPreparation
    {
        get { return uiRaidEventPreparation; }
    }
}
