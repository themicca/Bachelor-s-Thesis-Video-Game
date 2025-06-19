using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveUnitsUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Alive Units";
        description = "The amount of units still alive on the battlefield.";
    }
}
