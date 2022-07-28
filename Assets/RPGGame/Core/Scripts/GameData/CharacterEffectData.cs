using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterEffectData
{
    public GameEffect bodyEffect;
    public GameEffect floorEffect;

    public CharacterEffectData Clone()
    {
        var result = new CharacterEffectData();
        result.bodyEffect = bodyEffect;
        result.floorEffect = floorEffect;
        return result;
    }

    public List<GameEffect> InstantiatesTo(BaseCharacterEntity character)
    {
        var result = new List<GameEffect>();
        if (bodyEffect != null)
            result.Add(bodyEffect.InstantiateTo(character.bodyEffectContainer));
        if (floorEffect != null)
            result.Add(floorEffect.InstantiateTo(character.floorEffectContainer));
        return result;
    }
}
