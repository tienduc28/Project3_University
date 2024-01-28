using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Item")]
public class Item : ScriptableObject
{
    public const int STACKLIMIT = 64;
    public new ItemName name;
    public string description;
    public bool isStackable;
    public int count;

    public Sprite sprite;
    public SkinnedMeshRenderer mesh;

    public GameObject defaultPickupItem;
    public GameObject itemPickupGraphic;

    public delegate void OnStackUpdate();
    public OnStackUpdate StackUpdateCallback;
    public delegate void OnItemRemove(Item item);
    public OnItemRemove ItemRemoveCallback;
    #region Constructors
    public Item() { }
    protected Item(Item other)
    {
        this.name = other.name;
        this.sprite = other.sprite;
        this.mesh = other.mesh;
        this.defaultPickupItem = other.defaultPickupItem;
        this.itemPickupGraphic = other.itemPickupGraphic;
        this.count = other.count;
        this.isStackable = false;
    }
    #endregion
    public virtual void Use(InventoryManager inventoryManager)
    {
        Debug.Log("Used " + name + " from inventory " + inventoryManager);
    }

    public void RemoveFromInventory()
    {
        if (ItemRemoveCallback != null)
            ItemRemoveCallback(this);
    }

    public void DropFromInventory(InventoryManager inventory)
    {
        RemoveFromInventory();
        DropInWorld(inventory.transform);
    }

    public GameObject DropInWorld(Transform transform, int amount = 1)
    {
        GameObject pickup = Instantiate(defaultPickupItem, transform.position, Quaternion.identity);
        Item thisItem = Object.Instantiate(this);
        pickup.GetComponent<PickupItemBehaviour>().item = thisItem;
        if (thisItem.isStackable)
            thisItem.count = amount;
        GameObject pickupGraphic = Instantiate(itemPickupGraphic, transform.position, itemPickupGraphic.transform.rotation);
        pickupGraphic.transform.parent = pickup.transform;

        pickup.name = this.name.ToString();
        return pickup;
    }

    public virtual bool ReturnToPlayerInventory()
    {
        RemoveFromInventory();
        InventoryManager inventory = PlayerManager.instance.player.GetComponentInChildren<InventoryManager>();
        return inventory.Add(this);
    }

    public int UpdateStack(int changeInStack)
    {
        count += changeInStack;
        if (StackUpdateCallback != null)
            StackUpdateCallback();
        if (count > STACKLIMIT)
            count = STACKLIMIT;

        return count - STACKLIMIT;
    }
    public enum ItemName { Sword, Shield, Helmet, PlateBody, PlateLegs,
        Iron, Gold,
        PineLog, OakLog, BirchLog,
        WoodenPickaxe, IronPickaxe,
        None, Stick, Peddel,
        WoodenAxe,
        Coin
    };
}
