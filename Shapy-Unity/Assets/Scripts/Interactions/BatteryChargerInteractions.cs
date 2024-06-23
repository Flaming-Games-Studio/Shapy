using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BatteryChargerInteractions : InteractableObject
{
    private Animator[] unitAnimators;
    private Dictionary<int, List<GameObject>> unitGameobjectLists = new Dictionary<int, List<GameObject>>();
    private bool[,] slotsAvailability = new bool[3,3];
    private bool interacting;


    private void Start()
    {
        base.Start();
        GetUnitAnimators();
    }
    public override void Interact()
    {
        if (!interacting)
        {
            print("Interaction initiated " + gameObject.name + " and shit is " + ContextMenu.Instance.gameObject.name);
            UIController.Instance.ToggleUIControls();
            ContextMenu.Instance.worldMenu.SetAllListeners();
            if (canvasToLoad != null)
            {
                canvasToLoad.SetActive(!canvasToLoad.activeSelf);
            }
            if (interactionWorldCanvas != null)
            {
                interactionWorldCanvas.SetActive(!interactionWorldCanvas.activeSelf);
            }
            interacting = true;
        }
        else
        {
            print("Interaction canceled " + gameObject.name);
            UIController.Instance.ToggleUIControls(false);
            ContextMenu.Instance.worldMenu.RemoveAllListeners();
            ContextMenu.Instance.worldMenu.contextPanel.SetActive(false);

            if (canvasToLoad != null)
            {
                canvasToLoad.SetActive(!canvasToLoad.activeSelf);
            }
            if (interactionWorldCanvas != null)
            {
                interactionWorldCanvas.SetActive(!interactionWorldCanvas.activeSelf);
            }
            interacting = false;
        }
    }
    private void GetUnitAnimators()
    {
        unitAnimators = transform.GetComponentsInChildren<Animator>(true);

        for (int i = 0; i < unitAnimators.Length; i++)
        {
            unitAnimators[i].gameObject.GetComponent<BatteryAnimationState>().SetBCI(this);
            unitAnimators[i].gameObject.GetComponent<BatteryAnimationState>().SetIndex(i);
            List<GameObject> unitGameobjects = new List<GameObject>();
            unitGameobjects.Add(unitAnimators[i].transform.GetChild(0).gameObject);
            unitGameobjects.Add(unitAnimators[i].transform.GetChild(1).gameObject);
            unitGameobjects.Add(unitAnimators[i].transform.GetChild(2).gameObject);

            unitGameobjectLists.Add(i, unitGameobjects);
            ToggleUnitBatteryGameobject(i, false);
        }
    }
    public void ToggleUnitBatteryGameobject(int index, bool state)
    {
        foreach (GameObject obj in unitGameobjectLists[index])
        {
            obj.SetActive(state);
        }
    }
     
}
