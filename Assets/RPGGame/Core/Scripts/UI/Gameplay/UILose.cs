using UnityEngine.UI;

public partial class UILose : UIBase
{
    public Button buttonRevive;
    public Button buttonRestart;
    public Button buttonGiveUp;

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
        if (buttonRevive != null)
        {
            buttonRevive.onClick.RemoveListener(OnClickRevive);
            buttonRevive.onClick.AddListener(OnClickRevive);
        }
        if (buttonRestart != null)
        {
            buttonRestart.onClick.RemoveListener(OnClickRestart);
            buttonRestart.onClick.AddListener(OnClickRestart);
        }
        if (buttonGiveUp != null)
        {
            buttonGiveUp.onClick.RemoveListener(OnClickGiveUp);
            buttonGiveUp.onClick.AddListener(OnClickGiveUp);
        }
    }

    public void OnClickRevive()
    {
        Hide();
        Manager.Revive(Show);
    }

    public void OnClickRestart()
    {
        Hide();
        Manager.Restart();
    }

    public void OnClickGiveUp()
    {
        Hide();
        Manager.Giveup(Show);
    }
}
