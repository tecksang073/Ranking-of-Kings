using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchievementManager : UIBase
{
    public UIAchievement uiSelectedInfo;
    public UIAchievementList uiAchievementList;

    public override void Show()
    {
        base.Show();
        GameInstance.GameService.GetAchievementList((result) =>
        {
            GameInstance.Singleton.OnGameServiceAchievementListResult(result);
            ReloadList();
        });
    }

    public void ReloadList()
    {
        if (uiAchievementList != null)
        {
            uiAchievementList.selectionMode = UIDataItemSelectionMode.Toggle;
            uiAchievementList.eventSelect.RemoveListener(SelectItem);
            uiAchievementList.eventSelect.AddListener(SelectItem);
            uiAchievementList.eventDeselect.RemoveListener(DeselectItem);
            uiAchievementList.eventDeselect.AddListener(DeselectItem);
            uiAchievementList.ClearListItems();

            var list = new List<PlayerAchievement>();
            var doneList = new List<PlayerAchievement>();
            var undoneList = new List<PlayerAchievement>();
            var earnedList = new List<PlayerAchievement>();
            PlayerAchievement tempPlayerAchievement;
            foreach (var achievement in GameInstance.GameDatabase.achievements)
            {
                if (PlayerAchievement.TryGetData(achievement.Id, out tempPlayerAchievement))
                {
                    if (tempPlayerAchievement.Earned)
                    {
                        earnedList.Add(tempPlayerAchievement);
                    }
                    else if (tempPlayerAchievement.Progress < achievement.targetAmount)
                    {
                        undoneList.Add(tempPlayerAchievement);
                    }
                    else
                    {
                        doneList.Add(tempPlayerAchievement);
                    }
                }
                else
                {
                    undoneList.Add(new PlayerAchievement()
                    {
                        DataId = achievement.Id
                    });
                }
            }

            list.AddRange(doneList);
            list.AddRange(undoneList);
            list.AddRange(earnedList);

            uiAchievementList.SetListItems(list, (ui) =>
            {
                ui.uiAchievementManager = this;
            });
        }
    }

    public override void Hide()
    {
        base.Hide();
        if (uiAchievementList != null)
            uiAchievementList.ClearListItems();
    }

    protected virtual void SelectItem(UIDataItem ui)
    {
        if (uiSelectedInfo != null)
            uiSelectedInfo.SetData((ui as UIAchievement).data);
    }

    protected virtual void DeselectItem(UIDataItem ui)
    {
        // Don't deselect
        ui.Selected = true;
    }
}
