using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Equipment[] equipmentList;
    public SkinnedMeshRenderer[] equipmentMeshes;
    public SkinnedMeshRenderer targetMesh;
    public delegate void OnEquipmentChange(Equipment newEquipment, Equipment oldEquipment);
    public OnEquipmentChange equipmentChangeCallBack;
    public delegate void OnMainHandChange(Item item);
    public OnMainHandChange mainHandChangeCallBack;

    private void Start()
    {
        int noSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
        equipmentList = new Equipment[noSlots];
        equipmentMeshes = new SkinnedMeshRenderer[noSlots];
    }

    public void Equip(Equipment newEquipment)
    {
        //Unequip old item
        int slotIndex = (int)newEquipment.equipSlot;
        Equipment oldEquipment = Unequip(slotIndex);

        //Add to equipment list
        equipmentList[slotIndex] = newEquipment;

        //Add mesh to mesh list
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newEquipment.mesh);
        equipmentMeshes[slotIndex] = newMesh;
        newMesh.transform.parent = targetMesh.transform;
        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        SetEquipmentBlendShape(newEquipment, 100);

        //Call onEquipmentChange
        if(equipmentChangeCallBack != null)
            equipmentChangeCallBack.Invoke(newEquipment, oldEquipment);
    }

    public Equipment Unequip(int slotIndex)
    {
        Equipment oldEquipment = equipmentList[slotIndex];
        if (oldEquipment != null)
        {
            if (oldEquipment.ReturnToPlayerInventory())
            {
                if (equipmentMeshes[slotIndex] != null)
                {
                    Destroy(equipmentMeshes[slotIndex].gameObject);
                }

                SetEquipmentBlendShape(oldEquipment, 0);

                equipmentList[slotIndex] = null;

                if (equipmentChangeCallBack != null)
                    equipmentChangeCallBack.Invoke(null, oldEquipment);

                return oldEquipment;
            }
        }

        return null;
    }

    public void SetEquipmentBlendShape(Equipment equipment, int weight)
    {
        foreach (EquipmentMeshRegion meshRegion in equipment.coveredMeshRegions)
        {
            targetMesh.SetBlendShapeWeight((int)meshRegion, weight);
            Debug.Log("Mesh updated");
        }
    }

    public void EquipMainHand(Item item)
    {
        if (mainHandChangeCallBack != null)
            mainHandChangeCallBack(item);

        if (item == null)
        {
            Unequip((int)EquipSlot.MainHand);
        }
        else
        {
            Tool tool = item as Tool;
            if (tool != null)
            {
                //Debug.Log("Holding an Tool");
                Equip(tool);
            }
            else
            {
                //Debug.Log("Holding an Item");
                Tool newTool = new Tool(item);
                Equip(newTool);
            }
        }
        
    }
}
