using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Tool")]
public class Tool : Equipment
{
    public int toolLevel;
    public ToolType toolType;
    private void Reset()
    {
        equipSlot = EquipSlot.MainHand;
        coveredMeshRegions = new EquipmentMeshRegion[] { EquipmentMeshRegion.Arms };
    }

    #region Constructors
    public Tool() { }

    public Tool(Item item) : base(item)
    {
        equipSlot = EquipSlot.MainHand;
        coveredMeshRegions = new EquipmentMeshRegion[] { EquipmentMeshRegion.Arms };
        armorModifier = 0;
        damageModifier = 0;
        toolLevel = 0;
    }
    #endregion

    public override void Equip()
    {
        PlayerManager.instance.player.GetComponentInChildren<EquipmentManager>().Equip(this);
    }

    public override bool ReturnToPlayerInventory()
    {
        return true;
    }

    public enum ToolType { Axe, Pickage, None}
}
