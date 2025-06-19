using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructOutpostUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string content, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        Outpost.GetCost(out int woodCost, out int stoneCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Manpower(), popCost);
        header = "Construct Outpost";
        content = "In order to expand your empire you have to build outposts in new territories. Outposts can only be build near your borders.";
    }
}
