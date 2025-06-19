using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadUnitsUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Dead Units";
        description = "The amount of unis that died in the battle.";
    }
}
