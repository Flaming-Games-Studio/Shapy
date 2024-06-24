//using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContextMenuWorld : ContextMenu
{
    public LayerMask layerMask;

    private BatteryAnimationState battery;
    private int index;
    public override void Start()
    {
        contextPanel.SetActive(false);

        //SetAllListeners();
    }

    public override void SetAllListeners()
    {
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("RightClick").performed += ToggleContextMenu;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Click").performed += CloseContextMenuForItem;

        optionOneButton.onClick.AddListener(OptionOne);
        optionTwoButton.onClick.AddListener(OptionTwo);
        optionThreeButton.onClick.AddListener(OptionThree);
        optionFourButton.onClick.AddListener(OptionFour);
    }

    public override void RemoveAllListeners()
    {
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("RightClick").performed -= ToggleContextMenu;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Click").performed -= CloseContextMenuForItem;

        optionOneButton.onClick.RemoveAllListeners();
        optionTwoButton.onClick.RemoveAllListeners();
        optionThreeButton.onClick.RemoveAllListeners();
        optionFourButton.onClick.RemoveAllListeners();
    }

    public override void ToggleContextMenu(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, layerMask))
        {
            // Check if the ray hit a specific object type or tag
            if (raycastHit.collider.gameObject.CompareTag("ChargingUnit"))
            {
                // Get the script component attached to the collided GameObject
                BatteryAnimationState scriptComponent = raycastHit.collider.gameObject.GetComponent<BatteryAnimationState>();

                // Check if the script component is not null
                if (scriptComponent != null)
                {
                    // Now you can use methods or properties of the scriptComponent
                    print("Targeting charging unit " + scriptComponent.GetIndex());
                    contextPanel.SetActive(true);
                    battery = scriptComponent;
                    index = battery.GetIndex();
                    
                    // Get the world position of the hit point
                    Vector3 worldPosition = raycastHit.point;

                    // Convert world position to screen position
                    Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
                    contextPanel.transform.position = screenPosition;
                }
                else
                {
                    Debug.LogError("Failed to find script component on the collided object: " + raycastHit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Object with the specified tag not hit. " + raycastHit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No collision detected.");
        }

    }


    public override void OptionOne()
    {
        print("kliked option 1 on " + this.name);
        this.contextPanel.SetActive(false);

        BatteryStatus firstAvailableBattery = InventoryManager.Instance.ReturnFirstAvailableBattery();
        if (firstAvailableBattery == null)
        {
            print("No available battery in inventory!");
        }
        else
        {
            //play Loading animation
            //battery.batteryItemRef = firstAvailableBattery; //redundant?
            firstAvailableBattery.SetChargingStationInfo(battery, index);
            battery.anim.SetTrigger("LoadBattery");
            battery.occupied = true;
            battery.currentBattery = firstAvailableBattery;
            StartCoroutine(DelayedBatteryManipulation(true, 0.66f));
            firstAvailableBattery.charging = true;
            firstAvailableBattery.idle = false;
            print(firstAvailableBattery.batteryCurrentChargeValue);
            InventoryManager.Instance.UpdateBatteryQuantityTexts();
        }

        battery.GetBCI().Interact();
    }

    public override void OptionTwo()
    {
        print("kliked option 2 on " + this.name);
        this.contextPanel.SetActive(false);

        if (battery.occupied)
        {

            battery.currentBattery.charging = false;
            battery.currentBattery.idle = true;
            print(battery.currentBattery.batteryCurrentChargeValue);
            //play unload animation
            battery.anim.SetTrigger("UnloadBattery");
            AnimatorStateInfo stateInfo = battery.anim.GetCurrentAnimatorStateInfo(0);

            // Retrieve the length of the current animation
            float animationLength = stateInfo.length + 0.5f;
            StartCoroutine(DelayedBatteryManipulation(false, animationLength));
            InventoryManager.Instance.AddNewBatteryToInventory(battery.currentBattery.gameObject);
            battery.occupied = false;
        }
        else
        {
            print("Charging tube is empty cannot unload?");
        }

        //RemoveAllListeners();
        battery.GetBCI().Interact();

    }

     IEnumerator  DelayedBatteryManipulation(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        battery.GetBCI().ToggleUnitBatteryGameobject(index, state); //gasimo objekte baterije
    }

    public override void OptionThree()
    {
        print("kliked option 3 on " + this.name);
        contextPanel.SetActive(false);


        battery.GetBCI().Interact();
    }

    public override void OptionFour()
    {
        print("kliked option 4 on " + this.name);
        contextPanel.SetActive(false);

        battery.GetBCI().Interact();
    }
    public override void CloseContextMenuForItem(InputAction.CallbackContext context)
    {

        PointerEventData l_data = new PointerEventData(EventSystem.current);
        l_data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(l_data, results);

        InventoryItem item = null;
        ContextMenu cm = null;
        foreach (RaycastResult r in results)
        {
            //print(r.gameObject.name);
            r.gameObject.TryGetComponent<InventoryItem>(out item);
            if (r.gameObject.transform.parent.transform.parent != null)
            {
                r.gameObject.transform.parent.transform.parent.TryGetComponent<ContextMenu>(out cm);
            }
            if (r.gameObject.GetComponent<Button>() != null)
            {
                print("Invoking button");
                return;
            }
        }
        if (cm != null)
        {
            //print(cm.gameObject.name);
            return;
        }
        else if (item != null && selectedItem != null)
        {
            if (item.itemName == selectedItem.itemName)
            {
                return;
            }
        }

        if (cm == null && item == null)
        {
            print("Not on context menu or item with cursor");
            selectedItem = null;
            if (contextPanel != null)
            {
                contextPanel.SetActive(false);
            }
        }
    }
}
