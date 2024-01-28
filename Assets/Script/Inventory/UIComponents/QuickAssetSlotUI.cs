using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickAssetSlotUI : InventorySlot
{
    private Image backGround;
    private Color32 selectedColor = new Color32(100, 100, 100, 255);
    private bool isSelected;

    public delegate void OnSlotUpdated(Item item);
    public OnSlotUpdated slotUpdatedCallback;

    private void Awake()
    {
        backGround = GetComponentInChildren<Image>();    
    }

    public override void AddItem(Item newItem)
    {
        if (isSelected && slotUpdatedCallback != null)
            slotUpdatedCallback(newItem);
        base.AddItem(newItem);
    }

    public override Item RemoveItem()
    {
        if (isSelected && slotUpdatedCallback != null)
            slotUpdatedCallback(null);
        return base.RemoveItem();
    }

    public void GetSelect()
    {
        slotUpdatedCallback(item);
        backGround.color = selectedColor;
        isSelected = true;
    }

    public void GetUnselect()
    {
        backGround.color = Color.white;
        isSelected = false;
    }
}
