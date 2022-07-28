using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIWin : UIDataItem<FinishStageResult>
{
    public const string ANIM_KEY_BATTLE_RATING = "Rating";
    public Animator ratingAnimator;
    public UIPlayer uiPlayer;
    [Header("Win Rewards")]
    public Text textRewardPlayerExp;
    public Text textRewardCharacterExp;
    public UIItemList uiIncreasedExpCharacters;
    public UIItemList uiRewardItems;
    public UICurrency uiRewardCurrency;
    [Header("First Win Rewards")]
    public UIBase firstClearPanelRoot;
    public Text textFirstClearRewardPlayerExp;
    public UIItemList uiFirstClearRewardItems;
    public UICurrency uiFirstClearRewardSoftCurrency;
    public UICurrency uiFirstClearRewardHardCurrency;
    [Header("Buttons")]
    public Button buttonRestart;
    public Button buttonGoToManageScene;
    public Button buttonGoToNextStage;

    public BaseStage NextStage
    {
        get
        {
            var unlockStages = BaseGamePlayManager.PlayingStage.unlockStages;
            if (unlockStages != null && unlockStages.Length > 0)
                return unlockStages[0];
            return null;
        }
    }

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

    public override void Show()
    {
        base.Show();
        if (buttonRestart != null)
        {
            buttonRestart.onClick.RemoveListener(OnClickRestart);
            buttonRestart.onClick.AddListener(OnClickRestart);
        }
        if (buttonGoToManageScene != null)
        {
            buttonGoToManageScene.onClick.RemoveListener(OnClickGoToManageScene);
            buttonGoToManageScene.onClick.AddListener(OnClickGoToManageScene);
        }
        if (buttonGoToNextStage != null)
        {
            buttonGoToNextStage.onClick.RemoveListener(OnClickGoToNextStage);
            buttonGoToNextStage.onClick.AddListener(OnClickGoToNextStage);
            buttonGoToNextStage.interactable = NextStage != null;
        }

        if (ratingAnimator != null)
            ratingAnimator.SetInteger(ANIM_KEY_BATTLE_RATING, data.rating);
    }

    public override void Clear()
    {
        if (uiPlayer != null)
            uiPlayer.Clear();

        if (textRewardPlayerExp != null)
            textRewardPlayerExp.text = "0";

        if (textRewardCharacterExp != null)
            textRewardCharacterExp.text = "0";

        if (uiIncreasedExpCharacters != null)
            uiIncreasedExpCharacters.ClearListItems();

        if (uiRewardItems != null)
            uiRewardItems.ClearListItems();

        if (firstClearPanelRoot != null)
            firstClearPanelRoot.Hide();

        if (uiRewardCurrency != null)
            uiRewardCurrency.SetData(PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0));

        if (textFirstClearRewardPlayerExp != null)
            textFirstClearRewardPlayerExp.text = "0";

        if (uiFirstClearRewardItems != null)
            uiFirstClearRewardItems.ClearListItems();

        if (uiFirstClearRewardSoftCurrency != null)
            uiFirstClearRewardSoftCurrency.SetData(PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0));

        if (uiFirstClearRewardHardCurrency != null)
            uiFirstClearRewardHardCurrency.SetData(PlayerCurrency.HardCurrency.Clone().SetAmount(0, 0));
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (uiPlayer != null)
            uiPlayer.SetData(data.player);

        if (textRewardPlayerExp != null)
            textRewardPlayerExp.text = data.rewardPlayerExp.ToString("N0");

        if (textRewardCharacterExp != null)
            textRewardCharacterExp.text = data.rewardCharacterExp.ToString("N0");

        if (uiIncreasedExpCharacters != null)
        {
            var playerItems = new List<PlayerItem>();
            var formationName = Player.CurrentPlayer.SelectedFormation;
            if (BaseGamePlayManager.BattleType == EBattleType.Arena)
                formationName = Player.CurrentPlayer.SelectedArenaFormation;

            var formationList = PlayerFormation.GetList(formationName);
            foreach (var formation in formationList)
            {
                var itemId = formation.ItemId;
                PlayerItem item = null;
                if (!string.IsNullOrEmpty(itemId) && PlayerItem.DataMap.TryGetValue(itemId, out item))
                    playerItems.Add(item);
            }

            uiIncreasedExpCharacters.selectionMode = UIDataItemSelectionMode.Disable;
            uiIncreasedExpCharacters.SetListItems(playerItems);
            foreach (var uiItem in uiIncreasedExpCharacters.UIEntries.Values)
            {
                if (uiItem.uiLevel != null)
                    uiItem.uiLevel.increasingExp = data.rewardCharacterExp;
            }
        }

        if (uiRewardItems != null)
        {
            uiRewardItems.selectionMode = UIDataItemSelectionMode.Disable;
            uiRewardItems.SetListItems(data.rewardItems);
        }

        if (firstClearPanelRoot != null)
        {
            if (data.isFirstClear)
                firstClearPanelRoot.Show();
            else
                firstClearPanelRoot.Hide();
        }

        if (uiRewardCurrency != null)
            uiRewardCurrency.SetData(PlayerCurrency.SoftCurrency.Clone().SetAmount(data.rewardSoftCurrency, 0));

        if (textFirstClearRewardPlayerExp != null)
            textFirstClearRewardPlayerExp.text = data.firstClearRewardPlayerExp.ToString("N0");

        if (uiFirstClearRewardItems != null)
        {
            uiFirstClearRewardItems.selectionMode = UIDataItemSelectionMode.Disable;
            uiFirstClearRewardItems.SetListItems(data.firstClearRewardItems);
        }

        if (uiFirstClearRewardSoftCurrency != null)
            uiFirstClearRewardSoftCurrency.SetData(PlayerCurrency.SoftCurrency.Clone().SetAmount(data.firstClearRewardSoftCurrency, 0));

        if (uiFirstClearRewardHardCurrency != null)
            uiFirstClearRewardHardCurrency.SetData(PlayerCurrency.HardCurrency.Clone().SetAmount(data.firstClearRewardHardCurrency, 0));
    }

    public void OnClickRestart()
    {
        Manager.Restart();
    }

    public void OnClickGoToManageScene()
    {
        GameInstance.Singleton.LoadManageScene();
    }

    public void OnClickGoToNextStage()
    {
        var nextStage = NextStage;
        if (nextStage != null)
            BaseGamePlayManager.StartStage(nextStage, null);
    }
}
