using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateWorld : NetworkBehaviour
{
    public static CreateWorld Instance { get; private set; }

    public GameObject tilePrefabMountain;
    public GameObject tilePrefabForest;
    public GameObject tilePrefabPlainfield;
    public bool isFlatTopped;
    bool shouldOffset;
    float offset;
    float width;
    float height;
    float horizontalDistance;
    float verticalDistance;
    float yPosition;
    float xPosition;

    [SerializeField] private GamePlayer playerPrefab;
    [SerializeField] private GameObject clockPrefab;
    [SerializeField] private GameObject networkHandlerPrefab;

    List<Tile> emptyTiles;

    private void Awake()
    {
        Instance = this;
        emptyTiles = new List<Tile>();
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Player localPlayer = null;
        foreach (Player player in Settings.players)
        {
            if (AuthenticationService.Instance.PlayerId == player.Id)
            {
                localPlayer = player;
                break;
            }
        }

        PlayerManager.AddClientID(clientId, localPlayer);

        GamePlayer playerObject = Instantiate(playerPrefab.gameObject, transform.position, quaternion.identity).GetComponent<GamePlayer>();
        playerObject.SetColor(LobbyManager.Instance.GetPlayerColor(localPlayer));
        playerObject.SetEmpireName(localPlayer.Data[LobbyManager.KEY_EMPIRE_NAME].Value);
        playerObject.SetClientId(clientId);
        NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();
        networkObject.ChangeOwnership(clientId);

        Tile playerStartPosition = null, soldierTile = null;
        bool canSpawnPlayer = false;
        while (!canSpawnPlayer)
        {
            int random = Random.Range(0, emptyTiles.Count);
            playerStartPosition = emptyTiles.ElementAt(random);
            emptyTiles.RemoveAt(random);

            foreach (Tile tile in playerStartPosition.GetNeighbours())
            {
                soldierTile = tile;
                if (soldierTile.GetOwner() == null)
                {
                    canSpawnPlayer = true;
                    emptyTiles.Remove(soldierTile);
                    break;
                }

            }

            if (!canSpawnPlayer)
            {
                emptyTiles.Add(playerStartPosition);
            }
        }

        playerStartPosition.CreateSettlement(SettlementType.Town);
        playerStartPosition.SetOwner(playerObject);
        playerStartPosition.GetSettlement().SetGarrison(800);
        playerStartPosition.SetPopulation(2000);
        Unit unit = playerStartPosition.CreateUnit(UnitType.scout);

        networkObject = unit.GetComponent<NetworkObject>();
        networkObject.ChangeOwnership(clientId);
            
        unit.SendDataToClientServerRpc(clientId);

        if(!NetworkManager.Singleton.IsHost)
            Camera.main.transform.position = new Vector3(playerStartPosition.transform.position.x, playerStartPosition.transform.position.y);

        soldierTile.SetOwner(playerObject);
        soldierTile.CreateSettlement(SettlementType.Outpost);
        soldierTile.SetPopulation(1000);
        unit = soldierTile.CreateUnit(UnitType.soldier);

        networkObject = unit.GetComponent<NetworkObject>();
        networkObject.ChangeOwnership(clientId);
    }

    public void Generate(ref List<Tile> tiles)
    {
        Instantiate(clockPrefab, new Vector3(transform.position.x, transform.position.y), quaternion.identity);
        Instantiate(networkHandlerPrefab, new Vector3(transform.position.x, transform.position.y), quaternion.identity);

        width = tilePrefabMountain.transform.lossyScale.x;
        height = tilePrefabMountain.transform.lossyScale.y;
        for (int y = 0; y < Settings.gridSizeX; y++)
        {
            for (int x = 0; x < Settings.gridSizeY; x++)
            {
                if (!isFlatTopped)
                {
                    shouldOffset = (y % 2) == 0;

                    horizontalDistance = width;
                    verticalDistance = height * (3f / 4f);

                    offset = (shouldOffset) ? width / 2 - 0.066f : 0;

                    xPosition = (x * horizontalDistance - 0.13f * x) + offset;
                    yPosition = (y * verticalDistance);
                }
                else
                {
                    shouldOffset = (x % 2) == 0;

                    horizontalDistance = width * (3f / 4f);
                    verticalDistance = height;

                    offset = (shouldOffset) ? height / 2 - 0.066f : 0;

                    xPosition = (x * horizontalDistance);
                    yPosition = (y * verticalDistance - 0.13f * y) - offset;
                }
                tiles.Add(InstantiateTile(x, xPosition, y, yPosition).GetComponent<Tile>());
            }
        }
        TileManager.GetInstance().StartTileManager();

        GamePlayer computerPlayer = Instantiate(playerPrefab.gameObject, transform.position, quaternion.identity).GetComponent<GamePlayer>();
        computerPlayer.SetColor(Color.red);
        computerPlayer.SetEmpireName("Computer");
        computerPlayer.SetClientId(Constants.COMPUTER_PLAYER_ID);

        emptyTiles.AddRange(tiles);
        int random = Random.Range(0, emptyTiles.Count);
        Tile surroundedTile = emptyTiles.ElementAt(random);
        emptyTiles.RemoveAt(random);
        foreach (Tile tile in surroundedTile.GetNeighbours())
        {
            int randomSettlement = Random.Range(1, 3);
            emptyTiles.Remove(tile);
            tile.SetOwner(computerPlayer);
            tile.CreateUnit(UnitType.soldier);
            tile.CreateSettlement((SettlementType)randomSettlement);
            if (randomSettlement == 2)
            {
                tile.GetSettlement().SetGarrison(600);
                tile.SetPopulation(1000);
            }
        }

        GamePlayer playerObject = Instantiate(playerPrefab.gameObject, transform.position, quaternion.identity).GetComponent<GamePlayer>();
        playerObject.SetColor(Settings.playerColor);
        playerObject.SetEmpireName(Settings.empireName);
        playerObject.SetClientId(NetworkManager.Singleton.LocalClientId);

        Tile playerStartPosition = null, soldierTile = null;
        bool canSpawnPlayer = false;
        while (!canSpawnPlayer)
        {
            random = Random.Range(0, emptyTiles.Count);
            playerStartPosition = emptyTiles.ElementAt(random);
            emptyTiles.RemoveAt(random);

            foreach (Tile tile in playerStartPosition.GetNeighbours())
            {
                soldierTile = tile;
                if (soldierTile.GetOwner() == null)
                {
                    canSpawnPlayer = true;
                    emptyTiles.Remove(soldierTile);
                    break;
                }

            }

            if (!canSpawnPlayer)
            {
                emptyTiles.Add(playerStartPosition);
            }
        }

        playerStartPosition.CreateSettlement(SettlementType.Town);
        playerStartPosition.SetOwner(playerObject);
        playerStartPosition.GetSettlement().SetGarrison(800);
        playerStartPosition.SetPopulation(2000);
        playerStartPosition.CreateUnit(UnitType.scout);

        Camera.main.transform.position = new Vector3(playerStartPosition.transform.position.x, playerStartPosition.transform.position.y);

        soldierTile.SetOwner(playerObject);
        soldierTile.CreateSettlement(SettlementType.Outpost);
        soldierTile.SetPopulation(1000);
        soldierTile.CreateUnit(UnitType.soldier);

        for (int i = 0; i < Settings.gridSizeX; i++)
        {
            int randomSettlement = Random.Range(1, 3);
            random = Random.Range(0, emptyTiles.Count);
            Tile enemyPosition = emptyTiles.ElementAt(random);
            emptyTiles.RemoveAt(random);

            enemyPosition.SetOwner(computerPlayer);
            enemyPosition.CreateUnit(UnitType.soldier);
            enemyPosition.CreateSettlement((SettlementType) randomSettlement);
            if (randomSettlement == 2)
            {
                enemyPosition.GetSettlement().SetGarrison(600);
                enemyPosition.SetPopulation(1000);
            }
        }
    }

    public GameObject InstantiateTile(int x, float xPosition, int y, float yPosition)
    {
        TileType tileType;
        tileType = (TileType)Random.Range(0, 3);
        GameObject tile = SetTile(tileType, x, y);
        tile.name = $"{tile.GetComponent<Tile>().GetTileType()} tile C{x}:R{y}";
        return tile;
    }

    public GameObject SetTile(TileType tileType, int x, int y)
    {
        GameObject tile = null;
        switch (tileType)
        {
            case TileType.Plainfield:
                tile = Instantiate(tilePrefabPlainfield, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
                break;
            case TileType.Forest:
                tile = Instantiate(tilePrefabForest, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
                break;
            case TileType.Mountain:
                tile = Instantiate(tilePrefabMountain, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
                break;
        }

        tile.GetComponent<Tile>().SetOffsetCoordinates(new Vector2Int(x, y));
        tile.GetComponent<Tile>().SetCubeCoordinates(Utilities.OffsetToCube(tile.GetComponent<Tile>().GetOffsetCoordinates()));
        return tile;
    }
}
