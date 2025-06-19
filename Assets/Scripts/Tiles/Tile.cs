using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TileType
{
    Plainfield,
    Forest,
    Mountain
}

public abstract class Tile : NetworkBehaviour, IHoverTooltip, ISelectable
{
    protected TileType tileType;
    protected Vector2Int offsetCoordinates;
    protected Vector3Int cubeCoordinates;
    protected List<Tile> neighbours;
    protected int pathCost;
    protected SettlementBase settlement;
    protected List<Unit> units;
    protected GamePlayer owner;
    protected SpriteRenderer playerColorObject;
    protected Building[] buildings;
    protected int production;

    protected int totalWoodProduction;
    protected int totalStoneProduction;
    protected int totalFoodProduction;
    protected int totalOreProduction;

    protected int minPopulation;
    protected int population;
    protected int freePopulation;
    protected int employedPopulation;
    protected float populationGrowth;
    protected float stackedGrowth;

    private float count = 1f;

    NetworkVariable<Color> ownerColor = new();
    NetworkVariable<char> ownerIndex = new();
    NetworkVariable<Vector2Int> offsetCoordinatesNetwork = new();
    NetworkVariable<Vector3Int> cubeCoordinatesNetwork = new();
    protected NetworkVariable<int> pathCostNetwork = new();

    NetworkVariable<int> minPopulationNetwork = new();
    NetworkVariable<int> populationNetwork = new();
    NetworkVariable<int> freePopulationNetwork = new();
    NetworkVariable<int> employedPopulationNetwork = new();
    NetworkVariable<float> populationGrowthNetwork = new();
    NetworkVariable<float> stackedGrowthNetwork = new();

