using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum BuildingType
{
    Woodcutter,
    Stonemason,
    Farm,
    OreMine,
    Barracks
}

public abstract class Building : IPriceable
{
    protected int production;
    protected BuildingType buildingType;
    protected Tile tile;

    protected Building(Tile tile)
    { 
        this.tile = tile;
    }

    public abstract void AddProduction();
    public abstract void DecreaseProduction();

    public BuildingType GetBuildingType()
    {
        return buildingType;
    }
    public int GetProduction() { return production; }
    public abstract Sprite GetBuildingSprite();
    public abstract void TooltipDescription(out string header, out string description, out KeyValuePair<Resource, int> produce);
    public abstract bool CanPayCost(GamePlayer player);
    public abstract void PayCost(GamePlayer player);
}
