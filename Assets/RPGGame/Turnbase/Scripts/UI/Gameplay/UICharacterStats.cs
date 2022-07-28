using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterStats : UICharacterStatsGeneric
{
    public GameObject selectableObject;
    public GameObject activatingObject;

    protected override void Update()
    {
        base.Update();

        if (character == null)
            return;

        var castedCharacter = character as CharacterEntity;

        if (selectableObject != null)
            selectableObject.SetActive(castedCharacter.Selectable);

        if (activatingObject != null)
            activatingObject.SetActive(castedCharacter.IsActiveCharacter && castedCharacter.Hp > 0);
    }
}
