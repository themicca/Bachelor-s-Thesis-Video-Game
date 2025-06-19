using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Main Menu";
        description = "Quit to the Main Menu.";
    }
}
