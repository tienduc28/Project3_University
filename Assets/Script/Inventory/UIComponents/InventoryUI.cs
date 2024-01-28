using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    #region Singleton
    public static InventoryUI instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);

        instance = this;
    }

    #endregion

    #region NummericKeyCode
    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };
    #endregion
    public bool isInventoryOpen;
    public GameObject inventoryPanel;
    public InventorySlot[] inventorySlots;
    public InventoryManager playerInventory;

    public GameObject craftingGridPanel;
    [SerializeField]
    private GameObject craftingAndEquipmentPanel;
    public InventoryManager craftingInventory;
    public InventorySlot[] craftingSlots;

    public GameObject craftingOutputPanel;
    public GameObject craftingOutputWindow;
    public OutputItemButton[] availableRecipes;
    public GameObject outputButtomPrefab;
    public CraftingManager craftingManager;

    private int selectedSlotIndex = 0;
    public GameObject quickAcessPanel;
    public QuickAssetSlotUI[] quickAssetSlots;

    public GameObject equipmentPanel;
    public EquipmentSlot[] equipmentSlots;
    EquipmentManager equipment;

    public GameObject chestPanel;
    public InventorySlot[] chestSlots;
    public InventoryManager chestInventory;

    public RectTransform parentTransform;

    public delegate void OnCraftingPanelClose();
    public OnCraftingPanelClose craftingPanelCloseCallBack;

    private void Start()
    {
        playerInventory = PlayerManager.instance.player.GetComponentInChildren<InventoryManager>();
        equipment = PlayerManager.instance.player.GetComponentInChildren<EquipmentManager>();
        craftingInventory = PlayerManager.instance.player.transform.Find("CraftingManager").GetComponentInChildren<InventoryManager>();
        craftingManager = PlayerManager.instance.player.GetComponentInChildren<CraftingManager>();

        inventorySlots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        quickAssetSlots = quickAcessPanel.GetComponentsInChildren<QuickAssetSlotUI>();
        chestSlots = chestPanel.GetComponentsInChildren<InventorySlot>();
        craftingSlots = craftingAndEquipmentPanel.GetComponentsInChildren<CraftingSlot>();

        inventorySlots = inventorySlots.Concat(quickAssetSlots).ToArray<InventorySlot>();
        inventorySlots = inventorySlots.Concat(chestSlots).ToArray<InventorySlot>();
        inventorySlots = inventorySlots.Concat(craftingSlots).ToArray<InventorySlot>();
        equipmentSlots = equipmentPanel.GetComponentsInChildren<EquipmentSlot>(true);

        playerInventory.inventoryAddCallBack += UpdateInventoryAdd;
        playerInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
        craftingInventory.inventoryAddCallBack += UpdateInventoryAdd;
        craftingInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
        craftingManager.inputItemChangeCallback += ResetOutputItemUI;
        craftingManager.outputItemChangeCallback += ResetOutputItemUI;
        equipment.equipmentChangeCallBack += UpdateEquipment;

        craftingGridPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        chestPanel.SetActive(false);
        craftingOutputPanel.SetActive(false);

        for (int i = 0; i < quickAssetSlots.Length; i++)
            quickAssetSlots[i].slotUpdatedCallback += equipment.EquipMainHand;

        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].index = i;

        for (int i = 0; i < playerInventory.inventorySize; i++)
            inventorySlots[i].inventoryManager = playerInventory;

        for (int i = 0; i < craftingInventory.inventorySize; i++)
            craftingSlots[i].inventoryManager = craftingInventory;

        //foreach(KeyValuePair<Item.ItemName, Dictionary<Item.ItemName, int>> recipe in craftingManager.recipeDictionary)
        //{
        //    OutputItemButton newOutputItemButton = Instantiate(outputButtomPrefab, craftingOutputPanel.transform).GetComponent<OutputItemButton>();
        //    newOutputItemButton.item = 
            
        //}
        availableRecipes = craftingOutputPanel.GetComponentsInChildren<OutputItemButton>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (chestPanel.activeSelf)
                ToggleChestInventory(null);
            else if (craftingGridPanel.activeSelf)
                ToggleCraftingPanel();
            else
                ToggleInventory();
        }

        QuickAccessControl();     
    }

    private void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        equipmentPanel.SetActive(inventoryPanel.activeSelf);
        ToggleCraftingOutputPanel();


        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    public void ToggleCraftingPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        craftingGridPanel.SetActive(inventoryPanel.activeSelf);
        ToggleCraftingOutputPanel();

        if (!craftingGridPanel.activeSelf && craftingPanelCloseCallBack != null)
            craftingPanelCloseCallBack();

        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    private void ToggleCraftingOutputPanel()
    {
        craftingOutputPanel.SetActive(inventoryPanel.activeSelf);
        ResetOutputItemUI();
    }
    public void ToggleChestInventory(InventoryManager chestInventory)
    {
        if (chestInventory != null)
        {
            this.chestInventory = chestInventory;
            OpenChest(chestInventory);
        }
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        chestPanel.SetActive(inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = inventoryPanel.activeSelf;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
        
    }

    private void OpenChest(InventoryManager chestInventory)
    {
        if (chestInventory == null)
            return;
        for (int i = 0; i < chestSlots.Length; i++)
        {
            chestSlots[i].inventoryManager = chestInventory;
            chestSlots[i].AddItem(chestInventory.itemList[i]);
        }

        if (chestInventory.inventoryAddCallBack == null)
            chestInventory.inventoryAddCallBack += UpdateInventoryAdd;
        if (chestInventory.inventoryRemoveCallBack == null)
            chestInventory.inventoryRemoveCallBack += UpdateInventoryRemove;
    }
    private void QuickAccessControl()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                selectedSlotIndex = i;
                quickAssetSlots[i].GetSelect();
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (i != selectedSlotIndex)
                quickAssetSlots[i].GetUnselect();
        }
    }

    #region UpdateInventoryUI
    private void UpdateInventoryAdd(Item item, int index)
    {
        if (item != null)
            //Debug.Log(item.name + " added to inventory");
        if (index >= 0)
            inventorySlots[index].AddItem(item);
    }


    private void UpdateInventoryRemove(Item item, int index)
    {
        //Debug.Log("Removed from inventory");
        if (index >= 0)
            inventorySlots[index].RemoveItem();             
    }
    #endregion
    private void UpdateEquipment(Equipment newEquipment, Equipment oldEquipment)
    {
        int index;
       
        if (newEquipment != null && newEquipment.GetType() != typeof(Tool))
        {
            index = (int)newEquipment.equipSlot;
            equipmentSlots[index].AddEquipment(newEquipment);
        }
        else if (oldEquipment != null && oldEquipment.GetType() != typeof(Tool))
        {
            index = (int)oldEquipment.equipSlot;
            equipmentSlots[index].RemoveEquipment();
        }
    }

    private void ResetOutputItemUI()
    {
        //Debug.Log("Output UI reseted");
        foreach (OutputItemButton outputItemButton in availableRecipes)
        {
            if (craftingManager.TryCraft(outputItemButton.item))
                outputItemButton.gameObject.SetActive(true);
            else
                outputItemButton.gameObject.SetActive(false);

            if (outputItemButton.item != craftingManager.outputItem)
                outputItemButton.GetUnselect();
        }
    }    
}
