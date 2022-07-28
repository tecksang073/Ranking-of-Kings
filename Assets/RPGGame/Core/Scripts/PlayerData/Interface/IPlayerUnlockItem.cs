public interface IPlayerUnlockItem
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    int Amount { get; set; }
}
