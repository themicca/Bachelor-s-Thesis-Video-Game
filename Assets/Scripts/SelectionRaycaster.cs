using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

public class SelectionRaycaster : MonoBehaviour
{
    Collider2D colliderHit;
    private void Update()
    {
        if (IsMouseOverUI(out IHoverTooltip hover))
        {
            SelectionManager.GetInstance().OnHoverOverUI(hover);
            return;
        }
        colliderHit = GetRayTarget();
        if (colliderHit == null)
        {
            SelectionManager.GetInstance().OnHoverOverNothing();
            return;
        }
        if (Input.GetMouseButtonDown(0))
            MouseLeftClick();
        else if (Input.GetMouseButtonDown(1))
            MouseRightClick();
        else
            MouseHover();
    }

    private void MouseHover()
    {
        if (colliderHit.TryGetComponent(out Unit unitTarget))
        {
            unitTarget.OnHighlightUnit();
        }
        else if (colliderHit.TryGetComponent(out Tile tileTarget))
        {
            tileTarget.OnHighlightTile();
        }
    }

    private void MouseLeftClick()
    {
        if (colliderHit.TryGetComponent(out Tile tileTarget))
        {
            tileTarget.OnSelectTile();
        }
        else if (colliderHit.TryGetComponent(out Unit unitTarget))
        {
            if (unitTarget.IsOwner && unitTarget.GetOwner().ClientId.Value != Constants.COMPUTER_PLAYER_ID)
                unitTarget.OnSelectUnit();
        }
    }

    private void MouseRightClick()
    {
        if (colliderHit.TryGetComponent(out Tile tileTarget))
        {
            tileTarget.OnRightClickTile();
        }
    }

    private Collider2D GetRayTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        return hit2D.collider;
    }

    private bool IsMouseOverUI(out IHoverTooltip hover)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        hover = null;
        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                if (go.gameObject.TryGetComponent(out IHoverTooltip target))
                {
                    hover = target;
                    break;
                }
            }
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}
