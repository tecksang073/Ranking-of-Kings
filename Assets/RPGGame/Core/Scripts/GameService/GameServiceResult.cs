using System.Collections;
using System.Collections.Generic;

public class GameServiceErrorCode
{
    private const string ERROR_PREFIX = "ERROR_";
    public const string UNKNOW = ERROR_PREFIX + "UNKNOW";
    public const string NETWORK = ERROR_PREFIX + "NETWORK";
    public const string EMPTY_USERNAME_OR_PASSWORD = ERROR_PREFIX + "EMPTY_USERNAME_OR_PASSWORD";
    public const string EXISTED_USERNAME = ERROR_PREFIX + "EXISTED_USERNAME";
    public const string EMPTY_PROFILE_NAME = ERROR_PREFIX + "EMPTY_PROFILE_NAME";
    public const string EMPTY_CLAN_NAME = ERROR_PREFIX + "EMPTY_CLAN_NAME";
    public const string EXISTED_PROFILE_NAME = ERROR_PREFIX + "EXISTED_PROFILE_NAME";
    public const string INVALID_USERNAME_OR_PASSWORD = ERROR_PREFIX + "INVALID_USERNAME_OR_PASSWORD";
    public const string INVALID_LOGIN_TOKEN = ERROR_PREFIX + "INVALID_LOGIN_TOKEN";
    public const string INVALID_PLAYER_DATA = ERROR_PREFIX + "INVALID_PLAYER_DATA";
    public const string INVALID_PLAYER_ITEM_DATA = ERROR_PREFIX + "INVALID_PLAYER_ITEM_DATA";
    public const string INVALID_ITEM_DATA = ERROR_PREFIX + "INVALID_ITEM_DATA";
    public const string INVALID_FORMATION_DATA = ERROR_PREFIX + "INVALID_FORMATION_DATA";
    public const string INVALID_STAGE_DATA = ERROR_PREFIX + "INVALID_STAGE_DATA";
    public const string INVALID_CLAN_DONATION_DATA = ERROR_PREFIX + "INVALID_CLAN_DONATION_DATA";
    public const string INVALID_STAGE_NOT_AVAILABLE = ERROR_PREFIX + "INVALID_STAGE_NOT_AVAILABLE";
    public const string INVALID_LOOT_BOX_DATA = ERROR_PREFIX + "INVALID_LOOT_BOX_DATA";
    public const string INVALID_IAP_PACKAGE_DATA = ERROR_PREFIX + "INVALID_IAP_PACKAGE_DATA";
    public const string INVALID_IN_GAME_PACKAGE_DATA = ERROR_PREFIX + "INVALID_IN_GAME_PACKAGE_DATA";
    public const string INVALID_ACHIEVEMENT_DATA = ERROR_PREFIX + "INVALID_ACHIEVEMENT_DATA";
    public const string INVALID_ITEM_CRAFT_FORMULA_DATA = ERROR_PREFIX + "INVALID_ITEM_CRAFT_FORMULA_DATA";
    public const string INVALID_CURRENCY_DATA = ERROR_PREFIX + "INVALID_CURRENCY_DATA";
    public const string INVALID_STAMINA_DATA = ERROR_PREFIX + "INVALID_STAMINA_DATA";
    public const string INVALID_EQUIP_POSITION = ERROR_PREFIX + "INVALID_EQUIP_POSITION";
    public const string INVALID_BATTLE_SESSION = ERROR_PREFIX + "INVALID_BATTLE_SESSION";
    public const string NOT_ENOUGH_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_CURRENCY";
    public const string NOT_ENOUGH_SOFT_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_SOFT_CURRENCY";
    public const string NOT_ENOUGH_HARD_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_HARD_CURRENCY";
    public const string NOT_ENOUGH_STAGE_STAMINA = ERROR_PREFIX + "NOT_ENOUGH_STAGE_STAMINA";
    public const string NOT_ENOUGH_ARENA_STAMINA = ERROR_PREFIX + "NOT_ENOUGH_ARENA_STAMINA";
    public const string NOT_ENOUGH_ITEMS = ERROR_PREFIX + "NOT_ENOUGH_ITEMS";
    public const string ACHIEVEMENT_UNDONE = ERROR_PREFIX + "ACHIEVEMENT_UNDONE";
    public const string ACHIEVEMENT_EARNED = ERROR_PREFIX + "ACHIEVEMENT_EARNED";
    public const string CANNOT_EVOLVE = ERROR_PREFIX + "CANNOT_EVOLVE";
    public const string CANNOT_REFILL_STAMINA = ERROR_PREFIX + "CANNOT_REFILL_STAMINA";
    public const string JOINED_CLAN = ERROR_PREFIX + "JOINED_CLAN";
    public const string NOT_JOINED_CLAN = ERROR_PREFIX + "NOT_JOINED_CLAN";
    public const string NOT_HAVE_PERMISSION = ERROR_PREFIX + "NOT_HAVE_PERMISSION";
    public const string CLAN_OWNER_CANNOT_EXIT = ERROR_PREFIX + "CLAN_OWNER_CANNOT_EXIT";
    public const string NOT_AVAILABLE = ERROR_PREFIX + "NOT_AVAILABLE";
    public const string CANNOT_RECEIVE_ALL_ITEMS = ERROR_PREFIX + "CANNOT_RECEIVE_ALL_ITEMS";
}

