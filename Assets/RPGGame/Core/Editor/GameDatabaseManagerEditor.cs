using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDatabaseManagerEditor : EditorWindow
{
    private List<GameDataListSection> sections;
    private Vector2 menuScrollPosition;
    private int selectedMenuIndex;
    private GameDatabase selectedDatabase;

    [MenuItem("TBRPG/Game Database", false, 0)]
    public static void CreateNewEditor()
    {
        GetWindow<GameDatabaseManagerEditor>();
    }

    private void OnEnable()
    {
        selectedDatabase = null;
        sections = new List<GameDataListSection>()
        {
            new GameDataListSection<BaseItem>("items", "Items"),
            new GameDataListSection<BaseStage>("stages", "Stages"),
            new GameDataListSection<BaseRaidBossStage>("raidBossStages", "Raid-Boss Stages"),
            new GameDataListSection<BaseClanBossStage>("clanBossStages", "Clan-Boss Stages"),
            new GameDataListSection<Achievement>("achievements", "Achievements"),
            new GameDataListSection<ItemCraftFormula>("itemCrafts", "Item Crafts"),
            new GameDataListSection<LootBox>("lootBoxes", "Loot Boxes"),
            new GameDataListSection<RandomStore>("randomStores", "Random Stores"),
            new GameDataListSection<InGamePackage>("inGamePackages", "In-Game Packages"),
            new GameDataListSection<IapPackage>("iapPackages", "In-App Purchase\nPackages"),
        };
    }

    private void OnDisable()
    {
        EditorGlobalData.WorkingDatabase = null;
    }

    private void OnGUI()
    {
        titleContent = new GUIContent("Game Database", null, "Game Database");
        if (EditorGlobalData.WorkingDatabase == null)
        {
            Vector2 wndRect = new Vector2(500, 100);
            minSize = wndRect;

            GUILayout.BeginVertical("Game Database", "window");
            {
                GUILayout.BeginVertical("box");
                {
                    if (EditorGlobalData.WorkingDatabase == null)
                        EditorGUILayout.HelpBox("Select your game database which you want to manage", MessageType.Info);
                    EditorGlobalData.WorkingDatabase = EditorGUILayout.ObjectField("Game database", EditorGlobalData.WorkingDatabase, typeof(GameDatabase), true, GUILayout.ExpandWidth(true)) as GameDatabase;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            return;
        }
        else
        {
            Vector2 wndRect = new Vector2(500, 500);
            minSize = wndRect;
        }

        if (EditorGlobalData.WorkingDatabase != selectedDatabase)
        {
            selectedDatabase = EditorGlobalData.WorkingDatabase;
        }

        // Prepare GUI styles
        GUIStyle leftMenuButton = new GUIStyle(EditorStyles.label);
        leftMenuButton.fontSize = 10;
        leftMenuButton.alignment = TextAnchor.MiddleRight;

        GUIStyle selectedLeftMenuButton = new GUIStyle(EditorStyles.label);
        selectedLeftMenuButton.fontSize = 10;
        selectedLeftMenuButton.alignment = TextAnchor.MiddleRight;
        var background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        background.SetPixel(0, 0, EditorGUIUtility.isProSkin ? new Color(0.243f, 0.373f, 0.588f) : new Color(0.247f, 0.494f, 0.871f));
        background.Apply();
        selectedLeftMenuButton.active.background = selectedLeftMenuButton.normal.background = background;

        // Left menu
        GUILayout.BeginArea(new Rect(0, 0, 120, position.height), string.Empty, "box");
        {
            menuScrollPosition = GUILayout.BeginScrollView(menuScrollPosition);
            GUILayout.BeginVertical();
            {
                for (int i = 0; i < sections.Count; ++i)
                {
                    if (GUILayout.Button(sections[i].MenuTitle, (i == selectedMenuIndex ? selectedLeftMenuButton : leftMenuButton), GUILayout.Height(32)))
                    {
                        selectedMenuIndex = i;
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();

        // Content
        GUILayout.BeginArea(new Rect(120, 0, position.width - 120, position.height), string.Empty);
        {
            sections[selectedMenuIndex].OnGUI(position.width - 120, position.height);
        }
        GUILayout.EndArea();
    }
}
