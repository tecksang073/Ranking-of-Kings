public interface IPlayerItem
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    int Amount { get; set; }
    int Exp { get; set; }
    string EquipItemId { get; set; }
    string EquipPosition { get; set; }
    CalculatedAttributes RandomedAttributes { get; set; }
}
