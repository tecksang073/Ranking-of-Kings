[System.Serializable]

public partial class Mail : IMail
{
    public string id;
    public string Id { get { return id; } set { id = value; } }
    public string title;
    public string Title { get { return title; } set { title = value; } }
    public string content;
    public string Content { get { return content; } set { content = value; } }
    public string currencies;
    public string Currencies { get { return currencies; } set { currencies = value; } }
    public string items;
    public string Items { get { return items; } set { items = value; } }
    public bool hasReward;
    public bool HasReward { get { return hasReward; } set { hasReward = value; } }
    public bool isRead;
    public bool IsRead { get { return isRead; } set { isRead = value; } }
    public bool isClaim;
    public bool IsClaim { get { return isClaim; } set { isClaim = value; } }
    public bool isDelete;
    public bool IsDelete { get { return isDelete; } set { isDelete = value; } }
    public long sentTimestamp;
    public long SentTimestamp { get { return sentTimestamp; } set { sentTimestamp = value; } }

    public Mail Clone()
    {
        return CloneTo(this, new Mail());
    }

    public static T CloneTo<T>(IMail from, T to) where T : IMail
    {
        to.Id = from.Id;
        to.Title = from.Title;
        to.Content = from.Content;
        to.HasReward = from.HasReward;
        to.IsRead = from.IsRead;
        to.IsClaim = from.IsClaim;
        to.IsDelete = from.IsDelete;
        to.SentTimestamp = from.SentTimestamp;
        return to;
    }
}
