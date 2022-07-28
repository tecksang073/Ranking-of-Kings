using UnityEngine.Events;

public partial class SQLiteGameService
{
    protected override void DoCreateClan(string playerId, string loginToken, string clanName, UnityAction<CreateClanResult> onFinish)
    {
        var result = new CreateClanResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoFindClan(string playerId, string loginToken, string clanName, UnityAction<ClanListResult> onFinish)
    {
        var result = new ClanListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanJoinRequest(string playerId, string loginToken, string clanId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanJoinAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanJoinDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanMemberDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanJoinRequestDelete(string playerId, string loginToken, string clanId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClanMemberList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanOwnerTransfer(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanTerminate(string playerId, string loginToken, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClan(string playerId, string loginToken, UnityAction<ClanResult> onFinish)
    {
        var result = new ClanResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClanJoinRequestList(string playerId, string loginToken, UnityAction<PlayerListResult> onFinish)
    {
        var result = new PlayerListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClanJoinPendingRequestList(string playerId, string loginToken, UnityAction<ClanListResult> onFinish)
    {
        var result = new ClanListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanExit(string playerId, string loginToken, UnityAction<GameServiceResult> onFinish)
    {
        var result = new ClanResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanSetRole(string playerId, string loginToken, string targetPlayerId, byte clanRole, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanCheckin(string playerId, string loginToken, UnityAction<ClanCheckinResult> onFinish)
    {
        var result = new ClanCheckinResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClanCheckinStatus(string playerId, string loginToken, UnityAction<ClanCheckinStatusResult> onFinish)
    {
        var result = new ClanCheckinStatusResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClanDonation(string clanDonationDataId, string playerId, string loginToken, UnityAction<ClanDonationResult> onFinish)
    {
        var result = new ClanDonationResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetClanDonationStatus(string playerId, string loginToken, UnityAction<ClanDonationStatusResult> onFinish)
    {
        var result = new ClanDonationStatusResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }
}
