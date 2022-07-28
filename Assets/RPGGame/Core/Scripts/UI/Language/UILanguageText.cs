using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILanguageText : MonoBehaviour
{
    public string dataKey;
    [TextArea(1, 10)]
    public string defaultText;
    private string languageKey;
    private void Update()
    {
        if (languageKey != LanguageManager.CurrentLanguageKey)
        {
            var textComponent = GetComponent<Text>();
            if (textComponent != null)
            {
                var text = "";
                if (LanguageManager.Texts.TryGetValue(dataKey, out text))
                    textComponent.text = text;
                else
                    textComponent.text = defaultText;
            }
            languageKey = LanguageManager.CurrentLanguageKey;
        }
    }

    void OnValidate()
    {
        var textComponent = GetComponent<Text>();
        textComponent.text = defaultText;
    }
}
