using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : SettlementBase
{
    new private void Awake()
    {
        attack = 600;
        damage = 50;
        evasion = 100;
        defense = 20;
        hitPoints = 200;
        garrisonMax = 1000;

        type = SettlementType.Town;
        base.Awake();
    }

    public override void SetUp()
    {
        
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int foodCost, out int oreCost)
    {
        woodCost = 800; stoneCost = 800; foodCost = 1000; oreCost = 500;
    }

    public static bool CanPayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int foodCost, out int oreCost);
        return player.CanSubstractWood(woodCost) && player.CanSubstractStone(stoneCost) && player.CanSubstractFood(foodCost) && player.CanSubstractOre(oreCost);
    }

    public static void PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int foodCost, out int oreCost);
        player.SubstractWood(woodCost);
        player.SubstractStone(stoneCost);
        player.SubstractFood(foodCost);
        player.SubstractOre(oreCost);
    }
}
