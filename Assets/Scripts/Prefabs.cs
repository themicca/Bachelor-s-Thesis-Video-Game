using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Prefabs : MonoBehaviour
{
    [SerializeField] private Town townPrefab;
    [SerializeField] private Outpost outpostPrefab;
    [SerializeField] private ScoutUnit scoutPrefab;
    [SerializeField] private SoldierUnit soldierPrefab;
    [SerializeField] private GameObject unitDisplayPrefab;
    [SerializeField] private GameObject tileSelector;
    [SerializeField] private GameObject tileHighlighter;
    [SerializeField] private GameObject unitLineRendererPrefab;
    [SerializeField] private GameObject playerColorPrefab;
    [SerializeField] private GameObject scoutActionsCard;
    [SerializeField] private GameObject soldierActionsCard;
    [SerializeField] private Sprite farmSprite;
    [SerializeField] private Sprite woodcutterSprite;
    [SerializeField] private Sprite stonemasonSprite;
    [SerializeField] private Sprite oreMineSprite;
    [SerializeField] private Sprite barracksSprite;
    [SerializeField] private Sprite moneySprite;
    [SerializeField] private Sprite foodSprite;
    [SerializeField] private Sprite woodSprite;
    [SerializeField] private Sprite stoneSprite;
    [SerializeField] private Sprite oreSprite;
    [SerializeField] private Sprite manpowerSprite;
    [SerializeField] private Sprite populationSprite;
    [SerializeField] private CombatDisplay combatDisplayPrefab;
    [SerializeField] private GameObject unitUI;
    [SerializeField] private GameObject unitStackingUIPrefab;
    [SerializeField] private GameObject unitBookmarkPrefab;
    [SerializeField] private Transform unitsParent;
    [SerializeField] private Transform playersParent;
    [SerializeField] private Transform tileMapParent;

    private static Prefabs instance;

    private void Awake()
    {
        instance = this;
        if (NetworkManager.Singleton.IsHost)
        {
            unitsParent = Instantiate(unitsParent);
            playersParent = Instantiate(playersParent);
            tileMapParent = Instantiate(tileMapParent);
            unitsParent.GetComponent<NetworkObject>().Spawn();
            playersParent.GetComponent<NetworkObject>().Spawn();
            tileMapParent.GetComponent<NetworkObject>().Spawn();
        }
        /*else
        {
            Destroy(unitsParent.gameObject);
            Destroy(playersParent.gameObject);
            Destroy(tileMapParent.gameObject);
        }*/
    }

    public static Town GetTownPrefab () { return instance.townPrefab; }
    public static ScoutUnit GetScoutPrefab() { return instance.scoutPrefab; }
    public static SoldierUnit GetSoldierPrefab() { return instance.soldierPrefab; }
    public static GameObject GetUnitDisplayPrefab() { return instance.unitDisplayPrefab; }
    public static GameObject GetTileSelectorPrefab() { return instance.tileSelector; }
    public static GameObject GetTileHighlighterPrefab() { return instance.tileHighlighter; }
    public static GameObject GetUnitLineRendererPrefab() { return instance.unitLineRendererPrefab; }
    public static GameObject GetPlayerColorPrefab() { return instance.playerColorPrefab; }
    public static GameObject GetScoutActionsCard() { return instance.scoutActionsCard; }
    public static GameObject GetSoldierActionsCard() { return instance.soldierActionsCard; }
    public static Outpost GetOutpostPrefab() { return instance.outpostPrefab; }
    public static Sprite GetFarmSprite() { return instance.farmSprite; }
    public static Sprite GetWoodcutterSprite() { return instance.woodcutterSprite; }
    public static Sprite GetStonemasonSprite() { return instance.stonemasonSprite; }
    public static Sprite GetOreMineSprite() { return instance.oreMineSprite; }
    public static Sprite GetBarracksSprite() { return instance.barracksSprite; }
    public static Sprite GetMoneySprite() { return instance.moneySprite; }
    public static Sprite GetFoodSprite() { return instance.foodSprite; }
    public static Sprite GetWoodSprite() { return instance.woodSprite; }
    public static Sprite GetStoneSprite() { return instance.stoneSprite; }
    public static Sprite GetOreSprite() { return instance.oreSprite; }
    public static Sprite GetManpowerSprite() { return instance.manpowerSprite; }
    public static Sprite GetPopulationSprite() { return instance.populationSprite; }
    public static CombatDisplay GetCombatDisplayPrefab() { return instance.combatDisplayPrefab; }
    public static GameObject GetUnitUI() { return instance.unitUI; }
    public static GameObject GetUnitStackingUIPrefab() { return instance.unitStackingUIPrefab; }
    public static GameObject GetUnitBookmarkPrefab() { return instance.unitBookmarkPrefab; }
    public static Transform GetUnitsParent() { return instance.unitsParent; }
    public static Transform GetPlayersParent() { return instance.playersParent; }
    public static Transform GetTileMapParent() { return instance.tileMapParent; }
}
