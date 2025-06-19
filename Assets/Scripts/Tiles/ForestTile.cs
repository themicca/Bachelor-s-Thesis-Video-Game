using UnityEngine;
using Unity.Netcode;

public class ForestTile : Tile
{
    new private void Awake()
    {
        tileType = TileType.Forest;
        pathCost = 3;
        base.Awake();
    }

    public override void AddProduction()
    {
        GetOwner().AddProduction(EnumResources.Wood, production);
    }

    public override void ShowProduction()
    {
        totalWoodProduction = production;
        totalStoneProduction = 0;
        totalFoodProduction = 0;
        totalOreProduction = 0;

        ShowWichProduction();
    }

    public override void DecreaseProduction()
    {
        GetOwner().DecreaseProduction(EnumResources.Wood, production);
    }
}
