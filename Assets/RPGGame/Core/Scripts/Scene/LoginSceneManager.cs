using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    public static LoginSceneManager Singleton { get; private set; }
    public UIAuthentication loginDialog;
    public UIAuthentication registerDialog;
    public GameObject clickStartObject;
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        HideLoginDialog();
        ShowClickStart();
    }

    public void OnClickStart()
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        gameService.ValidateLoginToken(true, OnValidateLoginTokenSuccess, OnValidateLoginTokenError);
        HideClickStart();
    }

    private void OnValidateLoginTokenSuccess(PlayerResult result)
    {
        var gameInstance = GameInstance.Singleton;
        gameInstance.OnGameServiceLogin(result);
    }

    private void OnValidateLoginTokenError(string error)
    {
        var gameInstance = GameInstance.Singleton;
        ShowLoginDialog();
    }

    public void ShowLoginDialog()
    {
        if (loginDialog != null)
            loginDialog.Show();
    }

    public void HideLoginDialog()
    {
        if (loginDialog != null)
            loginDialog.Hide();
    }

    public void ShowRegisterDialog()
    {
        if (registerDialog != null)
            registerDialog.Show();
    }

    public void HideRegisterDialog()
    {
        if (registerDialog != null)
            registerDialog.Hide();
    }

    public void ShowClickStart()
    {
        if (clickStartObject != null)
            clickStartObject.SetActive(true);
    }

    public void HideClickStart()
    {
        if (clickStartObject != null)
            clickStartObject.SetActive(false);
    }
}
