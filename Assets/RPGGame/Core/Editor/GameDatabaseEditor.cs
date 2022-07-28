﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDatabase))]
public class GameDatabaseEditor : BaseCustomEditor
{
    protected override void SetFieldCondition()
    {
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Open Manager"))
        {
            EditorGlobalData.WorkingDatabase = target as GameDatabase;
            GameDatabaseManagerEditor.CreateNewEditor();
        }
    }
}
