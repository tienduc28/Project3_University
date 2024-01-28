using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : EnemyCombat
{
    float specialAttackCharge = 0;
    float specialAttackThreshold = 5;
    private bool specialAttackReady = false;
    public Collider specialAttackCol;

    protected override void Awake()
    {
        base.Awake();
        specialAttackCol = GameObject.Find("SpecialAttackPos").GetComponent<Collider>();
    }

    public override void AttackPattern()
    {
        if (specialAttackCharge < specialAttackThreshold)
            specialAttackCharge += Time.deltaTime * Mathf.Sqrt(enemyController.distance / attackRange);
       else
            specialAttackReady = true;
        
    }

    public void ResetSpecialAttack()
    {
        specialAttackReady = false;
        specialAttackCharge = 0;
    }

    public bool IsSpecialAttackReady()
    {
        return specialAttackReady;
    }
}
