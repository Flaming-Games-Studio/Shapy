using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleInteractions : InteractableObject
{
    [Header("PuzzleInteractions")]
    public Camera puzzleCam;
    public GameObject puzzleGO;
    public GameObject puzzlePlaceholder;
    private bool puzzleState = true;

    public void Start()
    {
        base.Start();
    }

    public override void Interact()
    {
        print("Engaging puzzle " + gameObject.name);
        SwapState(puzzleState);
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
                UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").ApplyBindingOverride(1,("<Keyboard>/escape"));
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
            puzzleGO.BroadcastMessage("StartGame", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            puzzleGO.BroadcastMessage("ResetGame", SendMessageOptions.DontRequireReceiver);
        }

        puzzleCam.gameObject.SetActive(state);
        puzzleCam.tag = "MainCamera";
        puzzleState = !state;
    }


}
