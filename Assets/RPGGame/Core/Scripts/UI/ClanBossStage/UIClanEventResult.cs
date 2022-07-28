using UnityEngine.UI;

public class UIClanEventResult : UIDataItem<FinishClanBossBattleResult>
{
    public Text textTotalDamage;
    public UIClanEvent uiClanEvent;
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

        if (uiClanEvent != null)
            uiClanEvent.Clear();
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (textTotalDamage != null)
            textTotalDamage.text = data.totalDamage.ToString("N0");

        if (uiClanEvent != null)
            uiClanEvent.SetData(data.clanEvent);
    }

    public void OnClickGoToManageScene()
    {
        GameInstance.Singleton.LoadManageScene();
    }
}
