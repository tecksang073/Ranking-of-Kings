using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItem : BaseActorItem
{
    [Header("Character Data")]
    public Elemental elemental;
    public List<BaseAttackAnimationData> attackAnimations;
    public List<BaseSkill> skills;
    public BaseCharacterEntity model;
    public CharacterItemEvolve evolveInfo;

    public override SpecificItemEvolve GetSpecificItemEvolve()
    {
        return evolveInfo;
    }

#if UNITY_EDITOR
    public override BaseActorItem CreateEvolveItemAsset(CreateEvolveItemData createEvolveItemData)
    {
        var newItem = ScriptableObjectUtility.CreateAsset<CharacterItem>(name);
        newItem.attackAnimations = new List<BaseAttackAnimationData>(attackAnimations);
        newItem.skills = new List<BaseSkill>(skills);
        newItem.model = model;
        newItem.evolveInfo = (CharacterItemEvolve)evolveInfo.Clone();
        return newItem;
    }
#endif
}

[System.Serializable]
public class CharacterItemAmount : BaseItemAmount<CharacterItem> { }

[System.Serializable]
public class CharacterItemDrop : BaseItemDrop<CharacterItem> { }

[System.Serializable]
public class CharacterItemEvolve : SpecificItemEvolve<CharacterItem>
{
    public override SpecificItemEvolve<CharacterItem> Create()
    {
        return new CharacterItemEvolve();
    }
}
