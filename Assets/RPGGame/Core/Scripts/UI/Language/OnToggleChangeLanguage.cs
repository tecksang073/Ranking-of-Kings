using UnityEngine;
using UnityEngine.UI;

public class OnToggleChangeLanguage : MonoBehaviour
{
    public string languageKey;

    private void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        toggle.isOn = LanguageManager.CurrentLanguageKey.Equals(languageKey);
        toggle.onValueChanged.AddListener(OnToggle);
    }

    public void OnToggle(bool selected)
    {
        if (selected)
        {
            LanguageManager.ChangeLanguage(languageKey);
            UIDataItem[] uis = FindObjectsOfType<UIDataItem>();
            for (int i = 0; i < uis.Length; ++i)
            {
                if (!uis[i].IsVisible())
                    continue;
                uis[i].ForceUpdate();
            }
        }
    }
}
