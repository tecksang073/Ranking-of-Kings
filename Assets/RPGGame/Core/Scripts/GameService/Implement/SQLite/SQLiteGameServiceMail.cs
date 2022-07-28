using UnityEngine.Events;

public partial class SQLiteGameService : BaseGameService
{
    protected override void DoGetMailList(string playerId, string loginToken, UnityAction<MailListResult> onFinish)
    {
        var result = new MailListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoReadMail(string playerId, string loginToken, string id, UnityAction<ReadMailResult> onFinish)
    {
        var result = new ReadMailResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoClaimMailRewards(string playerId, string loginToken, string id, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoDeleteMail(string playerId, string loginToken, string id, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoGetMailsCount(string playerId, string loginToken, UnityAction<MailsCountResult> onFinish)
    {
        var result = new MailsCountResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }
}

