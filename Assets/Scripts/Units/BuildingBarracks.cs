using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBarracks : Building
{
    public BuildingBarracks(Tile tile) : base(tile)
    {
        production = 0;
        buildingType = BuildingType.Barracks;
    }

    public override Sprite GetBuildingSprite()
    {
        return Prefabs.GetBarracksSprite();
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int popCost)
    {
        woodCost = 350; stoneCost = 400; popCost = 300;
    }

    public override bool CanPayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int popCost);
        return player.CanSubstractWood(woodCost) && player.CanSubstractStone(stoneCost) && tile.CanEmployPopulation(popCost);
    }

    public override void PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int popCost);
        player.SubstractWood(woodCost);
        player.SubstractStone(stoneCost);
        tile.EmployPopulation(popCost);
    }

    public override void TooltipDescription(out string header, out string description, out KeyValuePair<Resource, int> produce)
    {
        header = "Barracks";
        description = "Enables training of units in this town. Available units: Scout, Soldier.";
        produce = new();
    }

    public override void AddProduction() { }
    public override void DecreaseProduction() { }
}
