public partial interface IMail
{
    string Id { get; set; }
    string Title { get; set; }
    string Content { get; set; }
    string Currencies { get; set; }
    string Items { get; set; }
    bool HasReward { get; set; }
    bool IsRead { get; set; }
    bool IsClaim { get; set; }
    bool IsDelete { get; set; }
    long SentTimestamp { get; set; }
}
