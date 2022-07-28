public class PlayerPair : BasePlayerData, IPlayerPair
{
    public string Id { get { return GetId(PlayerId, PairPlayerId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string pairPlayerId;
    public string PairPlayerId { get { return pairPlayerId; } set { pairPlayerId = value; } }

    public PlayerPair Clone()
    {
        return CloneTo(this, new PlayerPair());
    }

    public static T CloneTo<T>(IPlayerPair from, T to) where T : IPlayerPair
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.PairPlayerId = from.PairPlayerId;
        return to;
    }

    public static string GetId(string playerId, string pairPlayerId)
    {
        return playerId + "_" + pairPlayerId;
    }
}
