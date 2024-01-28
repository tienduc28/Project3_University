using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Image icon;
    public Equipment equipment { get; private set; }
    [SerializeField] private EquipSlot equipmentSlot;

    public void AddEquipment(Equipment newEquipment)
    {
        equipment = newEquipment;
   
        icon.sprite = newEquipment.sprite;
        icon.enabled = true;
    }

    public void RemoveEquipment()
    {
        equipment = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void Unequip()
    {
        if (equipment != null)
        {
            EquipmentManager equipmentManager = PlayerManager.instance.player.GetComponentInChildren<EquipmentManager>();
            equipmentManager.Unequip((int)equipment.equipSlot);
        }
    }

    public void SwapEquipment(InventorySlot swapTarget)
    {
        Item targetItem = swapTarget.item;
        if (targetItem != null)
        {
            Equipment targetEquipment = targetItem as Equipment;
            if (targetEquipment != null && targetEquipment.equipSlot == equipmentSlot)
            {
                Unequip();
                targetEquipment.Equip();
            }
        }
    }
}