[System.Serializable]
public class GameServiceResult
{
    public string error;
    public bool Success { get { return string.IsNullOrEmpty(error); } }
}

[System.Serializable]
public class ServiceTimeResult : GameServiceResult
{
    public long serviceTime;
}

[System.Serializable]
public class PlayerResult : GameServiceResult
{
    public Player player;
}

[System.Serializable]
public class StartStageResult : GameServiceResult
{
    public PlayerStamina stamina;
    public string session;
}

[System.Serializable]
public class ItemResult : PlayerResult
{
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
    public List<PlayerItem> rewardItems = new List<PlayerItem>();
    public List<PlayerItem> createItems = new List<PlayerItem>();
    public List<PlayerItem> updateItems = new List<PlayerItem>();
    public List<string> deleteItemIds = new List<string>();
}

[System.Serializable]
public class FinishStageResult : ItemResult
{
    public int rating;
    public int rewardPlayerExp;
    public int rewardCharacterExp;
    public int rewardSoftCurrency;
    public bool isFirstClear;
    public int firstClearRewardSoftCurrency;
    public int firstClearRewardHardCurrency;
    public int firstClearRewardPlayerExp;
    public List<PlayerItem> firstClearRewardItems = new List<PlayerItem>();
    public PlayerClearStage clearStage;
}

