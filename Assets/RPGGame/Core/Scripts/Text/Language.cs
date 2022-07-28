using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameText
{
    private const string TEXT_PREFIX = "TEXT_";
    private const string VALUE_PREFIX = "VALUE_";
    private const string FORMAT_PREFIX = "FORMAT_";
    public const string TITLE_INFO_DIALOG = TEXT_PREFIX + "TITLE_INFO_DIALOG";
    public const string TITLE_ERROR_DIALOG = TEXT_PREFIX + "TITLE_ERROR_DIALOG";
    public const string TITLE_PROFILE_NAME_DIALOG = TEXT_PREFIX + "TITLE_PROFILE_NAME_DIALOG";
    public const string CONTENT_PROFILE_NAME_DIALOG = TEXT_PREFIX + "CONTENT_PROFILE_NAME_DIALOG";
    public const string PLACE_HOLDER_PROFILE_NAME = TEXT_PREFIX + "PLACE_HOLDER_PROFILE_NAME";
    public const string TITLE_SOFT_CURRENCY = TEXT_PREFIX + "TITLE_SOFT_CURRENCY";
    public const string TITLE_HARD_CURRENCY = TEXT_PREFIX + "TITLE_HARD_CURRENCY";
    public const string TITLE_STAGE_STAMINA = TEXT_PREFIX + "TITLE_STAGE_STAMINA";
    public const string TITLE_ARENA_STAMINA = TEXT_PREFIX + "TITLE_ARENA_STAMINA";
    // Combat
    public const string COMBAT_MISS = TEXT_PREFIX + "COMBAT_MISS";
    public const string COMBAT_RESIST = TEXT_PREFIX + "COMBAT_RESIST";
    // Item
    public const string TITLE_LEVEL = TEXT_PREFIX + "TITLE_LEVEL";
    public const string TITLE_COLLECT_EXP = TEXT_PREFIX + "TITLE_COLLECT_EXP";
    public const string TITLE_EVOLVE_PRICE = TEXT_PREFIX + "TITLE_EVOLVE_PRICE";
    public const string TITLE_NEXT_EXP = TEXT_PREFIX + "TITLE_NEXT_EXP";
    public const string TITLE_REQUIRE_EXP = TEXT_PREFIX + "TITLE_REQUIRE_EXP";
    public const string TITLE_PRICE = TEXT_PREFIX + "TITLE_PRICE";
    public const string TITLE_REWARD_EXP = TEXT_PREFIX + "TITLE_REWARD_EXP";
    public const string TITLE_BATTLE_POINT = TEXT_PREFIX + "TITLE_BATTLE_POINT";
    public const string TITLE_LEVEL_UP_PRICE = TEXT_PREFIX + "TITLE_LEVEL_UP_PRICE";
    // Attribute titles
    public const string TITLE_EXP_MAX = TEXT_PREFIX + "TITLE_EXP_MAX";
    public const string TITLE_ATTRIBUTE_HP = TEXT_PREFIX + "TITLE_ATTRIBUTE_HP";
    public const string TITLE_ATTRIBUTE_PATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_PATK";
    public const string TITLE_ATTRIBUTE_PDEF = TEXT_PREFIX + "TITLE_ATTRIBUTE_PDEF";
    public const string TITLE_ATTRIBUTE_MATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_MATK";
    public const string TITLE_ATTRIBUTE_MDEF = TEXT_PREFIX + "TITLE_ATTRIBUTE_MDEF";
    public const string TITLE_ATTRIBUTE_SPD = TEXT_PREFIX + "TITLE_ATTRIBUTE_SPD";
    public const string TITLE_ATTRIBUTE_EVA = TEXT_PREFIX + "TITLE_ATTRIBUTE_EVA";
    public const string TITLE_ATTRIBUTE_ACC = TEXT_PREFIX + "TITLE_ATTRIBUTE_ACC";
    public const string TITLE_ATTRIBUTE_HP_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_HP_RATE";
    public const string TITLE_ATTRIBUTE_PATK_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_PATK_RATE";
    public const string TITLE_ATTRIBUTE_PDEF_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_PDEF_RATE";
    public const string TITLE_ATTRIBUTE_MATK_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_MATK_RATE";
    public const string TITLE_ATTRIBUTE_MDEF_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_MDEF_RATE";
    public const string TITLE_ATTRIBUTE_SPD_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_SPD_RATE";
    public const string TITLE_ATTRIBUTE_EVA_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_EVA_RATE";
    public const string TITLE_ATTRIBUTE_ACC_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_ACC_RATE";
    public const string TITLE_ATTRIBUTE_CRIT_CHANCE = TEXT_PREFIX + "TITLE_ATTRIBUTE_CRIT_CHANCE";
    public const string TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE";
    public const string TITLE_ATTRIBUTE_BLOCK_CHANCE = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOCK_CHANCE";
    public const string TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE";
    public const string TITLE_ATTRIBUTE_RESISTANCE_CHANCE = TEXT_PREFIX + "TITLE_ATTRIBUTE_RESISTANCE";
    public const string TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_PATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_PATK";
    public const string TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_MATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_MATK";
    // Warn
    public const string WARN_TITLE_DELETE_FRIEND = TEXT_PREFIX + "WARN_TITLE_DELETE_FRIEND";
    public const string WARN_DESCRIPTION_DELETE_FRIEND = TEXT_PREFIX + "WARN_DESCRIPTION_DELETE_FRIEND";
    public const string WARN_TITLE_DELETE_REQUEST = TEXT_PREFIX + "WARN_TITLE_DELETE_REQUEST";
    public const string WARN_DESCRIPTION_DELETE_REQUEST = TEXT_PREFIX + "WARN_DESCRIPTION_DELETE_REQUEST";
    public const string WARN_TITLE_DELETE_CLAN_MEMBER = TEXT_PREFIX + "WARN_TITLE_DELETE_CLAN_MEMBER";
    public const string WARN_DESCRIPTION_DELETE_CLAN_MEMBER = TEXT_PREFIX + "WARN_DESCRIPTION_DELETE_CLAN_MEMBER";
    public const string WARN_TITLE_CLAN_OWNER_TRANSFER = TEXT_PREFIX + "WARN_TITLE_CLAN_OWNER_TRANSFER";
    public const string WARN_DESCRIPTION_CLAN_OWNER_TRANSFER = TEXT_PREFIX + "WARN_DESCRIPTION_CLAN_OWNER_TRANSFER";
    public const string WARN_TITLE_CLAN_SET_ROLE_TO_MEMBER = TEXT_PREFIX + "WARN_TITLE_CLAN_SET_ROLE_TO_MEMBER";
    public const string WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MEMBER = TEXT_PREFIX + "WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MEMBER";
    public const string WARN_TITLE_CLAN_SET_ROLE_TO_MANAGER = TEXT_PREFIX + "WARN_TITLE_CLAN_SET_ROLE_TO_MANAGER";
    public const string WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MANAGER = TEXT_PREFIX + "WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MANAGER";
    public const string WARN_TITLE_DELETE_CLAN_JOIN_REQUEST = TEXT_PREFIX + "WARN_TITLE_DELETE_CLAN_JOIN_REQUEST";
    public const string WARN_DESCRIPTION_DELETE_CLAN_JOIN_REQUEST = TEXT_PREFIX + "WARN_DESCRIPTION_DELETE_CLAN_JOIN_REQUEST";
    public const string WARN_TITLE_CLAN_TERMINATE = TEXT_PREFIX + "WARN_TITLE_CLAN_TERMINATE";
    public const string WARN_DESCRIPTION_CLAN_TERMINATE = TEXT_PREFIX + "WARN_DESCRIPTION_CLAN_TERMINATE";
    public const string WARN_TITLE_CLAN_EXIT = TEXT_PREFIX + "WARN_TITLE_CLAN_EXIT";
    public const string WARN_DESCRIPTION_CLAN_EXIT = TEXT_PREFIX + "WARN_DESCRIPTION_CLAN_EXIT";
    public const string WARN_TITLE_REFILL_STAMINA = TEXT_PREFIX + "WARN_TITLE_REFILL_STAMINA";
    public const string WARN_DESCRIPTION_REFILL_STAMINA = TEXT_PREFIX + "WARN_DESCRIPTION_REFILL_STAMINA";
    // Formats
    public const string FORMAT_INFO = FORMAT_PREFIX + "FORMAT_INFO";
    public const string FORMAT_ATTRIBUTE = FORMAT_PREFIX + "FORMAT_ATTRIBUTE";
    public const string FORMAT_BONUS = FORMAT_PREFIX + "FORMAT_BONUS";
}

