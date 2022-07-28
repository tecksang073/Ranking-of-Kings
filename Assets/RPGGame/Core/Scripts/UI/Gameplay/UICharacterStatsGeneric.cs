using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIFollowWorldObject))]
public class UICharacterStatsGeneric : UIBase
{
    public Text textTitle;
    public Image imageIcon;
    public GameObject characterStatsRoot;
    public Text textHpPerMaxHp;
    public Text textHpPercent;
    public Image imageHpGage;
    public UILevel uiLevel;
    public UICharacterBuff[] uiBuffs;
    public BaseCharacterEntity character;
    public bool notFollowCharacter;
    public bool hideIfCharacterIsBoss;

    public UIFollowWorldObject CacheObjectFollower { get; private set; }

    private Canvas attachedCanvas;
    private bool attachedWorldSpaceCanvas;

    protected override void Awake()
    {
        base.Awake();
        if (!characterStatsRoot)
            characterStatsRoot = root;
        CacheObjectFollower = GetComponent<UIFollowWorldObject>();
        attachedCanvas = GetComponent<Canvas>();
        if (attachedCanvas != null)
            attachedWorldSpaceCanvas = (attachedCanvas.renderMode == RenderMode.WorldSpace);
    }

    private void Start()
    {
        if (notFollowCharacter || attachedWorldSpaceCanvas)
        {
            CacheObjectFollower.enabled = false;
            CacheObjectFollower.CachePositionFollower.enabled = false;
        }
    }

    public void Attach(Transform uiContainer, BaseCharacterEntity character)
    {
        this.character = character;
        if (attachedWorldSpaceCanvas)
        {
            transform.SetParent(character.uiContainer, true);
        }
        else
        {
            transform.SetParent(uiContainer);
            transform.localScale = Vector3.one;
        }
        transform.localPosition = Vector3.zero;
    }

    protected virtual void Update()
    {
        if ((hideIfCharacterIsBoss && character.IsBoss) || !character)
        {
            HideCharacterStats();
            return;
        }

        if (!attachedWorldSpaceCanvas && !notFollowCharacter)
            CacheObjectFollower.targetObject = character.uiContainer;

        var itemData = character.Item.ItemData;
        var rate = (float)character.Hp / (float)character.MaxHp;

        if (textHpPerMaxHp != null)
            textHpPerMaxHp.text = character.Hp.ToString("N0") + "/" + character.MaxHp.ToString("N0");

        if (textHpPercent != null)
            textHpPercent.text = (rate * 100).ToString("N2") + "%";

        if (imageHpGage != null)
            imageHpGage.fillAmount = rate;

        if (textTitle != null)
            textTitle.text = itemData.Title;

        if (imageIcon != null)
            imageIcon.sprite = itemData.icon;

        if (uiLevel != null)
        {
            uiLevel.level = character.Item.Level;
            uiLevel.maxLevel = character.Item.MaxLevel;
            uiLevel.currentExp = character.Item.CurrentExp;
            uiLevel.requiredExp = character.Item.RequiredExp;
        }

        var i = 0;
        var buffKeys = character.Buffs.Keys;
        foreach (var buffKey in buffKeys)
        {
            if (i >= uiBuffs.Length)
                break;
            var ui = uiBuffs[i];
            ui.buff = character.Buffs[buffKey];
            ui.Show();
            ++i;
        }
        for (; i < uiBuffs.Length; ++i)
        {
            var ui = uiBuffs[i];
            ui.Hide();
        }
    }

    public void ShowCharacterStats()
    {
        if (hideIfCharacterIsBoss && character.IsBoss)
            return;
        characterStatsRoot.SetActive(true);
    }

    public void HideCharacterStats()
    {
        characterStatsRoot.SetActive(false);
    }
}
