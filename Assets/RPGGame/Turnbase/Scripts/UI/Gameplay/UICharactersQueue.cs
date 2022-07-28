using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UICharactersQueue : MonoBehaviour
{
    public Transform uiCharacterStatsContainer;
    public UICharacterStats uiCharacterStatsPrefab;
    public float moveProgressDuration = 1f;

    public GamePlayManager CastedManager { get { return BaseGamePlayManager.Singleton as GamePlayManager; } }
    public Slider ProgressSlider { get; private set; }

    public CharacterEntity ActiveCharacter
    {
        get { return CastedManager.ActiveCharacter; }
    }

    public bool IsPlayerCharacterActive
    {
        get { return ActiveCharacter != null && ActiveCharacter.IsPlayerCharacter; }
    }

    private Dictionary<string, UICharacterStats> UIStats = new Dictionary<string, UICharacterStats>();

    private void Awake()
    {
        ProgressSlider = GetComponent<Slider>();
        ProgressSlider.interactable = false;
        ProgressSlider.minValue = 0;
        ProgressSlider.maxValue = 1;
    }

    private void LateUpdate()
    {
        List<UICharacterStats> uiStats = new List<UICharacterStats>(UIStats.Values);
        foreach (var ui in uiStats)
        {
            if (ui == null) continue;
            if (ui.character != null && ui.character.Hp > 0)
                ui.Show();
            else
                ui.Hide();
        }
    }

    public void NewWavePrepared()
    {
        UIStats.Clear();
        uiCharacterStatsContainer.RemoveAllChildren();
        List<BaseCharacterEntity> characters = new List<BaseCharacterEntity>();
        characters.AddRange(CastedManager.teamAFormation.Characters.Values);
        characters.AddRange(CastedManager.teamBFormation.Characters.Values);
        characters.Sort((a, b) =>
        {
            return (a as CharacterEntity).CurrentTimeCount.CompareTo((b as CharacterEntity).CurrentTimeCount);
        });
        for (int i = 0; i < characters.Count; ++i)
        {
            UICharacterStats uiStats = Instantiate(uiCharacterStatsPrefab);
            uiStats.transform.SetParent(uiCharacterStatsContainer);
            uiStats.transform.localScale = Vector3.one;
            uiStats.transform.localPosition = Vector3.zero;
            uiStats.character = characters[i];
            uiStats.notFollowCharacter = true;
            UIStats[characters[i].Item.Id] = uiStats;
        }
        StartCoroutine(MoveProgresses(characters, (characters[characters.Count - 1] as CharacterEntity).CurrentTimeCount, true));
    }

    public void CharacterActivated()
    {
        List<BaseCharacterEntity> characters = new List<BaseCharacterEntity>();
        characters.AddRange(CastedManager.teamAFormation.Characters.Values);
        characters.AddRange(CastedManager.teamBFormation.Characters.Values);
        characters.Sort((a, b) =>
        {
            return (a as CharacterEntity).CurrentTimeCount.CompareTo((b as CharacterEntity).CurrentTimeCount);
        });
        StartCoroutine(MoveProgresses(characters, (characters[characters.Count - 1] as CharacterEntity).CurrentTimeCount, false));
    }

    private IEnumerator MoveProgresses(List<BaseCharacterEntity> characters, float highestCurrentTimeCount, bool setPositionImmediately)
    {
        BaseCharacterEntity activeCharacter = CastedManager.ActiveCharacter;
        float elapsed = 0f;
        Dictionary<string, float> velocities = new Dictionary<string, float>();
        do
        {
            yield return 0;
            elapsed += Time.deltaTime;
            for (int i = 0; i < characters.Count; ++i)
            {
                BaseCharacterEntity character = characters[i];
                string id = character.Item.Id;
                if (!UIStats.ContainsKey(id)) continue;
                var uiStats = UIStats[id];
                uiStats.transform.SetAsLastSibling();
                if (activeCharacter == character)
                {
                    ProgressSlider.value = 0;
                    uiStats.transform.position = ProgressSlider.handleRect.position;
                    activeCharacter = null;
                }
                // Set slider
                if ((character as CharacterEntity).CurrentTimeCount <= 0 || highestCurrentTimeCount <= 0)
                    ProgressSlider.value = 0;
                else
                    ProgressSlider.value = (float)(character as CharacterEntity).CurrentTimeCount / highestCurrentTimeCount;
                // Find velocity
                float v;
                if (!velocities.TryGetValue(id, out v))
                {
                    var s = Vector3.Distance(uiStats.transform.position, ProgressSlider.handleRect.position);
                    velocities[id] = v = s / moveProgressDuration;
                }
                // Move icon
                if (setPositionImmediately)
                    uiStats.transform.position = ProgressSlider.handleRect.position;
                else
                    uiStats.transform.position = Vector3.MoveTowards(uiStats.transform.position, ProgressSlider.handleRect.position, Time.deltaTime * v);
            }
        } while (elapsed < moveProgressDuration && !setPositionImmediately);
    }
}
