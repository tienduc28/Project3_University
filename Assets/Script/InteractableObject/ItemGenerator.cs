using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Interactalble
{
    [SerializeField] private Item item;
    float offSet = 1.0f;

    public override void Interact(Transform player)
    {
        item.DropInWorld(this.transform);
    }

    protected override void SetHoverText()
    {
        if (item != null)
        {
            hoverText = "Generate " + item.name;
        }
    }

    protected override void SnapToGround()
    {
    }
}
