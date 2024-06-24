using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractions : InteractableObject
{
    public InventoryItemData item;
    private BatteryStatus batteryStatus;
    public GameObject[] subobjectsToTurnOff;

    public void Start()
    {
        base.Start();
        batteryStatus = GetComponent<BatteryStatus>();
    }

    public override void Interact()
    {
        print("Picking up " + gameObject.name);
        InventoryManager.Instance.AddBatteryToInventory(item, gameObject);
        batteryStatus.idle = true;
        foreach (var item in subobjectsToTurnOff)
        {
            item.SetActive(false);
        }
        col.enabled = false;
    }

}
