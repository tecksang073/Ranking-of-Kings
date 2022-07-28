using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAuthentication : UIBase
{
    public InputField inputUsername;
    public InputField inputPassword;
    public UnityEvent eventLogin;
    public UnityEvent eventRegister;
    public UnityEvent eventError;

    public string Username
    {
        get { return inputUsername == null ? "" : inputUsername.text; }
        set { if (inputUsername != null) inputUsername.text = value; }
    }

    public string Password
    {
        get { return inputPassword == null ? "" : inputPassword.text; }
        set { if (inputPassword != null) inputPassword.text = value; }
    }

    public bool IsRememberLogin
    {
        get { return GameInstance.Singleton.isRememberLogin; }
        set { GameInstance.Singleton.isRememberLogin = value; }
    }

    public void OnClickLogin()
    {
        var gameService = GameInstance.GameService;
        gameService.Login(Username, Password, OnLoginSuccess, OnError);
    }

    public void OnClickRegister()
    {
        var gameService = GameInstance.GameService;
        gameService.Register(Username, Password, OnRegisterSuccess, OnError);
    }

    public void OnClickRegisterOrLogin()
    {
        var gameService = GameInstance.GameService;
        gameService.RegisterOrLogin(Username, Password, OnLoginSuccess, OnError);
    }

    public void OnClickGuestLogin()
    {
        var gameService = GameInstance.GameService;
        var duid = SystemInfo.deviceUniqueIdentifier;
        gameService.GuestLogin(duid, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(PlayerResult result)
    {
        var gameInstance = GameInstance.Singleton;
        gameInstance.OnGameServiceLogin(result);
        eventLogin.Invoke();
    }

    private void OnRegisterSuccess(PlayerResult result)
    {
        var gameInstance = GameInstance.Singleton;
        eventRegister.Invoke();
    }

    private void OnError(string error)
    {
        var gameInstance = GameInstance.Singleton;
        gameInstance.OnGameServiceError(error);
        eventError.Invoke();
    }
}
