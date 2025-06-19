using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject networkManager;

    public async void StartSingleplayerGame() 
    {
        Settings.singlePlayer = true;
        
        Debug.Log("Start game");

        await UnityServices.InitializeAsync();
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await RelayManager.Instance.CreateRelay(1);

        SceneManager.LoadScene(1);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void HideToolTip()
    {
        ShowTooltip.HideTooltip();
    }
}
