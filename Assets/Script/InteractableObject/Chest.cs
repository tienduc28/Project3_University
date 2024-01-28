using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactalble
{
    public InventoryManager chestInventory;
    protected override void Start()
    {
        base.Start();
        chestInventory.indexOffset = PlayerManager.instance.player.GetComponentInChildren<InventoryManager>().inventorySize;
    }
    protected override void SetHoverText()
    {
        hoverText = "Press E to open";
    }

    public override void Interact(Transform player)
    {
        Debug.Log("Opened Chest");
        InventoryUI.instance.ToggleChestInventory(this.chestInventory);
    }
}
