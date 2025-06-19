using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPointsUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Hit Points";
        description = " Hit points each unit has. They determine the amount of damage each unit can absorp.";
    }
}
