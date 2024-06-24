using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    public static ContextMenu Instance;
    public GameObject contextPanel;
    public Button optionOneButton, optionTwoButton, optionThreeButton, optionFourButton;
    public InventoryItem selectedItem;
    public RaycastItemPlacement placementItem = null;

    [Header("Dont touch on inheritors!")]
    public ContextMenuWorld worldMenu;
    public ContextMenuUI uiMenu;


    private void Awake()
    {
        if (Instance == null && gameObject.name == "Managers")
        {
            print("Setting static context menu!");
            Instance = this;
        }
    }
    public virtual void Start()
    {
        //contextPanel.SetActive(false);
       
        //SetAllListeners();
    }

    public virtual void ToggleContextMenu(InputAction.CallbackContext context)
    {
        print("Open some context menu");
    }

    public virtual void CloseContextMenuForItem(InputAction.CallbackContext context)
    {
        print("Close some context menu");
    }

    public virtual void SetAllListeners()
    {
        print("Setting all context menu listeners!");
    }

    public virtual void RemoveAllListeners()
    {
        print("Removing all context menu listeners!");
    }

    public virtual void OptionOne()
    {
        print("Option one on context menu sends its regards!");
    }

    public virtual void OptionTwo()
    {
        print("Option two on context menu sends its regards!");
    }

    public virtual void OptionThree()
    {
        print("Option three on context menu sends its regards!");
    }

    public virtual void OptionFour()
    {
        print("Option four on context menu sends its regards!");
    }
}

