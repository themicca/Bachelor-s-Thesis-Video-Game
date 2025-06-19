using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyScoutButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        ScoutUnit.GetCost(out int woodCost , out int foodCost, out int manpowerCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new  Food(), foodCost);
        costs.Add(new Manpower(), manpowerCost);
        header = "Buy Scout";
        description = "Scouts are weak in combat, but are very usefull for expanding the empire.";
    }

    public void Clicked()
    {
        if (!ScoutUnit.PayCost(PlayerActionCanvas.GetTile().GetOwner()))
        {
            return;
        }
        PlayerActionCanvas.GetTile().CreateUnit(UnitType.scout);
    }
}
