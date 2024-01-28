using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipSlot equipSlot;
    public EquipmentMeshRegion[] coveredMeshRegions;

    public int armorModifier;
    public int damageModifier;

    #region Constructors
    public Equipment() { }
    
    public Equipment(Item item) : base(item)
    {
    
    }
    #endregion
    public override void Use(InventoryManager inventoryManager)
    {
        base.Use(inventoryManager);
        Equip();
    }

    public virtual void Equip()
    {
        PlayerManager.instance.player.GetComponentInChildren<EquipmentManager>().Equip(this);
        RemoveFromInventory();
    }

}

public enum EquipSlot { Head, Chest, Legs, Feet, MainHand, OffHand }
public enum EquipmentMeshRegion { Legs, Arms, Torso}