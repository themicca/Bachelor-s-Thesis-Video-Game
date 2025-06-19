using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : SettlementBase
{
    private int basePopulation;

    new private void Awake()
    {
        basePopulation = 200;
        attack = 600;
        damage = 50;
        evasion = 100;
        defense = 20;
        hitPoints = 200;
        garrisonMax = 200;
        garrison = basePopulation;

        type = SettlementType.Outpost;
        base.Awake();
    }

    public override void SetUp()
    {
        tile.SetPopulation(basePopulation);
    }

    public static void GetCost(out int woodCost, out int stoneCost, out int manpowerCost)
    {
        woodCost = 100; stoneCost = 100; manpowerCost = 200;
    }

    public static bool CanPayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int manpowerCost);
        return player.CanSubstractWood(woodCost) && player.CanSubstractStone(stoneCost) && player.CanSubstractManpower(manpowerCost);
    }

    public static void PayCost(GamePlayer player)
    {
        GetCost(out int woodCost, out int stoneCost, out int manpowerCost);
        player.SubstractWood(woodCost);
        player.SubstractStone(stoneCost);
        player.SubstractManpower(manpowerCost);
    }
}
