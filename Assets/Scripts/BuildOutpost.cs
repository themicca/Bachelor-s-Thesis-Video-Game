using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOutpost : UnitAction
{
    public override void Action(Unit actionUnit, Tile actionTile)
    {
        GamePlayer player = actionUnit.GetOwner();
        bool neigboursBorder = false;
        foreach(Tile neighbour in actionTile.GetNeighbours())
        {
            if (neighbour.GetOwner() == player)
            {
                neigboursBorder = true;
                break;
            }
        }
        if (!neigboursBorder)
        {
            // to do - make error message popup "Can only build near your borders." on the tile
            return;
        }

        if (actionTile.GetSettlementType() != SettlementType.None && actionTile.GetOwner() != null)
        {
            // to do - make error message popup "This tile is already occupied." on the tile
            return;
        }

        Outpost.GetCost(out int woodCost, out int stoneCost, out int popCost);
        if (player.CanSubstractWood(woodCost) && player.CanSubstractStone(stoneCost) && player.CanSubstractManpower(popCost))
        {
            player.SubstractWood(woodCost);
            player.SubstractStone(stoneCost);
            player.SubstractManpower(popCost);
        }
        else
        {
            // to do - make error message popup "You do not have enough resources." on the tile
            return;
        }

        actionTile.CreateSettlement(SettlementType.Outpost);
        actionTile.SetOwner(player);
    }
}
