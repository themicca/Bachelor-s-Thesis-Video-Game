using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Wood";
        description = "Wood is primarily used for building structures.";
        production = new KeyValuePair<Resource, int>(new Wood(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Wood));
    }
}
