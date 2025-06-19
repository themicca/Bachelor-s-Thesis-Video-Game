using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum SettlementType
{
    None,
    Outpost,
    Town,
    Fort
}

public abstract class SettlementBase : NetworkBehaviour, IAttackable
{
    protected int attack;
    protected int damage;
    protected int evasion;
    protected int defense;
    protected int hitPoints;

    protected int garrisonMax;
    protected int garrison;

    protected SettlementType type;
    protected GameObject garrisonDisplay;
    protected Tile tile;
    protected Combat combat;

    NetworkVariable<int> garrisonMaxNetwork = new();
    NetworkVariable<int> garrisonNetwork = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            garrisonMaxNetwork.Value = garrisonMax;
            garrisonNetwork.Value = garrison;
            UpdateGarrisonClientRpc(garrison, garrisonMax);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    protected void Awake()
    {

        Vector3 garrisonDisplayPosition = transform.position - new Vector3(0, 0.2f);
        garrisonDisplay = Instantiate(Prefabs.GetUnitDisplayPrefab(), garrisonDisplayPosition, Quaternion.identity);
        garrisonDisplay.transform.SetParent(Prefabs.GetUnitUI().transform, true);
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();

        garrisonDisplay.SetActive(false);

        GameManager.AddSettlement(this);

        if (NetworkManager.Singleton.IsHost)
        {
            Utilities.SpawnNetworkObject(gameObject);
            Utilities.ReparentNetworkObject(Prefabs.GetUnitsParent().transform, transform);
        }
    }

    private void Start()
    {
        if (IsClient)
        {
            garrisonNetwork.OnValueChanged += OnGarrisonChanged;
        }
    }

    public abstract void SetUp();

    public int GetGarrison() { return garrison; }
    public int GetGarrisonMax() {  return garrisonMax; }
    public Combat GetCombat() { return combat; }
    public Tile GetTile() { return tile; }
    public GameObject GetObject() { return gameObject; }
    public GamePlayer GetOwner() { return tile.GetOwner(); }
    public SettlementType GetSettlementType() { return type; }
    public void GetStats(out int attack, out int damage, out int evasion, out int defense, out int hitPoints, out int manpower)
    {
        attack = this.attack;
        damage = this.damage;
        evasion = this.evasion;
        defense = this.defense;
        hitPoints = this.hitPoints;
        manpower = garrison;
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        if (GetComponent<NetworkObject>().IsSpawned) UpdateTileServerRpc(tile.gameObject);
    }
    public void SetCombat(Combat combat) { this.combat = combat; }
    public void SetGarrison(int amount)
    {
        garrison = amount;
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
        if (GetComponent<NetworkObject>().IsSpawned) UpdateGarrisonServerRpc(garrison, garrisonMax);
    }

    public void IncreaseGarrison(int amount)
    {
        garrison += amount;
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
        if (GetComponent<NetworkObject>().IsSpawned) UpdateGarrisonServerRpc(garrison, garrisonMax);
    }

    public void SelectedTile()
    {
        garrisonDisplay.SetActive(true);
    }

    public void UnselectedTile()
    {
        garrisonDisplay.SetActive(false);
    }

    public bool KillManpower()
    {
        garrison -= 1;
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
        if (GetComponent<NetworkObject>().IsSpawned) UpdateGarrisonServerRpc(garrison, garrisonMax);
        return (garrison <= 0);
    }

    public void ClearCombat() { combat = null; }

    // networking methods
    private void OnGarrisonChanged(int oldValue, int newValue)
    {
        garrison = newValue;
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (GetComponent<NetworkObject>().IsSpawned)
            SendDataToClientServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendDataToClientServerRpc(ulong clientId)
    {
        // Send the data to the client using a ClientRPC
        SendDataToClientClientRpc(clientId);
        UpdateTileClientRpc(tile.gameObject);
    }

    [ClientRpc]
    private void SendDataToClientClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            garrison = garrisonNetwork.Value;
            garrisonMax = garrisonMaxNetwork.Value;
            garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
            garrisonDisplay.transform.position = transform.position - new Vector3(0, 0.2f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateGarrisonServerRpc(int garrison, int garrisonMax)
    {
        garrisonNetwork.Value = garrison;
        garrisonMaxNetwork.Value = garrisonMax;
        UpdateGarrisonClientRpc(garrison, garrisonMax);
    }

    [ClientRpc]
    private void UpdateGarrisonClientRpc(int garrison, int garrisonMax)
    {
        this.garrison = garrison;
        this.garrisonMax = garrisonMax;
        garrisonDisplay.GetComponentInChildren<TMP_Text>().text = garrison.ToString();
        garrisonDisplay.transform.position = transform.position - new Vector3(0, 0.2f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateTileServerRpc(NetworkObjectReference target)
    {
        UpdateTileClientRpc(target);
    }

    [ClientRpc]
    private void UpdateTileClientRpc(NetworkObjectReference target)
    {
        NetworkObject networkObject = target;
        Tile tile = networkObject.GetComponent<Tile>();
        this.tile = tile;
    }
    //-------------------------------

    new private void OnDestroy()
    {
        Destroy(garrisonDisplay);
        GameManager.RemoveSettlement(this);
        Destroy(gameObject);
    }
}
