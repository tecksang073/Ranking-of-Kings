using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManagement : MonoBehaviour
{
    public void OnClickLogout()
    {
        GameInstance.GameService.Logout(() =>
        {
            SceneManager.LoadScene(GameInstance.Singleton.loginScene);
        });        
    }
}
