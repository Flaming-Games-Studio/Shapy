using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public Canvas inventoryCanvas, craftingCanvas;
    public ContextMenu uiContextCanvas, worldContextCanvas;
    public InputSystemUIInputModule inputModule;
    InventoryItem itemUnderMouse = null;
    public Vector2 dragPos;
    private bool dragging;

    public GlobalTimer timer = new GlobalTimer();

    private void OnDisable()
    {
        UnsubscribeUIActions();
    }

    private void OnEnable()
    {
        SubscribeUIActions();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        timer.GetCurrentTimestamp(out timer.startTime);
    }

    private void Set1()
    {
        DateTime time = DateTime.Now;
        TimeSpan ts;
        timer.CalculateTimeDifference(timer.startTime, time, out ts);
        AnalyticsService.Instance.CustomData("tutorialTotalTime", new Dictionary<string, object>
        {
            { "totalTaskTime", MathF.Truncate((float)ts.TotalSeconds * 100) / 100 },
        });
    }

    public void SubscribeUIActions()
    {
        print("Enabling UI Controls");
        inputModule.actionsAsset.actionMaps[0].Enable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").started += SelectItem;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").performed += Dragging;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragPosition").performed += context => { dragPos = context.ReadValue<Vector2>(); };
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").canceled += Dropping;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleInventory").performed += ToggleInventoryCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleCrafting").performed += ToggleCraftingCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("CloseUI").performed += CloseUI;
       
    }

    public void SubscribeUIActionsForKit()
    {
        print("Enabling UI Controls for kit UI");
        inputModule.actionsAsset.actionMaps[0].Enable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleInventory").performed += ToggleInventoryCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleCrafting").performed += ToggleCraftingCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("CloseUI").performed += CloseUI;
    }

    public void RemoveEscapeKeyBinding()
    {
        // Find the index of the ESC key binding within the action
        for (int i = 0; i < UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").bindings.Count; i++)
        {
            if (UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").bindings[i].path == "<Keyboard>/escape")
            {
                // Remove the binding
                UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").RemoveBindingOverride(i);
                Debug.Log("ESC key binding removed.");
                return; // Exit after removing the binding
            }
        }

        Debug.LogWarning("ESC key binding not found.");
    }

    public void UnsubscribeUIActions()
    {
        print("Disabling UI Controls");
        //inputModule.actionsAsset.actionMaps[0].Disable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").started -= SelectItem;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").performed -= Dragging;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragPosition").performed -= context => { dragPos = context.ReadValue<Vector2>(); };
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("DragAndMove").canceled -= Dropping;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleInventory").performed -= ToggleInventoryCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleCrafting").performed -= ToggleCraftingCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("CloseUI").performed -= CloseUI;
    }

    public void UnsubscribeUIActionsForKit()
    {
        print("Disabling UI Controls for kit UI");
        //inputModule.actionsAsset.actionMaps[0].Disable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleInventory").performed -= ToggleInventoryCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("ToggleCrafting").performed -= ToggleCraftingCanvas;
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("CloseUI").performed -= CloseUI;
    }

    public void SubscribeItemPlacementActions()
    {
        print("Enabling Placement Controls");
        inputModule.actionsAsset.actionMaps[1].Enable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[1].FindAction("Click").performed += ContextMenu.Instance.uiMenu.placementItem.PlaceItem;
    }

    public void UnsubscribeItemPlacementActions()
    {
        print("Disabling Placement Controls");
        inputModule.actionsAsset.actionMaps[1].Disable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[1].FindAction("Click").performed -= ContextMenu.Instance.uiMenu.placementItem.PlaceItem;
    }

    public void CloseUI(InputAction.CallbackContext context)
    {
        print("Closing UI");
        craftingCanvas.gameObject.SetActive(false);
        inventoryCanvas.gameObject.SetActive(false);
        ToggleUIControls(false);
    }

    /// <summary>
    /// Use false to enable movement and disable mouse cursor, use true to enable cursor and disable movement
    /// </summary>
    /// <param name="state"></param>
    public void ToggleUIControls(bool state)
    {
        print("Toggling UI Controls to " + state);
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        FPSController.Instance.canMove = !state;
        FPSController.Instance.playerCamera.gameObject.SetActive(!state);
    }

    public void ToggleUIControls()
    {
        print("Toggling UI Controls automaticly");
        Cursor.visible = Cursor.visible ? false : true;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        FPSController.Instance.canMove = !FPSController.Instance.canMove;
    }

    public void ToggleInventoryCanvas(InputAction.CallbackContext context)
    {
        print("Toggling Inventory " + inventoryCanvas.gameObject.activeSelf);
        if (!inventoryCanvas.gameObject.activeSelf)
        {
            inventoryCanvas.gameObject.SetActive(true);
            craftingCanvas.gameObject.SetActive(false);
            uiContextCanvas.gameObject.SetActive(true);
            worldContextCanvas.gameObject.SetActive(false);
            inventoryCanvas.sortingOrder = 0;
            craftingCanvas.sortingOrder = -50;
            uiContextCanvas.SetAllListeners();
            ToggleUIControls(true);
        }
        else
        {
            inventoryCanvas.gameObject.SetActive(false);
            uiContextCanvas.gameObject.SetActive(false);
            worldContextCanvas.gameObject.SetActive(true);
            uiContextCanvas.RemoveAllListeners();
            ToggleUIControls(false);
        }
    }

    public void ToggleCraftingCanvas(InputAction.CallbackContext context)
    {
        print("Toggling Crafting");
        if (!craftingCanvas.gameObject.activeSelf)
        {
            ToggleUIControls(true);
            craftingCanvas.gameObject.SetActive(true);
            inventoryCanvas.gameObject.SetActive(false);
            craftingCanvas.sortingOrder = 0;
            inventoryCanvas.sortingOrder = -50;
        }
        else
        {
            ToggleUIControls(false);
            craftingCanvas.gameObject.SetActive(false);
        }
    }

    public void SelectItem(InputAction.CallbackContext context)
    {
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
                itemUnderMouse = item;
                print("Selecting item " + item.itemData.itemName);
                item.SelectItem(context);
            }
        }
    }

    public void Dragging(InputAction.CallbackContext context)
    {
        if (itemUnderMouse == null)
        {
            return;
        }
        dragging = true;
        print("Dragging item " + itemUnderMouse.itemData.itemName);
        StartCoroutine(itemUnderMouse.Dragging());
    }

    public void Dropping(InputAction.CallbackContext context)
    {
        PointerEventData l_data = new PointerEventData(EventSystem.current);
        l_data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(l_data, results);
        InventoryTile tile = null;
        foreach (RaycastResult r in results)
        {
            r.gameObject.TryGetComponent<InventoryTile>(out tile);
            if (tile != null && itemUnderMouse != null && dragging)
            {
                print("Droping item " + itemUnderMouse.itemData.itemName);
                itemUnderMouse.Dropping(context, tile);
                itemUnderMouse = null;
            }
        }
        dragging = false;
    }
}
