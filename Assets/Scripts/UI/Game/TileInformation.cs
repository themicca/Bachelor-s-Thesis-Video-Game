using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformation : MonoBehaviour
{
    [SerializeField] StoneProductionUI stoneProduction;
    [SerializeField] WoodProductionUI woodProduction;
    [SerializeField] OreProductionUI oreProduction;
    [SerializeField] FoodProductionUI foodProduction;
    [SerializeField] TotalPopulationUI totalPopulation;
    [SerializeField] FreePopulationUI freePopulation;
    [SerializeField] EmployedPopulationUI employedPopulation;
    [SerializeField] PopulationGrowthUI populationGrowth;
    [SerializeField] GarrisonTextUI garrison;
    [SerializeField] GarrisonMaxUI garrisonMax;

    static TileInformation instance;

    private void Awake()
    {
        instance = this;
    }

    public static StoneProductionUI GetStoneProduction() { return instance.stoneProduction; }
    public static WoodProductionUI GetWoodProduction() { return instance.woodProduction; }
    public static OreProductionUI GetOreProduction() { return instance.oreProduction; }
    public static FoodProductionUI GetFoodProduction() { return instance.foodProduction; }
    public static TotalPopulationUI GetTotalPopulation() { return instance.totalPopulation; }
    public static FreePopulationUI GetFreePopulation() { return instance.freePopulation; }
    public static EmployedPopulationUI GetEmployedlPopulation() { return instance.employedPopulation; }
    public static PopulationGrowthUI GetPopulationGrowth() { return instance.populationGrowth; }
    public static GarrisonTextUI GetGarrison() { return instance.garrison; }
    public static GarrisonMaxUI GetGarrisonMax() { return instance.garrisonMax; }
}
