using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class BaseUISkill<TSkill> : UIDataItem<TSkill>
    where TSkill : BaseSkill
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(TSkill data)
    {
        if (textTitle != null)
            textTitle.text = data == null ? "" : data.Title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.Description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.icon;
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void ShowDataOnMessageDialog()
    {
        GameInstance.Singleton.ShowMessageDialog(data.Title, data.Description);
    }
}
