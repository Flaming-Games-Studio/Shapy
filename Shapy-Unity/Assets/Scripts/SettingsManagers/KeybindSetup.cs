using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindSetup : MonoBehaviour
{
    public KeybindManager keybindManager;

    public Button interactButton;
    public TextMeshProUGUI interactButtonText;
    public Button useItemButton;
    public TextMeshProUGUI useItemButtonText;
    public Button inventoryButton;
    public TextMeshProUGUI inventoryButtonText;
    public Button pushToTalkButton;
    public TextMeshProUGUI pushToTalkButtonText;
    public Button menuButton;
    public TextMeshProUGUI menuButtonText;
    public Button moveForwardButton;
    public TextMeshProUGUI moveForwardButtonText;
    public Button moveBackwardsButton;
    public TextMeshProUGUI moveBackwardsButtonText;
    public Button moveLeftButton;
    public TextMeshProUGUI moveLeftButtonText;
    public Button moveRightButton;
    public TextMeshProUGUI moveRightButtonText;
    public Button chatButton;
    public TextMeshProUGUI chatButtonText;



    void Start()
    {
        foreach (var keybind in keybindManager.keybinds)
        {
            if (keybind.actionName == "Interact")
            {
                keybind.button = interactButton;
                keybind.buttonText = interactButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "UseItem")
            {
                keybind.button = useItemButton;
                keybind.buttonText = useItemButtonText;
               // keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "Inventory")
            {
                keybind.button = inventoryButton;
                keybind.buttonText = inventoryButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "Chat")
            {
                keybind.button = chatButton;
                keybind.buttonText = chatButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "PushToTalk")
            {
                keybind.button = pushToTalkButton;
                keybind.buttonText = pushToTalkButtonText;
               // keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "Menu")
            {
                keybind.button = menuButton;
                keybind.buttonText = menuButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));
            }
            if (keybind.actionName == "MoveForward")
            {
                keybind.button = moveForwardButton;
                keybind.buttonText = moveForwardButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));

            }
            if (keybind.actionName == "MoveBackwards")
            {
                keybind.button = moveBackwardsButton;
                keybind.buttonText = moveBackwardsButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));

            }
            if (keybind.actionName == "MoveLeft")
            {
                keybind.button = moveLeftButton;
                keybind.buttonText = moveLeftButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));

            }
            if (keybind.actionName == "MoveRight")
            {
                keybind.button = moveRightButton;
                keybind.buttonText = moveRightButtonText;
                //keybind.button.onClick.AddListener(() => keybindManager.StartRebind(keybind));

            }
        }
    }
}