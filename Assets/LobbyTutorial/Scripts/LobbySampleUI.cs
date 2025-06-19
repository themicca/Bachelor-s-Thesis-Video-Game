using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySampleUI : MonoBehaviour {


    public static LobbySampleUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button changeMarineButton;
    [SerializeField] private Button changeNinjaButton;
    [SerializeField] private Button changeZombieButton;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button changeGameModeButton;


    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        changeMarineButton.onClick.AddListener(() => {
            LobbyManagerSample.Instance.UpdatePlayerCharacter(LobbyManagerSample.PlayerCharacter.Marine);
        });
        changeNinjaButton.onClick.AddListener(() => {
            LobbyManagerSample.Instance.UpdatePlayerCharacter(LobbyManagerSample.PlayerCharacter.Ninja);
        });
        changeZombieButton.onClick.AddListener(() => {
            LobbyManagerSample.Instance.UpdatePlayerCharacter(LobbyManagerSample.PlayerCharacter.Zombie);
        });

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManagerSample.Instance.LeaveLobby();
        });

        changeGameModeButton.onClick.AddListener(() => {
            LobbyManagerSample.Instance.ChangeGameMode();
        });
    }

    private void Start() {
        LobbyManagerSample.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManagerSample.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManagerSample.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        LobbyManagerSample.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManagerSample.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManagerSample.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManagerSample.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            /*LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                LobbyManagerSample.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);*/
        }

        changeGameModeButton.gameObject.SetActive(LobbyManagerSample.Instance.IsLobbyHost());

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = lobby.Data[LobbyManagerSample.KEY_GAME_MODE].Value;

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}