using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    private EquipmentManager equipmentManager;
    private LayerMask firstPersonRender = 12;

    private void Start()
    {
        equipmentManager = PlayerManager.instance.player.GetComponentInChildren<EquipmentManager>();
        equipmentManager.mainHandChangeCallBack += ChangeHoldingItem;
    }

    private void ChangeHoldingItem(Item item)
    {
        int count = transform.childCount;
        if (count != 0)
            Destroy(transform.GetChild(0).gameObject);
        
        if (item != null)
        {
            GameObject gameObject = Instantiate(item.itemPickupGraphic, transform);
            gameObject.layer = firstPersonRender;
        }
    }

}
