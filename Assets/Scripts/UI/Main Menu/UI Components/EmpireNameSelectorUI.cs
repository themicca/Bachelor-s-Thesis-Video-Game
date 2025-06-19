using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpireNameSelectorUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Empire Name";
        description = "Choose a name for your empire.";
    }
}
