using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterActionSkill : UICharacterAction
{
    public UISkill uiSkill;
    public Text textRemainsTurns;
    public Image imageRemainsTurnsGage;
    public int skillIndex;
    public CharacterSkill skill;

    private void Update()
    {
        if (skill == null)
            return;
        
        var rate = 1 - skill.GetCoolDownDurationRate();

        if (uiSkill != null)
            uiSkill.data = skill.Skill as Skill;

        if (textRemainsTurns != null)
            textRemainsTurns.text = skill.GetCoolDownDuration() <= 0 ? "" : skill.GetCoolDownDuration().ToString("N0");

        if (imageRemainsTurnsGage != null)
            imageRemainsTurnsGage.fillAmount = rate;

        TempToggle.interactable = !skill.Skill.isPassive;
    }

    protected override void OnActionSelected()
    {
        if (skill.Skill.isPassive)
        {
            // Skill is passive skill, so don't do any action
            return;
        }
        ActionManager.ActiveCharacter.SetAction(skillIndex);
    }
}
