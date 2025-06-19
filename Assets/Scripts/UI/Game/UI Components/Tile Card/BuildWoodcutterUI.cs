using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWoodcutterUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        BuildingWoodcutter.GetCost(out int woodCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Population(), popCost);
        header = "Build Woodcutter";
        description = "Produces wood. Wood is an all-purpose material used in all manner of things.";
        production = new KeyValuePair<Resource, int>(new Wood(), BuildingWoodcutter.GetBaseProduction());
    }
}
