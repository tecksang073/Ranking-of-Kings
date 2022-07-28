public class UIGameDataWithAmountList : UIDataItemList<UIGameDataWithAmount, IGameData>
{
    public UIGameDataWithAmount SetListItem(IGameData data, int amount)
    {
        if (data == null)
            return null;
        string id = System.Guid.NewGuid().ToString();
        var item = SetListItem(id);
        item.SetData(data);
        item.Amount = amount;
        return item;
    }
}
