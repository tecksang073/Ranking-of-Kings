using UnityEngine.Events;

public partial class SQLiteGameService
{
    protected override void DoGetHelperList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        // Random players from fake players list
        var result = new PlayerListResult();
        var gameDb = GameInstance.GameDatabase;
        foreach (var fakePlayer in gameDb.fakePlayers)
        {
            if (fakePlayer.level <= 0 || fakePlayer.mainCharacter == null || fakePlayer.mainCharacterLevel <= 0)
                continue;
            result.list.Add(fakePlayer.MakePlayer());
        }
        onFinish(result);
    }

    protected override void DoGetFriendList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        onFinish(result);
    }

    protected override void DoGetFriendRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        onFinish(result);
    }

    protected override void DoGetPendingRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        onFinish(result);
    }

    protected override void DoFindUser(string playerId, string loginToken, string displayName, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        onFinish(result);
    }

    protected override void DoFriendRequest(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        onFinish(result);
    }

    protected override void DoFriendAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }

    protected override void DoFriendDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }

    protected override void DoFriendDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }

    protected override void DoFriendRequestDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }
}
