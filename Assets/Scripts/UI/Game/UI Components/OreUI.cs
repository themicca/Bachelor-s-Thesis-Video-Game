using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Ore";
        description = "Ore is primarily used for weapons.";
        production = new KeyValuePair<Resource, int>(new Ore(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Ore));
    }
}
