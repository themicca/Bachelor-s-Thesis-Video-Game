using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLobbyUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Create New Lobby";
        description = "You will be able to specify the settings of the lobby.";
    }
}
