public partial interface IChatMessage
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string ClanId { get; set; }
    string ProfileName { get; set; }
    string ClanName { get; set; }
    string Message { get; set; }
    long ChatTime { get; set; }
}
