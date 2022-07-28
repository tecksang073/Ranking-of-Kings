using UnityEngine;

public class UIFormationCharacters : MonoBehaviour
{
    public Transform uiCharacterStatsContainer;
    public UICharacterStatsGeneric uiCharacterStatsPrefab;

    public void SetFormation(BaseGamePlayFormation formation)
    {
        uiCharacterStatsContainer.RemoveAllChildren();
        for (int i = 0; i < formation.Characters.Count; ++i)
        {
            UICharacterStatsGeneric uiStats = Instantiate(uiCharacterStatsPrefab);
            uiStats.transform.SetParent(uiCharacterStatsContainer);
            uiStats.transform.localScale = Vector3.one;
            uiStats.transform.localPosition = Vector3.zero;
            uiStats.character = formation.Characters[i];
            uiStats.notFollowCharacter = true;
            uiStats.Show();
        }
    }
}
