using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateCharacterEntityEditor : EditorWindow
{
    private GameObject selectedModel;
    private AnimationClip selectedIdleClip;
    private AnimationClip selectedMoveClip;
    private AnimationClip selectedHurtClip;
    private AnimationClip selectedDeadClip;
    private CharacterItem copyingItemData;
    private Animator selectedModelAnimator;

    [MenuItem("TBRPG/Create CharacterEnity", false, 100)]
    public static void CreateNewCharacter()
    {
        GetWindow<CreateCharacterEntityEditor>();
    }

    void OnGUI()
    {
        Vector2 wndRect = new Vector2(500, 200);
        maxSize = wndRect;
        minSize = wndRect;
        titleContent = new GUIContent("Character", null, "Character Entity Creator");
        GUILayout.BeginVertical("Character Entity Creator", "window");
        GUILayout.BeginVertical("box");

        if (selectedModel == null)
            EditorGUILayout.HelpBox("Select your FBX model which you want to use as character", MessageType.Info);
        else if (selectedModelAnimator == null)
            EditorGUILayout.HelpBox("Missing a Animator Component", MessageType.Error);

        selectedModel = EditorGUILayout.ObjectField("FBX Model", selectedModel, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
        selectedIdleClip = EditorGUILayout.ObjectField("Idle Clip", selectedIdleClip, typeof(AnimationClip), true, GUILayout.ExpandWidth(true)) as AnimationClip;
        selectedMoveClip = EditorGUILayout.ObjectField("Move Clip", selectedMoveClip, typeof(AnimationClip), true, GUILayout.ExpandWidth(true)) as AnimationClip;
        selectedHurtClip = EditorGUILayout.ObjectField("Hurt Clip", selectedHurtClip, typeof(AnimationClip), true, GUILayout.ExpandWidth(true)) as AnimationClip;
        selectedDeadClip = EditorGUILayout.ObjectField("Dead Clip", selectedDeadClip, typeof(AnimationClip), true, GUILayout.ExpandWidth(true)) as AnimationClip;
        copyingItemData = EditorGUILayout.ObjectField("Copy Data", copyingItemData, typeof(CharacterItem), true, GUILayout.ExpandWidth(true)) as CharacterItem;

        GUILayout.EndVertical();

        if (selectedModel != null)
            selectedModelAnimator = selectedModel.GetComponent<Animator>();

        if (selectedModel != null && selectedModelAnimator != null)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(true), GUILayout.Height(40)))
                Create();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Created the Third Person Controller
    /// </summary>
    void Create()
    {
        // base for the character
        var characterEntityObj = Instantiate(selectedModel, Vector3.zero, Quaternion.identity);
        if (characterEntityObj == null)
            return;

        var fileName = "Character_" + selectedModel.gameObject.name;
        var path = EditorUtility.SaveFolderPanel("Save data to folder", "Assets", "");
        path = path.Substring(path.IndexOf("Assets"));
        characterEntityObj.name = fileName;

        var rigidbody = characterEntityObj.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = characterEntityObj.AddComponent<Rigidbody>();
        var collider = characterEntityObj.GetComponent<CapsuleCollider>();
        if (collider == null)
            collider = characterEntityObj.AddComponent<CapsuleCollider>();
        var targetingRigidbody = characterEntityObj.GetComponent<TargetingRigidbody>();
        if (targetingRigidbody == null)
            targetingRigidbody = characterEntityObj.AddComponent<TargetingRigidbody>();
        var characterEntity = characterEntityObj.GetComponent<CharacterEntity>();
        if (characterEntity == null)
            characterEntity = characterEntityObj.AddComponent<CharacterEntity>();
        var animator = characterEntityObj.GetComponent<Animator>();

        // Get bounds
        Bounds bounds = GetMeshesBounds(characterEntityObj);
        float height = bounds.size.y;
        Vector3 center = new Vector3(0, (float)System.Math.Round(height * 0.5f, 2), 0);

        // Set containers
        var uiContainer = new GameObject("_uiContainer");
        uiContainer.transform.parent = characterEntityObj.transform;
        uiContainer.transform.localPosition = center + Vector3.up * bounds.extents.y;
        uiContainer.transform.localRotation = Quaternion.identity;
        uiContainer.transform.localScale = Vector3.zero;
        characterEntity.uiContainer = uiContainer.transform;

        var bodyEffectContainer = new GameObject("_bodyEffectContainer");
        bodyEffectContainer.transform.parent = characterEntityObj.transform;
        bodyEffectContainer.transform.localPosition = center;
        bodyEffectContainer.transform.localRotation = Quaternion.identity;
        bodyEffectContainer.transform.localScale = Vector3.zero;
        characterEntity.bodyEffectContainer = bodyEffectContainer.transform;

        var floorEffectContainer = new GameObject("_floorEffectContainer");
        floorEffectContainer.transform.parent = characterEntityObj.transform;
        floorEffectContainer.transform.localPosition = center + Vector3.down * bounds.extents.y;
        floorEffectContainer.transform.localRotation = Quaternion.identity;
        floorEffectContainer.transform.localScale = Vector3.zero;
        characterEntity.floorEffectContainer = floorEffectContainer.transform;

        var damageContainer = new GameObject("_damageContainer");
        damageContainer.transform.parent = characterEntityObj.transform;
        damageContainer.transform.localPosition = center;
        damageContainer.transform.localRotation = Quaternion.identity;
        damageContainer.transform.localScale = Vector3.zero;
        characterEntity.damageContainer = damageContainer.transform;

        // Setup rigidbody
        rigidbody.useGravity = false;

        // Setup capsule collider 
        collider.height = height;
        collider.center = center;
        collider.radius = (bounds.size.x + bounds.size.z) / 4f;

        // Set controller
        var controller = new AnimatorOverrideController(Resources.Load("Animation/_CharacterAnimatorController") as RuntimeAnimatorController);
        // Save animator controller
        var controllerPath = path + "\\" + fileName + ".overrideController";
        Debug.Log("Saving controller to " + controllerPath);
        AssetDatabase.DeleteAsset(controllerPath);
        AssetDatabase.CreateAsset(controller, controllerPath);
        controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(controllerPath);
        controller["_Idle"] = selectedIdleClip;
        controller["_Move"] = selectedMoveClip;
        controller["_Hurt"] = selectedHurtClip;
        controller["_Dead"] = selectedDeadClip;

        // Set animator
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;

        // Save character entity
        var prefabPath = path + "\\" + fileName + ".prefab";
        Debug.Log("Saving prefab to " + prefabPath);
        AssetDatabase.DeleteAsset(prefabPath);
        var savedPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(characterEntityObj, prefabPath, InteractionMode.AutomatedAction);

        if (copyingItemData != null && savedPrefab != null)
        {
            // Create item data
            var data = CreateInstance<CharacterItem>();
            var assetPath = path + "\\" + fileName + ".asset";
            Debug.Log("Saving data to " + assetPath);
            data.title = copyingItemData.title;
            data.description = copyingItemData.description;
            data.icon = copyingItemData.icon;
            data.category = copyingItemData.category;
            data.maxStack = copyingItemData.maxStack;
            data.useFixSellPrice = copyingItemData.useFixSellPrice;
            data.fixSellPrice = copyingItemData.fixSellPrice;
            data.useFixLevelUpPrice = copyingItemData.useFixLevelUpPrice;
            data.fixLevelUpPrice = copyingItemData.fixLevelUpPrice;
            data.useFixRewardExp = copyingItemData.useFixRewardExp;
            data.fixRewardExp = copyingItemData.fixRewardExp;
            data.itemTier = copyingItemData.itemTier;
            data.attributes = copyingItemData.attributes.Clone();
            int i = 0;
            List<BaseAttackAnimationData> attackAnimations = new List<BaseAttackAnimationData>();
            foreach (var copyingAnim in copyingItemData.attackAnimations)
            {
                var copiedAnim = Instantiate(copyingAnim);
                var animPath = path + "\\" + fileName + "_atk_" + i + ".asset";
                AssetDatabase.DeleteAsset(animPath);
                AssetDatabase.CreateAsset(copiedAnim, animPath);
                var createdAnim = AssetDatabase.LoadAssetAtPath<BaseAttackAnimationData>(animPath);
                attackAnimations.Add(createdAnim);
                i++;
            }
            i = 0;
            List<BaseSkill> skills = new List<BaseSkill>();
            foreach (var copyingSkill in copyingItemData.skills)
            {
                var copiedSkill = Instantiate(copyingSkill);
                var skillPath = path + "\\" + fileName + "_skill_" + i + ".asset";
                AssetDatabase.DeleteAsset(skillPath);
                AssetDatabase.CreateAsset(copiedSkill, skillPath);
                var createdSkill = AssetDatabase.LoadAssetAtPath<BaseSkill>(skillPath);
                skills.Add(createdSkill);
                i++;
            }
            data.attackAnimations = attackAnimations;
            data.skills = skills;
            //data.evolveInfo = (CharacterItemEvolve)copyingItemData.evolveInfo.Clone();
            data.model = savedPrefab.GetComponent<CharacterEntity>();
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(data, assetPath);
        }
        Close();
    }

    Bounds GetMeshesBounds(GameObject gameObject)
    {
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        Bounds? bounds = null;
        foreach (var renderer in renderers)
        {
            if (!bounds.HasValue)
                bounds = renderer.bounds;
            else
                bounds.Value.Encapsulate(renderer.bounds);
        }
        return bounds.Value;
    }
}
