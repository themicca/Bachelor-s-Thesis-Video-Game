using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Money";
        description = "Money is used for commerce.";
        production = new KeyValuePair<Resource, int>(new Money(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Money));
    }
}
