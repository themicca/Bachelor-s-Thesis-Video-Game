using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class TileManager : NetworkBehaviour
{
    Dictionary<Vector3Int, Tile> tileDictionary;
    static TileManager instance;

    private void Awake()
    {
        instance = this;
        tileDictionary = new Dictionary<Vector3Int, Tile>();
    }

    private void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public void StartTileManager()
    {
        Tile[] tiles = GetComponentsInChildren<Tile>();
        foreach (Tile tile in tiles)
        {
            RegisterTile(tile);
        }

        foreach (Tile tile in tiles)
        {
            List<Tile> neighbours = GetNeighbours(tile);
            tile.SetNeighbours(neighbours);
        }
    }

    public static TileManager GetInstance()
    {
        return instance;
    }

    public void RegisterTile(Tile tile)
    {
        tileDictionary.Add(tile.GetCubeCoordinates(), tile);
    }

    private List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        Vector3Int[] neighboursCoords = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, -1, 1),
        };

        foreach(Vector3Int neighbourCoord in neighboursCoords)
        {
            Vector3Int tileCoord = tile.GetCubeCoordinates();

            if(tileDictionary.TryGetValue(tileCoord + neighbourCoord, out Tile neighbour))
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private void OnClientConnected(ulong clientId)
    {
        SendDataToClientServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendDataToClientServerRpc(ulong clientId)
    {
        // Send the data to the client using a ClientRPC
        SendDataToClientClientRpc(clientId);
    }

    [ClientRpc]
    private void SendDataToClientClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            GetInstance().StartTileManager();
        }
    }
}
