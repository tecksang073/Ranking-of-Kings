using UnityEngine.Events;
using Mono.Data.Sqlite;

public partial class SQLiteGameService
{
    protected override void DoLogin(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else
        {
            Player player = null;
            if (!TryGetPlayer(AUTH_NORMAL, username, password, out player))
                result.error = GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD;
            else
            {
                player = UpdatePlayerLoginToken(player);
                UpdatePlayerStamina(player);
                result.player = player;
            }
        }
        onFinish(result);
    }

    protected override void DoRegisterOrLogin(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        if (IsPlayerWithUsernameFound(AUTH_NORMAL, username))
            DoLogin(username, password, onFinish);
        else
            DoRegister(username, password, onFinish);
    }

    protected override void DoGuestLogin(string deviceId, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(deviceId))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else if (IsPlayerWithUsernameFound(AUTH_GUEST, deviceId))
        {
            Player player = null;
            if (!TryGetPlayer(AUTH_GUEST, deviceId, deviceId, out player))
                result.error = GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD;
            else
            {
                player = UpdatePlayerLoginToken(player);
                UpdatePlayerStamina(player);
                result.player = player;
            }
        }
        else
        {
            var player = InsertNewPlayer(AUTH_GUEST, deviceId, deviceId);
            result.player = player;
        }
        onFinish(result);
    }

    protected override void DoValidateLoginToken(string playerId, string loginToken, bool refreshToken, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            if (refreshToken)
                player = UpdatePlayerLoginToken(player);
            UpdatePlayerStamina(player);
            result.player = player;
        }
        onFinish(result);
    }

    protected override void DoSetProfileName(string playerId, string loginToken, string profileName, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        var playerWithName = ExecuteScalar("SELECT COUNT(*) FROM player WHERE profileName=@profileName", new SqliteParameter("profileName", profileName));
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (string.IsNullOrEmpty(profileName))
            result.error = GameServiceErrorCode.EMPTY_PROFILE_NAME;
        else if (playerWithName != null && (long)playerWithName > 0)
            result.error = GameServiceErrorCode.EXISTED_PROFILE_NAME;
        else
        {
            player.ProfileName = profileName;
            ExecuteNonQuery(@"UPDATE player SET profileName=@profileName WHERE id=@id",
                new SqliteParameter("@profileName", player.ProfileName),
                new SqliteParameter("@id", player.Id));
            result.player = player;
        }
        onFinish(result);
    }

    protected override void DoRegister(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else if (IsPlayerWithUsernameFound(AUTH_NORMAL, username))
            result.error = GameServiceErrorCode.EXISTED_USERNAME;
        else
        {
            var player = InsertNewPlayer(AUTH_NORMAL, username, password);
            result.player = player;
        }
        onFinish(result);
    }
}
