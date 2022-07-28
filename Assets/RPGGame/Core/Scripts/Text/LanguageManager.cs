using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static Dictionary<string, Dictionary<string, string>> Languages { get; protected set; } = new Dictionary<string, Dictionary<string, string>>();
    public static Dictionary<string, string> Texts { get; protected set; } = new Dictionary<string, string>();
    public static string CurrentLanguageKey { get; protected set; }
    private static string currentPlayerPrefsKey = string.Empty;

    [Header("Language Manager Configs")]
    public string defaultLanguageKey = "ENG";
    public string playerPrefsKey = "USER_LANG";
    public List<Language> languageList = new List<Language>();

    [Header("Add New Language")]
    [Tooltip("You can add new language by `Add New Language` context menu")]
    public string newLanguageKey;
    [InspectorButton(nameof(AddNewLanguage))]
    public bool addNewLanguage;

    private void Awake()
    {
        currentPlayerPrefsKey = playerPrefsKey;
        CurrentLanguageKey = PlayerPrefs.GetString(currentPlayerPrefsKey, defaultLanguageKey);
        Languages.Clear();
        Dictionary<string, string> tempNewData;
        foreach (Language language in languageList)
        {
            tempNewData = new Dictionary<string, string>();
            foreach (LanguageData data in language.dataList)
            {
                if (tempNewData.ContainsKey(data.key))
                {
                    Debug.LogWarning("[LanguageManager] Language " + language.languageKey + " already contains " + data.key);
                    continue;
                }
                tempNewData.Add(data.key, data.value);
            }
            Languages[language.languageKey] = tempNewData;
        }
        ChangeLanguage(CurrentLanguageKey);
    }

    public Language GetLanguageFromList(string languageKey)
    {
        foreach (Language language in languageList)
        {
            if (language.languageKey == languageKey)
                return language;
        }
        return null;
    }

    [ContextMenu("Add New Language")]
    public void AddNewLanguage()
    {
        if (string.IsNullOrEmpty(newLanguageKey))
        {
            Debug.LogWarning("`New Language Key` is null or empty");
            return;
        }

        Language newLang = GetLanguageFromList(newLanguageKey);
        if (newLang == null)
        {
            newLang = new Language();
            newLang.languageKey = newLanguageKey;
            languageList.Add(newLang);
        }

        foreach (KeyValuePair<string, string> pair in DefaultLocale.Texts)
        {
            if (newLang.ContainKey(pair.Key))
                continue;

            newLang.dataList.Add(new LanguageData()
            {
                key = pair.Key,
                value = pair.Value,
            });
        }
    }

    public static void ChangeLanguage(string languageKey)
    {
        if (!Languages.ContainsKey(languageKey))
            return;

        CurrentLanguageKey = languageKey;
        Texts = Languages[languageKey];
        PlayerPrefs.SetString(currentPlayerPrefsKey, CurrentLanguageKey);
        PlayerPrefs.Save();
    }

    public static string GetText(string key, string defaultValue = "")
    {
        if (!string.IsNullOrEmpty(key))
        {
            if (Texts.ContainsKey(key))
                return Texts[key];
            if (DefaultLocale.Texts.ContainsKey(key))
                return DefaultLocale.Texts[key];
        }
        return defaultValue;
    }

    public static string ReplaceFormat(string key, string replacingText)
    {
        return ReplaceFormat("{" + key + "}", GetText(key), replacingText);
    }

    public static string ReplaceFormat(string key, string value, string replacingText)
    {
        return replacingText.Replace("{" + key + "}", value);
    }

    public static string FormatNumber(int value, bool asPercent = false)
    {
        return asPercent ? (value * 100).ToString("N0") + "%" : value.ToString("N0");
    }

    public static string FormatNumber(float value, bool asPercent = false)
    {
        return asPercent ? (value * 100).ToString("N0") + "%" : value.ToString("N0");
    }

    public static string FormatInfo(string key, int value, bool asPercent = false)
    {
        return string.Format(GetText(GameText.FORMAT_INFO), GetText(key), FormatNumber(value, asPercent));
    }

    public static string FormatInfo(string key, float value, bool asPercent = false)
    {
        return string.Format(GetText(GameText.FORMAT_INFO), GetText(key), FormatNumber(value, asPercent));
    }

    public static string FormatAttribute(string key, int value, int bonusValue, bool asPercent = false)
    {
        return string.Format(GetText(GameText.FORMAT_ATTRIBUTE), GetText(key), FormatNumber(value, asPercent), FormatBonus(bonusValue, asPercent));
    }

    public static string FormatAttribute(string key, float value, float bonusValue, bool asPercent = false)
    {
        return string.Format(GetText(GameText.FORMAT_ATTRIBUTE), GetText(key), FormatNumber(value, asPercent), FormatBonus(bonusValue, asPercent));
    }

    public static string FormatBonus(int value, bool asPercent = false)
    {
        if (value > 0)
            return string.Format(GetText(GameText.FORMAT_BONUS), "+", FormatNumber(value, asPercent));
        else if (value < 0)
            return string.Format(GetText(GameText.FORMAT_BONUS), "-", FormatNumber(value, asPercent));
        else
            return "";
    }

    public static string FormatBonus(float value, bool asPercent = false)
    {
        if (value > 0)
            return string.Format(GetText(GameText.FORMAT_BONUS), "+", FormatNumber(value, asPercent));
        else if (value < 0)
            return string.Format(GetText(GameText.FORMAT_BONUS), "-", FormatNumber(value, asPercent));
        else
            return "";
    }

    public static string ReplaceFormatAsFormatBonus(string key, int value, string replacingText, bool asPercent = false)
    {
        return replacingText.Replace("{" + key + "}", FormatBonus(value, asPercent));
    }

    public static string ReplaceFormatAsFormatBonus(string key, float value, string replacingText, bool asPercent = false)
    {
        return replacingText.Replace("{" + key + "}", FormatBonus(value, asPercent));
    }
}
