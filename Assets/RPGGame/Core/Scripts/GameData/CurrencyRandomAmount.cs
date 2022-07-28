[System.Serializable]
public struct CurrencyRandomAmount
{
    public string Id { get; set; }
    public int MinAmount { get; set; }
    public int MaxAmount { get; set; }

    public string ToJson()
    {
        return "{" +
            "\"id\":\"" + Id + "\"," +
            "\"minAmount\":" + MinAmount + "," +
            "\"maxAmount\":" + MaxAmount + "}";
    }
}
