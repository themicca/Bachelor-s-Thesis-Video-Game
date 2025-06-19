using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneProductionUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Stone Production";
        description = "Monthly stone production of this settlement.";
    }
}
