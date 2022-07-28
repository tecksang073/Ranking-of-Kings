public interface IPlayerAchievement
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    int Progress { get; set; }
    bool Earned { get; set; }
}
