using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

public class CloseUI : MonoBehaviour, IHoverTooltip
{
    private void OnDisable()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            ShowTooltip.HideTooltip();
    }

    public void CreateContent(ref string header, ref string content, ref Dictionary<Resource, int> costs, 
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Close";
    }
}