[System.Serializable]
public class CurrencyResult : GameServiceResult
{
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class AchievementListResult : GameServiceResult
{
    public List<PlayerAchievement> list = new List<PlayerAchievement>();
}

[System.Serializable]
public class AuthListResult : GameServiceResult
{
    public List<PlayerAuth> list = new List<PlayerAuth>();
}

[System.Serializable]
public class ItemListResult : GameServiceResult
{
    public List<PlayerItem> list = new List<PlayerItem>();
}

[System.Serializable]
public class CurrencyListResult : GameServiceResult
{
    public List<PlayerCurrency> list = new List<PlayerCurrency>();
}

[System.Serializable]
public class StaminaListResult : GameServiceResult
{
    public List<PlayerStamina> list = new List<PlayerStamina>();
}

[System.Serializable]
public class FormationListResult : GameServiceResult
{
    public List<PlayerFormation> list = new List<PlayerFormation>();
}

[System.Serializable]
public class UnlockItemListResult : GameServiceResult
{
    public List<PlayerUnlockItem> list = new List<PlayerUnlockItem>();
}

[System.Serializable]
public class ClearStageListResult : GameServiceResult
{
    public List<PlayerClearStage> list = new List<PlayerClearStage>();
}

[System.Serializable]
public class AvailableLootBoxListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

[System.Serializable]
public class AvailableIapPackageListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

[System.Serializable]
public class AvailableInGamePackageListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

[System.Serializable]
public class AvailableStageListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

[System.Serializable]
public class PlayerListResult : GameServiceResult
{
    public List<Player> list = new List<Player>();
}

[System.Serializable]
public class StartDuelResult : GameServiceResult
{
    public PlayerStamina stamina;
    public string session;
    public List<PlayerItem> opponentCharacters = new List<PlayerItem>();
    public List<PlayerItem> opponentEquipments = new List<PlayerItem>();
}

[System.Serializable]
public class FinishDuelResult : ItemResult
{
    public int rating;
    public int updateScore;
    public int rewardSoftCurrency;
    public int rewardHardCurrency;
}

[System.Serializable]
public class RaidEventListResult : GameServiceResult
{
    public List<RaidEvent> list = new List<RaidEvent>();
}

[System.Serializable]
public class StartRaidBossBattleResult : GameServiceResult
{
    public PlayerStamina stamina;
    public string session;
    public int remainingHp;
}

[System.Serializable]
public class FinishRaidBossBattleResult : GameServiceResult
{
    public int totalDamage;
    public RaidEvent raidEvent;
}

[System.Serializable]
public class RandomStoreResult : GameServiceResult
{
    public RandomStoreEvent store;
    public int endsIn;
}

[System.Serializable]
public class PurchaseRandomStoreItemResult : ItemResult
{
    public RandomStoreEvent store;
}

[System.Serializable]
public class RefreshRandomStoreResult : RandomStoreResult
{
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class ClanEventListResult : GameServiceResult
{
    public List<ClanEvent> list = new List<ClanEvent>();
}

[System.Serializable]
public class StartClanBossBattleResult : GameServiceResult
{
    public PlayerStamina stamina;
    public string session;
    public int remainingHp;
}

[System.Serializable]
public class FinishClanBossBattleResult : GameServiceResult
{
    public int totalDamage;
    public ClanEvent clanEvent;
}

[System.Serializable]
public class MailListResult : GameServiceResult
{
    public List<Mail> list = new List<Mail>();
}

[System.Serializable]
public class ReadMailResult : GameServiceResult
{
    public Mail mail;
}

[System.Serializable]
public class MailsCountResult : GameServiceResult
{
    public int count;
}

[System.Serializable]
public class EarnAchievementResult : ItemResult
{
    public int rewardPlayerExp;
    public int rewardSoftCurrency;
    public int rewardHardCurrency;
}

[System.Serializable]
public class HardCurrencyConversionResult : GameServiceResult
{
    public int requireHardCurrency;
    public int receiveSoftCurrency;
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class ClanResult : GameServiceResult
{
    public Clan clan;
}

[System.Serializable]
public class ClanListResult : GameServiceResult
{
    public List<Clan> list = new List<Clan>();
}

[System.Serializable]
public class ClanCheckinResult : GameServiceResult
{
    public Clan clan;
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class ClanDonationResult : GameServiceResult
{
    public Clan clan;
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class CreateClanResult : GameServiceResult
{
    public Clan clan;
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

[System.Serializable]
public class ClanCheckinStatusResult : GameServiceResult
{
    public bool alreadyCheckin;
}

[System.Serializable]
public class ClanDonationStatusResult : GameServiceResult
{
    public int clanDonateCount;
    public int maxClanDonation;
}

[System.Serializable]
public class ChatMessageListResult : GameServiceResult
{
    public List<ChatMessage> list = new List<ChatMessage>();
}

[System.Serializable]
public class RefillStaminaResult : GameServiceResult
{
    public PlayerStamina stamina;
    public PlayerCurrency currency;
}

[System.Serializable]
public class RefillStaminaInfoResult : GameServiceResult
{
    public int requireHardCurrency;
    public int refillAmount;
}

[System.Serializable]
public class FormationCharactersAndEquipmentsResult : GameServiceResult
{
    public List<PlayerItem> characters = new List<PlayerItem>();
    public List<PlayerItem> equipments = new List<PlayerItem>();
}