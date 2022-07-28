using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIStageList<TPreparation, TUI, TStage> : UIDataItemList<TUI, TStage>
    where TPreparation : UIDataItem<TStage>
    where TUI : BaseUIStage<TPreparation, TStage>
    where TStage : BaseStage
{
    public TPreparation uiStagePreparation;
    public List<string> stageTags = new List<string>();
    public bool showOnlyAvailableStages;

    public override void Show()
    {
        base.Show();
        GetAvailableStageList();
    }

    private void GetAvailableStageList()
    {
        ClearListItems();
        GameInstance.GameService.GetAvailableStageList(OnGetAvailableStageListSuccess, OnGetAvailableStageListFail);
    }

    private void OnGetAvailableStageListSuccess(AvailableStageListResult result)
    {
        foreach (var entry in result.list)
        {
            if (showOnlyAvailableStages && !GameInstance.GameDatabase.Stages.ContainsKey(entry))
                continue;
            var stage = GameInstance.GameDatabase.Stages[entry];
            if (stageTags == null || stageTags.Count == 0 || stageTags.Contains(stage.tag))
            {
                var ui = SetListItem(entry);
                ui.data = stage as TStage;
                ui.uiStagePreparation = uiStagePreparation;
                ui.Show();
            }
        }
    }

    private void OnGetAvailableStageListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, GetAvailableStageList);
    }
}
