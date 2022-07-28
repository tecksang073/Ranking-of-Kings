using System.Collections.Generic;

[System.Serializable]
public partial class DbPlayer : IPlayer
{
    public string id;
    [LiteDB.BsonId]
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

    public static List<Player> CloneList(IEnumerable<DbPlayer> list)
    {
        var result = new List<Player>();
        foreach (var entry in list)
        {
            result.Add(Player.CloneTo(entry, new Player()));
        }
        return result;
    }
}
