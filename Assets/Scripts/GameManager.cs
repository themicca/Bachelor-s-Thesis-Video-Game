using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    static List<Tile> tiles;
    static Dictionary<Unit, MovementController> movements;
    static List<Combat> combats;
    static Dictionary<StackedSelectorUI, List<UnitBookmarkUI>> stackedSelector;
    static Dictionary<UnitBookmarkUI, StackedSelectorUI> stackedSelectorInverse;
    static List<SettlementBase> settlements;

    void Start()
    {
        tiles = new List<Tile>();
        movements = new Dictionary<Unit, MovementController>();
        stackedSelector = new Dictionary<StackedSelectorUI, List<UnitBookmarkUI>>();
        stackedSelectorInverse = new Dictionary<UnitBookmarkUI, StackedSelectorUI>();
        combats = new List<Combat>();
        settlements = new List<SettlementBase>();
        if (NetworkManager.Singleton.IsHost)
        {
            GetComponent<CreateWorld>().Generate(ref tiles);
        }
    }

    public static void IncreasePopulation()
    {
        foreach (Tile t in tiles)
        {
            t.IncreasePopulation();
            if (PlayerActionCanvas.GetTile() == t)
            {
                PlayerActionCanvas.UpdateProduction();
                PlayerActionCanvas.UpdateDraftButton();
                PlayerActionCanvas.UpdateFillGarrisonButton();
            }
        }
    }

    // get methods
    public static Dictionary<Unit, MovementController> GetMovements() { return movements; }
    public static List<Combat> GetCombats() { return combats; }
    // -------------------------------

    // add methods
    public static void AddMovement(MovementController movement, Unit unit) { movements[unit] = movement; }
    public static void AddCombat(Combat combat) { combats.Add(combat); }
    public static void AddSettlement(SettlementBase settlement) {  settlements.Add(settlement); }

    public static void AddStackedSelector(StackedSelectorUI selector, UnitBookmarkUI bookmark)
    {
        if (stackedSelector.ContainsKey(selector))
        {
            stackedSelector[selector].Add(bookmark);
        }
        else
        {
            stackedSelector[selector] = new List<UnitBookmarkUI> { bookmark };
        }
        stackedSelectorInverse[bookmark] = selector;
    }
    // -------------------------------

    // remove methods
    public static void RemoveMovement(Unit unit) { movements.Remove(unit); }
    public static void RemoveCombat(Combat combat) { combats.Remove(combat); }
    public static void RemoveSettlement(SettlementBase settlement) { settlements.Remove(settlement); }

    public static void RemoveBookmark(UnitBookmarkUI bookmark)
    {
        if (stackedSelectorInverse.ContainsKey(bookmark))
        {
            StackedSelectorUI selector = stackedSelectorInverse[bookmark];
            stackedSelectorInverse.Remove(bookmark);
            stackedSelector[selector].Remove(bookmark);
            if (stackedSelector[selector].Count == 0)
            {
                stackedSelector.Remove(selector);
                Destroy(selector.gameObject);
            }
        }
    }
    // -------------------------------

    // other methods
    public static StackedSelectorUI FindSelector(UnitBookmarkUI bookmark)
    {
        return stackedSelectorInverse[bookmark];
    }
    // -------------------------------
}
