using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionEvent : MonoBehaviour
{
    AttackPos enemyAttackPos;

    private void Awake()
    {
        enemyAttackPos = transform.parent.GetComponentInChildren<AttackPos>();
    }

    public void ActiveAttackHitbox()
    {
        enemyAttackPos.GetComponent<Collider>().enabled = true;
    }

    public void DeactiveAttackHitbox()
    {
        enemyAttackPos.GetComponent<Collider>().enabled = false;
    }
}
