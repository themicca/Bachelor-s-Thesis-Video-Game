using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPriceable
{
    bool CanPayCost(GamePlayer player);
    void PayCost(GamePlayer player);
}
