using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private Dictionary<Item.ItemName, Item> itemInGrid;
    public Item outputItem { private set; get; }
    public Dictionary<Item.ItemName, Dictionary<Item.ItemName, int>> recipeDictionary;

    private InventoryManager playerInventory;
    private InventoryManager craftingInventory;

    public delegate void OnOutputItemChange();
    public OnOutputItemChange outputItemChangeCallback;
    public delegate void OnItemInGridChange();
    public OnItemInGridChange inputItemChangeCallback;
    private void Awake()
    {
        craftingInventory = GetComponent<InventoryManager>();

        recipeDictionary = new Dictionary<Item.ItemName, Dictionary<Item.ItemName, int>>();
        itemInGrid = new Dictionary<Item.ItemName, Item>();

        Dictionary<Item.ItemName, int> recipe = new Dictionary<Item.ItemName, int>();
        recipe[Item.ItemName.PineLog] = 2;
        recipe[Item.ItemName.Peddel] = 1;
        recipeDictionary[Item.ItemName.WoodenPickaxe] = recipe;

        recipe = new Dictionary<Item.ItemName, int>();
        recipe[Item.ItemName.PineLog] = 2;
        recipe[Item.ItemName.Peddel] = 1;
        recipeDictionary[Item.ItemName.WoodenAxe] = recipe;

        recipe = new Dictionary<Item.ItemName, int>();
        recipe[Item.ItemName.OakLog] = 2;
        recipe[Item.ItemName.Iron] = 1;
        recipeDictionary[Item.ItemName.IronPickaxe] = recipe;

        recipe = new Dictionary<Item.ItemName, int>();
        recipe[Item.ItemName.Iron] = 5;
        recipeDictionary[Item.ItemName.Helmet] = recipe;

        recipe = new Dictionary<Item.ItemName, int>();
        recipe[Item.ItemName.Iron] = 8;
        recipeDictionary[Item.ItemName.PlateBody] = recipe;
    }

    private void Start()
    {
        playerInventory = PlayerManager.instance.player.GetComponentInChildren<InventoryManager>();
        InventoryUI.instance.craftingPanelCloseCallBack += ReturnItemToInventory;
        InventoryUI.instance.craftingPanelCloseCallBack += RemoveOutoutItem;
    }
    public void AddItem(Item item)
    {
        if (item != null)
        {
            itemInGrid[item.name] = item;
            //Debug.Log(item.name.ToString() + " x " + itemInGrid[item.name].count + " added to crafting grid");
        }
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public void RemoveItem(Item item)
    {
        if (item != null)
        {
            itemInGrid.Remove(item.name);
            //Debug.Log(item.name.ToString() + " removed from crafting grid");
        }
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public bool TryCraft(Item item)
    {
        if (!recipeDictionary.ContainsKey(item.name))
            return false;

        foreach(KeyValuePair<Item.ItemName,int> requireMaterial in recipeDictionary[item.name])
        {
            if (!itemInGrid.ContainsKey(requireMaterial.Key) || itemInGrid[requireMaterial.Key].count < requireMaterial.Value)
                return false;
        }
        return true;
    }

    public Item CraftOutputItem()
    {
        List<Item> toRemove = new List<Item>();
        if (TryCraft(outputItem))
        {
            foreach (KeyValuePair<Item.ItemName, Item> itemMaterial in itemInGrid)
            {
                if (recipeDictionary[outputItem.name].ContainsKey(itemMaterial.Key))
                    itemMaterial.Value.UpdateStack( - recipeDictionary[outputItem.name][itemMaterial.Key]);
                if (itemMaterial.Value.count <= 0)
                    toRemove.Add(itemMaterial.Value);
            }

            foreach (Item itemToRemove in toRemove)
                itemToRemove.RemoveFromInventory();

            //Debug.Log(outputItem.name.ToString() + " crafted");
            return Object.Instantiate(outputItem);
        }
        else
        {
            //Debug.Log(outputItem.name.ToString() + " cant not be crafted");
            return null;
        }
    }

    public void AddOutputItem(Item item)
    {
        outputItem = item;
        //Debug.Log(item.name.ToString() + " selected for crafting");
        if (outputItemChangeCallback != null)
            outputItemChangeCallback();
    }

    public void CraftAndAdd()
    {
        playerInventory.Add(CraftOutputItem());
        if (inputItemChangeCallback != null)
            inputItemChangeCallback();
    }

    public void ReturnItemToInventory()
    {
        for (int i = 0; i < craftingInventory.inventorySize; i++)
        {
            Item itemToRemove = craftingInventory.itemList[i];
            if (itemToRemove != null)
                itemToRemove.ReturnToPlayerInventory();
        }
    }

    private void RemoveOutoutItem()
    {
        outputItem = null;
    }
}
