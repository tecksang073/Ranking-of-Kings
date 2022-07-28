using UnityEngine;

public class OnClickChangeLanguage : MonoBehaviour
{
    public string languageKey;
    public void OnClick()
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
