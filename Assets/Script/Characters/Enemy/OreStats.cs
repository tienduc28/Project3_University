using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreStats : EnemyStats
{
    [SerializeField] private Tool.ToolType toolType;
    [SerializeField] private int toolLevel;
    public override void TakeDamage(GameObject damageSource, float damageTaken)
    {
        Tool usedTool = (Tool)damageSource.GetComponentInChildren<EquipmentManager>().equipmentList[(int)EquipSlot.MainHand];
        if (usedTool == null)
            return;
        Debug.Log(usedTool.toolType);

        if ((toolType == Tool.ToolType.None || toolType == usedTool.toolType) && toolLevel <= usedTool.toolLevel)
        {
            Debug.Log(usedTool.toolLevel);
            base.TakeDamage(damageSource, damageTaken);
        }
        else
        {
            Debug.Log("Wrong type of tool or tool not strong enough");
        }
    }
}
