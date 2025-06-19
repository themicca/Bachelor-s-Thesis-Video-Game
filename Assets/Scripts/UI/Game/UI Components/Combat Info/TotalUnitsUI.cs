using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalUnitsUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Total Units";
        description = "Total amount of all units from all armies.";
    }
}
