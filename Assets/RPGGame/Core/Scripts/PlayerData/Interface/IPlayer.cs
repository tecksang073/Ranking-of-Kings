public partial interface IPlayer
{
    string Id { get; set; }
    string ProfileName { get; set; }
    string LoginToken { get; set; }
    int Exp { get; set; }
    string SelectedFormation { get; set; }
    string SelectedArenaFormation { get; set; }
    string MainCharacter { get; set; }
    int MainCharacterExp { get; set; }
    int ArenaScore { get; set; }
    int HighestArenaRank { get; set; }
    int HighestArenaRankCurrentSeason { get; set; }
    string ClanId { get; set; }
    byte ClanRole { get; set; }
}
