using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    scout,
    soldier
}

public abstract class Unit : NetworkBehaviour, ISelectable, IAttackable, IHoverTooltip
{
    protected Tile tile;
    protected int attack;
    protected int damage;
    protected int evasion;
    protected int defense;
    protected int hitPoints;
    protected int manpower;
    protected string unitName;
    protected UnitType unitType;
    protected GameObject unitDisplay;
    protected LineRenderer _rendererPathMove;
    protected LineRenderer _rendererPathFind;
    protected UnitAction unitAction; // not used
    protected GamePlayer owner;
    protected Combat combat;
    protected UnitBookmarkUI unitBookmark;

    private float count = 1f;

    NetworkVariable<int> ownerIndex = new();
    NetworkVariable<Vector3> tilePositionNetwork = new();
    NetworkVariable<int> manpowerNetwork = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            manpowerNetwork.Value = manpower;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    protected void Awake()
    {
        Vector3 unitDisplayPosition = transform.position - new Vector3(0, 0.2f);
        unitDisplay = Instantiate(Prefabs.GetUnitDisplayPrefab(), unitDisplayPosition, Quaternion.identity);
        unitDisplay.transform.SetParent(Prefabs.GetUnitUI().transform, true);
        unitDisplay.GetComponentInChildren<TMP_Text>().text = manpower.ToString();

        _rendererPathMove = Instantiate(Prefabs.GetUnitLineRendererPrefab(), transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        _rendererPathFind = Instantiate(Prefabs.GetUnitLineRendererPrefab(), transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        _rendererPathFind.startColor = new Color(1, 1, 1, 0.5f);
        _rendererPathFind.endColor = new Color(1, 1, 1, 0.5f);
        _rendererPathFind.transform.SetParent(transform, true);
        _rendererPathMove.transform.SetParent(transform, true);

        if (NetworkManager.Singleton.IsHost)
        {
            Utilities.SpawnNetworkObject(gameObject);
            Utilities.ReparentNetworkObject(Prefabs.GetUnitsParent().transform, transform);
        }
    }

    private void Update()
    {
        unitDisplay.transform.position = transform.position - new Vector3(0, 0.2f);
        Countdown();
        if (tile != null && !tile.GetUnits().Contains(this))
        {
            tile.AddUnit(this);
        }
    }

    private void Start()
    {
        if (IsClient)
        {
            ownerIndex.OnValueChanged += OnOwnerChanged;
            tilePositionNetwork.OnValueChanged += OnPositionChanged;
            manpowerNetwork.OnValueChanged += OnManpowerChanged;
        }
    }

    private void Countdown()
    {
        count -= Time.deltaTime;
        if (count < 0)
        {
            count = 1f;
            owner = PlayerManager.GetPlayer(ownerIndex.Value);
            transform.position = new Vector3(tilePositionNetwork.Value.x - 0.1f, tilePositionNetwork.Value.y);
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y);
            foreach (Tile tile in FindFirstObjectByType<TileMap>().transform.GetComponentsInChildren<Tile>())
            {
                if (tile.transform.position == tilePositionNetwork.Value)
                {
                    this.tile = tile;
                }
            }
        }
    }

    // call whenever creating new unit
    public void SetUp(Tile tile, GamePlayer owner)
    {
        SetPosition(tile);
        SetOwner(owner);
    }

    // get methods
    public Tile GetTile() { return tile; }
    public string GetUnitName() { return unitName; }
    public LineRenderer GetPathMoveRenderer() { return _rendererPathMove; }
    public UnitAction GetUnitAction() { return unitAction; }
    public GamePlayer GetOwner() { return owner; }
    public Combat GetCombat() { return combat; }
    public GameObject GetObject() { return gameObject; }
    public UnitBookmarkUI GetSelector() { return unitBookmark;}
    public void GetStats(out int attack, out int damage, out int evasion, out int defense, out int hitPoints, out int manpower)
    { 
        attack = this.attack;
        damage = this.damage;
        evasion = this.evasion;
        defense = this.defense;
        hitPoints = this.hitPoints;
        manpower = this.manpower;
    }
    public UnitType GetUniType() { return unitType; }
    //-------------------------------

    // abstract methods
    public abstract GameObject GetActionsCard();
    //-------------------------------

    // set methods
    public void SetOwner(GamePlayer owner)
    {
        this.owner = owner;
        ownerIndex.Value = PlayerManager.GetPlayerIndex(owner);
    }
    public void SetCombat(Combat combat) { this.combat = combat;}
    public void SetSelector(StackedSelectorUI selectorParent)
    { 
        unitBookmark = Instantiate(Prefabs.GetUnitBookmarkPrefab(), selectorParent.transform).GetComponent<UnitBookmarkUI>();
        unitBookmark.SetUnit(this);
        unitBookmark.transform.GetComponentInChildren<TMP_Text>().text = unitName;
        GameManager.AddStackedSelector(selectorParent, unitBookmark);
    }
    //-------------------------------

    // finding path and movement
    public void SetPosition(Tile tile)
    {
        if (this.tile != null)
        {
            TileRemoveUnitServerRpc(gameObject);
        }
        UpdateTileServerRpc(tile.gameObject);
        TileAddUnitServerRpc(gameObject);
        transform.position = tile.transform.position;
        if (NetworkManager.Singleton.IsClient) SetPositionServerRpc(tile.transform.position);
        if (NetworkManager.Singleton.IsHost) tilePositionNetwork.Value = tile.transform.position;
        unitDisplay.transform.position = transform.position - new Vector3(0, 0.2f);
    }

    public void Move(Tile destination)
    {
        if (combat != null) { return; }
        List<Tile> tiles = PathFinder.FindPath(GetTile(), destination, GetOwner());
        if (tiles == null) return;
        GameManager.RemoveMovement(this);
        GameManager.AddMovement(new MovementController(tiles, this), this);
    }

    public void FindPathToDestination(Tile destination)
    {
        UpdateLineRenderer(FindPath(destination), _rendererPathFind);
    }

    private List<Tile> FindPath(Tile destination)
    {
        return PathFinder.FindPath(GetTile(), destination, GetOwner());
    }

    public void ClearPathMoveRenderer()
    {
        UpdateLineRenderer(new List<Tile>(), _rendererPathMove);
    }

    public void ClearPathFindRenderer()
    {
        UpdateLineRenderer(new List<Tile>(), _rendererPathFind);
    }

    public void UpdateLineRenderer(List<Tile> tiles, LineRenderer _renderer)
    {
        if (_renderer == null) { return; }
        if (tiles == null)
        {
            _renderer.positionCount = 0;
            return;
        }

        List<Vector3> points = new List<Vector3>();
        foreach (Tile tile in tiles)
        {
            points.Add(tile.transform.position);
        }

        _renderer.positionCount = points.Count;
        _renderer.SetPositions(points.ToArray());
    }
    //-------------------------------

    // user UI and interacting with tiles
    public void OnHighlightUnit() { SelectionManager.GetInstance().OnHightlightObject(gameObject); }
    public void OnSelectUnit() { SelectionManager.GetInstance().OnSelectObject(gameObject); }

    public void Highlight()
    {
        BorderUI[] borders = unitDisplay.GetComponentsInChildren<BorderUI>();
        foreach (BorderUI b in borders)
        {
            b.GetComponent<Image>().color = Color.gray;
        }
        if (unitBookmark != null)
        {
            unitBookmark.Highlight();
        }
    }

    public void Select()
    {
        BorderUI[] borders = unitDisplay.GetComponentsInChildren<BorderUI>();
        foreach (BorderUI b in borders)
        {
            b.GetComponent<Image>().color = Color.black;
        }
        if (unitBookmark != null)
        {
            unitBookmark.Select();
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
            unitDisplay.transform.SetAsLastSibling();
        }
    }

    public void ClearSelection()
    {
        ClearHighlight();
    }

    public void ClearHighlight()
    {
        BorderUI[] borders = unitDisplay.GetComponentsInChildren<BorderUI>();
        foreach (BorderUI b in borders)
        {
            b.GetComponent<Image>().color = Color.white;
        }
        if (unitBookmark != null)
        {
            unitBookmark.ClearHighlight();
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }
    //-------------------------------

    // networking methods
    private void OnOwnerChanged(int oldValue, int newValue)
    {
        owner = PlayerManager.GetPlayer(newValue);
    }

    private void OnPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        transform.position = newValue;
    }

    private void OnManpowerChanged(int oldValue, int newValue)
    {
        manpower = newValue;
        unitDisplay.GetComponentInChildren<TMP_Text>().text = manpower.ToString();
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
            owner = PlayerManager.GetPlayer(ownerIndex.Value);
            transform.position = new Vector3(tilePositionNetwork.Value.x - 0.01f, tilePositionNetwork.Value.y);
            foreach (Tile tile in FindFirstObjectByType<TileMap>().transform.GetComponentsInChildren<Tile>())
            {
                if (tile.transform.position == tilePositionNetwork.Value)
                {
                    this.tile = tile;
                }
            }
            unitDisplay.GetComponentInChildren<TMP_Text>().text = manpower.ToString();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPositionServerRpc(Vector3 position)
    {
        tilePositionNetwork.Value = position;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateManpowerServerRpc(int manpower)
    {
        manpowerNetwork.Value = manpower;
        UpdateManpowerClientRpc(manpower);
    }

    [ClientRpc]
    private void UpdateManpowerClientRpc(int manpower)
    {
        this.manpower = manpower;
        unitDisplay.GetComponentInChildren<TMP_Text>().text = manpower.ToString();
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

    [ServerRpc(RequireOwnership = false)]
    private void TileAddUnitServerRpc(NetworkObjectReference target)
    {
        TileAddUnitClientRpc(target);
    }

    [ClientRpc]
    private void TileAddUnitClientRpc(NetworkObjectReference target)
    {
        NetworkObject networkObject = target;
        Unit unit = networkObject.GetComponent<Unit>();
        tile.AddUnit(unit);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TileRemoveUnitServerRpc(NetworkObjectReference target)
    {
        TileRemoveUnitClientRpc(target);
    }

    [ClientRpc]
    private void TileRemoveUnitClientRpc(NetworkObjectReference target)
    {
        NetworkObject networkObject = target;
        Unit unit = networkObject.GetComponent<Unit>();
        tile.RemoveUnit(unit);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyUnitServerRpc()
    {
        Destroy(gameObject);
    }
    //-------------------------------

    // other methods
    public void DoAction(UnitAction action)
    {
        action.Action(this, tile);
    }

    public bool KillManpower()
    {
        manpower -= 1;
        unitDisplay.GetComponentInChildren<TMP_Text>().text = manpower.ToString();
        UpdateManpowerServerRpc(manpower);
        return (manpower <= 0);
    }

    public void ClearTile() { tile = null; }
    public void ClearCombat() { combat = null; }
    public void ClearSelector() { Destroy(unitBookmark.gameObject); }

    private new void OnDestroy()
    {
        Destroy(unitDisplay);
        if (PlayerActionCanvas.GetUnit() == this) PlayerActionCanvas.DeactivateUnitCard();
        if (unitBookmark != null) Destroy(unitBookmark.gameObject);
        tile.RemoveUnit(this);
    }
    //-------------------------------

    // not used
    public void ClearAction()
    {
        unitAction = null;
    }
    public void SetAction(UnitAction unitAction) { this.unitAction = unitAction; }

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = GetUnitName();
        description = "Owner: " + GetOwner().GetEmpireName();
        description += "\nClass: " + GetUniType();
        if (GameManager.GetMovements().Keys.Contains(this))
        {
            description += $"\nArrives in: {GameManager.GetMovements()[this].GetCurrentCost()} days";
        }
    }
}
