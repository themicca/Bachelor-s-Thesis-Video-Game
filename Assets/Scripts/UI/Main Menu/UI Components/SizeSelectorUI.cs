using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSelectorUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Game Size";
        description = "The size of the game map. The actual size in tiles is \"size squared\".";
    }
}
