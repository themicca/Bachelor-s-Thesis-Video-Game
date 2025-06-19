using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMenu : MonoBehaviour
{
    [SerializeField] private GameObject exitMenu;

    public void ClickMenu()
    {
        exitMenu.SetActive(true);
    }

    public static void JumpToMainMenu()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(0);
            LobbyManager.Instance.LeaveLobby();
            AuthenticationService.Instance.SignOut();
        }
        else
        {
            NetworkHandlerCustom.Instance.DisconnectPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            LobbyManager.Instance.LeaveLobby();
            AuthenticationService.Instance.SignOut();
        }
    }

    public void Continue()
    {
        exitMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
