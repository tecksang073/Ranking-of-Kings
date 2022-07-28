using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class UICharacterActionManager : UIBase
{
    public UICharacterAction[] uiActions;
    public UICharacterStats uiStats;
    public GamePlayManager CastedManager { get { return BaseGamePlayManager.Singleton as GamePlayManager; } }
    private readonly List<UICharacterActionSkill> UICharacterSkills = new List<UICharacterActionSkill>();

    public ToggleGroup CacheToggleGroup { get; private set; }

    public CharacterEntity ActiveCharacter
    {
        get { return CastedManager.ActiveCharacter; }
    }

    public bool IsPlayerCharacterActive
    {
        get { return ActiveCharacter != null && ActiveCharacter.IsPlayerCharacter; }
    }

    protected override void Awake()
    {
        base.Awake();
        CacheToggleGroup = GetComponent<ToggleGroup>();
        CacheToggleGroup.allowSwitchOff = false;
        var skillIndex = 0;
        foreach (var uiAction in uiActions)
        {
            uiAction.ActionManager = this;
            uiAction.IsOn = false;
            var uiSkill = uiAction as UICharacterActionSkill;
            if (uiSkill != null)
            {
                uiSkill.skillIndex = skillIndex;
                UICharacterSkills.Add(uiSkill);
                ++skillIndex;
            }
        }
        if (uiStats != null)
            uiStats.notFollowCharacter = true;
    }

    private void Update()
    {
        if (!IsPlayerCharacterActive || ActiveCharacter.IsDoingAction)
            Hide();
        if (uiStats != null)
            uiStats.character = ActiveCharacter;
    }

    private void OnEnable()
    {
        var i = 0;
        for (i = 0; i < uiActions.Length; ++i)
        {
            uiActions[i].IsOn = false;
            if (i == 0)
                uiActions[i].IsOn = true;
        }
        i = 0;
        foreach (var skill in CastedManager.ActiveCharacter.Skills)
        {
            if (i >= UICharacterSkills.Count)
                break;
            var ui = UICharacterSkills[i];
            ui.skill = skill as CharacterSkill;
            ui.Show();
            ++i;
        }
        for (; i < UICharacterSkills.Count; ++i)
        {
            var ui = UICharacterSkills[i];
            ui.Hide();
        }
    }
}
