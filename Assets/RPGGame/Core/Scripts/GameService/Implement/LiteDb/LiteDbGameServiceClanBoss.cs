using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoGetClanEventList(string playerId, string loginToken, UnityAction<ClanEventListResult> onFinish)
    {
        var result = new ClanEventListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoStartClanBossBattle(string playerId, string loginToken, string eventId, UnityAction<StartClanBossBattleResult> onFinish)
    {
        var result = new StartClanBossBattleResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoFinishClanBossBattle(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishClanBossBattleResult> onFinish)
    {
        var result = new FinishClanBossBattleResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }
}
