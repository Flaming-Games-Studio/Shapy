using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SwitchBalance : MonoBehaviour
{
    public GameObject[] switchesGameobjects;
    private List<int> upperSwitchValues = new List<int>();
    private List<int> lowerSwitchValues = new List<int>();
    private int upperSum;
    private int lowerSum;
    public LayerMask puzzleLayerMask;

    private List<SwitchStateControler> switchStates = new List<SwitchStateControler>();

    // UI elements
    public TextMeshProUGUI[] upperRowValues, lowerRowValues;
    public TextMeshProUGUI leftValue, rightValue;

    public Image leftSide, rightSide;
    int totalSum;
    bool enableControls = true;

    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private List<string> buttonPressOrder = new List<string>();
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private int lastIndex = 0;
    private GlobalTimer timer;
    private DateTime currentTime;
    private TimeSpan timespan;
    private string positionSwitch=null;
    private string winCombo=null;

    //sounds
    private AudioSource audioS;
    public AudioClip switchSound;
    public AudioClip winSound;

    //public GameObject instructionsCanvas;


    void Start()
    {
        audioS = GetComponent<AudioSource>();
       
        foreach (var switches in switchesGameobjects)
        {
            SwitchStateControler ssc = switches.AddComponent<SwitchStateControler>();
            switchStates.Add(ssc);
            ssc.Init();
            AudioSource switchAS = switches.GetComponent<AudioSource>();
            switchAS.clip = switchSound;
        }
        NewGame();
        enabled = false;
        //instructionsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        enabled = true;
        //instructionsCanvas.SetActive(true);
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        //analytics end
    }

    public void ResetGame()
    {
        enabled = false;
        //instructionsCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && enableControls)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 500, puzzleLayerMask))
            {
                SwitchStateControler ssc;
                hit.collider.gameObject.TryGetComponent<SwitchStateControler>(out ssc);
                if (ssc != null)
                {
                    ssc.ManageSwitchState();
                    UpdateSums();
                    print(ssc.name);
                    //analytics
                    for (int i = 0; i < switchesGameobjects.Length; i++)
                    {
                        if(hit.collider.gameObject == switchesGameobjects[i])
                        {            
                            if (ssc.IsSwitchedOn())
                            {
                                positionSwitch = "Top";
                            }
                            else
                            {
                                positionSwitch = "Bottom";
                            }
                            string switchMoved = "Switch: " + (i + 1) + " Position: " + positionSwitch;
                            timer.GetCurrentTimestamp(out currentTime);
                            AddToOrderAndTimeLists(switchMoved, ConvertDateTimeToString(currentTime), currentTime);
                        }
                    }
                    //analytics ending
                    CheckWinCondition();
                }
            }
        }
    }

    void GenerateSwitchValues()
    {
        print("Generating values...");
        System.Random rand = new System.Random();
        upperSwitchValues.Clear();
        lowerSwitchValues.Clear();

        // Generate random values for both rows
        for (int i = 0; i < switchesGameobjects.Length; i++)
        {
            int upperValue = rand.Next(1, 10);
            int lowerValue = rand.Next(1, 10);
            upperSwitchValues.Add(upperValue);
            lowerSwitchValues.Add(lowerValue);

            upperRowValues[i].text = upperValue.ToString();
            lowerRowValues[i].text = lowerValue.ToString();
        }
    }

    void UpdateSums()
    {
        print("Updating sums...");
        upperSum = 0;
        lowerSum = 0;

        for (int i = 0; i < switchStates.Count; i++)
        {
            if (switchStates[i].IsSwitchedOn())
            {
                upperSum += upperSwitchValues[i];
            }
            else
            {
                lowerSum += lowerSwitchValues[i];
            }
        }

        leftValue.text = upperSum.ToString();
        rightValue.text = lowerSum.ToString();

        if (upperSum > 0)
        {
            leftSide.fillAmount = map(upperSum, 0, upperSum + lowerSum/*GetMaxUpperValues()*/, 0, 1);
        }
        else
        {
            leftSide.fillAmount = 0;
        }
        if (lowerSum > 0)
        {
            rightSide.fillAmount = map(lowerSum, 0, upperSum + lowerSum /*GetMaxLowerValues()*/, 0, 1);
        }
        else
        {
            rightSide.fillAmount = 0;
        }
    }

    public void CheckWinCondition()
    {
        print("Checking win condition...");
        if (upperSum == lowerSum)
        {
            audioS.PlayOneShot(winSound);
            print("You Win!");
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.parent.gameObject);
            //Analytics
            analyticsMethods.SwitchesPuzzleFinished(true);
            analyticsMethods.SwitchesPuzzleTimesAndOrder(winCombo, buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
            ResetCountersAndListsForAnalytics();
            //analytics end
            
            enableControls = false;
            Invoke(nameof(NewGame), 2);
        }
    }

    void PrintWinningCombination()
    {
        // Try to find a combination that balances the sums
        int numSwitches = switchesGameobjects.Length;
        int[] switchStatesArray = new int[numSwitches];
        bool solution = false;
        for (int i = 0; i < (1 << numSwitches); i++)
        {
            for (int j = 0; j < numSwitches; j++)
            {
                switchStatesArray[j] = (i & (1 << j)) != 0 ? 1 : 0;
            }

            int tempUpperSum = 0;
            int tempLowerSum = 0;

            for (int k = 0; k < numSwitches; k++)
            {
                if (switchStatesArray[k] == 1)
                {
                    tempUpperSum += upperSwitchValues[k];
                }
                else
                {
                    tempLowerSum += lowerSwitchValues[k];
                }
            }

            if (tempUpperSum == tempLowerSum)
            {
                totalSum = tempUpperSum + tempLowerSum;
                string combination = string.Join(",", switchStatesArray);
                //analytics
                for (int whatIndex = 0; whatIndex < switchStatesArray.Length; whatIndex++)
                {
                    if (switchStatesArray[whatIndex] == 1)
                    {
                        winCombo += ", Top";
                    }
                    else
                    {
                        winCombo += ", Bottom";
                    }
                }
                //analytics end
                Debug.Log("Winning Combination: " + combination);
                solution = true;
                return;
            }
        }

        if (!solution)
        {
            Debug.Log("No winning combination found.");
            NewGame();
        }

        
    }

    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        float res = (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        return res;
    }

    public void NewGame()
    {
        print("Generating new game...");
        foreach (var item in switchStates)
        {
            item.SetToTopPosition();
        }
        enableControls = true;
        GenerateSwitchValues();
        PrintWinningCombination();
        UpdateSums();

    }

    #region Analytics

    private void ResetCountersAndListsForAnalytics()
    {
        buttonPressOrder.Clear();
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
        positionSwitch=null;
        winCombo=null;
}

    private string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }

    private void AddToOrderAndTimeLists(string switchMoved, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + switchMoved);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + switchMoved);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }
    #endregion
}

