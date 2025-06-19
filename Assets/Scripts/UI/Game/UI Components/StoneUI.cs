using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Stone";
        description = "Stone is primarily used for building structures.";
        production = new KeyValuePair<Resource, int>(new Stone(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Stone));
    }
}
