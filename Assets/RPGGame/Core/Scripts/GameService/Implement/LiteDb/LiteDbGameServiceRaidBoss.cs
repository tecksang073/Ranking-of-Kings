using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoGetRaidEventList(string playerId, string loginToken, UnityAction<RaidEventListResult> onFinish)
    {
        var result = new RaidEventListResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoStartRaidBossBattle(string playerId, string loginToken, string eventId, UnityAction<StartRaidBossBattleResult> onFinish)
    {
        var result = new StartRaidBossBattleResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }

    protected override void DoFinishRaidBossBattle(string playerId, string loginToken, string session, EBattleResult battleResult, int totalDamage, int deadCharacters, UnityAction<FinishRaidBossBattleResult> onFinish)
    {
        var result = new FinishRaidBossBattleResult();
        result.error = GameServiceErrorCode.NOT_AVAILABLE;
        onFinish(result);
    }
}
