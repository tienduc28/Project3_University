using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CharacterCombat
{
    [SerializeField]
    private float attackCooldown = 0;
    public bool isDead
    { get; private set; }

    protected override void Start()
    {
        base.Start();
        ((PlayerStats)myStats).deadCallBack += PlayerDied;
        isDead = false;
    }

    protected override void Update()
    {
        if (isDead) return;
        base.Update();
        attackCooldown -= Time.deltaTime;

        if (InventoryUI.instance.isInventoryOpen)
            return;

        if (Input.GetMouseButtonDown(0))
            InitiateAttack();
    }
    void InitiateAttack()
    {
        if (attackCooldown <= 0)
        {
            attackCooldown = 1 / myStats.statData.attackSpeed.GetValue();
            Attack();
            attackPos.GetComponent<Collider>().enabled = true;
        }
    }

    protected override void SetTargetTag()
    {
        base.SetTargetTag();
        targetTag = "Enemy";
    }

    public void PlayerDied()
    {
        isDead = true;
    }

    public void RevivePlayer()
    {
        isDead = false;
        myStats.Heal();
    }
}
