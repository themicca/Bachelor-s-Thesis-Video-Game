using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    bool KillManpower();
    void GetStats(out int attack, out int damage, out int evasion, out int defense, out int hitPoints, out int manpower);
    Combat GetCombat();
    void SetCombat(Combat combat);
    GamePlayer GetOwner();
    GameObject GetObject();
    void ClearCombat();
}
