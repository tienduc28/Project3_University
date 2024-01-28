using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : Interactalble
{
    public int goldToOpen;
    private PlayerEconManager playerEconManager;
    private Animator animator;
    private TreasureLoot loot;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        loot = GetComponentInChildren<TreasureLoot>();
    }

    protected override void Start()
    {
        base.Start();
        loot.gameObject.SetActive(false);
    }
    protected override void SetHoverText()
    {
        hoverText = goldToOpen + " gold to open";
    }

    public override void Interact(Transform player)
    {
        playerEconManager = player.GetComponent<PlayerEconManager>();
        if (playerEconManager.SpendGold(goldToOpen))
        {
            Debug.Log("Opened Treasure Chest");
            animator.SetTrigger("open");
            DisableHoverText();
        }
        else
            Debug.Log("Not enough gold");
    }

    public void RevealLoot()
    {
        loot.gameObject.SetActive(true);
    }
}
