using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoutUnit : Unit
{ 
    new private void Awake()
    {
        attack = 200;
        defense = 5;
        damage = 15;
        evasion = 50;
        hitPoints = 80;
        manpower = 200;
        unitName = "Scout";
        unitType = UnitType.scout;
        base.Awake();
    }

    public override GameObject GetActionsCard() { return Prefabs.GetScoutActionsCard(); }

    public static void GetCost(out int woodCost, out int foodCost, out int manpowerCost)
    {
        woodCost = 150; foodCost = 200; manpowerCost = 200;
    }

    public static bool PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int foodCost, out int manpowerCost);
        if (player.CanSubstractWood(woodCost) && player.CanSubstractFood(foodCost) && player.CanSubstractManpower(manpowerCost))
        {
            player.SubstractWood(woodCost);
            player.SubstractFood(foodCost);
            player.SubstractManpower(manpowerCost);
            return true;
        }
        return false;
    }
}
