using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastItemPlacement : MonoBehaviour
{
    public enum PlacementAnchor
    {
        [EnumMember(Value = "")]
        None = 0,
        [EnumMember(Value = "Wall")]
        Wall = 1,
        [EnumMember(Value = "Ceiling")]
        Ceiling = 2,
        [EnumMember(Value = "Floor")]
        Floor = 3
    }

    public PlacementAnchor anchor = PlacementAnchor.None;
    public LayerMask hitLayer;

    private Collider objCol;
    private ItemGhosting ig;
    private List<Collider> colliders = new List<Collider>();
    private bool itemPlaced = false;
    public InventoryItemData item;

    private void Start()
    {
        objCol = GetComponent<Collider>();
        ig = GetComponent<ItemGhosting>();

        print(anchor.ToString());
    }

    private void Update()
    {
        if (!itemPlaced)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, hitLayer))
            {
                if (/*colliders.Count == 1 && */colliders.Contains(raycastHit.collider) && raycastHit.collider.CompareTag(anchor.ToString()))
                {
                    ig.PlacingEnabled(true);
                }
                else
                {
                    ig.PlacingEnabled(false);
                }
                Vector3 newPos = Vector3.zero;
                if (transform.forward == Vector3.up) newPos = new Vector3(raycastHit.point.x, raycastHit.point.y + objCol.bounds.extents.y, raycastHit.point.z);
                if (transform.forward == Vector3.left) newPos = new Vector3(raycastHit.point.x - objCol.bounds.extents.x, raycastHit.point.y, raycastHit.point.z);
                if (transform.forward == Vector3.right) newPos = new Vector3(raycastHit.point.x + objCol.bounds.extents.x, raycastHit.point.y, raycastHit.point.z);
                if (transform.forward == Vector3.down) newPos = new Vector3(raycastHit.point.x, raycastHit.point.y - objCol.bounds.extents.y, raycastHit.point.z);
                if (transform.forward == Vector3.forward) newPos = new Vector3(raycastHit.point.x, raycastHit.point.y - objCol.bounds.extents.y, raycastHit.point.z + objCol.bounds.extents.z);
                if (transform.forward == Vector3.back) newPos = new Vector3(raycastHit.point.x, raycastHit.point.y - objCol.bounds.extents.y, raycastHit.point.z - objCol.bounds.extents.z);

                transform.position = newPos;
                transform.forward = raycastHit.normal;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other))
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other))
        {
            colliders.Remove(other);
        }
    }

    public void PlaceItem(InputAction.CallbackContext context)
    {
        if (ig.ReturnItemPlacementStatus())
        {
            itemPlaced = true;
            ig.OriginalMaterial();
            UIController.Instance.UnsubscribeItemPlacementActions();
            UIController.Instance.SubscribeUIActions();
            UIController.Instance.ToggleUIControls(false);
            ContextMenu.Instance.placementItem = null;
            //ContextMenu.Instance.SetAllListeners();
            InventoryManager.Instance.RemoveItemFromInventory(item);
            this.enabled = false;
        }
        else
        {
            print("cant place item here, check the size of the collider on your item dummy!");
        }
    }
}
