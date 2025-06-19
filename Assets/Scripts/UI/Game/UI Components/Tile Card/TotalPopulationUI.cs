using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalPopulationUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Population";
        description = "Total population of this settlement. Use the free pops to fill up jobs or draft them into the army as manpower." +
            "\nManpower can be used to produce units and armies.";
    }
}
