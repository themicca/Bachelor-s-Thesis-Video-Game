using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Defense";
        description = "Determines how armored a unit is. Lowers the damage dealt to the unit.";
    }
}
