using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int indexOffset = 0;
    public List<Item> itemList;
    public int inventorySize;
    public delegate void OnInventoryUpdate(Item item, int index);
    public OnInventoryUpdate inventoryAddCallBack;
    public OnInventoryUpdate inventoryRemoveCallBack;

    public void Start()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            itemList.Add(null);
        }
    }
    public virtual bool Add(Item item)
    {
        if (item == null)
            return false;

        if (item.isStackable)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (itemList[i] != null && item.name == itemList[i].name && itemList[i].count < Item.STACKLIMIT)
                {
                    return Add(item, i + indexOffset);
                }
            }
        }
        
        for (int i = 0; i < inventorySize; i++)
            if (itemList[i] == null)
            {
                //Debug.Log("Found empty slot");
                return Add(item, i + indexOffset);
            }

        return false;
        
    }

    public virtual bool Add(Item item, int index)
    {
        //Debug.Log(this.gameObject.name);
        if (item == null)
        {
            //Debug.Log("Null item");
            return false;
        }
        if (index < inventorySize + indexOffset && index >= indexOffset)
        {
            //Debug.Log("Add to slot number " + index);

            if (itemList[index - indexOffset] != null && itemList[index - indexOffset].name == item.name)
            {
                item.UpdateStack(itemList[index].count);
            }

            itemList[index - indexOffset] = item;
            item.ItemRemoveCallback += Remove;
            if (inventoryAddCallBack != null)
                inventoryAddCallBack.Invoke(item, index);
            return true;
        }

        return false;
    }

    public virtual void Remove(Item item)
    {
        if (item == null)
        {
            //Debug.Log("Null item");
            return;
        }

        int itemIndex = itemList.IndexOf(item);
        if (itemIndex >= 0 && itemIndex < inventorySize)
        {
            itemList[itemIndex] = null;
        }
        //Debug.Log(itemIndex);
        item.ItemRemoveCallback -= Remove;
        if (inventoryRemoveCallBack != null)
            inventoryRemoveCallBack.Invoke(item, itemIndex + indexOffset);
    }
}
