using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimItemLevelUp : MonoBehaviour
{
    public UIItem uiLevelUpItem;
    public UIItem[] uiMaterials;
    public UILevel uiLevel;
    public Animator animAnimator;
    public float gageDuration = 0.75f;
    [Tooltip("Total duration = `animationDurationEachMaterials` * {materials amount} + `extraAnimationDuration`")]
    public float animationDurationEachMaterials = 0.8f;
    [Tooltip("Total duration = `animationDurationEachMaterials` * {materials amount} + `extraAnimationDuration`")]
    public float extraAnimationDuration = 0f;
    public ObjectsActivatingByItemTiers objectsActivatingByItemTiers = new ObjectsActivatingByItemTiers();
    public UnityEvent onLevelUp = new UnityEvent();
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onEnd = new UnityEvent();

    private PlayerItem oldItem;
    private PlayerItem newItem;
    private float collectExp;
    private float targetExp;
    private float nextExp;
    private bool isLevelUp;
    private int afterLevelUpLevel;
    private float afterLevelUpCollectExp;
    private float afterLevelUpTargetExp;
    private float afterLevelUpNextExp;
    private float increaseExpPerFrame;
    private bool isPlayedLevelUp;
    private bool isPlaying;
    private bool isEnd;

    public void Play(PlayerItem oldItem, PlayerItem newItem, List<PlayerItem> materials)
    {
        objectsActivatingByItemTiers.Activate(oldItem.Tier);

        if (oldItem.Exp == newItem.Exp)
            return;

        onStart.Invoke();
        this.oldItem = oldItem;
        this.newItem = newItem;
        uiLevelUpItem.SetData(oldItem);
        for (var i = 0; i < uiMaterials.Length; ++i)
        {
            var uiMaterial = uiMaterials[i];
            if (i < materials.Count)
            {
                uiMaterial.SetData(materials[i]);
                uiMaterial.Show();
            }
            else
            {
                uiMaterial.Hide();
            }
        }

        uiLevel.gameObject.SetActive(oldItem.ActorItemData != null);
        uiLevel.level = oldItem.Level;
        uiLevel.maxLevel = oldItem.MaxLevel;
        uiLevel.currentExp = oldItem.CurrentExp;
        uiLevel.requiredExp = oldItem.RequiredExp;
        // Setup data to play animation
        collectExp = oldItem.CurrentExp;
        targetExp = oldItem.CurrentExp + (newItem.Exp - oldItem.Exp);
        nextExp = oldItem.RequiredExp;
        // Level up, prepare data for next level
        isLevelUp = newItem.Level > oldItem.Level;
        if (isLevelUp)
            targetExp = oldItem.RequiredExp;
        increaseExpPerFrame = (targetExp - collectExp) / gageDuration;
        afterLevelUpLevel = newItem.Level;
        afterLevelUpCollectExp = 0;
        afterLevelUpTargetExp = newItem.CurrentExp;
        afterLevelUpNextExp = newItem.RequiredExp;
        isPlayedLevelUp = false;
        isPlaying = false;
        isEnd = false;
        // Enable update function after delay
        gameObject.SetActive(true);

        if (animAnimator != null)
            animAnimator.SetInteger("MaterialsAmount", materials.Count);

        StartCoroutine(DelayBeforePlay(animationDurationEachMaterials * materials.Count + extraAnimationDuration));
    }

    IEnumerator DelayBeforePlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying || isEnd)
            return;

        collectExp += Time.deltaTime * increaseExpPerFrame;
        if (collectExp >= targetExp && !isPlayedLevelUp && isLevelUp)
        {
            isPlayedLevelUp = true;
            uiLevel.level = afterLevelUpLevel;
            collectExp = afterLevelUpCollectExp;
            targetExp = afterLevelUpTargetExp;
            nextExp = afterLevelUpNextExp;
            increaseExpPerFrame = (targetExp - collectExp) / gageDuration;
            onLevelUp.Invoke();
        }

        if (collectExp >= targetExp)
        {
            collectExp = targetExp;
            if (!isEnd && (!isLevelUp || (isLevelUp && isPlayedLevelUp)))
            {
                // End, after end it will not after and not able to skip
                isEnd = true;
                uiLevelUpItem.SetData(newItem);
                onEnd.Invoke();
            }
        }
        uiLevel.currentExp = (int)collectExp;
        uiLevel.requiredExp = (int)nextExp;
        // Smooth fill amount
        if (uiLevel.imageExpGage != null)
            uiLevel.imageExpGage.fillAmount = collectExp / nextExp;
    }

    public void Skip()
    {
        if (isEnd)
            return;

        isEnd = true;
        // When skip, stop update and set current exp, target exp and level to last result
        collectExp = targetExp;
        if (isLevelUp)
        {
            uiLevel.level = afterLevelUpLevel;
            collectExp = targetExp = afterLevelUpTargetExp;
            onLevelUp.Invoke();
        }
        uiLevelUpItem.SetData(newItem);
        onEnd.Invoke();
    }
}
