using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingStonemason : Building
{
    private static int baseProduction = 5;

    public BuildingStonemason(Tile tile) : base(tile)
    {
        production = baseProduction;
        buildingType = BuildingType.Stonemason;
        AddProduction();
    }

    public static int GetBaseProduction() { return baseProduction; }

    public override Sprite GetBuildingSprite()
    {
        return Prefabs.GetStonemasonSprite();
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int popCost)
    {
        woodCost = 150; stoneCost = 50; popCost = 300;
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
        header = "Stonemason";
        description = "Produces stone. Stone is primarily used as a building material.";
        produce = new KeyValuePair<Resource, int>(new Stone(), production);
    }

    public override void AddProduction()
    {
        tile.GetOwner().AddProduction(EnumResources.Stone, production);
    }

    public override void DecreaseProduction()
    {
        tile.GetOwner().DecreaseProduction(EnumResources.Stone, production);
    }
}
