using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : CharacterAnimator
{
    public EnemyCombat enemyCombat;

    protected override void Start()
    {
        base.Start();
        enemyCombat = GetComponent<EnemyCombat>();
        enemyCombat.initiateAttackCallback += SetAttackTrigger;
    }

    public void SetAttackTrigger()
    {
        characterAnimator.SetTrigger("attack");
        //Debug.Log("Play attack animation");
    }
}
