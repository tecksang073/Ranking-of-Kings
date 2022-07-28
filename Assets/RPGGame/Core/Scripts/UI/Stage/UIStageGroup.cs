using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStageGroup : MonoBehaviour
{
    [Tooltip("When player clear this stage `Lock Game Objects` will visible if not, `Unlock Game Objects` will visible")]
    public BaseStage unlockByStage;
    public GameObject[] lockGameObjects;
    public GameObject[] unlockGameObjects;
    public GameObject stageContainer;
    [HideInInspector]
    public UIStageGroupManager stageGroupManager;

    void Update()
    {
        var isUnlocked = unlockByStage == null || PlayerClearStage.IsUnlock(unlockByStage);

        foreach (var lockGameObject in lockGameObjects)
        {
            lockGameObject.SetActive(!isUnlocked);
        }

        foreach (var unlockGameObject in unlockGameObjects)
        {
            unlockGameObject.SetActive(isUnlocked);
        }
    }

    public void ShowStages()
    {
        stageGroupManager.HideAllStages();
        if (stageContainer != null)
            stageContainer.SetActive(true);
    }

    public void HideStages()
    {
        if (stageContainer != null)
            stageContainer.SetActive(false);
    }
}
