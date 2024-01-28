    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange;
    private float lookRange = 3;
    private int layerMask;
    private int playerLayer;
    private int firstPersonRendererLayer;
    private int playerGraphic;
    private KeyCode interactKey;
    Interactalble lookingAt;

    private void Start()
    {
        interactKey = KeyCode.E;
        playerLayer = LayerMask.NameToLayer("Player");
        firstPersonRendererLayer = LayerMask.NameToLayer("FirstPersonRenderer");
        playerGraphic = LayerMask.NameToLayer("PlayerGraphic");
        layerMask = 1 << playerLayer | firstPersonRendererLayer | playerGraphic;

        layerMask = ~layerMask;
    }
    private void Update()
    {
        lookingAt = null;
        LookAt();
        if (lookingAt != null && Input.GetKeyDown(interactKey) && !InventoryUI.instance.isInventoryOpen)
            lookingAt.Interact(transform.parent);
    }

    private void LookAt()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, lookRange, layerMask))
        {
            if (true)//hit.transform.tag == "Interactable")
            {
                lookingAt = hit.transform.GetComponent<Interactalble>();
                if (lookingAt != null)
                    lookingAt.isLookedAt = true;
            }
        }
    }
}
