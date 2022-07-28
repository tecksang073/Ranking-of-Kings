public interface IPlayerBattle
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    string Session { get; set; }
    byte BattleResult { get; set; }
    int Rating { get; set; }
    byte BattleType { get; set; }
}
