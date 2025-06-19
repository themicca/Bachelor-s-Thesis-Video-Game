using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalArmiesUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Total Armies";
        description = "All armies that take part in the battle.";
    }
}
