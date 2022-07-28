using UnityEngine;
using UnityEngine.UI;

public class UIRaidEventPreparation : UIDataItem<RaidEvent>
{
    public UIFormation uiCurrentFormation;
    public UIItem uiFormationSlotPrefab;
    public Text textTeamBattlePointPerRecommend;
    public Color colorEnoughBattlePoint = Color.white;
    public Color colorNotEnoughBattlePoint = Color.red;
    public UIRaidEvent uiRaidEvent;

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
                textTeamBattlePointPerRecommend.text = uiCurrentFormation.TeamBattlePoint + "/" + data.RaidBossStage.recommendBattlePoint;
                textTeamBattlePointPerRecommend.color = uiCurrentFormation.TeamBattlePoint >= data.RaidBossStage.recommendBattlePoint ? colorEnoughBattlePoint : colorNotEnoughBattlePoint;
            }
        }
    }

    public override void UpdateData()
    {
        if (uiRaidEvent != null)
            uiRaidEvent.SetData(data);
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

    public void OnClickStartRaidBossBattle()
    {
        BaseGamePlayManager.StartRaidBossBattle(data);
    }
}
