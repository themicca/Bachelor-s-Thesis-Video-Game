using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class PlayerActionCanvas : MonoBehaviour
{
    [SerializeField] private GameObject tileCard;
    [SerializeField] private GameObject unitCard;
    [SerializeField] private GameObject combatCard;
    [SerializeField] private GameObject tileCamera;
    [SerializeField] private TMP_Text tileNameText;
    [SerializeField] private TMP_Text unitNameText;
    [SerializeField] private GameObject buildingChooserUI;
    [SerializeField] private Transform tileInformation;
    [SerializeField] private GameObject buySoldierButton;
    [SerializeField] private GameObject buyScoutButton;
    [SerializeField] private GameObject settlementUpgradeButton;
    [SerializeField] private GameObject draftPopulationButton;
    [SerializeField] private GameObject fillGarrisonButton;

    private Tile tile;
    private Unit unit;
    private BuildingSlotUI[] buildingSlots;
    private static PlayerActionCanvas instance;

    private static bool barracksPresent;

    private void Awake()
    {
        instance = this;
        buildingSlots = tileCard.GetComponentsInChildren<BuildingSlotUI>();
    }

    public static void ActivateTileCard(Tile tile)
    {
        if(!instance.tileCard.activeSelf) instance.tileCard.SetActive(true);
        instance.tile = tile;
        barracksPresent = false;
        instance.tile.SetLayer("Selected Tile");
        instance.tileCamera.transform.position = new Vector3(instance.tile.transform.position.x, instance.tile.transform.position.y, instance.tileCamera.transform.position.z);
        instance.tileNameText.text = $"{instance.tile.GetTileType()} tile | " +
            $"{(instance.tile.GetOwner() == null ? "Unoccupied" : $"Owner - {instance.tile.GetOwner().GetEmpireName()}" )}";
        UpdateDraftButton();
        UpdateFillGarrisonButton();
        instance.UpdateSettlementUpgradeButton();
        UpdateBuildingSlots();
        UpdateProduction();
    }

    public static void UpdateTileCard()
    {
        UpdateDraftButton();
        UpdateFillGarrisonButton();
        instance.UpdateSettlementUpgradeButton();
        UpdateBuildingSlots();
        UpdateProduction();
    }

    public static void DeactivateTileCard()
    {
        if (instance.tile != null)
        {
            instance.buildingChooserUI.SetActive(false);
            instance.tileCard.gameObject.SetActive(false);
            instance.buyScoutButton.SetActive(false);
            instance.buySoldierButton.SetActive(false);

            instance.tile.ClearLayerForCamera();
            foreach (BuildingSlotUI slot in instance.buildingSlots)
            {
                RemoveBuildingFromSlot(slot);
                slot.GetComponent<Button>().interactable = false;
            }
            foreach (Transform childInfo in instance.tileInformation)
            {
                foreach (Transform child in childInfo)
                {
                    child.gameObject.SetActive(false);
                }
            }
            barracksPresent = false;
            instance.tile = null;
        }
    }

    public static void ActivateUnitCard(Unit unit)
    {
        if (!instance.unitCard.activeSelf) instance.unitCard.SetActive(true);
        instance.unit = unit;
        instance.unit.GetActionsCard().SetActive(true);
        instance.unitNameText.text = instance.unit.GetUnitName();
    }

    public static void DeactivateUnitCard()
    {
        if (instance.unitCard != null)
        {
            instance.unit.GetActionsCard().SetActive(false);
            instance.unitCard.gameObject.SetActive(false);
            instance.unit = null;
        }
    }

    public static void UpdateCombatCard(Combat combat)
    {
        if (!instance.combatCard.activeSelf) instance.combatCard.SetActive(true);
        instance.combatCard.GetComponent<CombatCard>().Display(combat);
    }

    public static void DeactivateCombatCard()
    {
        CombatDisplay.ClearInstance();
        instance.combatCard.SetActive(false);
    }

    public static void UpdateNewBuilding(Building building, BuildingSlotUI slot)
    {
        if (!building.CanPayCost(instance.tile.GetOwner()) || building.GetBuildingType() == BuildingType.Barracks && barracksPresent
           || instance.tile.GetBuildings()[Array.IndexOf(instance.buildingSlots, slot)] != null)
        {
            return;
        }
        building.PayCost(instance.tile.GetOwner());
        instance.buildingChooserUI.SetActive(false);
        instance.tile.SetBuilding(building, slot.transform.GetSiblingIndex());
        UpdateBuildingSlots();
        UpdateProduction();
    }

    public void BuildOutpost() { DoAction(new BuildOutpost()); }

    public void DoAction(UnitAction action)
    {
        instance.unit.DoAction(action);
    }

    public static void UpdateDraftButton()
    {
        if (instance.tile.IsOwner)
        {
            instance.draftPopulationButton.SetActive(true);
            if (instance.tile.GetFreePopulation() > 0 && instance.tile.GetPopulation() > instance.tile.GetMinPopulation())
                instance.draftPopulationButton.GetComponent<Button>().interactable = true;
            else instance.draftPopulationButton.GetComponent<Button>().interactable = false;
        }
        else instance.draftPopulationButton.SetActive(false);
    }

    public static void UpdateFillGarrisonButton()
    {
        if (instance.tile.IsOwner)
        {
            instance.fillGarrisonButton.SetActive(true);
            if (instance.tile.GetFreePopulation() > 0 && instance.tile.GetSettlement().GetGarrisonMax() > instance.tile.GetSettlement().GetGarrison()
                && instance.tile.GetPopulation() > instance.tile.GetMinPopulation())
                instance.fillGarrisonButton.GetComponent<Button>().interactable = true;
            else instance.fillGarrisonButton.GetComponent<Button>().interactable = false;
        }
        else instance.fillGarrisonButton.SetActive(false);
    }

    private void UpdateSettlementUpgradeButton()
    {
        if (tile.GetSettlementType() == SettlementType.Outpost && tile.IsOwner && tile.GetPopulation() >= 1000)
        {
            settlementUpgradeButton.SetActive(true);
            settlementUpgradeButton.GetComponent<Button>().interactable = true;
        }
        else if (tile.GetSettlementType() == SettlementType.Outpost && tile.IsOwner && tile.GetPopulation() < 1000)
        {
            settlementUpgradeButton.SetActive(true);
            settlementUpgradeButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            settlementUpgradeButton.GetComponent<Button>().interactable = false;
            settlementUpgradeButton.SetActive(false);
        }
    }

    private static void UpdateBuildingSlots()
    {
        if (instance.tile.GetSettlementType() == SettlementType.Town && instance.tile.IsOwner)
        {
            for (int i = 0; i < Constants.BUILDING_SLOT_COUNT; i++)
            {
                instance.buildingSlots[i].GetComponent<Button>().interactable = true;
                Building building = instance.tile.GetBuildings()[i];
                if (building != null)
                {
                    FillBuildingSlot(instance.buildingSlots[i], building);
                    if (building.GetBuildingType() == BuildingType.Barracks && !barracksPresent)
                    { 
                        barracksPresent = true;
                        if(instance.buildingChooserUI.GetComponentInChildren<BuildBarracksUI>() != null) instance.buildingChooserUI.GetComponentInChildren<BuildBarracksUI>().gameObject.SetActive(false);
                        instance.buyScoutButton.SetActive(true);
                        instance.buySoldierButton.SetActive(true);
                    }
                }
            }
        }
    }

    public static void UpdateProduction()
    {
        instance.tile.ShowProduction();
    }

    private static void FillBuildingSlot(BuildingSlotUI slot, Building building)
    {
        slot.transform.GetChild(0).gameObject.SetActive(true);
        slot.transform.GetChild(0).GetComponent<Image>().sprite = building.GetBuildingSprite();
        slot.transform.GetChild(1).gameObject.SetActive(false);
    }

    private static void RemoveBuildingFromSlot(BuildingSlotUI slot)
    {
        slot.transform.GetChild(0).gameObject.SetActive(false);
        slot.transform.GetChild(1).gameObject.SetActive(true);
    }

    public static Building GetBuildingsOnTile(BuildingSlotUI slot)
    {
        for (int i = 0; i < instance.buildingSlots.Length; i++)
        {
            if(slot == instance.buildingSlots[i])
            {
                return instance.tile.GetBuildings()[i];
            }
        }
        return null;
    }

    public static void DraftPopulation()
    {
        instance.tile.Draft();
        UpdateTileCard();
    }

    public static void FillGarrison()
    {
        instance.tile.FillGarrison();
        UpdateTileCard();
    }

    public void UpgradeToTown()
    {
        GamePlayer player = tile.GetOwner();
        if (!Town.CanPayCost(player)) return;
        Town.PayCost(player);
        tile.CreateSettlement(SettlementType.Town);
        UpdateTileCard();
    }

    public static Tile GetTile() { return instance.tile; }
    public static Unit GetUnit() { return instance.unit; }
}
