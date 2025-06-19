using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MovementController
{
    List<Tile> path;
    Tile nextTile;
    Unit unit;
    private LineRenderer _renderer;
    int currentCost;

    public MovementController(List<Tile> path, Unit unit)
    {
        this.path = path;
        this.unit = unit;
        _renderer = unit.GetPathMoveRenderer();
        this.unit.UpdateLineRenderer(path, _renderer);
        currentCost = unit.GetTile().GetPathCost();
    }

    public void HandleMovement()
    {
        if (path == null || path.Count <= 1)
        {
            nextTile = null;
            unit.UpdateLineRenderer(new List<Tile>(), _renderer);
            GameManager.RemoveMovement(unit);
        }
        else if (currentCost == 1)
        {
            nextTile = path[1];
            currentCost = nextTile.GetPathCost();

            if (nextTile.GetUnits().Count > 0 && nextTile.GetUnits().First().GetOwner() == unit.GetOwner())
            {
                if (unit.GetSelector() != null)
                {
                    unit.ClearSelector();
                    if (unit.GetTile().GetUnits().Count == 2)
                    {
                        unit.GetTile().GetUnits().Find(x => x != unit).ClearSelector();
                    }
                }
                if (nextTile.GetUnits().Count == 1)
                {
                    StackedSelectorUI selector = UnityEngine.Object.Instantiate(Prefabs.GetUnitStackingUIPrefab(), Prefabs.GetUnitUI().transform).GetComponent<StackedSelectorUI>();
                    unit.SetSelector(selector);
                    nextTile.GetUnits().First().SetSelector(selector);
                    selector.transform.position = nextTile.transform.position + new Vector3(0.2f, 0.2f);
                }
                else
                {
                    StackedSelectorUI selector = GameManager.FindSelector(nextTile.GetUnits().First().GetSelector());
                    unit.SetSelector(selector);
                    selector.transform.position = nextTile.transform.position + new Vector3(0.2f, 0.2f);
                }

            }
            else if (nextTile.GetUnits().Count > 0 && nextTile.GetUnits().First().GetOwner() != unit.GetOwner() ||
                nextTile.GetOwner() != unit.GetOwner() && nextTile.GetSettlementType() != SettlementType.None)
            {
                if (nextTile.GetUnits().Count > 0 && nextTile.GetUnits().First().GetCombat() != null)
                {
                    nextTile.GetUnits().First().GetCombat().AddArmy(unit, CombatSide.attacker);
                    EndMovement();
                    return;
                }
                else if (nextTile.GetSettlement().GetCombat() != null)
                {
                    nextTile.GetSettlement().GetCombat().AddArmy(unit, CombatSide.attacker);
                    EndMovement();
                    return;
                }
                List<IAttackable> defenders = new();
                defenders.AddRange(nextTile.GetUnits());
                if (nextTile.GetSettlementType() != SettlementType.None) defenders.Add(nextTile.GetSettlement());

                new Combat(new List<IAttackable>() { unit }, defenders);
                EndMovement();
                return;
            }
            else if (unit.GetSelector() != null)
            {
                unit.ClearSelector();
                if (unit.GetTile().GetUnits().Count == 2)
                {
                    unit.GetTile().GetUnits().Find(x => x != unit).ClearSelector();
                }
            }

            path.RemoveAt(0);
            unit.SetPosition(nextTile);
            unit.UpdateLineRenderer(path, _renderer);
            if (path == null || path.Count <= 1)
            {
                nextTile = null;
                unit.UpdateLineRenderer(new List<Tile>(), _renderer);
                GameManager.RemoveMovement(unit);
            }
        }
        else
        {
            currentCost--;
        }
    }

    public int GetCurrentCost() { return currentCost; }

    private void EndMovement()
    {
        currentCost = 0;
        path = null;
        nextTile = null;
        unit.UpdateLineRenderer(new List<Tile>(), _renderer);
        GameManager.RemoveMovement(unit);
    }

    public override bool Equals(object obj)
    {
        return obj is MovementController controller &&
               EqualityComparer<Unit>.Default.Equals(unit, controller.unit);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
