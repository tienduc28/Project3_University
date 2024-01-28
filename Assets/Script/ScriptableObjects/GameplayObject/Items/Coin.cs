using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Coin")]
public class Coin : Item
{
    public int value;
    public override void Use(InventoryManager inventoryManager)
    {
        base.Use(inventoryManager);
        PlayerEconManager playerEcon = inventoryManager.transform.parent.GetComponent<PlayerEconManager>();
        if (playerEcon != null)
        {
            playerEcon.GainGold(value * count);
            Debug.Log("Player gains " + value + " gold");
            RemoveFromInventory();
        }
        else
            Debug.Log("Coin not in player inventory");
    }
}
