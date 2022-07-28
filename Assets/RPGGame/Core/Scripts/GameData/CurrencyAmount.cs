[System.Serializable]
public struct CurrencyAmount
{
    public string id;
    public int amount;

    public string ToJson()
    {
        return "{" +
            "\"id\":\"" + id + "\"," +
            "\"amount\":" + amount + "}";
    }
}
