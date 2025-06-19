using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlainfieldTile : Tile
{
    new private void Awake()
    {
        tileType = TileType.Plainfield;
        pathCost = 3;
        base.Awake();
    }

    public override void AddProduction()
    {
        GetOwner().AddProduction(EnumResources.Food, production);
    }

    public override void ShowProduction()
    {
        totalWoodProduction = 0;
        totalStoneProduction = 0;
        totalFoodProduction = production;
        totalOreProduction = 0;

        ShowWichProduction();
    }

    public override void DecreaseProduction()
    {
        GetOwner().DecreaseProduction(EnumResources.Food, production);
    }
}
