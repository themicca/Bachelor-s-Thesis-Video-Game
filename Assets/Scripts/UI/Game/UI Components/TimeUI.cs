using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Time";
        description = $"Current date. You can pause by pressing 'space'. Current speed: {(Clock.GetGameSpeed() + 3) / 4}";
    }
}
