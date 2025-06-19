using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDisplay : MonoBehaviour
{
    Combat combat;
    private static Combat active;

    private void Awake()
    {
        transform.SetParent(Prefabs.GetUnitUI().transform, false);
    }

    public void SetCombat(Combat combat) { this.combat = combat; }
    public Combat GetCombat() { return combat; }

    public void Clicked()
    {
        PlayerActionCanvas.UpdateCombatCard(combat);
        active = combat;
    }

    public static void ClearInstance() { active = null; }
    public static Combat GetActiveInstance() { return active; }
}