    NetworkVariable<int> totalWoodProductiobNetwork = new();
    NetworkVariable<int> totalStoneProductiobNetwork = new();
    NetworkVariable<int> totalFoodProductiobNetwork = new();
    NetworkVariable<int> totalOreProductiobNetwork = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            playerColorObject = Instantiate(Prefabs.GetPlayerColorPrefab(), transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            Utilities.SpawnNetworkObject(playerColorObject.gameObject);
            Utilities.ReparentNetworkObject(transform, playerColorObject.transform);
            pathCostNetwork.Value = pathCost;
            ownerIndex.Value = Constants.NO_OWNER;

            minPopulationNetwork.Value = minPopulation;
            populationNetwork.Value = population;
            freePopulationNetwork.Value = freePopulation;
            employedPopulationNetwork.Value = employedPopulation;
            populationGrowthNetwork.Value = populationGrowth;
            stackedGrowthNetwork.Value = stackedGrowth;

            totalWoodProductiobNetwork.Value = totalWoodProduction;
            totalStoneProductiobNetwork.Value = totalStoneProduction;
            totalFoodProductiobNetwork.Value = totalFoodProduction;
            totalOreProductiobNetwork.Value = totalOreProduction;

            UpdatePopulationClientRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    protected void Awake()
    {
        buildings = new Building[Constants.BUILDING_SLOT_COUNT];
        units = new List<Unit>();
        production = 2;
        population = 0;
        freePopulation = population;
        employedPopulation = 0;
        populationGrowth = 0;
        minPopulation = 200;

        if (NetworkManager.Singleton.IsHost)
        {
            Utilities.SpawnNetworkObject(gameObject);
            Utilities.ReparentNetworkObject(Prefabs.GetTileMapParent().transform, transform);
        }
    }

    private void Start()
    {
        if (IsClient)
        {
            ownerColor.OnValueChanged += OnColorChanged;
            ownerIndex.OnValueChanged += OnOwnerChanged;
        }
    }

    private void Update()
    {
        Countdown();
    }

    private void Countdown()
    {
        count -= Time.deltaTime;
        if (count < 0)
        {
            count = 1f;
            if (settlement != null)
            {
                if (NetworkManager.Singleton.IsHost && settlement.GetComponent<NetworkObject>().IsSpawned)
                    UpdateSettlementClientRpc(settlement.gameObject);
                else if (!NetworkManager.Singleton.IsHost && settlement.GetComponent<NetworkObject>().IsSpawned)
                    UpdateSettlementServerRpc(settlement.gameObject);
            }
        }
    }

    // get methods
    public TileType GetTileType() { return tileType; }
    public Vector2Int GetOffsetCoordinates() { return offsetCoordinates; }
    public Vector3Int GetCubeCoordinates() { return cubeCoordinates; }
    public List<Tile> GetNeighbours() { return neighbours; }
    public int GetPathCost() { return pathCost; }
    public GamePlayer GetOwner() { return owner; }
    public Building[] GetBuildings() { return buildings; }
    public List<Unit> GetUnits() { return units; }
    public SettlementBase GetSettlement() { return settlement; }
    public SettlementType GetSettlementType()
    {
        if (settlement != null) return settlement.GetSettlementType();
        return SettlementType.None;
    }
    public int GetPopulation() { return population; }
    public int GetFreePopulation() { return freePopulation; }
    public int GetEmployedPopulation() { return employedPopulation; }
    public float GetPopulationGrowth() { return populationGrowth; }
    public int GetMinPopulation() { return minPopulation; }
    // -------------------------------

    // set methods
    public void SetLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
        if (settlement != null) settlement.gameObject.layer = LayerMask.NameToLayer(layerName);
        playerColorObject.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public void SetOwner(GamePlayer owner)
    {
        if (this.owner != null)
        {
            DecreaseProduction();
            foreach (Building building in buildings)
            {
                if (building != null)
                {
                    building.DecreaseProduction();
                }
            }
        }

        this.owner = owner;
        if (owner != null)
        {
            playerColorObject.color = owner.GetPlayerColor();
            if (NetworkManager.Singleton.IsHost)
            {
                ownerColor.Value = playerColorObject.color;
                ownerIndex.Value = (char)PlayerManager.GetPlayerIndex(owner);
            }
            else
            {
                SetOwnerServerRpc(playerColorObject.color, (char)PlayerManager.GetPlayerIndex(owner));
            }
            owner.AddTile(this);
            AddProduction();
            foreach (Building building in buildings)
            {
                if (building != null)
                {
                    building.AddProduction();
                }
            }
            ChangeOnwershipServerRpc(owner.ClientId.Value);
            return;
        }
        playerColorObject.color = new Color(0, 0, 0, 0);
        if (NetworkManager.Singleton.IsHost)
        {
            ownerColor.Value = playerColorObject.color;
            ownerIndex.Value = Constants.NO_OWNER;
        }
        else
        {
            SetOwnerServerRpc(playerColorObject.color, Constants.NO_OWNER);
        }
    }

    public void SetOffsetCoordinates(Vector2Int coordinates)
    {
        offsetCoordinates = coordinates;
        offsetCoordinatesNetwork.Value = offsetCoordinates;
    }
    public void SetCubeCoordinates(Vector3Int coordinates)
    {
        cubeCoordinates = coordinates;
        cubeCoordinatesNetwork.Value = cubeCoordinates;
    }
    public void SetNeighbours(List<Tile> neighbours) { this.neighbours = neighbours; }
    public void SetBuilding(Building building, int index)
    {
        buildings[index] = building;
        UpdateBuildingServerRpc(building.GetBuildingType(), index);
    }
    public void SetPopulation(int amount)
    {
        population = amount;
        freePopulation = amount;
        populationGrowth = population * Constants.BASE_POPULATION_GROWTH;
        UpdatePopulationServerRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
    }
    // -------------------------------

    // abstract methods
    public abstract void AddProduction();
    public abstract void DecreaseProduction();
    public abstract void ShowProduction();
    // -------------------------------


    // user UI and interacting with tiles
    public void OnHighlightTile() { SelectionManager.GetInstance().OnHightlightObject(gameObject); }
    public void OnSelectTile() { SelectionManager.GetInstance().OnSelectObject(gameObject); }
    public void OnRightClickTile() { SelectionManager.GetInstance().OnRightClickTile(this); }

    public void ClearLayerForCamera()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (settlement != null) settlement.gameObject.layer = LayerMask.NameToLayer("Default");
        playerColorObject.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void CreateContent(ref string header, ref string content, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = GetTileType().ToString();
        if (GetOwner() == null)
        {
            content = "Unoccupied";
        }
        else
        {
            content = "Owner: " + GetOwner().GetEmpireName();
        }
        content += "\nSettlemet: " + GetSettlementType();
    }

    public void Highlight()
    {
        GameObject highlighter = Prefabs.GetTileHighlighterPrefab();
        if (settlement != null) { settlement.SelectedTile(); }
        highlighter.transform.position = transform.position;
        highlighter.SetActive(true);
    }

    public void Select()
    {
        GameObject selector = Prefabs.GetTileSelectorPrefab();
        if (settlement != null) { settlement.SelectedTile(); }
        selector.transform.position = transform.position;
        selector.SetActive(true);
        Prefabs.GetTileHighlighterPrefab().SetActive(false);
    }

    public void ClearSelection()
    {
        if (settlement != null) { settlement.UnselectedTile(); }
        Prefabs.GetTileSelectorPrefab().SetActive(false);
    }

    public void ClearHighlight()
    {
        if (settlement != null) { settlement.UnselectedTile(); }
        Prefabs.GetTileHighlighterPrefab().SetActive(false);
    }

    protected void ShowWichProduction()
    {
        foreach (Building building in buildings)
        {
            if (building == null) continue;
            if (building.GetBuildingType() == BuildingType.Stonemason)
            {
                totalStoneProduction += building.GetProduction();
            }
            else if (building.GetBuildingType() == BuildingType.Woodcutter)
            {
                totalWoodProduction += building.GetProduction();
            }
            else if (building.GetBuildingType() == BuildingType.Farm)
            {
                totalFoodProduction += building.GetProduction();
            }
            else if (building.GetBuildingType() == BuildingType.OreMine)
            {
                totalOreProduction += building.GetProduction();
            }
        }

        GameObject info;
        info = TileInformation.GetStoneProduction().gameObject;
        info.SetActive(true);
        info.GetComponentInChildren<TMP_Text>().text = "+ " + totalStoneProduction + " ";

        info = TileInformation.GetWoodProduction().gameObject;
        info.SetActive(true);
        info.GetComponentInChildren<TMP_Text>().text = "+ " + totalWoodProduction + " ";

        info = TileInformation.GetFoodProduction().gameObject;
        info.SetActive(true);
        info.GetComponentInChildren<TMP_Text>().text = "+ " + totalFoodProduction + " ";

        info = TileInformation.GetOreProduction().gameObject;
        info.SetActive(true);
        info.GetComponentInChildren<TMP_Text>().text = "+ " + totalOreProduction + " ";
        
        if (GetSettlementType() != SettlementType.None)
        {
            info = TileInformation.GetTotalPopulation().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = GetPopulation() + " ";

            info = TileInformation.GetFreePopulation().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = $"{GetFreePopulation()} ";

            info = TileInformation.GetEmployedlPopulation().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = $"{GetEmployedPopulation()} ";

            info = TileInformation.GetPopulationGrowth().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = $"+ {GetPopulationGrowth():N1} ";

            info = TileInformation.GetGarrison().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = $"{settlement.GetGarrison()} ";

            info = TileInformation.GetGarrisonMax().gameObject;
            info.SetActive(true);
            info.GetComponentInChildren<TMP_Text>().text = $"{settlement.GetGarrisonMax()} ";
        }
    }
    // -------------------------------

    // population control
    public void IncreasePopulation()
    {
        stackedGrowth += populationGrowth;
        population += (int)stackedGrowth;
        freePopulation += (int)stackedGrowth;
        stackedGrowth -= (int)stackedGrowth;
        populationGrowth = population * Constants.BASE_POPULATION_GROWTH;

        UpdatePopulationServerRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
    }

    public void DecreasePopulation(int amount)
    {
        population -= amount;
        freePopulation -= amount;
        populationGrowth = population * Constants.BASE_POPULATION_GROWTH;

        UpdatePopulationServerRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
    }

    public bool CanEmployPopulation(int amount)
    {
        return (amount + employedPopulation < population);
    }

    public void EmployPopulation(int amount)
    {
        employedPopulation += amount;
        freePopulation -= amount;

        UpdatePopulationServerRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
    }

    public void Draft()
    {
        int amount = population - minPopulation;
        amount = Math.Min(amount, freePopulation);
        GetOwner().AddManpower(amount);
        DecreasePopulation(amount);
    }

    public void FillGarrison()
    {
        if (settlement.GetGarrisonMax() > settlement.GetGarrison())
        {
            int garrisonLimit = settlement.GetGarrisonMax() - settlement.GetGarrison();
            int minPop = population - minPopulation;
            int amount = Math.Min(Math.Min(garrisonLimit, minPop), freePopulation);
            settlement.IncreaseGarrison(amount);
            DecreasePopulation(amount);
        }
    }
    //-------------------------------

    // networking methods
    private void OnColorChanged(Color oldValue, Color newValue)
    {
        playerColorObject.color = newValue;
    }

    private void OnOwnerChanged(char oldValue, char newValue)
    {
        if (newValue != Constants.NO_OWNER) 
            owner = PlayerManager.GetPlayer(newValue);
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
            playerColorObject =  transform.GetChild(0).GetComponent<SpriteRenderer>();
            playerColorObject.color = ownerColor.Value;
            if(ownerIndex.Value != Constants.NO_OWNER) owner = PlayerManager.GetPlayer(ownerIndex.Value);
            pathCost = pathCostNetwork.Value;
            offsetCoordinates = offsetCoordinatesNetwork.Value;
            cubeCoordinates = cubeCoordinatesNetwork.Value;

            minPopulation = minPopulationNetwork.Value;
            population = populationNetwork.Value;
            freePopulation = freePopulationNetwork.Value;
            employedPopulation = employedPopulationNetwork.Value;
            populationGrowth = populationGrowthNetwork.Value;
            stackedGrowth = stackedGrowthNetwork.Value;

            totalWoodProduction = totalWoodProductiobNetwork.Value;
            totalStoneProduction = totalStoneProductiobNetwork.Value;
            totalFoodProduction = totalFoodProductiobNetwork.Value;
            totalOreProduction = totalOreProductiobNetwork.Value;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOwnerServerRpc(Color32 color, char index)
    {
        ownerColor.Value = color;
        ownerIndex.Value = index;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeOnwershipServerRpc(ulong clientId)
    {
        if (GetComponent<NetworkObject>().OwnerClientId != clientId) GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    private void ChangeOnwershipMethod(ulong clientId, ServerRpcParams serverParams)
    {
       if (GetComponent<NetworkObject>().OwnerClientId != serverParams.Receive .SenderClientId)  GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateSettlementServerRpc(NetworkObjectReference target)
    {
        NetworkObject targetObject = target;
        settlement = targetObject.GetComponent<SettlementBase>();
    }

    [ClientRpc]
    public void UpdateSettlementClientRpc(NetworkObjectReference target)
    {
        NetworkObject targetObject = target;
        settlement = targetObject.GetComponent<SettlementBase>();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateBuildingServerRpc(BuildingType type, int index)
    {
        CreateBuilding(type, index);
        UpdateBuildingClientRpc(type, index);
    }

    [ClientRpc]
    private void UpdateBuildingClientRpc(BuildingType type, int index)
    {
        CreateBuilding(type, index);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePopulationServerRpc(int minPopulation, int population, int freePopulation, int employedPopulation, float populationGrowth, float stackedGrowth)
    {
        minPopulationNetwork.Value = minPopulation;
        populationNetwork.Value = population;
        freePopulationNetwork.Value = freePopulation;
        employedPopulationNetwork.Value = employedPopulation;
        populationGrowthNetwork.Value = populationGrowth;
        stackedGrowthNetwork.Value = stackedGrowth;
        UpdatePopulationClientRpc(minPopulation, population, freePopulation, employedPopulation, populationGrowth, stackedGrowth);
    }

    [ClientRpc]
    private void UpdatePopulationClientRpc(int minPopulation, int population, int freePopulation, int employedPopulation, float populationGrowth, float stackedGrowth)
    {
        this.minPopulation = minPopulation;
        this.population = population;
        this.freePopulation = freePopulation;
        this.employedPopulation = employedPopulation;
        this.populationGrowth = populationGrowth;
        this.stackedGrowth = stackedGrowth;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateSettlementServerRpc(SettlementType type)
    {
        CreateSettlement(type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateUnitServerRpc(UnitType type, ulong clientId)
    {
        Unit unit = CreateUnit(type);
        unit.GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    [ClientRpc]
    private void UnitCreateNewSelectorClientRpc(NetworkObjectReference target)
    {
        NetworkObject networkObject = target;
        Unit unit = networkObject.GetComponent<Unit>();
        StackedSelectorUI selector = Instantiate(Prefabs.GetUnitStackingUIPrefab(), Prefabs.GetUnitUI().transform).GetComponent<StackedSelectorUI>();
        unit.SetSelector(selector);
        GetUnits().First().SetSelector(selector);
        selector.transform.position = transform.position + new Vector3(0.2f, 0.2f);
    }

    [ClientRpc]
    private void UnitSetExistingSelectorClientRpc(NetworkObjectReference target)
    {
        NetworkObject networkObject = target;
        Unit unit = networkObject.GetComponent<Unit>();
        StackedSelectorUI selector = GameManager.FindSelector(GetUnits().First().GetSelector());
        unit.SetSelector(selector);
    }
    //-------------------------------

    // other methods
    public SettlementBase CreateSettlement(SettlementType settlementType)
    {
        if (!NetworkManager.Singleton.IsHost) { CreateSettlementServerRpc(settlementType); return null; }
        SettlementBase settlement = null;
        switch (settlementType)
        {
            case SettlementType.Outpost:
                settlement = Instantiate(Prefabs.GetOutpostPrefab(), transform.position, Quaternion.identity);
                settlement.SetTile(this);
                settlement.SetUp();
                break;
            case SettlementType.Town:
                settlement = Instantiate(Prefabs.GetTownPrefab(), transform.position, Quaternion.identity);
                settlement.SetTile(this);
                if (this.settlement != null) settlement.SetGarrison(this.settlement.GetGarrison());
                break;
            case SettlementType.Fort:
                break;
        }

        SettlementBase s = this.settlement;
        this.settlement = settlement;
        this.settlement.gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));

        if (s != null) Destroy(s);
        return this.settlement;
    }

    public Unit CreateUnit(UnitType unitType)
    {
        if (!NetworkManager.Singleton.IsHost) { CreateUnitServerRpc(unitType, owner.ClientId.Value); return null; }
        Unit unit = null;
        switch (unitType)
        {
            case UnitType.scout:
                unit = Instantiate(Prefabs.GetScoutPrefab(), transform.position, Quaternion.identity);
                break;
            case UnitType.soldier:
                unit = Instantiate(Prefabs.GetSoldierPrefab(), transform.position, Quaternion.identity);
                break;
        }

        unit.SetUp(this, owner);

        if (GetUnits().Count == 2)
        {
            StackedSelectorUI selector = Instantiate(Prefabs.GetUnitStackingUIPrefab(), Prefabs.GetUnitUI().transform).GetComponent<StackedSelectorUI>();
            unit.SetSelector(selector);
            GetUnits().First().SetSelector(selector);
            selector.transform.position = transform.position + new Vector3(0.2f, 0.2f);
            UnitCreateNewSelectorClientRpc(unit.gameObject);
        }
        else if (GetUnits().Count > 2)
        {
            StackedSelectorUI selector = GameManager.FindSelector(GetUnits().First().GetSelector());
            unit.SetSelector(selector);
            UnitSetExistingSelectorClientRpc(unit.gameObject);
        }

        return unit;
    }

    public Building CreateBuilding(BuildingType buildingType, int index)
    {
        switch (buildingType)
        {
            case BuildingType.Woodcutter:
                buildings[index] = new BuildingWoodcutter(this);
                break;
            case BuildingType.Stonemason:
                buildings[index] = new BuildingStonemason(this);
                break;
            case BuildingType.OreMine:
                buildings[index] = new BuildingOreMine(this);
                break;
            case BuildingType.Farm:
                buildings[index] = new BuildingFarm(this);
                break;
            case BuildingType.Barracks:
                buildings[index] = new BuildingBarracks(this);
                break;
        }

        return buildings[index];
    }

    public void AddUnit(Unit unit) { units.Add(unit); }
    public void RemoveSettlement(){ Destroy(settlement); }
    public void RemoveUnit(Unit unit) { units.Remove(unit); }
    //-------------------------------
}