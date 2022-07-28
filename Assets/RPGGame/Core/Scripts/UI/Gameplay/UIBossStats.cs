using UnityEngine;

[RequireComponent(typeof(UICharacterStatsGeneric))]
public class UIBossStats : MonoBehaviour
{
    public UICharacterStatsGeneric CacheUIStats { get; private set; }

    private void Awake()
    {
        CacheUIStats = GetComponent<UICharacterStatsGeneric>();
        CacheUIStats.notFollowCharacter = true;
        CacheUIStats.hideIfCharacterIsBoss = false;
    }

    private void Update()
    {
        CacheUIStats.character = BaseGamePlayManager.Singleton.GetBossCharacterEntity();
        if (CacheUIStats.character != null)
            CacheUIStats.ShowCharacterStats();
        else
            CacheUIStats.HideCharacterStats();
    }
}
