using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class GamePlayer : NetworkBehaviour
{
    private Dictionary<EnumResources, int> resources;

    private Dictionary<EnumResources, int> production;

    private TMP_Text textMoney;
    private TMP_Text textWood;
    private TMP_Text textStone;
    private TMP_Text textOre;
    private TMP_Text textFood;
    private TMP_Text textManpower;

    private Color playerColor;
    private string empireName;
    private List<Tile> controledTiles;

    NetworkVariable<FixedString32Bytes> empireNameNetwork = new();
    public NetworkVariable<ulong> ClientId { get; private set; } = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            empireNameNetwork.Value = empireName;
        }
    }

    private void Awake()
    {
        controledTiles = new List<Tile>();

        production = new Dictionary<EnumResources, int>()
        {
            { EnumResources.Wood, 0 }, { EnumResources.Food, 0 }, { EnumResources.Stone, 0 },
            { EnumResources.Money, 0}, { EnumResources.Ore, 0 }, {EnumResources.Manpower, 0 }
        };

        resources = new Dictionary<EnumResources, int>()
        {
            { EnumResources.Wood, 5000 }, { EnumResources.Food, 5000 }, { EnumResources.Stone, 5000 },
            { EnumResources.Money, 500}, { EnumResources.Ore, 5000 }, {EnumResources.Manpower, 4000 }
        };

        playerColor = Settings.playerColor;
        empireName = Settings.empireName;
        name = Settings.empireName;

        Utilities.SpawnNetworkObject(gameObject);
        Utilities.ReparentNetworkObject(Prefabs.GetPlayersParent(), transform);

        textMoney = GameObject.Find("Money Text").GetComponent<TMP_Text>();
        textWood = GameObject.Find("Wood Text").GetComponent<TMP_Text>();
        textStone = GameObject.Find("Stone Text").GetComponent<TMP_Text>();
        textOre = GameObject.Find("Ore Text").GetComponent<TMP_Text>();
        textFood = GameObject.Find("Food Text").GetComponent<TMP_Text>();
        textManpower = GameObject.Find("Manpower Text").GetComponent<TMP_Text>();

        PlayerManager.AddPlayer(this);

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public void UpdateResourceText()
    {
        textMoney.text = resources[EnumResources.Money].ToString();
        textWood.text = resources[EnumResources.Wood].ToString();
        textStone.text = resources[EnumResources.Stone].ToString();
        textOre.text = resources[EnumResources.Ore].ToString();
        textFood.text = resources[EnumResources.Food].ToString();
        textManpower.text = resources[EnumResources.Manpower].ToString();
    }

    public void CalculateResources()
    {
        foreach (EnumResources resource in production.Keys)
        {
             resources[resource] += production[resource];
        }
    }

    // get methods
    public int GetMoney() { return resources[EnumResources.Money]; }
    public int GetWood() {  return resources[EnumResources.Wood]; }
    public int GetStone() { return resources[EnumResources.Stone]; }
    public int GetFood() {  return resources[EnumResources.Money]; }
    public int GetManpower() {  return resources[EnumResources.Money]; }
    public int GetOre() {  return resources[EnumResources.Money]; }
    public Color GetPlayerColor() { return playerColor; }
    public string GetEmpireName() { return empireName; }
    public int GetProduction(EnumResources resource)
    {
        return production[resource];
    }
    //-------------------------------

    // methods for checking recource calculations
    public bool CanSubstractMoney(int money)
    {
        return resources[EnumResources.Money] - money >= 0;
    }
    public bool CanSubstractWood(int wood)
    {
        return resources[EnumResources.Wood] - wood >= 0;
    }
    public bool CanSubstractStone(int stone)
    {
        return resources[EnumResources.Stone] - stone >= 0;
    }
    public bool CanSubstractFood(int food)
    {
        return resources[EnumResources.Food] - food >= 0;
    }
    public bool CanSubstractManpower(int manpower)
    {
        return resources[EnumResources.Manpower] - manpower >= 0;
    }
    public bool CanSubstractOre(int ore)
    {
        return resources[EnumResources.Ore] - ore >= 0;
    }

    public void SubstractMoney(int money)
    {
        resources[EnumResources.Money] -= money;
    }
    public void SubstractWood(int wood)
    {
        resources[EnumResources.Wood] -= wood;
    }
    public void SubstractStone(int stone)
    {
        resources[EnumResources.Stone] -= stone;
    }
    public void SubstractFood(int food)
    {
        resources[EnumResources.Food] -= food;
    }
    public void SubstractManpower(int manpower)
    {
        resources[EnumResources.Manpower] -= manpower;
    }
    public void SubstractOre(int ore)
    {
        resources[EnumResources.Ore] -= ore;
    }

    public void AddMoney(int money) { resources[EnumResources.Money] += money; }
    public void AddWood(int wood) { resources[EnumResources.Wood] += wood; }
    public void AddStone(int stone) { resources[EnumResources.Stone] += stone; }
    public void AddFood(int food) { resources[EnumResources.Food] += food; }
    public void AddManpower(int manpower) { resources[EnumResources.Manpower] += manpower; }
    public void AddOre(int ore) { resources[EnumResources.Ore] += ore; }
    public void AddProduction(EnumResources resource, int value) { production[resource] += value; }
    public void DecreaseProduction(EnumResources resource, int value) { production[resource] -= value; }
    //-------------------------------

    // set methods
    public void SetColor(Color color) { playerColor = color; }
    public void SetEmpireName(string name)
    {
        empireName = name;
        empireNameNetwork.Value = empireName;
    }
    public void SetClientId(ulong id) { ClientId.Value = id; }
    //-------------------------------

    // networking methods
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
            empireName = empireNameNetwork.Value.ToString();
        }
    }
    //-------------------------------

    public void AddTile(Tile tile) { controledTiles.Add(tile); }
    public void RemoveTile(Tile tile) { controledTiles.Remove(tile); }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            SceneManager.LoadScene(0);
    }
}
