using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployedPopulationUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Employed Population";
        description = "Population employed in jobs in the settlement.";
    }
}
