using System.Collections.Generic;

[System.Serializable]
public partial class Player : BasePlayerData, ILevel, IPlayer
{
    public static string CurrentPlayerId { get; private set; }
    public static readonly Dictionary<string, Player> DataMap = new Dictionary<string, Player>();
    public static Player CurrentPlayer
    {
        get
        {
            if (!string.IsNullOrEmpty(CurrentPlayerId) && DataMap.ContainsKey(CurrentPlayerId))
                return DataMap[CurrentPlayerId];
            return null;
        }
        set
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                CurrentPlayerId = string.Empty;
                return;
            }
            SetData(value);
            CurrentPlayerId = value.Id;
        }
    }
    public string id;
    public string Id { get { return id; } set { id = value; } }
    public string profileName;
    public string ProfileName { get { return profileName; } set { profileName = value; } }
    public string loginToken;
    public string LoginToken { get { return loginToken; } set { loginToken = value; } }
    public int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    public string selectedFormation;
    public string SelectedFormation { get { return selectedFormation; } set { selectedFormation = value; } }
    public string selectedArenaFormation;
    public string SelectedArenaFormation { get { return selectedArenaFormation; } set { selectedArenaFormation = value; } }
    public string mainCharacter;
    public string MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }
    public int mainCharacterExp;
    public int MainCharacterExp { get { return mainCharacterExp; } set { mainCharacterExp = value; } }
    public int arenaScore;
    public int ArenaScore { get { return arenaScore; } set { arenaScore = value; } }
    public int highestArenaRank;
    public int HighestArenaRank { get { return highestArenaRank; } set { highestArenaRank = value; } }
    public int highestArenaRankCurrentSeason;
    public int HighestArenaRankCurrentSeason { get { return highestArenaRankCurrentSeason; } set { highestArenaRankCurrentSeason = value; } }
    public string clanId;
    public string ClanId { get { return clanId; } set { clanId = value; } }
    public byte clanRole;
    public byte ClanRole { get { return clanRole; } set { clanRole = value; } }
    public bool isFriend;

    public bool JoinedClan
    {
        get
        {
            if (string.IsNullOrEmpty(ClanId))
                return false;
            int intId;
            if (int.TryParse(clanId, out intId) && intId <= 0)
                return false;
            return true;
        }
    }

    public bool IsClanManager
    {
        get
        {
            return JoinedClan && ClanRole == 1;
        }
    }

    public bool IsClanLeader
    {
        get
        {
            return JoinedClan && ClanRole == 2;
        }
    }

    private int level = -1;
    private int collectExp = -1;
    private int dirtyExp = -1;  // Exp for dirty check to calculate `Level` and `CollectExp` fields

    public Player()
    {
        Id = "";
        ProfileName = "";
        LoginToken = "";
        Exp = 0;
    }

    public Player Clone()
    {
        return CloneTo(this, new Player());
    }

    public static T CloneTo<T>(IPlayer from, T to) where T : IPlayer
    {
        to.Id = from.Id;
        to.ProfileName = from.ProfileName;
        to.LoginToken = from.LoginToken;
        to.Exp = from.Exp;
        to.SelectedFormation = from.SelectedFormation;
        to.SelectedArenaFormation = from.SelectedArenaFormation;
        to.MainCharacter = from.MainCharacter;
        to.MainCharacterExp = from.MainCharacterExp;
        to.ArenaScore = from.ArenaScore;
        to.HighestArenaRank = from.HighestArenaRank;
        to.HighestArenaRankCurrentSeason = from.HighestArenaRankCurrentSeason;
        to.ClanId = from.ClanId;
        to.ClanRole = from.ClanRole;
        return to;
    }

    #region Non Serialize Fields
    public int Level
    {
        get
        {
            CalculateLevelAndRemainExp();
            return level;
        }
    }
    
    public int CurrentExp
    {
        get
        {
            CalculateLevelAndRemainExp();
            return collectExp;
        }
    }
    
    public int MaxLevel
    {
        get { return GameDatabase == null ? 1 : GameDatabase.playerMaxLevel; }
    }
    
    public int RequiredExp
    {
        get { return GameDatabase == null ? 0 : GameDatabase.playerExpTable.Calculate(Level, GameDatabase.playerMaxLevel); }
    }

    public int ArenaLevel
    {
        get
        {
            int level = 0;
            foreach (var arenaRank in GameDatabase.arenaRanks)
            {
                if (ArenaScore < arenaRank.scoreToRankUp)
                    break;
                level++;
            }
            return level;
        }
    }

    public ArenaRank ArenaRank
    {
        get
        {
            int level = ArenaLevel;
            if (level >= GameDatabase.arenaRanks.Length)
                level = GameDatabase.arenaRanks.Length - 1;
            return level >= 0 ? GameDatabase.arenaRanks[level] : null;
        }
    }
    #endregion

    private void CalculateLevelAndRemainExp()
    {
        if (GameDatabase == null)
        {
            level = 1;
            collectExp = 0;
            return;
        }
        if (dirtyExp == -1 || dirtyExp != Exp)
        {
            dirtyExp = Exp;
            var remainExp = Exp;
            var maxLevel = GameDatabase.playerMaxLevel;
            for (level = 1; level < maxLevel; ++level)
            {
                var nextExp = GameDatabase.playerExpTable.Calculate(level, maxLevel);
                if (remainExp - nextExp < 0)
                    break;
                remainExp -= nextExp;
            }
            collectExp = remainExp;
        }
    }

    public static void SetData(Player data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<Player> list)
    {
        foreach (var data in list)
        {
            SetData(data);
        }
    }

    public static void RemoveDataRange(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            RemoveData(id);
        }
    }
}
