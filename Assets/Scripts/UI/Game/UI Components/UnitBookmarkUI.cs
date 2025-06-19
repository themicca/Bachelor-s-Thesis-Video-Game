using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitBookmarkUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectable, IHoverTooltip
{
    Unit unit;
    BorderUI[] borders;

    private void Awake()
    {
        borders = GetComponentsInChildren<BorderUI>();
    }

    // always assign when creating a bookmark
    public void SetUnit(Unit unit) { this.unit = unit; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip.HideTooltip();
        ShowTooltip.ShowTooltipToPlayer(gameObject);
        SelectionManager.GetInstance().OnHightlightObject(unit.gameObject);
        Utilities.SetMouseOverUIElement(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowTooltip.HideTooltip();
        Utilities.ClearMouseOverUIElement();
    }

    private void OnDestroy()
    {
        GameManager.RemoveBookmark(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.GetInstance().OnSelectObject(unit.gameObject);
    }

    public void Highlight()
    {
        foreach (var b in borders)
        {
            b.GetComponent<Image>().color = Color.gray;
        }
    }

    public void Select()
    {
        foreach (var b in borders)
        {
            b.GetComponent<Image>().color = Color.black;
        }
    }

    public void ClearHighlight()
    {
        foreach (var b in borders)
        {
            b.GetComponent<Image>().color = Color.white;
        }
    }

    public void ClearSelection()
    {
        ClearHighlight();
    }

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = $"Select {unit.GetUnitName()}";
    }
}
