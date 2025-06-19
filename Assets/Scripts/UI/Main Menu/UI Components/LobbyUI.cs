using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour, IHoverTooltip
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;

    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.Instance.JoinLobby(lobby);
        });
    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
    }

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Join the lobby";
        description = "Click to join this lobby if there are available slots.";
    }
}
