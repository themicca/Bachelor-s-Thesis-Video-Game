using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStartButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Start";
        description = "Press this once you're ready to start the game. You will be given an option to specify the map settings before starting the game.";
    }
}
