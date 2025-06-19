using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOreMineUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string content, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        BuildingOreMine.GetCost(out int woodCost, out int stoneCost, out int popCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Population(), popCost);
        header = "Build Ore Miner";
        content = "Produces ore. Ore is primarily used for weapons.";
        production = new KeyValuePair<Resource, int>(new Ore(), BuildingOreMine.GetBaseProduction());
    }
}
