using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSlotUI : MonoBehaviour, IHoverTooltip
{
    [SerializeField] private GameObject buildingChooserUI;
    [SerializeField] private RectTransform canvasRT;

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        Building building = PlayerActionCanvas.GetBuildingsOnTile(this);
        if (building != null)
        {
            building.TooltipDescription(out string headerBuilding, out string descriptionBuilding, out KeyValuePair<Resource, int> producuctionBuilding);
            production = producuctionBuilding;
            header = headerBuilding;
            description = descriptionBuilding;
        }
        else if (!GetComponent<Button>().interactable)
        {
            header = "Building Slot";
            description = "Cannot build on unowned tiles or tiles which does not contain a town.";
        }
        else if (GetComponent<Button>().interactable)
        {
            header = "Building Slot";
            description = "Choose a building to be built here.";
        }
    }

    public void BuildingSlotClicked()
    {
        buildingChooserUI.SetActive(!buildingChooserUI.activeInHierarchy);
        if (buildingChooserUI.activeInHierarchy)
        {
            RectTransform rt = GetComponent<RectTransform>();
            RectTransform rtBuildingChooser = buildingChooserUI.GetComponent<RectTransform>();
            //rtBuildingChooser.anchoredPosition = rt.anchoredPosition;  + new Vector3(rt.sizeDelta.x * rt.localScale.x, 0);
            rtBuildingChooser.pivot = rt.pivot;
            //Vector2 anchoredPosition = rt.anchoredPosition / canvasRT.localScale.x;
            rtBuildingChooser.localPosition = rt.localPosition; //+ new Vector3(rtBuildingChooser.rect.width, 0); //+ new Vector3(300f, 0)) * canvasRT.localScale.x;
            //rtBuildingChooser.anchoredPosition = anchoredPosition;
            //rtBuildingChooser.localPosition = rt.localPosition;

            UITooltip[] buttons = buildingChooserUI.GetComponentsInChildren<UITooltip>();
            foreach (UITooltip button in buttons)
            {
                button.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            buildingChooserUI.GetComponentInChildren<BuildFarmButtonUI>().GetComponent<Button>().onClick.AddListener(BuildFarm);
            buildingChooserUI.GetComponentInChildren<BuildWoodcutterUI>().GetComponent<Button>().onClick.AddListener(BuildWoodcutter);
            buildingChooserUI.GetComponentInChildren<BuildStonemasonUI>().GetComponent<Button>().onClick.AddListener(BuildStonemason);
            buildingChooserUI.GetComponentInChildren<BuildOreMineUI>().GetComponent<Button>().onClick.AddListener(BuildOreMine);
            if(buildingChooserUI.GetComponentInChildren<BuildBarracksUI>() != null) buildingChooserUI.GetComponentInChildren<BuildBarracksUI>().GetComponent<Button>().onClick.AddListener(BuildBarracks);
        }
    }

    public void BuildFarm()
    {
        PlayerActionCanvas.UpdateNewBuilding(new BuildingFarm(PlayerActionCanvas.GetTile()), this);
    }

    public void BuildWoodcutter()
    {
        PlayerActionCanvas.UpdateNewBuilding(new BuildingWoodcutter(PlayerActionCanvas.GetTile()), this);
    }

    public void BuildStonemason()
    {
        PlayerActionCanvas.UpdateNewBuilding(new BuildingStonemason(PlayerActionCanvas.GetTile()), this);
    }

    public void BuildOreMine()
    {
        PlayerActionCanvas.UpdateNewBuilding(new BuildingOreMine(PlayerActionCanvas.GetTile()), this);
    }

    public void BuildBarracks()
    {
        PlayerActionCanvas.UpdateNewBuilding(new BuildingBarracks(PlayerActionCanvas.GetTile()), this);
    }
}
