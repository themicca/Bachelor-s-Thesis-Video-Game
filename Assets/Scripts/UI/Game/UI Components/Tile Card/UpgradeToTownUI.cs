using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeToTownUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        Town.GetCost(out int woodCost, out int stoneCost, out int foodCost, out int oreCost);
        costs.Add(new Wood(), woodCost);
        costs.Add(new Stone(), stoneCost);
        costs.Add(new Food(), foodCost);
        costs.Add(new Ore(), oreCost);
        header = "Upgrade to Town";
        description = "Upgrades outpost into town enabling construction of various buildings.";
        string conditionText = "Population greater than 1000";
        bool conditionStatus = (PlayerActionCanvas.GetTile().GetPopulation() >= 1000);
        Condition condition = new Condition(conditionText, conditionStatus);
        conditions.Add(condition);
    }
}
