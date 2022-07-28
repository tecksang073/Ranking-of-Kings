using UnityEngine.UI;

public class UIMailsCount : UIBase
{
    public Text textCount;

    private void OnEnable()
    {
        if (textCount != null)
            textCount.text = "0";
        GetMailsCount();
    }

    public void GetMailsCount()
    {
        GameInstance.GameService.GetMailsCount(OnGetMailsCountSuccess, OnGetMailsCountFail);
    }

    private void OnGetMailsCountSuccess(MailsCountResult result)
    {
        if (textCount != null)
            textCount.text = result.count > 99 ? "99+" : result.count.ToString("N0");
    }

    private void OnGetMailsCountFail(string error)
    {
        // Do nothing, it's not so important
    }
}
