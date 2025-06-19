using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Services.Authentication;

public class PlayerTemplateUI : MonoBehaviour, IHoverTooltip
{
    [field: SerializeField] public TMP_Text PlayerNameText { get; private set; }
    [field: SerializeField] public TMP_Text EmpireNameText { get; private set; }
    [field: SerializeField] public Image PlayerColor { get; private set; }

    Player player;

    public void UpdatePlayer(Player player)
    {
        this.player = player;

        PlayerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        EmpireNameText.text = player.Data[LobbyManager.KEY_EMPIRE_NAME].Value;
        PlayerColor.color = LobbyManager.Instance.GetPlayerColor(player);

        if (AuthenticationService.Instance.PlayerId == player.Id)
        {
            GetComponent<Button>().onClick.AddListener(() => { JoinedLobby.Instance.ShowPlayerSettings(); });
        }
    }

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        if (player.Id == AuthenticationService.Instance.PlayerId)
        {
            header = "You";
            description = "Click to change your player settings.";
        }
        else
        {
            header = "Player " + player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        }
    }
}
