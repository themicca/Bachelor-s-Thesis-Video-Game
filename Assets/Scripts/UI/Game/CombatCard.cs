using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatCard : MonoBehaviour
{
    private Combat combat;

    [SerializeField] private TMP_Text leftSideName;
    [SerializeField] private TMP_Text leftTotalArmies;
    [SerializeField] private TMP_Text leftTotalUnits;
    [SerializeField] private TMP_Text leftAliveUnits;
    [SerializeField] private TMP_Text leftDeadUnits;
    [SerializeField] private TMP_Text leftAttack;
    [SerializeField] private TMP_Text leftDamage;
    [SerializeField] private TMP_Text leftEvasion;
    [SerializeField] private TMP_Text leftDefense;
    [SerializeField] private TMP_Text leftHitpoints;

    [SerializeField] private TMP_Text rightSideName;
    [SerializeField] private TMP_Text rightTotalArmies;
    [SerializeField] private TMP_Text rightTotalUnits;
    [SerializeField] private TMP_Text rightAliveUnits;
    [SerializeField] private TMP_Text rightDeadUnits;
    [SerializeField] private TMP_Text rightAttack;
    [SerializeField] private TMP_Text rightDamage;
    [SerializeField] private TMP_Text rightEvasion;
    [SerializeField] private TMP_Text rightDefense;
    [SerializeField] private TMP_Text rightHitPoints;

    public void Display(Combat combat)
    {
        this.combat = combat;
        combat.GetAttackerStats(out string attackerName, out int attackers, out int totalAttackerManpower, out int aliveAttackerManpower,
        out int deadAttackerManpower, out int averageAttackerAttack, out int averageAttackerDamage, out int averageAttackerEvasion,
        out int averageAttackerDefense, out int averageAttackerHitPoints);
        leftSideName.text = attackerName + "'s side";
        leftTotalArmies.text = "Total Armies: " + attackers.ToString();
        leftTotalUnits.text = "Total Units: " + totalAttackerManpower.ToString();
        leftAliveUnits.text = "Units Alive: " + aliveAttackerManpower.ToString();
        leftDeadUnits.text = "Dead´Units: " + deadAttackerManpower.ToString();
        leftAttack.text = "Attack: " + (averageAttackerAttack / 10).ToString();
        leftDamage.text = "Damage: " + averageAttackerDamage.ToString();
        leftEvasion.text = "Evasion: " + (averageAttackerEvasion / 10).ToString();
        leftDefense.text = "Defense: " + averageAttackerDefense.ToString();
        leftHitpoints.text = "Hit Points: " + averageAttackerHitPoints.ToString();
        combat.GetDefenderStats(out string defenderName, out int defenders, out int totalDefenderManpower, out int aliveDefenderManpower,
        out int deadDefenderManpower, out int averageDefenderAttack, out int averageDefenderDamage, out int averageDefenderEvasion,
        out int averageDefenderDefense, out int averageDefenderHitPoints);
        rightSideName.text = defenderName + "'s side";
        rightTotalArmies.text = "Total Armies: " + defenders.ToString();
        rightTotalUnits.text = "Total Units: " + totalDefenderManpower.ToString();
        rightAliveUnits.text = "Units Alive: " + aliveDefenderManpower.ToString();
        rightDeadUnits.text = "Dead Units: " + deadDefenderManpower.ToString();
        rightAttack.text = "Attack: " + (averageDefenderAttack / 10).ToString();
        rightDamage.text = "Damage: " + averageDefenderDamage.ToString();
        rightEvasion.text = "Evasion: " + (averageDefenderEvasion / 10).ToString();
        rightDefense.text = "Defense: " + averageDefenderDefense.ToString();
        rightHitPoints.text = "Hit Points: " + averageDefenderHitPoints.ToString();
    }
}
