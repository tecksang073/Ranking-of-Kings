using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : UIGamePlayGeneric
{
    public Text textWave;

    protected override void Update()
    {
        base.Update();
        var gamePlayManager = Manager as GamePlayManager;
        if (textWave != null)
            textWave.text = gamePlayManager.CurrentWave <= 0 ? "" : "Wave " + gamePlayManager.CurrentWave + "/" + gamePlayManager.MaxWave;
    }
}
