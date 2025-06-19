using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFarmButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        BuildingFarm.GetCost(out int woodCost, out int stoneCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Population(), popCost);
        header = "Build Farm";
        description = "Produces food. Food is consumed by population and is need for building units.";
        production = new KeyValuePair<Resource, int>(new Food(), BuildingFarm.GetBaseProduction());
    }
}
