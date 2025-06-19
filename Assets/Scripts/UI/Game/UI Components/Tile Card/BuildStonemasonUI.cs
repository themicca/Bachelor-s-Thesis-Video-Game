using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStonemasonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        BuildingStonemason.GetCost(out int woodCost, out int stoneCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Population(), popCost);
        header = "Build Stonemason";
        description = "Produces stone. Stone is primarily used as a building material.";
        production = new KeyValuePair<Resource, int>(new Stone(), BuildingStonemason.GetBaseProduction());
    }
}
