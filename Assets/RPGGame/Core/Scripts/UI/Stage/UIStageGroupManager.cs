using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStageGroupManager : MonoBehaviour
{
    public UIStageGroup[] stageGroups;

    private void Start()
    {
        foreach (var stageGroup in stageGroups)
        {
            stageGroup.stageGroupManager = this;
        }
    }

    public void HideAllStages()
    {
        foreach (var stageGroup in stageGroups)
        {
            stageGroup.HideStages();
        }
    }
}
