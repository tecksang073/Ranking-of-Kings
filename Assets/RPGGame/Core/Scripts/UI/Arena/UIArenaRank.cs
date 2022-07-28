using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArenaRank : UIDataItem<ArenaRank>
{
	public Text textTitle;
	public Text textDescription;
	public Image imageIcon;
	public Image imageHighlight;

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(ArenaRank data)
    {
        if (data == null)
            return;

        if (textTitle != null)
            textTitle.text = data.Title;

        if (textDescription != null)
            textDescription.text = data.Description;

		if (imageIcon != null)
			imageIcon.sprite = data.icon;

		if (imageHighlight != null)
			imageHighlight.sprite = data.highlight;
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }
}
