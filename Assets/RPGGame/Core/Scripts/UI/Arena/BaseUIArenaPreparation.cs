using UnityEngine.Events;

public abstract class BaseUIArenaPreparation : UIBase
{
    public UIFormation uiCurrentFormation;
    public UIItem uiFormationSlotPrefab;
    public UIStamina uiRequireStamina;
    public UIArenaOpponentList uiArenaOpponentList;
    public UnityEvent onNoOpponentSelected;

    public override void Show()
    {
        base.Show();

        if (uiCurrentFormation != null)
        {
            if (string.IsNullOrEmpty(Player.CurrentPlayer.SelectedArenaFormation) ||
                !GameInstance.GameDatabase.Formations.ContainsKey(Player.CurrentPlayer.SelectedArenaFormation) ||
                GameInstance.GameDatabase.Formations[Player.CurrentPlayer.SelectedArenaFormation].formationType != EFormationType.Arena)
            {
                // Try set selected formation to arena formation
                foreach (var formation in GameInstance.GameDatabase.Formations.Values)
                {
                    if (formation.formationType == EFormationType.Arena)
                    {
                        Player.CurrentPlayer.SelectedArenaFormation = formation.id;
                        GameInstance.GameService.SelectFormation(formation.id, EFormationType.Arena);
                        break;
                    }
                }
            }
            uiCurrentFormation.formationName = Player.CurrentPlayer.SelectedArenaFormation;
            uiCurrentFormation.SetFormationData(uiFormationSlotPrefab);
        }

        if (uiRequireStamina != null)
        {
            var staminaData = PlayerStamina.ArenaStamina.Clone().SetAmount(1, 0);
            uiRequireStamina.SetData(staminaData);
        }
    }

    public void OnClickStartDuel()
    {
        var opponent = GetOpponent();
        if (opponent == null)
        {
            onNoOpponentSelected.Invoke();
            return;
        }
        BaseGamePlayManager.StartDuel(opponent.Id);
    }

    public Player GetOpponent()
    {
        if (uiArenaOpponentList != null)
        {
            var list = uiArenaOpponentList.GetSelectedDataList();
            if (list.Count > 0)
                return list[0];
        }
        return null;
    }
}
