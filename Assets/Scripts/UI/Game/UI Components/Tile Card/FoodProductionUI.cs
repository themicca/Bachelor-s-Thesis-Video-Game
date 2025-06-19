using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProductionUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Food Production";
        description = "Monthly food production of this settlement.";
    }
}
