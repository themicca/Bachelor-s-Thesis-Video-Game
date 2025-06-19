using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JoinedLobby : MonoBehaviour
{
    public static JoinedLobby Instance { get; private set; }

    [SerializeField] private Transform playerUIPrefab;
    [SerializeField] private Transform playerContainer;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Image playerColor;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject playerSettingsWindow;
    [SerializeField] private TMP_InputField empireNameInput;
    [SerializeField] private Image playerColorButton;

    private void Awake()
    {
        Instance = this;

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });
    }

    private void OnEnable()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
    }

    public void MaxCharacters()
    {
        int maxChars = 20;
        empireNameInput.characterLimit = maxChars;
    }

    public void UpdateEmpireName()
    {
        LobbyManager.Instance.UpdateEmpireName(empireNameInput.text);
    }

    public void UpdatePlayerColor()
    {
        LobbyManager.Instance.UpdatePlayerColor(playerColorButton.color);
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
    {
        ClearLobby();
        Hide();
    }
    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        ClearLobby();

        foreach (Player player in lobby.Players)
        {
            GameObject playerTemplate = Instantiate(playerUIPrefab, playerContainer).gameObject;
            PlayerTemplateUI playerUI = playerTemplate.GetComponent<PlayerTemplateUI>();
            playerUI.UpdatePlayer(player);
        }

        startGameButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

        Show();
    }

    private void ClearLobby()
    {
        foreach (Transform child in playerContainer)
        {
            if (child == playerUIPrefab) continue;
            Destroy(child.gameObject);
        }
    }

    public void ShowPlayerSettings()
    {
        playerSettingsWindow.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
