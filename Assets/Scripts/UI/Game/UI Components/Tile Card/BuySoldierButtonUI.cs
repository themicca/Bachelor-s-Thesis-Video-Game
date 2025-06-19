using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySoldierButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        SoldierUnit.GetCost(out int woodCost, out int foodCost, out int manpowerCost, out int oreCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Food(), foodCost);
        costs.Add(new Manpower(), manpowerCost);
        costs.Add(new Ore(), oreCost);
        header = "Buy Soldier";
        description = "Soldiers excel in combat. Use them for defeating enemies and defending your empire.";
    }

    public void Clicked()
    {
        if (!SoldierUnit.PayCost(PlayerActionCanvas.GetTile().GetOwner()))
        {
            return;
        }
        PlayerActionCanvas.GetTile().CreateUnit(UnitType.soldier);
    }
}
