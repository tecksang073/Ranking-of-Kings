using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayFormation : BaseGamePlayFormation
{
    public GamePlayManager CastedManager { get { return BaseGamePlayManager.Singleton as GamePlayManager; } }
    public GamePlayFormation foeFormation;
    public readonly Dictionary<int, UICharacterStats> UIStats = new Dictionary<int, UICharacterStats>();

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == GameInstance.Singleton.manageScene)
            SetFormationCharactersForStage();
    }

    public override BaseCharacterEntity SetCharacter(int position, PlayerItem item, bool isBoss)
    {
        var character = base.SetCharacter(position, item, isBoss) as CharacterEntity;

        if (character == null)
            return null;

        UICharacterStats uiStats;
        if (UIStats.TryGetValue(position, out uiStats))
        {
            Destroy(uiStats.gameObject);
            UIStats.Remove(position);
        }

        if (CastedManager != null)
        {
            UICharacterStats prefab;
            if (CastedManager.CurrentTeamFormation == this)
                prefab = CastedManager.uiCharacterStatsPrefab;
            else
                prefab = CastedManager.uiFoeStatsPrefab;
            InstantiateStatsUI(prefab, CastedManager.uiCharacterStatsContainer, character);
        }

        return character;
    }

    public override BaseCharacterEntity SetHelperCharacter(PlayerItem item)
    {
        var character = base.SetHelperCharacter(item) as CharacterEntity;

        if (character == null)
            return null;

        var position = character.Position;

        UICharacterStats uiStats;
        if (UIStats.TryGetValue(position, out uiStats))
        {
            Destroy(uiStats.gameObject);
            UIStats.Remove(position);
        }

        if (CastedManager != null)
        {
            UICharacterStats prefab = CastedManager.uiHelperStatsPrefab;
            InstantiateStatsUI(prefab, CastedManager.uiCharacterStatsContainer, character);
        }

        return character;
    }

    public void InstantiateStatsUI(UICharacterStats prefab, Transform container, CharacterEntity entity)
    {
        if (prefab != null && container != null)
        {
            UICharacterStats uiStats = Instantiate(prefab);
            uiStats.Attach(container, entity);
            uiStats.Show();
            entity.UiCharacterStats = uiStats;
        }
    }

    public void Revive()
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            character.Revive();
        }
    }

    public bool IsAnyCharacterAlive()
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            if (character.Hp > 0)
                return true;
        }
        return false;
    }

    public bool TryGetHeadingToFoeRotation(out Quaternion rotation)
    {
        if (foeFormation != null)
        {
            var rotateHeading = foeFormation.transform.position - transform.position;
            rotation = Quaternion.LookRotation(rotateHeading);
            return true;
        }
        rotation = Quaternion.identity;
        return false;
    }

    public Coroutine MoveCharactersToFormation(bool stillForceMoving)
    {
        return StartCoroutine(MoveCharactersToFormationRoutine(stillForceMoving));
    }

    private IEnumerator MoveCharactersToFormationRoutine(bool stillForceMoving)
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            var castedCharacter = character as CharacterEntity;
            castedCharacter.ForcePlayMoving = stillForceMoving;
            castedCharacter.MoveTo(character.Container.position, CastedManager.formationMoveSpeed);
        }
        while (true)
        {
            yield return 0;
            var ifEveryoneReachedTarget = true;
            foreach (var character in characters)
            {
                var castedCharacter = character as CharacterEntity;
                if (castedCharacter.IsMovingToTarget)
                {
                    ifEveryoneReachedTarget = false;
                    break;
                }
            }
            if (ifEveryoneReachedTarget)
                break;
        }
    }

    public void SetActiveDeadCharacters(bool isActive)
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            if (character.Hp <= 0)
                character.gameObject.SetActive(isActive);
        }
    }

    public Coroutine ForceCharactersPlayMoving(float duration)
    {
        return StartCoroutine(ForceCharactersPlayMovingRoutine(duration));
    }

    private IEnumerator ForceCharactersPlayMovingRoutine(float duration)
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            var castedCharacter = character as CharacterEntity;
            castedCharacter.ForcePlayMoving = true;
        }
        yield return new WaitForSeconds(duration);
        foreach (var character in characters)
        {
            var castedCharacter = character as CharacterEntity;
            castedCharacter.ForcePlayMoving = false;
        }
    }

    public void SetCharactersSelectable(bool selectable, bool deadCharacter = false)
    {
        var characters = Characters.Values;
        foreach (var character in characters)
        {
            var castedCharacter = character as CharacterEntity;
            if (deadCharacter && castedCharacter.Hp > 0)
            {
                castedCharacter.Selectable = false;
                continue;
            }
            if (!deadCharacter && castedCharacter.Hp <= 0)
            {
                castedCharacter.Selectable = false;
                continue;
            }
            castedCharacter.Selectable = selectable;
        }
    }

    public int CountDeadCharacters()
    {
        return Characters.Values.Where(a => a.Hp <= 0).ToList().Count;
    }
}
