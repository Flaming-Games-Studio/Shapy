using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoryboardInteractions : InteractableObject
{
    [Header("StoryboardInteractions")]
    public Camera boardCam;
    
    private bool active = true;
    public override void Interact()
    {
        SwapState(active);
    }

    public void SwapState(bool state)
    {
        puzzle = state;
        interactionWorldCanvas.SetActive(!state);
        if (state)
        {
            UIController.Instance.UnsubscribeUIActions();
            if (UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").bindings.Count < 2)
            {
                UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").AddBinding(("<Keyboard>/escape"));
            }
            else
            {
                UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").ApplyBindingOverride(1, ("<Keyboard>/escape"));
            }
        }
        else
        {
            UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").ApplyBindingOverride(1, "");
            UIController.Instance.SubscribeUIActions();
        }

        UIController.Instance.ToggleUIControls(state);

        if (state)
        {
            BroadcastMessage("StartInteraction", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            BroadcastMessage("EndInteraction", SendMessageOptions.DontRequireReceiver);
        }

        boardCam.gameObject.SetActive(state);
        boardCam.tag = "MainCamera";
        active = !state;
    }

}
