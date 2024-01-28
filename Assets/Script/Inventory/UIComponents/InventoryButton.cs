using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryButton : InterativeButton
{
    public InventorySlot inventorySlot;
    public ToolTip toolTip;

    protected override void Awake()
    {
        base.Awake();
        inventorySlot = transform.parent.GetComponent<InventorySlot>();
        toolTip = GetComponentInChildren<ToolTip>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isMouseOverButtom)
            inventorySlot.DropItem();
    }

    public override void ActionOnClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            inventorySlot.UseItem();
    }

    public override void ActionOnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryButton inventoryButton = eventData.pointerDrag.GetComponent<InventoryButton>();
            if (inventoryButton != null)
            {
                //Debug.Log("Drop from " + inventoryButton.transform.parent.parent.name);
                inventorySlot.SwapItemSlot(inventoryButton.inventorySlot);
            }
            else
            {
                Debug.Log("Drop from equipment");
                EquipmentButton equipmentButton = eventData.pointerDrag.GetComponent<EquipmentButton>();
                if (equipmentButton != null)
                {
                    equipmentButton.equipmentSlot.Unequip();
                }

            }
        }
    }

    public override void ActionOnMouseEnter(PointerEventData eventData)
    {
        if (inventorySlot.item != null)
        {
            toolTip.GetComponent<ToolTip>().SetText(inventorySlot.item.name.ToString() + '\n' + inventorySlot.item.description);
            toolTip.ShowTooltip();
        }
    }

    public override void ActionOnMouseExit(PointerEventData eventData)
    {
        toolTip.HideTooltip();
    }

}
