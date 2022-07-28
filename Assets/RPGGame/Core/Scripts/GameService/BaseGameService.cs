using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract partial class BaseGameService : MonoBehaviour
{
    public const string AUTH_NORMAL = "NORMAL";
    public const string AUTH_GUEST = "GUEST";
    public UnityEvent onServiceStart;
    public UnityEvent onServiceFinish;
    public long RTT { get; private set; }
    public long ServiceTimeOffset { get; private set; }
    public long Timestamp { get { return (long)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds; } }
    private float updateTimeCounter;

    private void Update()
    {
        updateTimeCounter -= Time.unscaledDeltaTime;
        if (updateTimeCounter <= 0f)
        {
            // Update time offset every 30 seconds
            updateTimeCounter = 30f;
            GetServiceTime();
        }
    }

    public void SetPrefsLogin(string playerId, string loginToken)
    {
        string oldPlayerId = PlayerPrefs.GetString(Consts.KeyPlayerId, string.Empty);
        // Delete gameplay settings when difference player login
        if (!oldPlayerId.Equals(playerId))
        {
            PlayerPrefs.DeleteKey(Consts.KeyIsAutoPlay);
            PlayerPrefs.DeleteKey(Consts.KeyIsSpeedMultiply);
        }
        PlayerPrefs.SetString(Consts.KeyPlayerId, playerId);
        PlayerPrefs.SetString(Consts.KeyLoginToken, loginToken);
        PlayerPrefs.Save();
    }

    public string GetPrefsPlayerId()
    {
        return PlayerPrefs.GetString(Consts.KeyPlayerId);
    }

    public string GetPrefsLoginToken()
    {
        return PlayerPrefs.GetString(Consts.KeyLoginToken);
    }

    public void Logout(UnityAction onLogout = null)
    {
        Player.CurrentPlayer = null;
        Player.ClearData();
        PlayerAchievement.ClearData();
        PlayerAuth.ClearData();
        PlayerCurrency.ClearData();
        PlayerFormation.ClearData();
        PlayerItem.ClearData();
        PlayerStamina.ClearData();
        PlayerUnlockItem.ClearData();
        SetPrefsLogin("", "");
        onLogout();
    }

    public void HandleServiceCall()
    {
        onServiceStart.Invoke();
    }

    public void HandleResult<T>(T result, UnityAction<T> onSuccess, UnityAction<string> onError) where T : GameServiceResult
    {
        onServiceFinish.Invoke();
        if (result.Success)
        {
            if (onSuccess != null)
                onSuccess(result);
        }
        else
        {
            if (onError != null)
                onError(result.error);
        }
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void Register(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: Register");
        HandleServiceCall();
        DoRegister(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void Login(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: Login");
        HandleServiceCall();
        DoLogin(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// RegisterOrLogin
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void RegisterOrLogin(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: RegisterOrLogin");
        HandleServiceCall();
        DoRegisterOrLogin(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GuestLogin
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GuestLogin(string deviceId, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GuestLogin");
        HandleServiceCall();
        DoGuestLogin(deviceId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// ValidateLoginToken
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void ValidateLoginToken(bool refreshToken, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ValidateLoginToken");
        var playerId = GetPrefsPlayerId();
        var loginToken = GetPrefsLoginToken();
        HandleServiceCall();
        DoValidateLoginToken(playerId, loginToken, refreshToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SetProfileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SetProfileName(string profileName, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SetProfileName");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSetProfileName(playerId, loginToken, profileName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// LevelUpItem
    /// </summary>
    /// <param name="itemId">`playerItem.Id` for item which will levelling up</param>
    /// <param name="materials">This is Key-Value Pair for `playerItem.Id`, `Using Amount`</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void LevelUpItem(string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: LevelUpItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoLevelUpItem(playerId, loginToken, itemId, materials, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// EvolveItem
    /// </summary>
    /// <param name="itemId">`playerItem.Id` for item which will evolving</param>
    /// <param name="materials">This is Key-Value Pair for `playerItem.Id`, `Using Amount`</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void EvolveItem(string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EvolveItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoEvolveItem(playerId, loginToken, itemId, materials, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// Sell Items
    /// </summary>
    /// <param name="items">List of `playerItem.Id` for items that will selling</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SellItems(Dictionary<string, int> items, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SellItems");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSellItems(playerId, loginToken, items, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// StartStage
    /// </summary>
    /// <param name="stageDataId"></param>
    /// <param name="helperPlayerId"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void StartStage(string stageDataId, string helperPlayerId, UnityAction<StartStageResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: StartStage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoStartStage(playerId, loginToken, stageDataId, helperPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// FinishStage
    /// </summary>
    /// <param name="session"></param>
    /// <param name="battleResult"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void FinishStage(string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishStageResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FinishStage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFinishStage(playerId, loginToken, session, battleResult, totalDamage, deadCharacters, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// ReviveCharacters
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void ReviveCharacters(UnityAction<CurrencyResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ReviveCharacters");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoReviveCharacters(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SelectFormation
    /// </summary>
    /// <param name="formationName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SelectFormation(string formationName, EFormationType formationType, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SelectFormation");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSelectFormation(playerId, loginToken, formationName, formationType, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// EquipItem
    /// </summary>
    /// <param name="characterId">`playerItem.Id` for character whom equipping equipment</param>
    /// <param name="equipmentId">`playerItem.Id` for equipment which will be equipped</param>
    /// <param name="equipPosition">Equipping position</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void EquipItem(string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EquipItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoEquipItem(playerId, loginToken, characterId, equipmentId, equipPosition, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// UnEquipItem
    /// </summary>
    /// <param name="equipmentId">`playerItem.Id` for equipment which will be unequipped</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void UnEquipItem(string equipmentId, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: UnEquipItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoUnEquipItem(playerId, loginToken, equipmentId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenLootBox
    /// </summary>
    /// <param name="lootBoxDataId"></param>
    /// <param name="packIndex"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenLootBox(string lootBoxDataId, int packIndex, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: OpenLootBox");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenLootBox(playerId, loginToken, lootBoxDataId, packIndex, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenIAPPackage_iOS
    /// </summary>
    /// <param name="receipt"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenIapPackage_iOS(string iapPackageDataId, string receipt, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: OpenIAPPackage_iOS");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenIapPackage_iOS(playerId, loginToken, iapPackageDataId, receipt, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenIAPPackage_Android
    /// </summary>
    /// <param name="receipt"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenIapPackage_Android(string iapPackageDataId, string data, string signature, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: DoOpenIAPPackage_Android");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenIapPackage_Android(playerId, loginToken, iapPackageDataId, data, signature, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAchievementList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAchievementList(UnityAction<AchievementListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAchievementList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetAchievementList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAuthList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAuthList(UnityAction<AuthListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAuthList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetAuthList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetItemList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetItemList(UnityAction<ItemListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetItemList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetItemList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetCurrencyList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetCurrencyList(UnityAction<CurrencyListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetCurrencyList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetCurrencyList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetStaminaList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetStaminaList(UnityAction<StaminaListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetStaminaList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetStaminaList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetFormationList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetFormationList(UnityAction<FormationListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFormationList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFormationList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SetFormation
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="formationName"></param>
    /// <param name="position"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SetFormation(string characterId, string formationName, int position, UnityAction<FormationListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SetFormation");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSetFormation(playerId, loginToken, characterId, formationName, position, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetUnlockItemList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetUnlockItemList(UnityAction<UnlockItemListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetUnlockItemList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetUnlockItemList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetClearStageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetClearStageList(UnityAction<ClearStageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClearStageList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClearStageList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableLootBoxList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableLootBoxList");
        HandleServiceCall();
        DoGetAvailableLootBoxList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableIapPackageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableIapPackageList(UnityAction<AvailableIapPackageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableIapPackageList");
        HandleServiceCall();
        DoGetAvailableIapPackageList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableInGamePackageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableInGamePackageList(UnityAction<AvailableInGamePackageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableInGamePackageList");
        HandleServiceCall();
        DoGetAvailableInGamePackageList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableStageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableStageList(UnityAction<AvailableStageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableStageList");
        HandleServiceCall();
        DoGetAvailableStageList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetHelperList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetRandomPlayerList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetHelperList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetFriendList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFriendList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFriendList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetFriendRequestList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFriendRequestList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFriendRequestList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetPendingRequestList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetPendingRequestList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetPendingRequestList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FindUser(string displayName, UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FindUser");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFindUser(playerId, loginToken, displayName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendRequest(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FriendRequest");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendRequest(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendAccept(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FriendAccept");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendAccept(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendDecline(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FriendDecline");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendDecline(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendDelete(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FriendDelete");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendDelete(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendRequestDelete(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FriendRequestDelete");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendRequestDelete(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    private long? requestServiceTime;
    public void GetServiceTime()
    {
        if (requestServiceTime.HasValue)
            return;
        requestServiceTime = Timestamp;
        DoGetServiceTime((result) =>
        {
            RTT = Timestamp - requestServiceTime.Value;
            ServiceTimeOffset = result.serviceTime - Timestamp - RTT;
            requestServiceTime = null;
        });
    }

    public void GetArenaOpponentList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetArenaOpponentList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetArenaOpponentList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void StartDuel(string targetPlayerId, UnityAction<StartDuelResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: StartDuel");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoStartDuel(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FinishDuel(string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishDuelResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FinishDuel");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFinishDuel(playerId, loginToken, session, battleResult, totalDamage, deadCharacters, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void EarnAchievementReward(string achievementId, UnityAction<EarnAchievementResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EarnAchievementReward");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoEarnAchievementReward(playerId, loginToken, achievementId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ConvertHardCurrency(int requireHardCurrency, UnityAction<HardCurrencyConversionResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ConvertHardCurrency");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoConvertHardCurrency(playerId, loginToken, requireHardCurrency, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void OpenInGamePackage(string inGamePackageDataId, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: OpenInGamePackage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenInGamePackage(playerId, loginToken, inGamePackageDataId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void CreateClan(string clanName, UnityAction<CreateClanResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: CreateClan");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoCreateClan(playerId, loginToken, clanName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FindClan(string clanName, UnityAction<ClanListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FindClan");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFindClan(playerId, loginToken, clanName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanJoinRequest(string clanId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanJoinRequest");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanJoinRequest(playerId, loginToken, clanId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanJoinAccept(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanJoinAccept");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanJoinAccept(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanJoinDecline(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanJoinDecline");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanJoinDecline(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanMemberDelete(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanMemberDelete");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanMemberDelete(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanJoinRequestDelete(string clanId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanJoinRequestDelete");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanJoinRequestDelete(playerId, loginToken, clanId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanMemberList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanMemberList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanMemberList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanOwnerTransfer(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanOwnerTransfer");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanOwnerTransfer(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanTerminate(UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanTerminate");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanTerminate(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClan(UnityAction<ClanResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClan");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClan(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanCheckinStatus(UnityAction<ClanCheckinStatusResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanCheckinStatus");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanCheckinStatus(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanCheckin(UnityAction<ClanCheckinResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanCheckin");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanCheckin(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanDonationStatus(UnityAction<ClanDonationStatusResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanDonationStatus");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanDonationStatus(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanDonation(string clanDonationDataId, UnityAction<ClanDonationResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanDonation");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanDonation(clanDonationDataId, playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanJoinRequestList(UnityAction<PlayerListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanJoinRequestList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanJoinRequestList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanJoinPendingRequestList(UnityAction<ClanListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanJoinPendingRequestList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanJoinPendingRequestList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanExit(UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanExit");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanExit(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClanSetRole(string targetPlayerId, byte clanRole, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClanSetRole");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClanSetRole(playerId, loginToken, targetPlayerId, clanRole, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void CraftItem(string itemCraftId, Dictionary<string, int> materials, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: CraftItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoCraftItem(playerId, loginToken, itemCraftId, materials, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetChatMessages(long lastTime, UnityAction<ChatMessageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetChatMessages");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoGetChatMessages(playerId, loginToken, lastTime, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanChatMessages(long lastTime, UnityAction<ChatMessageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanChatMessages");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoGetClanChatMessages(playerId, loginToken, lastTime, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void EnterChatMessage(string message, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EnterChatMessage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoEnterChatMessage(playerId, loginToken, message, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void EnterClanChatMessage(string message, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EnterClanChatMessage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoEnterClanChatMessage(playerId, loginToken, message, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void RefillStamina(string staminaDataId, UnityAction<RefillStaminaResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: RefillStamina");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoRefillStamina(playerId, loginToken, staminaDataId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetRefillStaminaInfo(string staminaDataId, UnityAction<RefillStaminaInfoResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetRefillStaminaInfo");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        DoGetRefillStaminaInfo(playerId, loginToken, staminaDataId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetRaidEventList(UnityAction<RaidEventListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetRaidEventList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetRaidEventList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void StartRaidBossBattle(string eventId, UnityAction<StartRaidBossBattleResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: StartRaidBossBattle");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoStartRaidBossBattle(playerId, loginToken, eventId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FinishRaidBossBattle(string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishRaidBossBattleResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FinishRaidBossBattle");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFinishRaidBossBattle(playerId, loginToken, session, battleResult, totalDamage, deadCharacters, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetMailList(UnityAction<MailListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetMailList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetMailList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ReadMail(string id, UnityAction<ReadMailResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ReadMail");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoReadMail(playerId, loginToken, id, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void ClaimMailRewards(string id, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ClaimMailRewards");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoClaimMailRewards(playerId, loginToken, id, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void DeleteMail(string id, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: DeleteMail");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoDeleteMail(playerId, loginToken, id, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetMailsCount(UnityAction<MailsCountResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetMailsCount");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetMailsCount(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetClanEventList(UnityAction<ClanEventListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClanEventList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClanEventList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void StartClanBossBattle(string eventId, UnityAction<StartClanBossBattleResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: StartClanBossBattle");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoStartClanBossBattle(playerId, loginToken, eventId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FinishClanBossBattle(string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishClanBossBattleResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FinishClanBossBattle");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFinishClanBossBattle(playerId, loginToken, session, battleResult, totalDamage, deadCharacters, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetRandomStore(string id, UnityAction<RandomStoreResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetRandomStore");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetRandomStore(playerId, loginToken, id, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void PurchaseRandomStoreItem(string id, int index, UnityAction<PurchaseRandomStoreItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: PurchaseRandomStoreItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoPurchaseRandomStoreItem(playerId, loginToken, id, index, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void RefreshRandomStore(string id, UnityAction<RefreshRandomStoreResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: RefreshRandomStore");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoRefreshRandomStore(playerId, loginToken, id, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetFormationCharactersAndEquipments(string playerId, string formationDataId, UnityAction<FormationCharactersAndEquipmentsResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFormationCharactersAndEquipments");
        HandleServiceCall();
        DoGetFormationCharactersAndEquipments(playerId, formationDataId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetArenaFormationCharactersAndEquipments(string playerId, UnityAction<FormationCharactersAndEquipmentsResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetArenaFormationCharactersAndEquipments");
        HandleServiceCall();
        DoGetArenaFormationCharactersAndEquipments(playerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    protected abstract void DoRegister(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoLogin(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoRegisterOrLogin(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoGuestLogin(string deviceId, UnityAction<PlayerResult> onFinish);
    protected abstract void DoValidateLoginToken(string playerId, string loginToken, bool refreshToken, UnityAction<PlayerResult> onFinish);
    protected abstract void DoSetProfileName(string playerId, string loginToken, string profileName, UnityAction<PlayerResult> onFinish);
    protected abstract void DoLevelUpItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish);
    protected abstract void DoEvolveItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish);
    protected abstract void DoSellItems(string playerId, string loginToken, Dictionary<string, int> items, UnityAction<ItemResult> onFinish);
    protected abstract void DoStartStage(string playerId, string loginToken, string stageDataId, string helperPlayerId, UnityAction<StartStageResult> onFinish);
    protected abstract void DoFinishStage(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishStageResult> onFinish);
    protected abstract void DoReviveCharacters(string playerId, string loginToken, UnityAction<CurrencyResult> onFinish);
    protected abstract void DoSelectFormation(string playerId, string loginToken, string formationName, EFormationType formationType, UnityAction<PlayerResult> onFinish);
    protected abstract void DoEquipItem(string playerId, string loginToken, string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onFinish);
    protected abstract void DoUnEquipItem(string playerId, string loginToken, string equipmentId, UnityAction<ItemResult> onFinish);
    protected abstract void DoGetAchievementList(string playerId, string loginToken, UnityAction<AchievementListResult> onFinish);
    protected abstract void DoGetAuthList(string playerId, string loginToken, UnityAction<AuthListResult> onFinish);
    protected abstract void DoGetItemList(string playerId, string loginToken, UnityAction<ItemListResult> onFinish);
    protected abstract void DoGetCurrencyList(string playerId, string loginToken, UnityAction<CurrencyListResult> onFinish);
    protected abstract void DoGetStaminaList(string playerId, string loginToken, UnityAction<StaminaListResult> onFinish);
    protected abstract void DoGetFormationList(string playerId, string loginToken, UnityAction<FormationListResult> onFinish);
    protected abstract void DoGetUnlockItemList(string playerId, string loginToken, UnityAction<UnlockItemListResult> onFinish);
    protected abstract void DoGetClearStageList(string playerId, string loginToken, UnityAction<ClearStageListResult> onFinish);
    protected abstract void DoGetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onFinish);
    protected abstract void DoGetAvailableIapPackageList(UnityAction<AvailableIapPackageListResult> onFinish);
    protected abstract void DoGetAvailableInGamePackageList(UnityAction<AvailableInGamePackageListResult> onFinish);
    protected abstract void DoGetAvailableStageList(UnityAction<AvailableStageListResult> onFinish);
    protected abstract void DoSetFormation(string playerId, string loginToken, string characterId, string formationName, int position, UnityAction<FormationListResult> onFinish);
    protected abstract void DoOpenLootBox(string playerId, string loginToken, string lootBoxDataId, int packIndex, UnityAction<ItemResult> onFinish);
    protected abstract void DoOpenIapPackage_iOS(string playerId, string loginToken, string iapPackageDataId, string receipt, UnityAction<ItemResult> onFinish);
    protected abstract void DoOpenIapPackage_Android(string playerId, string loginToken, string iapPackageDataId, string data, string signature, UnityAction<ItemResult> onFinish);
    protected abstract void DoGetHelperList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoGetFriendList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoGetFriendRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoGetPendingRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoFindUser(string playerId, string loginToken, string displayName, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoFriendRequest(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendRequestDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoGetServiceTime(UnityAction<ServiceTimeResult> onFinish);
    protected abstract void DoGetArenaOpponentList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoStartDuel(string playerId, string loginToken, string targetPlayerId, UnityAction<StartDuelResult> onFinish);
    protected abstract void DoFinishDuel(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishDuelResult> onFinish);
    protected abstract void DoEarnAchievementReward(string playerId, string loginToken, string achievementId, UnityAction<EarnAchievementResult> onFinish);
    protected abstract void DoConvertHardCurrency(string playerId, string loginToken, int requireHardCurrency, UnityAction<HardCurrencyConversionResult> onFinish);
    protected abstract void DoOpenInGamePackage(string playerId, string loginToken, string inGamePackageDataId, UnityAction<ItemResult> onFinish);
    protected abstract void DoCreateClan(string playerId, string loginToken, string clanName, UnityAction<CreateClanResult> onFinish);
    protected abstract void DoFindClan(string playerId, string loginToken, string clanName, UnityAction<ClanListResult> onFinish);
    protected abstract void DoClanJoinRequest(string playerId, string loginToken, string clanId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanJoinAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanJoinDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanMemberDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanJoinRequestDelete(string playerId, string loginToken, string clanId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoGetClanMemberList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoClanOwnerTransfer(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanTerminate(string playerId, string loginToken, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoGetClan(string playerId, string loginToken, UnityAction<ClanResult> onFinish);
    protected abstract void DoGetClanCheckinStatus(string playerId, string loginToken, UnityAction<ClanCheckinStatusResult> onFinish);
    protected abstract void DoClanCheckin(string playerId, string loginToken, UnityAction<ClanCheckinResult> onFinish);
    protected abstract void DoGetClanDonationStatus(string playerId, string loginToken, UnityAction<ClanDonationStatusResult> onFinish);
    protected abstract void DoClanDonation(string clanDonationDataId, string playerId, string loginToken, UnityAction<ClanDonationResult> onFinish);
    protected abstract void DoGetClanJoinRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish);
    protected abstract void DoGetClanJoinPendingRequestList(string playerId, string loginToken, UnityAction<ClanListResult> onFinish);
    protected abstract void DoClanExit(string playerId, string loginToken, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoClanSetRole(string playerId, string loginToken, string targetPlayerId, byte clanRole, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoCraftItem(string playerId, string loginToken, string itemCraftId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish);
    protected abstract void DoGetChatMessages(string playerId, string loginToken, long lastTime, UnityAction<ChatMessageListResult> onFinish);
    protected abstract void DoGetClanChatMessages(string playerId, string loginToken, long lastTime, UnityAction<ChatMessageListResult> onFinish);
    protected abstract void DoEnterChatMessage(string playerId, string loginToken, string message, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoEnterClanChatMessage(string playerId, string loginToken, string message, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoRefillStamina(string playerId, string loginToken, string staminaDataId, UnityAction<RefillStaminaResult> onFinish);
    protected abstract void DoGetRefillStaminaInfo(string playerId, string loginToken, string staminaDataId, UnityAction<RefillStaminaInfoResult> onFinish);
    protected abstract void DoGetRaidEventList(string playerId, string loginToken, UnityAction<RaidEventListResult> onFinish);
    protected abstract void DoStartRaidBossBattle(string playerId, string loginToken, string eventId, UnityAction<StartRaidBossBattleResult> onFinish);
    protected abstract void DoFinishRaidBossBattle(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishRaidBossBattleResult> onFinish);
    protected abstract void DoGetMailList(string playerId, string loginToken, UnityAction<MailListResult> onFinish);
    protected abstract void DoReadMail(string playerId, string loginToken, string id, UnityAction<ReadMailResult> onFinish);
    protected abstract void DoClaimMailRewards(string playerId, string loginToken, string id, UnityAction<ItemResult> onFinish);
    protected abstract void DoDeleteMail(string playerId, string loginToken, string id, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoGetMailsCount(string playerId, string loginToken, UnityAction<MailsCountResult> onFinish);
    protected abstract void DoGetClanEventList(string playerId, string loginToken, UnityAction<ClanEventListResult> onFinish);
    protected abstract void DoStartClanBossBattle(string playerId, string loginToken, string eventId, UnityAction<StartClanBossBattleResult> onFinish);
    protected abstract void DoFinishClanBossBattle(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishClanBossBattleResult> onFinish);
    protected abstract void DoGetRandomStore(string playerId, string loginToken, string id, UnityAction<RandomStoreResult> onFinish);
    protected abstract void DoPurchaseRandomStoreItem(string playerId, string loginToken, string id, int index, UnityAction<PurchaseRandomStoreItemResult> onFinish);
    protected abstract void DoRefreshRandomStore(string playerId, string loginToken, string id, UnityAction<RefreshRandomStoreResult> onFinish);
    protected abstract void DoGetFormationCharactersAndEquipments(string playerId, string formationDataId, UnityAction<FormationCharactersAndEquipmentsResult> onFinish);
    protected abstract void DoGetArenaFormationCharactersAndEquipments(string playerId, UnityAction<FormationCharactersAndEquipmentsResult> onFinish);
}
