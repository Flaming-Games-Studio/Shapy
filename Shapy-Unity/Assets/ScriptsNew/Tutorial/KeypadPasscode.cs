using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class KeypadPasscode : InteractableObject
{
    public string passcode;
    [Header("0,1,2... for loop")]
    public Button[] numKeyButtons;
    public Button clear, enter;

    public TextMeshProUGUI[] digits;

    GlobalTimer timer;

    TimeSpan diff;
    DateTime lastCall, currentCall;

    bool interacting;

    private UnityAction<int> addNumberAction;
    private void Start()
    {
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        
        SubscribeButtons();
        
        base.canvasToLoad.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public override void Interact()
    {
        if (!interacting)
        {
            print("Interaction initiated " + gameObject.name);
            UIController.Instance.ToggleUIControls(true);
            UIController.Instance.UnsubscribeUIActions();
            //ContextMenu.Instance.RemoveUIListeners();
            canvasToLoad.SetActive(!canvasToLoad.activeSelf);
            interactionWorldCanvas.SetActive(!interactionWorldCanvas.activeSelf);
            OverrideSubscriptionsToLeftClick();
            interacting = true;
        }
        else
        {
            print("Interaction canceled " + gameObject.name);
            UIController.Instance.ToggleUIControls(false);
            UIController.Instance.SubscribeUIActions();
            //ContextMenu.Instance.RemoveUIListeners();
            canvasToLoad.SetActive(!canvasToLoad.activeSelf);
            interactionWorldCanvas.SetActive(!interactionWorldCanvas.activeSelf);
            ReturnSubscriptions();
            interacting = false;
        }
    }

    private void OnEnable()
    {
        OverrideSubscriptionsToLeftClick();
    }

    private void OnDisable()
    {
        ReturnSubscriptions();
    }
    private void AnalyticsTotalTaskTimeCall()
    {
        DateTime time = DateTime.Now;
        TimeSpan ts;
        timer.CalculateTimeDifference(timer.startTime, time, out ts);
        AnalyticsService.Instance.CustomData("tutorialTotalTime", new Dictionary<string, object>
        {
            { "totalTaskTime", MathF.Truncate((float)ts.TotalSeconds * 100) / 100 },
        });
    }

    private void SubscribeButtons()
    {
        for (int i = 0; i < numKeyButtons.Length; i++)
        {
            int val = i;
            addNumberAction = new UnityAction<int>(AddNumberToPasscode);
            numKeyButtons[val].onClick.AddListener(() => addNumberAction.Invoke(val));
        }

        clear.onClick.AddListener(ClearPasscode);
        enter.onClick.AddListener(StartCheckCode);
    }
   
    public void AddNumberToPasscode(int digit)
    {
        for(int i = 0;i < digits.Length;i++)
        {
            if (digits[i].text.Length == 0)
            {
                digits[i].text = digit.ToString();
                return;
            }
        }
    }
    public void StartCheckCode()
    {
        print("Checking");
        StartCoroutine(CheckPasscode());
    }
    public IEnumerator CheckPasscode()
    {
        string code = "";
        for (int i = 0; i < digits.Length; i++)
        {
            code += digits[i].text;
        }
        if (code == passcode)
        {
            //win
            UIController.Instance.ToggleUIControls(false);
            //FPSController.Instance.ResetPlayerPostion();
            ReturnSubscriptions();
            yield return new WaitForSeconds(0.1f);
            //FPSController.Instance.ResetPlayerPostion();
            SceneManager.LoadScene("Home");
        }
        else
        {
            ClearPasscode();
        }
    }

    private void ReturnSubscriptions()
    {
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Click").performed -= CheckNumberUnderMouse;
        ContextMenu.Instance.SetAllListeners();
        UIController.Instance.SubscribeUIActions();
    }

    public void ClearPasscode()
    {
        print("Clear");
        for (int j = 0; j < digits.Length; j++)
        {
            digits[j].text = "";
        }
    }

    public void OverrideSubscriptionsToLeftClick()
    {
        print("Override subs");
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].Enable();
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Click").performed += CheckNumberUnderMouse;
    }

    private void CheckNumberUnderMouse(InputAction.CallbackContext context)
    {
        if (!gameObject.activeSelf)
        {
            print("not yet");
            return;
        }
        print("checking number under mouse");
        if (lastCall.Year != 0001)
        {
            UIController.Instance.timer.GetCurrentTimestamp(out currentCall);
            UIController.Instance.timer.CalculateTimeDifference(lastCall, currentCall, out diff);
            if (Mathf.Abs((float)diff.TotalMilliseconds) < 420)
            {
                print("last click " + Mathf.Abs((float)diff.TotalMilliseconds).ToString() + " ago");
                return;
            }
        }

        PointerEventData l_data = new PointerEventData(EventSystem.current);
        l_data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(l_data, results);

        Button item = null;
        foreach (RaycastResult r in results)
        {
            r.gameObject.TryGetComponent<Button>(out item);
            if (item != null)
            {
                print(item.gameObject.name);
                item.onClick.Invoke();
                UIController.Instance.timer.GetCurrentTimestamp(out lastCall);
                item = null;
            }
        }
    }
}
