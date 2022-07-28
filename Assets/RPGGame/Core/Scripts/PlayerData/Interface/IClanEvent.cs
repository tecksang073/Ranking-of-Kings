public partial interface IClanEvent
{
    string Id { get; set; }
    string DataId { get; set; }
    int RemainingHp { get; set; }
    long StartTime { get; set; }
    long EndTime { get; set; }
}
