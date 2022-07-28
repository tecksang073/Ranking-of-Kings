using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GamePlayManager : BaseGamePlayManager
{
    public Camera inputCamera;
    [Header("Formation/Spawning")]
    [FormerlySerializedAs("playerFormation")]
    public GamePlayFormation teamAFormation;
    [FormerlySerializedAs("foeFormation")]
    public GamePlayFormation teamBFormation;
    public EnvironmentManager environmentManager;
    public Transform mapCenter;
    public float spawnOffset = 5f;
    [Header("Speed/Delay")]
    public float formationMoveSpeed = 5f;
    public float doActionMoveSpeed = 8f;
    public float actionDoneMoveSpeed = 10f;
    public float beforeMoveToNextWaveDelay = 2f;
    public float moveToNextWaveDelay = 1f;
    [Header("UI")]
    public Transform uiCharacterStatsContainer;
    public UICharacterStats uiCharacterStatsPrefab;
    public UICharacterStats uiFoeStatsPrefab;
    public UICharacterStats uiHelperStatsPrefab;
    public UICharactersQueue uiCharactersQueue;
    public UICharacterActionManager uiCharacterActionManager;
    public CharacterEntity ActiveCharacter { get; protected set; }
    public int CurrentWave { get; protected set; }
    public Stage CastedStage { get { return PlayingStage as Stage; } }
    public int MaxWave
    { 
        get
        { 
            if (BattleType == EBattleType.Stage)
                return CastedStage.waves.Length;
            return 1;
        }
    }

    public Vector3 MapCenterPosition
    {
        get
        {
            if (mapCenter == null)
                return Vector3.zero;
            return mapCenter.position;
        }
    }
    public GamePlayFormation CurrentTeamFormation { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        if (inputCamera == null)
            inputCamera = Camera.main;
        if (uiFoeStatsPrefab == null)
            uiFoeStatsPrefab = uiCharacterStatsPrefab;
        if (uiHelperStatsPrefab == null)
            uiHelperStatsPrefab = uiCharacterStatsPrefab;
        // Setup uis
        uiCharacterActionManager.Hide();
        SetupTeamAFormation();
        SetupTeamBFormation();
        SetupEnvironment();
    }

    protected virtual void SetupTeamAFormation()
    {
        teamAFormation.ClearCharacters();
        teamAFormation.foeFormation = teamBFormation;
        teamAFormation.SetFormationCharacters(BattleType);
        CurrentTeamFormation = teamAFormation;
    }

    protected virtual void SetupTeamBFormation()
    {
        teamBFormation.ClearCharacters();
        teamBFormation.foeFormation = teamAFormation;
    }

    public virtual void SetupEnvironment()
    {
        if (BattleType == EBattleType.Stage)
            environmentManager.spawningObjects = CastedStage.environment.environmentObjects;
        else if (BattleType == EBattleType.Arena)
            environmentManager.spawningObjects = (GameInstance.GameDatabase.arenaEnvironment as EnvironmentData).environmentObjects;
        else if (BattleType == EBattleType.RaidBoss)
            environmentManager.spawningObjects = (PlayingRaidEvent.RaidBossStage as RaidBossStage).environment.environmentObjects;
        else if (BattleType == EBattleType.ClanBoss)
            environmentManager.spawningObjects = (PlayingClanEvent.ClanBossStage as ClanBossStage).environment.environmentObjects;
        environmentManager.SpawnObjects();
        environmentManager.isPause = true;
    }

    protected virtual void Start()
    {
        CurrentWave = 0;
        StartCoroutine(StartGame());
    }

    protected virtual void Update()
    {
        if (uiPauseGame.IsVisible())
        {
            Time.timeScale = 0;
            return;
        }

        if (IsAutoPlay != isAutoPlayDirty)
        {
            if (IsAutoPlay)
            {
                uiCharacterActionManager.Hide();
                if (ActiveCharacter != null)
                    ActiveCharacter.RandomAction();
            }
            isAutoPlayDirty = IsAutoPlay;
        }

        Time.timeScale = !isEnding && IsSpeedMultiply ? 2 : 1;

        if (Input.GetMouseButtonDown(0) && ActiveCharacter != null && ActiveCharacter.IsPlayerCharacter)
        {
            // Select target character
            Ray ray = inputCamera.ScreenPointToRay(InputManager.MousePosition());
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo))
                return;

            var targetCharacter = hitInfo.collider.GetComponent<CharacterEntity>();
            if (targetCharacter != null)
            {
                if (ActiveCharacter.DoAction(targetCharacter))
                {
                    teamAFormation.SetCharactersSelectable(false);
                    teamBFormation.SetCharactersSelectable(false);
                }
            }
        }
    }

    protected IEnumerator StartGame()
    {
        yield return new WaitForEndOfFrame();
        if (BattleType == EBattleType.Stage)
        {
            yield return CurrentTeamFormation.MoveCharactersToFormation(true);
            environmentManager.isPause = false;
            yield return CurrentTeamFormation.ForceCharactersPlayMoving(moveToNextWaveDelay);
            environmentManager.isPause = true;
            NextWave();
            yield return CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
            if (CurrentTeamFormation.foeFormation.Characters.Count > 0)
            {
                NewTurn();
            }
            else
            {
                if (CurrentWave >= CastedStage.waves.Length)
                {
                    StartCoroutine(WinGameRoutine());
                }
                StartCoroutine(MoveToNextWave());
            }
        }
        else if (BattleType == EBattleType.Arena)
        {
            CurrentTeamFormation.MoveCharactersToFormation(false);
            NextWave();
            CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
            yield return new WaitForSeconds(moveToNextWaveDelay);
            if (CurrentTeamFormation.foeFormation.Characters.Count > 0)
            {
                NewTurn();
            }
            else
            {
                StartCoroutine(WinGameRoutine());
            }
        }
        else if (BattleType == EBattleType.RaidBoss)
        {
            yield return CurrentTeamFormation.MoveCharactersToFormation(true);
            environmentManager.isPause = false;
            yield return CurrentTeamFormation.ForceCharactersPlayMoving(moveToNextWaveDelay);
            environmentManager.isPause = true;
            NextWave();
            yield return CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
            if (CurrentTeamFormation.foeFormation.Characters.Count > 0)
            {
                NewTurn();
            }
            else
            {
                StartCoroutine(WinGameRoutine());
            }
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            yield return CurrentTeamFormation.MoveCharactersToFormation(true);
            environmentManager.isPause = false;
            yield return CurrentTeamFormation.ForceCharactersPlayMoving(moveToNextWaveDelay);
            environmentManager.isPause = true;
            NextWave();
            yield return CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
            if (CurrentTeamFormation.foeFormation.Characters.Count > 0)
            {
                NewTurn();
            }
            else
            {
                StartCoroutine(WinGameRoutine());
            }
        }
    }

    public virtual void NextWave()
    {
        PlayerItem[] characters = null;
        StageFoe[] foes = null;
        List<int> bossIndexes = new List<int>();
        if (BattleType == EBattleType.Stage)
        {
            var wave = CastedStage.waves[CurrentWave];
            if (!wave.useRandomFoes && wave.foes.Length > 0)
                foes = wave.foes;
            else
                foes = CastedStage.RandomFoes().foes;

            characters = new PlayerItem[foes.Length];
            for (var i = 0; i < characters.Length; ++i)
            {
                var foe = foes[i];
                if (foe.character != null)
                {
                    var character = PlayerItem.CreateActorItemWithLevel(foe.character, foe.level);
                    characters[i] = character;
                    if (foe.isBoss)
                        bossIndexes.Add(i);
                }
            }
        }
        else if (BattleType == EBattleType.Arena)
        {
            characters = ArenaOpponentCharacters.ToArray();
        }
        else if (BattleType == EBattleType.RaidBoss)
        {
            characters = new PlayerItem[] { PlayingRaidEvent.RaidBossStage.GetCharacter() };
            bossIndexes.Add(0);
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            characters = new PlayerItem[] { PlayingClanEvent.ClanBossStage.GetCharacter() };
            bossIndexes.Add(0);
        }

        if (characters == null || characters.Length == 0)
            Debug.LogError("Missing Foes Data");

        CurrentTeamFormation.foeFormation.SetCharacters(characters, bossIndexes);
        CurrentTeamFormation.foeFormation.Revive();

        if (BattleType == EBattleType.RaidBoss)
        {
            // Change character's current HP
            CurrentTeamFormation.foeFormation.Characters[0].Hp = PlayingRaidEvent.RemainingHp;
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            // Change character's current HP
            CurrentTeamFormation.foeFormation.Characters[0].Hp = PlayingClanEvent.RemainingHp;
        }

        ++CurrentWave;

        if (uiCharactersQueue != null)
            uiCharactersQueue.NewWavePrepared();
    }

    protected IEnumerator MoveToNextWave()
    {
        yield return new WaitForSeconds(beforeMoveToNextWaveDelay);
        CurrentTeamFormation.foeFormation.ClearCharacters();
        CurrentTeamFormation.SetActiveDeadCharacters(false);
        environmentManager.isPause = false;
        yield return CurrentTeamFormation.ForceCharactersPlayMoving(moveToNextWaveDelay);
        environmentManager.isPause = true;
        CurrentTeamFormation.SetActiveDeadCharacters(true);
        NextWave();
        yield return CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
        NewTurn();
    }

    public virtual void UpdateActivatingCharacter()
    {
        if (ActiveCharacter != null)
            ActiveCharacter.CurrentTimeCount = 0;
        CharacterEntity activatingCharacter = null;
        List<BaseCharacterEntity> characters = new List<BaseCharacterEntity>();
        characters.AddRange(teamAFormation.Characters.Values);
        characters.AddRange(teamBFormation.Characters.Values);
        var higestTimeCount = int.MinValue;
        for (int i = 0; i < characters.Count; ++i)
        {
            CharacterEntity character = characters[i] as CharacterEntity;
            if (character != null)
            {
                if (character.Hp > 0)
                {
                    int spd = (int)character.GetTotalAttributes().spd;
                    if (spd <= 0)
                        spd = 1;
                    character.CurrentTimeCount += spd;
                    if (character.CurrentTimeCount > higestTimeCount)
                    {
                        higestTimeCount = character.CurrentTimeCount;
                        activatingCharacter = character;
                    }
                }
                else
                {
                    character.CurrentTimeCount = 0;
                }
            }
        }
        if (uiCharactersQueue != null)
            uiCharactersQueue.CharacterActivated();
        ActiveCharacter = activatingCharacter;
    }

    public virtual void NewTurn()
    {
        UpdateActivatingCharacter();
        ActiveCharacter.DecreaseBuffsTurn();
        ActiveCharacter.DecreaseSkillsTurn();
        ActiveCharacter.ResetStates();
        if (ActiveCharacter.Hp > 0 &&
            !ActiveCharacter.IsStun)
        {
            if (ActiveCharacter.IsPlayerCharacter)
            {
                if (IsAutoPlay)
                    ActiveCharacter.RandomAction();
                else
                    uiCharacterActionManager.Show();
            }
            else
                ActiveCharacter.RandomAction();
        }
        else
            ActiveCharacter.NotifyEndAction();
    }

    /// <summary>
    /// This will be called by Character class to show target scopes or do action
    /// </summary>
    /// <param name="character"></param>
    public void ShowTargetScopesOrDoAction(CharacterEntity character)
    {
        var allyTeamFormation = character.IsPlayerCharacter ? CurrentTeamFormation : CurrentTeamFormation.foeFormation;
        var foeTeamFormation = !character.IsPlayerCharacter ? CurrentTeamFormation : CurrentTeamFormation.foeFormation;
        allyTeamFormation.SetCharactersSelectable(false);
        foeTeamFormation.SetCharactersSelectable(false);
        if (character.Action == CharacterEntity.ACTION_ATTACK)
            foeTeamFormation.SetCharactersSelectable(true);
        else
        {
            switch (character.SelectedSkill.CastedSkill.usageScope)
            {
                case SkillUsageScope.Self:
                    character.Selectable = true;
                    break;
                case SkillUsageScope.Ally:
                    allyTeamFormation.SetCharactersSelectable(true);
                    break;
                case SkillUsageScope.Enemy:
                    foeTeamFormation.SetCharactersSelectable(true);
                    break;
                case SkillUsageScope.All:
                    allyTeamFormation.SetCharactersSelectable(true);
                    foeTeamFormation.SetCharactersSelectable(true);
                    break;
                case SkillUsageScope.DeadAlly:
                    allyTeamFormation.SetCharactersSelectable(true, true);
                    break;
            }
        }
    }

    public List<BaseCharacterEntity> GetAllCharacters(bool deadCharacter = false)
    {
        List<BaseCharacterEntity> result = new List<BaseCharacterEntity>();
        if (deadCharacter)
        {
            result.AddRange(teamAFormation.Characters.Values.Where(a => a.Hp <= 0).ToList());
            result.AddRange(teamBFormation.Characters.Values.Where(a => a.Hp <= 0).ToList());
        }
        else
        {
            result.AddRange(teamAFormation.Characters.Values.Where(a => a.Hp > 0).ToList());
            result.AddRange(teamBFormation.Characters.Values.Where(a => a.Hp > 0).ToList());
        }
        return result;
    }

    public List<BaseCharacterEntity> GetAllies(CharacterEntity character, bool deadCharacter = false)
    {
        if (deadCharacter)
        {
            if (character.IsPlayerCharacter)
                return CurrentTeamFormation.Characters.Values.Where(a => a.Hp <= 0).ToList();
            else
                return CurrentTeamFormation.foeFormation.Characters.Values.Where(a => a.Hp <= 0).ToList();
        }
        else
        {
            if (character.IsPlayerCharacter)
                return CurrentTeamFormation.Characters.Values.Where(a => a.Hp > 0).ToList();
            else
                return CurrentTeamFormation.foeFormation.Characters.Values.Where(a => a.Hp > 0).ToList();
        }
    }

    public List<BaseCharacterEntity> GetFoes(CharacterEntity character, bool deadCharacter = false)
    {
        if (deadCharacter)
        {
            if (character.IsPlayerCharacter)
                return CurrentTeamFormation.foeFormation.Characters.Values.Where(a => a.Hp <= 0).ToList();
            else
                return CurrentTeamFormation.Characters.Values.Where(a => a.Hp <= 0).ToList();
        }
        else
        {
            if (character.IsPlayerCharacter)
                return CurrentTeamFormation.foeFormation.Characters.Values.Where(a => a.Hp > 0).ToList();
            else
                return CurrentTeamFormation.Characters.Values.Where(a => a.Hp > 0).ToList();
        }
    }

    public virtual void NotifyEndAction(CharacterEntity character)
    {
        if (character != ActiveCharacter)
            return;

        if (!CurrentTeamFormation.IsAnyCharacterAlive())
        {
            ActiveCharacter = null;
            StartCoroutine(LoseGameRoutine());
        }
        else if (!CurrentTeamFormation.foeFormation.IsAnyCharacterAlive())
        {
            ActiveCharacter = null;
            switch (BattleType) 
            {
                case EBattleType.Stage:
                    if (CurrentWave >= CastedStage.waves.Length)
                    {
                        StartCoroutine(WinGameRoutine());
                        return;
                    }
                    break;
                case EBattleType.Arena:
                case EBattleType.RaidBoss:
                case EBattleType.ClanBoss:
                    // Arena or Raid Boss or Clan Boss, Win immediately, there is 1 wave
                    StartCoroutine(WinGameRoutine());
                    return;
            }
            StartCoroutine(MoveToNextWave());
        }
        else
            NewTurn();
    }

    public override void OnRevive()
    {
        base.OnRevive();
        CurrentTeamFormation.Revive();
        NewTurn();
    }

    public override int CountDeadCharacters()
    {
        return CurrentTeamFormation.CountDeadCharacters();
    }

    public override BaseCharacterEntity GetBossCharacterEntity()
    {
        foreach (var character in CurrentTeamFormation.foeFormation.Characters.Values)
        {
            if (character.IsBoss && character.Hp > 0f)
                return character;
        }
        return null;
    }
}
