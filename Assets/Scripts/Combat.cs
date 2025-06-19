using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum CombatSide
{
    defender,
    attacker
}

public class Combat
{
    CombatDisplay display;

    private int totalAttackerManpower;
    private int totalDefenderManpower;
    private int aliveAttackerManpower;
    private int aliveDefenderManpower;
    private int deadAttackerManpower;
    private int deadDefenderManpower;
    private int aliveDefenderUnitManpower;

    private int attackerAttack;
    private int defenderAttack;
    private int attackerDamage;
    private int defenderDamage;
    private int attackerEvasion;
    private int defenderEvasion;
    private int attackerDefense;
    private int defenderDefense;
    private int attackerHitPoints;
    private int defenderHitPoints;

    private int averageAttackerAttack;
    private int averageDefenderAttack;
    private int averageAttackerDamage;
    private int averageDefenderDamage;
    private int averageAttackerEvasion;
    private int averageDefenderEvasion;
    private int averageAttackerDefense;
    private int averageDefenderDefense;
    private int averageAttackerHitPoints;
    private int averageDefenderHitPoints;

    List<IAttackable> attackers;
    List<IAttackable> attackersWithLivingUnits;
    List<IAttackable> defenders;
    List<IAttackable> defendersWithLivingUnits;
    SettlementBase defendingSettlement;

    public Combat(List<IAttackable> attackers, List<IAttackable> defenders)
    {
        totalAttackerManpower = 0; totalDefenderManpower = 0; attackerAttack = 0; defenderAttack = 0; attackerDamage = 0; defenderDamage = 0;
        attackerEvasion = 0; defenderEvasion = 0; attackerDefense = 0; defenderDefense = 0; attackerHitPoints = 0; defenderHitPoints = 0;
        aliveAttackerManpower = 0; aliveDefenderManpower = 0; aliveDefenderUnitManpower = 0;
        this.attackers = new List<IAttackable>();
        this.defenders = new List<IAttackable>();
        attackersWithLivingUnits = new List<IAttackable>();
        defendersWithLivingUnits = new List<IAttackable>();

        foreach (IAttackable combatParticipant in attackers)
        {
            AddArmy(combatParticipant, CombatSide.attacker);
        }
        foreach (IAttackable combatParticipant in defenders)
        {
            if (combatParticipant is SettlementBase)
            {
                defendingSettlement = (SettlementBase) combatParticipant;
            }
            AddArmy(combatParticipant, CombatSide.defender);
        }

        if (defendersWithLivingUnits.Count == 0)
        {
            defendersWithLivingUnits.Add(defendingSettlement);
        }

        display = Object.Instantiate(Prefabs.GetCombatDisplayPrefab(), defenders.First().GetObject().transform.position, Quaternion.identity);
        display.SetCombat(this);
        GameManager.AddCombat(this);
    }

    public void Fight()
    {
        float attackerChanceToHit = (averageAttackerAttack - averageDefenderEvasion) / 10;
        float defenderChanceToHit = (averageDefenderAttack - averageAttackerEvasion) / 10;
        float attackerDamageDealt = averageAttackerDamage - averageDefenderDefense;
        float defenderDamageDealt = averageDefenderDamage - averageAttackerDefense;
        float attackerCurrentHitPoints = averageAttackerHitPoints;
        float defenderCurrentHitPoints = averageDefenderHitPoints;
        int attackerCurrentDeadUnits = 0;
        int defenderCurrentDeadUnits = 0;

        for (int i = 0; i < aliveAttackerManpower; i++)
        {
            float chance = Random.Range(0, 1000) / 10;
            if (chance <= attackerChanceToHit)
            {
                defenderCurrentHitPoints -= attackerDamageDealt;
                if (defenderCurrentHitPoints < 0)
                {
                    defenderCurrentDeadUnits++;
                    aliveDefenderUnitManpower--;
                    int random = Random.Range(0, defendersWithLivingUnits.Count);
                    IAttackable unit;
                    unit = defendersWithLivingUnits.ElementAt(random);
                    
                    if (unit.KillManpower())
                    {
                        if (defendersWithLivingUnits.ElementAt(random) is not SettlementBase)
                        {
                            defendersWithLivingUnits.RemoveAt(random);
                            ((Unit)unit).DestroyUnitServerRpc();
                        } else { defendersWithLivingUnits.RemoveAt(random); }
                    }
                    if (aliveDefenderUnitManpower == 0 && defendingSettlement != null)
                    {
                        defendersWithLivingUnits.Clear();
                        defendersWithLivingUnits.Add(defendingSettlement);
                    }
                    if (defenderCurrentDeadUnits + deadDefenderManpower == totalDefenderManpower)
                    {
                        if (defendingSettlement != null)
                        {
                            defendingSettlement.GetTile().SetOwner(attackers.First().GetOwner());
                            defendingSettlement.GetTile().SetPopulation(defendingSettlement.GetTile().GetMinPopulation());
                        }
                        break;
                    }
                    defenderCurrentHitPoints = averageDefenderHitPoints + defenderCurrentHitPoints;
                }
            }
        }
        for (int i = 0; i < aliveDefenderManpower; i++)
        {
            float chance = Random.Range(0, 1000) / 10;
            if (chance <= defenderChanceToHit)
            {
                attackerCurrentHitPoints -= defenderDamageDealt;
                if (attackerCurrentHitPoints < 0)
                {
                    attackerCurrentDeadUnits++;
                    int random = Random.Range(0, attackersWithLivingUnits.Count);
                    Unit unit = (Unit) attackersWithLivingUnits.ElementAt(random);
                    if (unit.KillManpower())
                    {
                        attackersWithLivingUnits.RemoveAt(random);
                        unit.DestroyUnitServerRpc();
                    }
                    if (attackerCurrentDeadUnits + deadAttackerManpower == totalAttackerManpower)
                    {
                        break;
                    }
                    attackerCurrentHitPoints = averageAttackerHitPoints + attackerCurrentHitPoints;
                }
            }
        }

        deadAttackerManpower += attackerCurrentDeadUnits;
        deadDefenderManpower += defenderCurrentDeadUnits;
        aliveAttackerManpower -= attackerCurrentDeadUnits;
        aliveDefenderManpower -= defenderCurrentDeadUnits;

        if (deadAttackerManpower == totalAttackerManpower || deadDefenderManpower == totalDefenderManpower)
        {
            foreach (IAttackable survivedUnit in attackers)
            {
                if (survivedUnit != null)
                {
                    survivedUnit.ClearCombat();
                }
            }
            foreach (IAttackable survivedUnit in defenders)
            {
                if (survivedUnit != null)
                {
                    survivedUnit.ClearCombat();
                }
            }
            Object.Destroy(display.gameObject);
            GameManager.RemoveCombat(this);
        }
    }

