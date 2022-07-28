using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGamePlayManager : MonoBehaviour
{
    public static BaseGamePlayManager Singleton { get; private set; }
    public static string BattleSession { get; private set; }
    public static BaseStage PlayingStage { get; protected set; }
    public static RaidEvent PlayingRaidEvent { get; protected set; }
    public static ClanEvent PlayingClanEvent { get; protected set; }
    public static Player Helper { get; protected set; }
    public static EBattleType BattleType { get; protected set; }
    public static int TotalDamage { get; protected set; }
    public static List<PlayerItem> ArenaOpponentCharacters { get; protected set; }
    [Header("Combat Texts")]
    public Transform combatTextContainer;
    public UICombatText combatDamagePrefab;
    public UICombatText combatCriticalPrefab;
    public UICombatText combatBlockPrefab;
    public UICombatText combatHealPrefab;
    public UICombatText combatPoisonPrefab;
    public UICombatText combatMissPrefab;
    public UICombatText combatResistPrefab;
    [Header("Gameplay UI")]
    public UIWin uiWin;
    public UILose uiLose;
    public UIArenaResult uiArenaResult;
    public UIRaidEventResult uiRaidEventResult;
    public UIClanEventResult uiClanEventResult;
    public UIPlayer uiFriendRequest;
    public UIPauseGame uiPauseGame;
    public float winGameDelay = 2f;
    public float loseGameDelay = 2f;
    public float multiHitCountCombatTextSpawnDelay = 0.5f;

    private bool? isAutoPlay;
    public bool IsAutoPlay
    {
        get
        {
            if (!isAutoPlay.HasValue)
                isAutoPlay = PlayerPrefs.GetInt(Consts.KeyIsAutoPlay, 0) > 0;
            return isAutoPlay.Value;
        }
        set
        {
            isAutoPlay = value;
            PlayerPrefs.SetInt(Consts.KeyIsAutoPlay, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    private bool? isSpeedMultiply;
    public bool IsSpeedMultiply
    {
        get
        {
            if (!isSpeedMultiply.HasValue)
                isSpeedMultiply = PlayerPrefs.GetInt(Consts.KeyIsSpeedMultiply, 0) > 0;
            return isSpeedMultiply.Value;
        }
        set
        {
            isSpeedMultiply = value;
            PlayerPrefs.SetInt(Consts.KeyIsSpeedMultiply, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    protected bool isAutoPlayDirty;
    protected bool isEnding;

    protected virtual void Awake()
    {
        Singleton = this;
    }

    public void SpawnDamageText(int amount, BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatAmountText(combatDamagePrefab, amount, character, hitCount));
    }

    public void SpawnCriticalText(int amount, BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatAmountText(combatCriticalPrefab, amount, character, hitCount));
    }

    public void SpawnBlockText(int amount, BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatAmountText(combatBlockPrefab, amount, character, hitCount));
    }

    public void SpawnHealText(int amount, BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatAmountText(combatHealPrefab, amount, character, hitCount));
    }

    public void SpawnPoisonText(int amount, BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatAmountText(combatPoisonPrefab, amount, character, hitCount));
    }

    public void SpawnMissText(BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatText(combatMissPrefab, LanguageManager.GetText(GameText.COMBAT_MISS), character, hitCount));
    }

    public void SpawnResistText(BaseCharacterEntity character, int hitCount)
    {
        StartCoroutine(SpawnCombatText(combatResistPrefab, LanguageManager.GetText(GameText.COMBAT_RESIST), character, hitCount));
    }

    public IEnumerator SpawnCombatAmountText(UICombatText prefab, int amount, BaseCharacterEntity character, int hitCount)
    {
        var waiting = new WaitForSeconds(multiHitCountCombatTextSpawnDelay);
        for (int i = 0; i < hitCount; ++i)
        {
            var combatText = Instantiate(prefab, combatTextContainer);
            combatText.transform.localScale = Vector3.one;
            combatText.TempObjectFollower.targetObject = character.bodyEffectContainer;
            combatText.Amount = amount;
            yield return waiting;
        }
    }

    public IEnumerator SpawnCombatText(UICombatText prefab, string text, BaseCharacterEntity character, int hitCount)
    {
        var waiting = new WaitForSeconds(multiHitCountCombatTextSpawnDelay);
        for (int i = 0; i < hitCount; ++i)
        {
            var combatText = Instantiate(prefab, combatTextContainer);
            combatText.transform.localScale = Vector3.one;
            combatText.TempObjectFollower.targetObject = character.bodyEffectContainer;
            combatText.Amount = 0;
            combatText.TempText.text = text;
            yield return waiting;
        }
    }


    protected IEnumerator WinGameRoutine()
    {
        isEnding = true;
        yield return new WaitForSeconds(winGameDelay);
        WinGame();
    }

    protected virtual void WinGame()
    {
        if (BattleType == EBattleType.Stage)
        {
            GameInstance.GameService.FinishStage(BattleSession, EBattleResult.Win, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.OnGameServiceFinishStageResult(result);
                uiWin.SetData(result);
                if (uiFriendRequest != null && Helper != null && !Helper.isFriend)
                {
                    uiFriendRequest.SetData(Helper);
                    uiFriendRequest.eventFriendRequestSuccess.AddListener(() =>
                    {
                        uiFriendRequest.Hide();
                    });
                    uiFriendRequest.eventHide.AddListener(() =>
                    {
                        uiWin.Show();
                    });
                    uiFriendRequest.Show();
                }
                else
                {
                    uiWin.Show();
                }
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.Arena)
        {
            GameInstance.GameService.FinishDuel(BattleSession, EBattleResult.Win, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.OnGameServiceFinishDuelResult(result);
                uiArenaResult.SetData(result);
                uiArenaResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.RaidBoss)
        {
            GameInstance.GameService.FinishRaidBossBattle(BattleSession, EBattleResult.Win, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                uiRaidEventResult.SetData(result);
                uiRaidEventResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            GameInstance.GameService.FinishClanBossBattle(BattleSession, EBattleResult.Win, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                uiClanEventResult.SetData(result);
                uiClanEventResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
    }

    protected IEnumerator LoseGameRoutine()
    {
        isEnding = true;
        yield return new WaitForSeconds(loseGameDelay);
        if (BattleType == EBattleType.Stage)
        {
            uiLose.Show();
        }
        else if (BattleType == EBattleType.Arena)
        {
            GameInstance.GameService.FinishDuel(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.OnGameServiceFinishDuelResult(result);
                uiArenaResult.SetData(result);
                uiArenaResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.RaidBoss)
        {
            GameInstance.GameService.FinishRaidBossBattle(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                uiRaidEventResult.SetData(result);
                uiRaidEventResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            GameInstance.GameService.FinishClanBossBattle(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                uiClanEventResult.SetData(result);
                uiClanEventResult.Show();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
    }

    public virtual void Revive(UnityAction onError)
    {
        if (BattleType == EBattleType.Stage)
        {
            GameInstance.GameService.ReviveCharacters((result) =>
            {
                OnRevive();
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, onError);
            });
        }
    }

    public virtual void Giveup(UnityAction onError)
    {
        if (BattleType == EBattleType.Stage)
        {
            GameInstance.GameService.FinishStage(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.GetAllPlayerData(GameInstance.LoadAllPlayerDataState.GoToManageScene);
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, onError);
            });
        }
        else if (BattleType == EBattleType.Arena)
        {
            GameInstance.GameService.FinishDuel(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.GetAllPlayerData(GameInstance.LoadAllPlayerDataState.GoToManageScene);
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, onError);
            });
        }
        else if (BattleType == EBattleType.RaidBoss)
        {
            GameInstance.GameService.FinishRaidBossBattle(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.GetAllPlayerData(GameInstance.LoadAllPlayerDataState.GoToManageScene);
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
        else if (BattleType == EBattleType.ClanBoss)
        {
            GameInstance.GameService.FinishClanBossBattle(BattleSession, EBattleResult.Lose, TotalDamage, CountDeadCharacters(), (result) =>
            {
                isEnding = true;
                Time.timeScale = 1;
                GameInstance.Singleton.GetAllPlayerData(GameInstance.LoadAllPlayerDataState.GoToManageScene);
            }, (error) =>
            {
                GameInstance.Singleton.OnGameServiceError(error, WinGame);
            });
        }
    }

    public virtual void Restart()
    {
        if (BattleType == EBattleType.Stage)
            StartStage(PlayingStage, Helper);
    }

    public static void StartStage(BaseStage data, Player helper)
    {
        var formationName = Player.CurrentPlayer.SelectedFormation;
        if (PlayerFormation.DataMap.Values.Where(a => a.DataId == formationName && !string.IsNullOrEmpty(a.ItemId)).Count() == 0)
            return;

        PlayingStage = data;
        Helper = helper;
        BattleType = EBattleType.Stage;
        string helperPlayerId = Helper != null ? Helper.Id : string.Empty;
        GameInstance.GameService.StartStage(data.Id, helperPlayerId, (result) =>
        {
            GameInstance.Singleton.OnGameServiceStartStageResult(result);
            BattleSession = result.session;
            TotalDamage = 0;
            GameInstance.Singleton.LoadBattleScene();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error);
        });
    }

    public static void StartDuel(string opponentId)
    {
        var formationName = Player.CurrentPlayer.SelectedArenaFormation;
        if (PlayerFormation.DataMap.Values.Where(a => a.DataId == formationName && !string.IsNullOrEmpty(a.ItemId)).Count() == 0)
            return;

        Helper = null;
        BattleType = EBattleType.Arena;
        GameInstance.GameService.StartDuel(opponentId, (result) =>
        {
            GameInstance.Singleton.OnGameServiceStartDuelResult(result);
            BattleSession = result.session;
            TotalDamage = 0;
            ArenaOpponentCharacters = result.opponentCharacters;
            GameInstance.Singleton.LoadBattleScene();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error);
        });
    }

    public static void StartRaidBossBattle(RaidEvent data)
    {
        var formationName = Player.CurrentPlayer.SelectedFormation;
        if (PlayerFormation.DataMap.Values.Where(a => a.DataId == formationName && !string.IsNullOrEmpty(a.ItemId)).Count() == 0)
            return;

        PlayingRaidEvent = data;
        Helper = null;
        BattleType = EBattleType.RaidBoss;
        GameInstance.GameService.StartRaidBossBattle(data.Id, (result) =>
        {
            GameInstance.Singleton.OnGameServiceStartRaidBossBattleResult(result);
            BattleSession = result.session;
            TotalDamage = 0;
            GameInstance.Singleton.LoadBattleScene();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error);
        });
    }

    public static void StartClanBossBattle(ClanEvent data)
    {
        var formationName = Player.CurrentPlayer.SelectedFormation;
        if (PlayerFormation.DataMap.Values.Where(a => a.DataId == formationName && !string.IsNullOrEmpty(a.ItemId)).Count() == 0)
            return;

        PlayingClanEvent = data;
        Helper = null;
        BattleType = EBattleType.ClanBoss;
        GameInstance.GameService.StartClanBossBattle(data.Id, (result) =>
        {
            GameInstance.Singleton.OnGameServiceStartClanBossBattleResult(result);
            BattleSession = result.session;
            TotalDamage = 0;
            GameInstance.Singleton.LoadBattleScene();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error);
        });
    }

    /// <summary>
    /// Call this increase total damage which will be saved when the battle ends
    /// </summary>
    /// <param name="damage"></param>
    public static void IncreaseTotalDamage(int damage)
    {
        TotalDamage += damage;
    }

    public virtual void OnRevive()
    {
        isEnding = false;
    }

    public abstract int CountDeadCharacters();
    public abstract BaseCharacterEntity GetBossCharacterEntity();
}
