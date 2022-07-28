using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUIStagePreparation<TUI, TStage> : UIDataItem<TStage>
    where TUI : UIDataItem<TStage>
    where TStage : BaseStage
{
    public UIFormation uiCurrentFormation;
    public UIItem uiFormationSlotPrefab;
    public Text textTeamBattlePointPerRecommend;
    public Color colorEnoughBattlePoint = Color.white;
    public Color colorNotEnoughBattlePoint = Color.red;
    public TUI uiStage;
    public UIHelperList uiHelperList;
    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    protected override void Update()
    {
        base.Update();
        if (textTeamBattlePointPerRecommend != null)
        {
            if (IsEmpty() || uiCurrentFormation == null)
            {
                textTeamBattlePointPerRecommend.text = "N/A";
                textTeamBattlePointPerRecommend.color = colorNotEnoughBattlePoint;
            }
            else
            {
                textTeamBattlePointPerRecommend.text = uiCurrentFormation.TeamBattlePoint + "/" + data.recommendBattlePoint;
                textTeamBattlePointPerRecommend.color = uiCurrentFormation.TeamBattlePoint >= data.recommendBattlePoint ? colorEnoughBattlePoint : colorNotEnoughBattlePoint;
            }
        }
    }

    public override void UpdateData()
    {
        if (uiStage != null)
            uiStage.SetData(data);
    }

    public override void Show()
    {
        base.Show();

        if (uiCurrentFormation != null)
        {
            if (!GameInstance.GameDatabase.Formations.ContainsKey(Player.CurrentPlayer.SelectedFormation) ||
                GameInstance.GameDatabase.Formations[Player.CurrentPlayer.SelectedFormation].formationType != EFormationType.Stage)
            {
                // Try set selected formation to arena formation
                foreach (var formation in GameInstance.GameDatabase.Formations.Values)
                {
                    if (formation.formationType == EFormationType.Stage)
                    {
                        Player.CurrentPlayer.SelectedFormation = formation.id;
                        GameInstance.GameService.SelectFormation(formation.id, EFormationType.Stage);
                        break;
                    }
                }
            }
            uiCurrentFormation.formationName = Player.CurrentPlayer.SelectedFormation;
            uiCurrentFormation.SetFormationData(uiFormationSlotPrefab);
        }
    }

    public void OnClickStartStage()
    {
        BaseGamePlayManager.StartStage(data, GetHelper());
    }

    public Player GetHelper()
    {
        if (uiHelperList != null)
        {
            var list = uiHelperList.GetSelectedDataList();
            if (list.Count > 0)
                return list[0];
        }
        return null;
    }
}
