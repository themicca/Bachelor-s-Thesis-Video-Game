using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class MountainTile : Tile
{
    new private void Awake()
    {
        tileType = TileType.Mountain;
        pathCost = 5;
        base.Awake();
    }

    public override void AddProduction()
    {
        GetOwner().AddProduction(EnumResources.Stone, production);
    }

    public override void ShowProduction()
    {
        totalWoodProduction = 0;
        totalStoneProduction = production;
        totalFoodProduction = 0;
        totalOreProduction = 0;

        ShowWichProduction();
    }

    public override void DecreaseProduction()
    {
        GetOwner().DecreaseProduction(EnumResources.Stone, production);
    }
}
