using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManpowerUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Manpower";
        description = "Manpower is available people that can be recruited into army.";
        production = new KeyValuePair<Resource, int>(new Manpower(), PlayerManager.GetPlayer(1).GetProduction(EnumResources.Manpower));
    }
}
