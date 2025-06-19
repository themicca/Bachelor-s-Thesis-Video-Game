using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip.HideTooltip();
        ShowTooltip.ShowTooltipToPlayer(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowTooltip.HideTooltip();
    }
}
