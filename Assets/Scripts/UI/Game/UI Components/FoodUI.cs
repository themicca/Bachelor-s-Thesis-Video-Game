using System.Collections.Generic;
using UnityEngine;

public class FoodUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Food";
        description = "People have to consume food.";
        production = new KeyValuePair<Resource, int>(new Food(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Food));
    }
}
