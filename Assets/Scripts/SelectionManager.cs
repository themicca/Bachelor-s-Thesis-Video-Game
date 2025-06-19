using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selector;
    [SerializeField] private GameObject highlighter;
    GameObject hover; // hovered tile
    GameObject selected; // selected tile (left clicked)
    private static SelectionManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static SelectionManager GetInstance()
    {
        return instance;
    }

    public void OnHightlightObject(GameObject _object)
    {
        MouseSelection(_object, ref hover);
        ShowTooltip.ShowTooltipToPlayer(_object);
    }

    public void OnSelectObject(GameObject _object)
    {
        if (selected != null && selected.TryGetComponent(out Tile deactivateTile))
        {
            PlayerActionCanvas.DeactivateTileCard();
        }
        else if (selected != null && selected.TryGetComponent(out Unit _))
        {
            PlayerActionCanvas.DeactivateUnitCard();
        }
        MouseSelection(_object, ref selected);
        if (_object.TryGetComponent(out Tile activateTile))
        {
            PlayerActionCanvas.ActivateTileCard(activateTile);
        }
        else if (_object.TryGetComponent(out Unit unit))
        {
            PlayerActionCanvas.ActivateUnitCard(unit);
        }
    }

    public void OnRightClickTile(Tile tile)
    {
        if (selected != null && selected.TryGetComponent(out Unit target))
        {
            target.Move(tile);
        }
    }

    public void OnHoverOverUI(IHoverTooltip hoveredUI)
    {
        if (hoveredUI == null) ShowTooltip.HideTooltip();
        if (hover != null) ClearSelection(ref hover);
    }

    public void OnHoverOverNothing()
    {
        ShowTooltip.HideTooltip();
        if (hover != null) ClearSelection(ref hover);
    }

    private void MouseSelection(GameObject _object, ref GameObject actionType)
    {
        if (_object != hover && hover != null && hover != selected) { hover.GetComponent<ISelectable>().ClearHighlight(); }
        GameObject previousSelected = selected;
        actionType = _object;
        if (previousSelected != selected && previousSelected != null)
        {
            if (previousSelected.TryGetComponent(out Unit target))
                target.ClearPathFindRenderer();
            previousSelected.GetComponent<ISelectable>().ClearSelection();
        }

        if (_object != null)
        {
            if (_object == selected)
            {
                _object.GetComponent<ISelectable>().Select();
            }
            else if (_object != selected)
            {
                if (selected != null && selected.TryGetComponent(out Unit target) && _object.TryGetComponent(out Tile destination))
                {
                    target.FindPathToDestination(destination);
                }
                _object.GetComponent<ISelectable>().Highlight();
            }
        }
    }

    private void ClearSelection(ref GameObject actionType)
    {
        if (actionType == hover && selected != hover) actionType.GetComponent<ISelectable>().ClearHighlight();
        //else actionType.GetComponent<ISelectable>().ClearSelection();
        if (selected != null && selected.TryGetComponent(out Unit target))
        {
            target.ClearPathFindRenderer();
        }
        actionType = null;
    }
}
