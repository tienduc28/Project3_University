using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemBehaviour : Interactalble
{
    public Item item;
    public InventoryManager playerInventory;

    protected override void SetHoverText()
    {
        hoverText = "Press E to pick up";
    }
    public override void Interact(Transform player)
    {
        //Debug.Log(player.name);
        playerInventory = player.GetComponentInChildren<InventoryManager>();
        //Debug.Log("Pick up");
        if (playerInventory.Add(item))
            Destroy(gameObject);
        else
            Debug.Log("Inventory Full");
    }

}
