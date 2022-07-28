using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : BaseGameData
{
    public ElementalEffectiveness[] elementalEffectivenesses;

    private Dictionary<Elemental, float> cacheElementEffectiveness;
    public Dictionary<Elemental, float> CacheElementEffectiveness
    {
        get
        {
            if (cacheElementEffectiveness == null)
            {
                cacheElementEffectiveness = new Dictionary<Elemental, float>();
                foreach (ElementalEffectiveness elementalEffectiveness in elementalEffectivenesses)
                {
                    cacheElementEffectiveness[elementalEffectiveness.elemental] = elementalEffectiveness.effectiveness;
                }
            }
            return cacheElementEffectiveness;
        }
    }
}

[System.Serializable]
public struct ElementalEffectiveness
{
    public Elemental elemental;
    [Range(0.01f, 2)]
    public float effectiveness;
}