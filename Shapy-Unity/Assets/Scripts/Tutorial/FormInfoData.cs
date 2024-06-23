using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormInfoData : InteractableObject
{
    public TMP_InputField firstName, lastName;
    public TMP_Dropdown day, month, year;
    public TMP_InputField favFood;

    public TextMeshProUGUI names, date, food;

    private string separator = "/";

    private void OnEnable()
    {
        print("Enabling UI inputs");
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].Enable();
    }

    public override void Interact()
    {
        print("Interaction initiated " + gameObject.name);
        UIController.Instance.ToggleUIControls(true);
        UIController.Instance.UnsubscribeUIActions();
        //ContextMenu.Instance.RemoveUIListeners();
        canvasToLoad.SetActive(true);
        if (interactionWorldCanvas.activeSelf)
        {
            interactionWorldCanvas.SetActive(false);
        }
        if (col != null)
        {
            col.enabled = false;
        }
    }
    public void CollectAllData()
    {
        UIController.Instance.CloseUI(new InputAction.CallbackContext());
        names.transform.parent.transform.parent.gameObject.SetActive(true);
        names.text = firstName.text + " " + lastName.text;
        date.text = day.options[day.value].text + separator + month.options[month.value].text + separator + year.options[year.value].text;
        food.text = "Fav food: \n" + favFood.text;
        AnalyticsNumberOfCharachtersInEachField();
        UIController.Instance.SubscribeUIActions();
        //FPSController.Instance.tutorialDone = true;
    }

    private void AnalyticsNumberOfCharachtersInEachField()
    {
        AnalyticsService.Instance.CustomData("numsOfChars", new Dictionary<string, object>
        {
            { "numberOfCharsInInputField", firstName.text.Length },
        });
        AnalyticsService.Instance.CustomData("numsOfChars", new Dictionary<string, object>
        {
            { "numberOfCharsInInputField", lastName.text.Length },
        });
        AnalyticsService.Instance.CustomData("numsOfChars", new Dictionary<string, object>
        {
            { "numberOfCharsInInputField", favFood.text.Length },
        });
    }
}