public static class DefaultLocale
{
    public static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    static DefaultLocale()
    {
        Texts.Add(GameText.TITLE_INFO_DIALOG, "Info");
        Texts.Add(GameText.TITLE_ERROR_DIALOG, "Error");
        Texts.Add(GameText.TITLE_PROFILE_NAME_DIALOG, "Name");
        Texts.Add(GameText.CONTENT_PROFILE_NAME_DIALOG, "Enter your name");
        Texts.Add(GameText.PLACE_HOLDER_PROFILE_NAME, "Enter your name...");
        Texts.Add(GameText.TITLE_SOFT_CURRENCY, "Gold(s)");
        Texts.Add(GameText.TITLE_HARD_CURRENCY, "Gem(s)");
        Texts.Add(GameText.TITLE_STAGE_STAMINA, "Stamina");
        Texts.Add(GameText.TITLE_ARENA_STAMINA, "Stamina");
        // Combat
        Texts.Add(GameText.COMBAT_MISS, "Miss");
        // Item
        Texts.Add(GameText.TITLE_LEVEL, "Level");
        Texts.Add(GameText.TITLE_COLLECT_EXP, "Collect Exp");
        Texts.Add(GameText.TITLE_EVOLVE_PRICE, "Evolve Price");
        Texts.Add(GameText.TITLE_NEXT_EXP, "Next Exp");
        Texts.Add(GameText.TITLE_REQUIRE_EXP, "Require Exp");
        Texts.Add(GameText.TITLE_PRICE, "Price");
        Texts.Add(GameText.TITLE_REWARD_EXP, "Reward Exp");
        Texts.Add(GameText.TITLE_BATTLE_POINT, "Battle Point");
        Texts.Add(GameText.TITLE_LEVEL_UP_PRICE, "Level Up Price");
        // Attributes
        Texts.Add(GameText.TITLE_EXP_MAX, "Max");
        Texts.Add(GameText.TITLE_ATTRIBUTE_HP, "Hp");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PATK, "P.Atk");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PDEF, "P.Def");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MATK, "M.Atk");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MDEF, "M.Def");
        Texts.Add(GameText.TITLE_ATTRIBUTE_SPD, "Speed");
        Texts.Add(GameText.TITLE_ATTRIBUTE_EVA, "Evasion");
        Texts.Add(GameText.TITLE_ATTRIBUTE_ACC, "Accuracy");
        Texts.Add(GameText.TITLE_ATTRIBUTE_HP_RATE, "Hp Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PATK_RATE, "P.Atk Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PDEF_RATE, "P.Def Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MATK_RATE, "M.Atk Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MDEF_RATE, "M.Def Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_SPD_RATE, "Speed Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_EVA_RATE, "Evasion Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_ACC_RATE, "Accuracy Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_CRIT_CHANCE, "Critical Chance");
        Texts.Add(GameText.TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE, "Critical Damage Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOCK_CHANCE, "Block Chance");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE, "Block Damage Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_RESISTANCE_CHANCE, "Resistance");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_PATK, "Blood Steal Rate by P.Atk");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOOD_STEAL_RATE_BY_MATK, "Blood Steal Rate by M.Atk");
        // Warn
        Texts.Add(GameText.WARN_TITLE_DELETE_FRIEND, "Delete Friend");
        Texts.Add(GameText.WARN_DESCRIPTION_DELETE_FRIEND, "Do you want to delete this player from your friend list?");
        Texts.Add(GameText.WARN_TITLE_DELETE_REQUEST, "Delete Friend Request");
        Texts.Add(GameText.WARN_DESCRIPTION_DELETE_REQUEST, "Do you want to delete this friend request?");
        Texts.Add(GameText.WARN_TITLE_DELETE_CLAN_MEMBER, "Expel Member");
        Texts.Add(GameText.WARN_DESCRIPTION_DELETE_CLAN_MEMBER, "Do you want to expel this clan member?");
        Texts.Add(GameText.WARN_TITLE_CLAN_OWNER_TRANSFER, "Promote");
        Texts.Add(GameText.WARN_DESCRIPTION_CLAN_OWNER_TRANSFER, "Do you want to promote this clan member to be clan leader?");
        Texts.Add(GameText.WARN_TITLE_CLAN_SET_ROLE_TO_MEMBER, "Demote");
        Texts.Add(GameText.WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MEMBER, "Do you want to demote this clan member?");
        Texts.Add(GameText.WARN_TITLE_CLAN_SET_ROLE_TO_MANAGER, "Promote");
        Texts.Add(GameText.WARN_DESCRIPTION_CLAN_SET_ROLE_TO_MANAGER, "Do you want to promote this clan member to be clan manager?");
        Texts.Add(GameText.WARN_TITLE_DELETE_CLAN_JOIN_REQUEST, "Delete Join Request");
        Texts.Add(GameText.WARN_DESCRIPTION_DELETE_CLAN_JOIN_REQUEST, "Do you want to delete this clan join request?");
        Texts.Add(GameText.WARN_TITLE_CLAN_TERMINATE, "Terminate");
        Texts.Add(GameText.WARN_DESCRIPTION_CLAN_TERMINATE, "Do you want to terminate the clan?");
        Texts.Add(GameText.WARN_TITLE_CLAN_EXIT, "Leave");
        Texts.Add(GameText.WARN_DESCRIPTION_CLAN_EXIT, "Do you want to leave the clan?");
        Texts.Add(GameText.WARN_TITLE_REFILL_STAMINA, "Refilling stamina");
        Texts.Add(GameText.WARN_DESCRIPTION_REFILL_STAMINA, "Do you want to refill the stamina?");
        // Format
        Texts.Add(GameText.FORMAT_INFO, "{0}{1}");
        Texts.Add(GameText.FORMAT_ATTRIBUTE, "{0}: {1}{2}");
        Texts.Add(GameText.FORMAT_BONUS, "{0}{1}");
        // Error texts
        Texts.Add(GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD, "Username or password is empty");
        Texts.Add(GameServiceErrorCode.EXISTED_USERNAME, "Username is already used");
        Texts.Add(GameServiceErrorCode.EMPTY_PROFILE_NAME, "Name is empty");
        Texts.Add(GameServiceErrorCode.EMPTY_CLAN_NAME, "Name is empty");
        Texts.Add(GameServiceErrorCode.EXISTED_PROFILE_NAME, "Name is already used");
        Texts.Add(GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD, "Username or password is invalid");
        Texts.Add(GameServiceErrorCode.INVALID_LOGIN_TOKEN, "Invalid login token");
        Texts.Add(GameServiceErrorCode.INVALID_PLAYER_DATA, "Invalid player data");
        Texts.Add(GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA, "Invalid player item data");
        Texts.Add(GameServiceErrorCode.INVALID_ITEM_DATA, "Invalid item data");
        Texts.Add(GameServiceErrorCode.INVALID_FORMATION_DATA, "Invalid formation data");
        Texts.Add(GameServiceErrorCode.INVALID_STAGE_DATA, "Invalid stage data");
        Texts.Add(GameServiceErrorCode.INVALID_CLAN_DONATION_DATA, "Invalid clan donation data");
        Texts.Add(GameServiceErrorCode.INVALID_STAGE_NOT_AVAILABLE, "Stage not available to enter");
        Texts.Add(GameServiceErrorCode.INVALID_LOOT_BOX_DATA, "Invalid loot box data");
        Texts.Add(GameServiceErrorCode.INVALID_IAP_PACKAGE_DATA, "Invalid IAP package data");
        Texts.Add(GameServiceErrorCode.INVALID_IN_GAME_PACKAGE_DATA, "Invalid In-game package data");
        Texts.Add(GameServiceErrorCode.INVALID_ACHIEVEMENT_DATA, "Invalid achievement data");
        Texts.Add(GameServiceErrorCode.INVALID_ITEM_CRAFT_FORMULA_DATA, "Invalid item craft formula data");
        Texts.Add(GameServiceErrorCode.INVALID_CURRENCY_DATA, "Invalid currency data");
        Texts.Add(GameServiceErrorCode.INVALID_STAMINA_DATA, "Invalid stamina data");
        Texts.Add(GameServiceErrorCode.INVALID_EQUIP_POSITION, "Invalid equip position");
        Texts.Add(GameServiceErrorCode.INVALID_BATTLE_SESSION, "Invalid battle session");
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_CURRENCY, "Not enough currency");
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY, "Not enough " + Texts[GameText.TITLE_SOFT_CURRENCY]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY, "Not enough " + Texts[GameText.TITLE_HARD_CURRENCY]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_STAGE_STAMINA, "Not enough " + Texts[GameText.TITLE_STAGE_STAMINA]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_ARENA_STAMINA, "Not enough " + Texts[GameText.TITLE_ARENA_STAMINA]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_ITEMS, "Not enough items");
        Texts.Add(GameServiceErrorCode.ACHIEVEMENT_UNDONE, "Achievement's task isn't done");
        Texts.Add(GameServiceErrorCode.ACHIEVEMENT_EARNED, "Achievement's rewards earned");
        Texts.Add(GameServiceErrorCode.CANNOT_EVOLVE, "Cannot evolve");
        Texts.Add(GameServiceErrorCode.CANNOT_REFILL_STAMINA, "Cannot refill stamina");
        Texts.Add(GameServiceErrorCode.NOT_AVAILABLE, "Services not available");
        Texts.Add(GameServiceErrorCode.CANNOT_RECEIVE_ALL_ITEMS, "Cannot receive all items");
        Texts.Add(GameServiceErrorCode.JOINED_CLAN, "Already joined clan");
        Texts.Add(GameServiceErrorCode.NOT_JOINED_CLAN, "Not joined clan");
        Texts.Add(GameServiceErrorCode.NOT_HAVE_PERMISSION, "You don't have permission to do it");
        Texts.Add(GameServiceErrorCode.CLAN_OWNER_CANNOT_EXIT, "Clan owner cannot exit from clan");
        Texts.Add(GameServiceErrorCode.UNKNOW, "Unknow Error");
    }
}

[System.Serializable]
public class Language
{
    public string languageKey;
    public List<LanguageData> dataList = new List<LanguageData>();

    public bool ContainKey(string key)
    {
        foreach (LanguageData entry in dataList)
        {
            if (string.IsNullOrEmpty(entry.key))
                continue;
            if (entry.key.Equals(key))
                return true;
        }
        return false;
    }

    public static string GetText(IEnumerable<LanguageData> langs, string defaultValue)
    {
        if (langs != null)
        {
            foreach (LanguageData entry in langs)
            {
                if (string.IsNullOrEmpty(entry.key))
                    continue;
                if (entry.key.Equals(LanguageManager.CurrentLanguageKey))
                    return entry.value;
            }
        }
        return defaultValue;
    }
}

[System.Serializable]
public struct LanguageData
{
    public string key;
    [TextArea]
    public string value;
}
 