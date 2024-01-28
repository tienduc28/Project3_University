using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputItemButton : MonoBehaviour
{
    private Button button;
    [SerializeField]private Image icon;
    private Image backGround;
    public Item item;
    private Color32 selectedColor = new Color32(100, 100, 100, 255);

    [SerializeField] private CraftingManager craftingManager;
    private void Awake()
    {
        button = this.GetComponent<Button>();
        backGround = this.GetComponent<Image>();

        icon.sprite = item.sprite;
        button.onClick.AddListener(GetSelect);
    }

    private void Start()
    {
        craftingManager = PlayerManager.instance.player.GetComponentInChildren<CraftingManager>();
    }

    private void SelectOutputItem()
    {
        craftingManager.AddOutputItem(item);
    }
    
    public void GetSelect()
    {
        backGround.color = selectedColor;
        SelectOutputItem();
    }

    public void GetUnselect()
    {
        backGround.color = Color.white;
    }
}
