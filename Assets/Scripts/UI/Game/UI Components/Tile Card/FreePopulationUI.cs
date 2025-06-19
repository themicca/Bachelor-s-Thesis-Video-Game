using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePopulationUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Free Population";
        description = "Population free of any work or job. You can hire them for jobs in town or draft them into army.";
    }
}
