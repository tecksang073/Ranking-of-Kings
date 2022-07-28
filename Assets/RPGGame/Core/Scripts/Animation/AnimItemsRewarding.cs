using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimItemsRewarding : MonoBehaviour
{
    public AnimItemRewarding animCharacterRewarding;
    public AnimItemRewarding animEquipmentRewarding;
    public bool autoPlay;
    public float delayEachItem = 0.5f;
    private List<PlayerItem> rewards;
    private int currentIndex;
    private float lastShowTime;

    public void Play(List<PlayerItem> rewards)
    {
        currentIndex = -1;
        this.rewards = rewards;
        gameObject.SetActive(true);
        Next();
    }

    private void Update()
    {
        if (Time.time - lastShowTime < delayEachItem || !autoPlay)
            return;
        Next();
    }

    public void Next()
    {
        if (rewards == null || currentIndex + 1 >= rewards.Count)
        {
            gameObject.SetActive(false);
            if (rewards != null && rewards.Count > 0)
                GameInstance.Singleton.ShowRewardItemsDialog(rewards);
            return;
        }

        lastShowTime = Time.time;
        var reward = rewards[++currentIndex];
        if (reward.CharacterData != null && animCharacterRewarding != null)
        {
            animCharacterRewarding.Play(reward);
            return;
        }
        else
        {
            Next();
        }

        if (reward.EquipmentData != null && animEquipmentRewarding != null)
        {
            animEquipmentRewarding.Play(reward);
            return;
        }
        else
        {
            Next();
        }
    }
}
