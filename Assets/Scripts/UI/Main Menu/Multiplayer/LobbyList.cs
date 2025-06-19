using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyList : MonoBehaviour
{
    public static LobbyList Instance { get; private set; }

    [SerializeField] private TMP_Text maxPlayersText;
    [SerializeField] private GameObject maxPlayersChooser;
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private GameObject privateLobbyText;
    [SerializeField] private Button createButton;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyPrefab;
    [SerializeField] private GameObject joinedLobby;
    [SerializeField] private GameObject createLobbyWindow;

    string lobbyName = "Name";
    int maxPlayers = 2;
    bool isPrivate = false;

    private void Awake()
    {
        Instance = this;

        createButton.onClick.AddListener(() => {
            LobbyManager.Instance.CreateLobby(
                lobbyName,
                maxPlayers,
                isPrivate
            );
        });
    }

    private void OnEnable()
    {
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    public void ShowMaxPlayersChooser()
    {
        maxPlayersChooser.SetActive(!maxPlayersChooser.activeInHierarchy);
    }

    public void PrivateButtonClick()
    {
        privateLobbyText.SetActive(!privateLobbyText.activeInHierarchy);
        isPrivate = privateLobbyText.activeInHierarchy;
    }

    public void UpdateMaxPlayers(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
        maxPlayersText.text = maxPlayers.ToString();
    }

    public void MaxCharacters()
    {
        int maxChars = 20;
        lobbyNameInput.characterLimit = maxChars;
    }

    public void UpdateLobbyName()
    {
        lobbyName = lobbyNameInput.text;
    }

    public void RefreshButtonClick()
    {
        LobbyManager.Instance.RefreshLobbyList();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList) {
            Debug.Log(lobby);
            Transform lobbyTransform = Instantiate(lobbyPrefab, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyUI>().UpdateLobby(lobby);
        }

        Debug.Log("Lobbies Refreshed!");
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        JoinLobby();
    }

    private void JoinLobby()
    {
        createLobbyWindow.SetActive(false);
        joinedLobby.SetActive(true);
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
