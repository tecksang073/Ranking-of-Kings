using UnityEngine;
using UnityEngine.UI;

public class UIArenaResult : UIDataItem<FinishDuelResult>
{
    public const string ANIM_KEY_BATTLE_RATING = "Rating";
    public Animator ratingAnimator;
    public UIPlayer uiPlayer;
    public UIItemList uiRewardItems;
    public UICurrency uiRewardSoftCurrency;
    public UICurrency uiRewardHardCurrency;
    public Text textUpdateScore;
    public Button buttonGoToManageScene;

    public override void Show()
    {
        base.Show();
        if (buttonGoToManageScene != null)
        {
            buttonGoToManageScene.onClick.RemoveListener(OnClickGoToManageScene);
            buttonGoToManageScene.onClick.AddListener(OnClickGoToManageScene);
        }

        if (ratingAnimator != null)
            ratingAnimator.SetInteger(ANIM_KEY_BATTLE_RATING, data.rating);
    }

    public override void Clear()
    {
        if (uiPlayer != null)
            uiPlayer.Clear();

        if (uiRewardItems != null)
            uiRewardItems.ClearListItems();

        if (uiRewardSoftCurrency != null)
        {
            var softCurrencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiRewardSoftCurrency.SetData(softCurrencyData);
        }

        if (uiRewardHardCurrency != null)
        {
            var hardCurrencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(0, 0);
            uiRewardHardCurrency.SetData(hardCurrencyData);
        }

        if (textUpdateScore != null)
            textUpdateScore.text = "+0";
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (uiPlayer != null)
            uiPlayer.SetData(data.player);
			
        if (uiRewardItems != null)
        {
            uiRewardItems.selectionMode = UIDataItemSelectionMode.Disable;
            uiRewardItems.SetListItems(data.rewardItems);
        }

        if (uiRewardSoftCurrency != null)
        {
            var softCurrencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(data.rewardSoftCurrency, 0);
            uiRewardSoftCurrency.SetData(softCurrencyData);
        }

        if (uiRewardHardCurrency != null)
        {
            var hardCurrencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(data.rewardHardCurrency, 0);
            uiRewardHardCurrency.SetData(hardCurrencyData);
        }

        if (textUpdateScore != null)
        {
            var sign = data.updateScore >= 0 ? "+" : "";
            textUpdateScore.text = sign + data.updateScore.ToString("N0");
        }
    }

    public void OnClickGoToManageScene()
    {
        GameInstance.Singleton.LoadManageScene();
    }
}
