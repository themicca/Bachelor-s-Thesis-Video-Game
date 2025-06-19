using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFarm : Building
{
    private static int baseProduction = 5;

    public BuildingFarm(Tile tile) : base(tile)
    {
        production = baseProduction;
        buildingType = BuildingType.Farm;
        AddProduction();
    }

    public static int GetBaseProduction() { return baseProduction; }

    public override Sprite GetBuildingSprite()
    {
        return Prefabs.GetFarmSprite();
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int popCost)
    {
        woodCost = 150; stoneCost = 150; popCost = 300;
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
        header = "Farm";
        description = "Produces food. Food is consumed by population and is need for building units.";
        produce = new KeyValuePair<Resource, int>(new Food(), production);
    }

    public override void AddProduction()
    {
        tile.GetOwner().AddProduction(EnumResources.Food, production);
    }

    public override void DecreaseProduction()
    {
        tile.GetOwner().DecreaseProduction(EnumResources.Food, production);
    }
}
