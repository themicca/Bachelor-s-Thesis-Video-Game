using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport.Error;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static List<GamePlayer> players;
    static Dictionary<ulong, Player> clientIds;
    private static PlayerManager instance;
    [SerializeField] private GameObject exitMenu;

    private void Awake()
    {
        instance = this;
        players = new List<GamePlayer>();
        clientIds = new Dictionary<ulong, Player>();
    }

    void Update()
    {
        UpdatePlayers();
        
    }

    private void UpdatePlayers()
    {
        foreach (GamePlayer player in players)
        {
            player.UpdateResourceText();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitMenu.SetActive(!exitMenu.activeInHierarchy);
        }
    }

    public static void ResourceCalculation()
    {
        foreach (GamePlayer player in players)
        {
            player.CalculateResources();
        }
    }

    public static void AddPlayer(GamePlayer player) { players.Add(player); }
    public static void AddClientID(ulong clientId, Player player) { clientIds[clientId] = player; }

    public static GamePlayer GetPlayer(int index) { return players.ElementAt(index); }

    public static int GetPlayerIndex(GamePlayer player) { return players.IndexOf(player); }
    public static Player GetPlayerByID(ulong id) { return clientIds[id]; }
}
