using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingOreMine : Building
{
    private static int baseProduction = 5;

    public BuildingOreMine(Tile tile) : base(tile)
    {
        production = baseProduction;
        buildingType = BuildingType.OreMine;
        AddProduction();
    }

    public static int GetBaseProduction() { return baseProduction; }

    public override Sprite GetBuildingSprite()
    {
        return Prefabs.GetOreMineSprite();
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int popCost)
    {
        woodCost = 100; stoneCost = 250; popCost = 300;
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
        header = "Ore Mine";
        description = "Used to produce ore. Ore is primarily used for weapon.";
        produce = new KeyValuePair<Resource, int>(new Ore(), production);
    }
    public override void AddProduction()
    {
        tile.GetOwner().AddProduction(EnumResources.Ore, production);
    }

    public override void DecreaseProduction()
    {
        tile.GetOwner().DecreaseProduction(EnumResources.Ore, production);
    }
}
