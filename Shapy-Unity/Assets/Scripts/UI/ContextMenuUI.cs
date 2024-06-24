using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContextMenuUI : ContextMenu
{
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

    public override void OptionOne()
    {
        print("kliked option 1 on " + this.name);
        if (placementItem != null)
        {
            return;
        }
        contextPanel.SetActive(false);
        UIController.Instance.CloseUI(new InputAction.CallbackContext());

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 2.0f;       // we want 2m away from the camera position

        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);

        placementItem = Instantiate(selectedItem.itemData.itemPrefab, objectPos, Quaternion.identity).GetComponent<RaycastItemPlacement>();
        placementItem.item = selectedItem.itemData;
        RemoveAllListeners();
        UIController.Instance.UnsubscribeUIActions();
        UIController.Instance.SubscribeItemPlacementActions();
        print("Using item " + placementItem.item.itemName);
    }

    public override void OptionTwo()
    {
        contextPanel.SetActive(false);
        print("Inspecting item " + selectedItem.itemName);

    }

    public override void OptionThree()
    {
        contextPanel.SetActive(false);
        print("Dropping item " + selectedItem.itemName);

    }

    public override void OptionFour()
    {
        contextPanel.SetActive(false);
        print("Destroying item " + selectedItem.itemName);

    }

    public override void ToggleContextMenu(InputAction.CallbackContext context)
    {
        if (!gameObject.activeSelf) return;
        PointerEventData l_data = new PointerEventData(EventSystem.current);
        l_data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(l_data, results);

        InventoryItem item = null;
        foreach (RaycastResult r in results)
        {
            r.gameObject.TryGetComponent<InventoryItem>(out item);
            if (item != null)
            {
                transform.position = Input.mousePosition;
                selectedItem = item;
                break;
            }
        }
        if (item == null)
        {
            contextPanel.SetActive(false);
            transform.position = new Vector2(0, 0);
        }
        else
        {

            contextPanel.SetActive(true);
        }
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
