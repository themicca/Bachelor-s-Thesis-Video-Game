using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Evasion";
        description = "The ability to evade attacks. Lowers the chance of a successful enemy attack.";
    }
}
