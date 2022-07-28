using System.Collections.Generic;

[System.Serializable]
public class DbPlayerAuth : IPlayerAuth
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string type;
    public string Type { get { return type; } set { type = value; } }
    public string username;
    public string Username { get { return username; } set { username = value; } }
    public string password;
    public string Password { get { return password; } set { password = value; } }

    public static List<PlayerAuth> CloneList(IEnumerable<DbPlayerAuth> list)
    {
        var result = new List<PlayerAuth>();
        foreach (var entry in list)
        {
            result.Add(PlayerAuth.CloneTo(entry, new PlayerAuth()));
        }
        return result;
    }
}
