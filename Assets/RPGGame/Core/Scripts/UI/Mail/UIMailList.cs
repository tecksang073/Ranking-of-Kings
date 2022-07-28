public class UIMailList : UIDataItemList<UIMail, Mail>
{
    public UIMail uiReadingMail;

    public override void Show()
    {
        base.Show();
        uiReadingMail.Hide();
        GetMailList();
    }

    public void GetMailList()
    {
        ClearListItems();
        GameInstance.GameService.GetMailList(OnGetMailListSuccess, OnGetMailListFail);
    }

    private void OnGetMailListSuccess(MailListResult result)
    {
        foreach (var entry in result.list)
        {
            var ui = SetListItem(entry.Id);
            ui.data = entry;
            ui.uiMailList = this;
            ui.Show();
        }
    }

    private void OnGetMailListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error, GetMailList);
    }

    public void ShowReadingMail(Mail mail)
    {
        uiReadingMail.data = mail;
        uiReadingMail.uiMailList = this;
        uiReadingMail.Show();
    }

    public void HideReadingMail()
    {
        uiReadingMail.Hide();
    }
}
