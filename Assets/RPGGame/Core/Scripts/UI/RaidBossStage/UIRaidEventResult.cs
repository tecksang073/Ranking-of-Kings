using UnityEngine.UI;

public class UIRaidEventResult : UIDataItem<FinishRaidBossBattleResult>
{
    public Text textTotalDamage;
    public UIRaidEvent uiRaidEvent;
    public Button buttonGoToManageScene;

    public override void Show()
    {
        base.Show();
        if (buttonGoToManageScene != null)
        {
            buttonGoToManageScene.onClick.RemoveListener(OnClickGoToManageScene);
            buttonGoToManageScene.onClick.AddListener(OnClickGoToManageScene);
        }
    }

    public override void Clear()
    {
        if (textTotalDamage != null)
            textTotalDamage.text = "0";

        if (uiRaidEvent != null)
            uiRaidEvent.Clear();
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (textTotalDamage != null)
            textTotalDamage.text = data.totalDamage.ToString("N0");

        if (uiRaidEvent != null)
            uiRaidEvent.SetData(data.raidEvent);
    }

    public void OnClickGoToManageScene()
    {
        GameInstance.Singleton.LoadManageScene();
    }
}
