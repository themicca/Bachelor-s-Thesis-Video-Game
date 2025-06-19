using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWoodcutter : Building
{
    private static int baseProduction = 5;

    public BuildingWoodcutter(Tile tile) : base(tile)
    {
        production = baseProduction;
        buildingType = BuildingType.Woodcutter;
        AddProduction();
    }

    public static int GetBaseProduction() { return baseProduction; }

    public override Sprite GetBuildingSprite()
    {
        return Prefabs.GetWoodcutterSprite();
    }

    public static void GetCost(out int woodCost, out int popCost)
    {
        woodCost = 200; popCost = 300;
    }

    public override bool CanPayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int popCost);
        return player.CanSubstractWood(woodCost) && tile.CanEmployPopulation(popCost);
    }

    public override void PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int popCost);
        player.SubstractWood(woodCost);
        tile.EmployPopulation(popCost);
    }

    public override void TooltipDescription(out string header, out string description, out KeyValuePair<Resource, int> produce)
    {
        header = "Woodcutter";
        description = "Produces wood. Wood is an all-purpose material used in all manner of things.";
        produce = new KeyValuePair<Resource, int>(new Wood(), production);
    }
    public override void AddProduction()
    {
        tile.GetOwner().AddProduction(EnumResources.Wood, production);
    }

    public override void DecreaseProduction()
    {
        tile.GetOwner().DecreaseProduction(EnumResources.Wood, production);
    }
}