    public void AddArmy(IAttackable combatParticipant, CombatSide side)
    {
        combatParticipant.SetCombat(this);
        combatParticipant.GetStats(out int attack, out int damage, out int evasion, out int defense, out int hitPoints, out int manpower);
        if (side == CombatSide.defender)
        {
            defenders.Add(combatParticipant);
            if (combatParticipant is Unit)
            {
                defendersWithLivingUnits.Add(combatParticipant);
                aliveDefenderUnitManpower += manpower;
            }

            totalDefenderManpower += manpower;
            aliveDefenderManpower += manpower;
            defenderAttack += attack;
            defenderDamage += damage;
            defenderDefense += defense;
            defenderEvasion += evasion;
            defenderHitPoints += hitPoints;

            averageDefenderAttack = defenderAttack / defenders.Count;
            averageDefenderDamage = defenderDamage / defenders.Count;
            averageDefenderDefense = defenderDefense / defenders.Count;
            averageDefenderEvasion = defenderEvasion / defenders.Count;
            averageDefenderHitPoints = defenderHitPoints / defenders.Count;
        }
        else
        {
            attackers.Add(combatParticipant);
            if (combatParticipant is Unit)
            {
                attackersWithLivingUnits.Add(combatParticipant);
            }

            totalAttackerManpower += manpower;
            aliveAttackerManpower += manpower;
            attackerAttack += attack;
            attackerDamage += damage;
            attackerDefense += defense;
            attackerEvasion += evasion;
            attackerHitPoints += hitPoints;

            averageAttackerAttack = attackerAttack / attackers.Count;
            averageAttackerDamage = attackerDamage / attackers.Count;
            averageAttackerDefense = attackerDefense / attackers.Count;
            averageAttackerEvasion = attackerEvasion / attackers.Count;
            averageAttackerHitPoints = attackerHitPoints / attackers.Count;
        }
    }

    public void GetAttackerStats(out string attackerName, out int attackers, out int totalAttackerManpower, out int aliveAttackerManpower,
        out int deadAttackerManpower, out int averageAttackerAttack, out int averageAttackerDamage,
        out int averageAttackerEvasion, out int averageAttackerDefense, out int averageAttackerHitPoints)
    {
        attackerName = this.attackers.First().GetOwner().GetEmpireName();
        attackers = this.attackers.Count;
        totalAttackerManpower = this.totalAttackerManpower;
        aliveAttackerManpower = this.aliveAttackerManpower;
        deadAttackerManpower = this.deadAttackerManpower;
        averageAttackerAttack = this.averageAttackerAttack;
        averageAttackerDamage = this.averageAttackerDamage;
        averageAttackerEvasion = this.averageAttackerEvasion;
        averageAttackerDefense = this.averageAttackerDefense;
        averageAttackerHitPoints = this.averageAttackerHitPoints;
    }

    public void GetDefenderStats(out string defenderName, out int defenders, out int totalDefenderManpower, out int aliveDefenderManpower,
        out int deadDefenderManpower, out int averageDefenderAttack, out int averageDefenderDamage,
        out int averageDefenderEvasion, out int averageDefenderDefense, out int averageDefenderHitPoints)
    {
        defenderName = this.defenders.First().GetOwner().GetEmpireName();
        defenders = this.defenders.Count;
        totalDefenderManpower = this.totalDefenderManpower;
        aliveDefenderManpower = this.aliveDefenderManpower;
        deadDefenderManpower = this.deadDefenderManpower;
        averageDefenderAttack = this.averageDefenderAttack;
        averageDefenderDamage = this.averageDefenderDamage;
        averageDefenderEvasion = this.averageDefenderEvasion;
        averageDefenderDefense = this.averageDefenderDefense;
        averageDefenderHitPoints = this.averageDefenderHitPoints;
    }
}