[RequireComponent(typeof(AudioSource))]
public class SwitchStateControler : MonoBehaviour
{
    private Quaternion topRotation = Quaternion.Euler(-15, 0, 0);
    private Quaternion midRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion lowRotation = Quaternion.Euler(15, 0, 0);
    private bool switchedOn;
    private Transform switchingTransform;
    public GameObject highlightTop, highlightBottom;
    private AudioSource audioS;

    public void Init()
    {
        switchingTransform = transform.GetChild(0);
        highlightTop = transform.GetChild(1).GetChild(0).gameObject;
        highlightBottom = transform.GetChild(2).GetChild(0).gameObject;
        audioS = GetComponent<AudioSource>();
        SetToTopPosition();
    }

    public void ManageSwitchState()
    {
        audioS.Play();
        if (switchedOn)
        {
            SetToLowPosition();
        }
        else
        {
            SetToTopPosition();
        }
    }

    public void SetToTopPosition()
    {
        switchingTransform.localRotation = topRotation;
        highlightBottom.SetActive(false);
        highlightTop.SetActive(true);
        switchedOn = true;
    }

    public void SetToMidPosition()
    {
        highlightBottom.SetActive(false);
        highlightTop.SetActive(false);
        switchingTransform.localRotation = midRotation;
    }

    public void SetToLowPosition()
    {
        switchingTransform.localRotation = lowRotation;
        highlightBottom.SetActive(true);
        highlightTop.SetActive(false);
        switchedOn = false;
    }

    public bool IsSwitchedOn()
    {
        return switchedOn;
    }
}
