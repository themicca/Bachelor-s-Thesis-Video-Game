using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBarracksUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        BuildingBarracks.GetCost(out int woodCost, out int stoneCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Population(), popCost);
        header = "Build Barracks";
        description = "Enables training of units in this town. Can only be built once in each town. Available units: Scout, Soldier.";
    }
}
