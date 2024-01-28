using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public Item item;
    public int amount;
}

[RequireComponent(typeof(EnemyStats))]
public class EnemyCombat : CharacterCombat
{
    protected EnemyController enemyController;

    public List<Loot> loots;
    public float attackRange;
    protected Transform target;
    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController>();
    }

    protected override void Start()
    {
        base.Start();
        target = PlayerManager.instance.player.transform;
        attackRange = myStats.statData.attackRange.GetValue();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        AttackPattern();
    }

    public virtual void AttackPattern()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else if (enemyController.distance < attackRange)
        {
            Attack();
            attackTimer = 1 / myStats.statData.attackSpeed.GetValue();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died");
        DropLoot();
        Destroy(gameObject);
    }

    public void DropLoot()
    {
        Debug.Log("Drop Loot");
        var random = new System.Random();
        int index = random.Next(loots.Count);
        if (index >= 0 && index < loots.Count)
            loots[index].item.DropInWorld(transform, loots[index].amount);
    }

    protected override void SetTargetTag()
    {
        base.SetTargetTag();
        targetTag = "Player";
    }
}
