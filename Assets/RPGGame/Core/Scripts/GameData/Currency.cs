using UnityEngine;

[System.Serializable]
public struct Currency : IGameData
{
    public string id;
    public Sprite icon;
    public Sprite icon2;
    public Sprite icon3;
    public int startAmount;

    public string Id { get { return id; } }
    public Sprite Icon { get { return icon; } }
    public Sprite Icon2 { get { return icon2; } }
    public Sprite Icon3 { get { return icon3; } }

    public string ToJson()
    {
        return "{\"id\":\"" + id + "\"," +
            "\"startAmount\":" + startAmount + "}";
    }
}
