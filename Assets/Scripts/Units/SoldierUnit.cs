using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : Unit
{
    new private void Awake()
    {
        attack = 600;
        defense = 20;
        damage = 50;
        evasion = 100;
        hitPoints = 200;
        manpower = 500;
        unitName = "Soldier";
        unitType = UnitType.soldier;
        base.Awake();
    }

    public override GameObject GetActionsCard() { return Prefabs.GetSoldierActionsCard(); }

    public static void GetCost(out int woodCost, out int foodCost, out int manpowerCost, out int oreCost)
    {
        woodCost = 300; foodCost = 600; oreCost = 300; manpowerCost = 500;
    }

    public static bool PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int foodCost, out int manpowerCost, out int oreCost);
        if (player.CanSubstractWood(woodCost) && player.CanSubstractFood(foodCost) && player.CanSubstractManpower(manpowerCost) && player.CanSubstractOre(oreCost))
        {
            player.SubstractWood(woodCost);
            player.SubstractFood(foodCost);
            player.SubstractManpower(manpowerCost);
            player.SubstractOre(oreCost);
            return true;
        }
        return false;
    }
}
