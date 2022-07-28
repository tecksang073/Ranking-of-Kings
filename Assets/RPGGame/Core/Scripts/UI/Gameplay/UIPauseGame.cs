using UnityEngine.UI;

public partial class UIPauseGame : UIBase
{
    public Button buttonContinue;
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
        if (buttonContinue != null)
        {
            buttonContinue.onClick.RemoveListener(OnClickContinue);
            buttonContinue.onClick.AddListener(OnClickContinue);
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

    public void OnClickContinue()
    {
        Hide();
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
