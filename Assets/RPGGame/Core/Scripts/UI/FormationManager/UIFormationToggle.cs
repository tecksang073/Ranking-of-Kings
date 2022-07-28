using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIFormationToggle : MonoBehaviour
{
    public Text textTitle;
    public Image imageIcon;
    public Formation formation;

    private Toggle tempToggle;
    public Toggle TempToggle
    {
        get
        {
            if (tempToggle == null)
                tempToggle = GetComponent<Toggle>();
            return tempToggle;
        }
    }

    public bool IsOn
    {
        get { return TempToggle.isOn; }
        set { TempToggle.isOn = value; }
    }

    private void Update()
    {
        if (formation != null)
        {
            if (textTitle != null)
                textTitle.text = formation.title;

            if (imageIcon != null)
                imageIcon.sprite = formation.icon;
        }
    }
}
