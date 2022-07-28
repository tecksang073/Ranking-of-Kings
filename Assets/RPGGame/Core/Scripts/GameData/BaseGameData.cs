using UnityEngine;

public abstract class BaseGameData : ScriptableObject, IGameData
{
    [Tooltip("Game data ID, if this is empty it will uses file's name as ID")]
    public string id;
    [Tooltip("Default title")]
    public string title;
    [Tooltip("Titles by language keys")]
    public LanguageData[] titles;
    public string Title
    {
        get { return Language.GetText(titles, title); }
    }
    [Multiline]
    public string description;
    [Tooltip("Descriptions by language keys")]
    public LanguageData[] descriptions;
    public string Description
    {
        get { return Language.GetText(descriptions, description); }
    }

    public string tag;
    public string category;
    public Sprite icon;
    public Sprite icon2;
    public Sprite icon3;

    public virtual string Id { get { return string.IsNullOrEmpty(id) ? name : id; } }
    public Sprite Icon { get { return icon; } }
    public Sprite Icon2 { get { return icon2; } }
    public Sprite Icon3 { get { return icon3; } }
    protected virtual void OnValidate() { }
}
