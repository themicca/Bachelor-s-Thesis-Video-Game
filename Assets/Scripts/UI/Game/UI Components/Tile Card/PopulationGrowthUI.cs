using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationGrowthUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Population Growth";
        description = "Monthly population increase in this settlement. The growth depends on the total population.";
    }
}
