using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class ImplementedCreateScriptableObject
{
    [MenuItem("Assets/Create/ScriptableObject (TurnbaseRPG)", priority = -5000)]
    public static void ShowWindow()
    {
        CreateScriptableObject.assemblyNames = new string[] { "BaseRPG", "TurnbaseRPG", "RPGTurnbase" };
        var win = EditorWindow.GetWindow<CreateScriptableObject>(true, "Create ScriptableObject (TurnbaseRPG)");
        win.ShowPopup();
    }
}
