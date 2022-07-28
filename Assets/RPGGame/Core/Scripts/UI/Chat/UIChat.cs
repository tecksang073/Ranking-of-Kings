using UnityEngine.UI;

public class UIChat : UIDataItem<ChatMessage>
{
    public Text textProfileName;
    public Text textClanName;
    public Text textMessage;
    public UIChatManager uiChatManager;

    public override void Clear()
    {
        if (textProfileName != null)
            textProfileName.text = string.Empty;

        if (textClanName != null)
            textClanName.text = string.Empty;

        if (textMessage != null)
            textMessage.text = string.Empty;
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (textProfileName != null)
            textProfileName.text = data == null ? string.Empty : data.ProfileName;

        if (textClanName != null)
            textClanName.text = data == null ? string.Empty : data.ClanName;

        if (textMessage != null)
            textMessage.text = data == null ? string.Empty : data.Message;
    }
}
