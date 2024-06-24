using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodeCracking : MonoBehaviour
{
    [Header("Koji set je za win condition? \n Setovi su: \n 0(1-6) \n 1(5-0) \n 2(4-9) \n 3(3-8) \n 4(2-7) \n Vidi dial sprite sheet")]
    public int outerTargetSet = 5;
    public int innerTargetSet = 5;
    public Transform innerDial;
    public Transform outerDial;
    public Transform leftKnob, rightKnob;
    public float rotateStepDegrees = 36f; // 10 images by 360
    public float rotationSpeed = 50f; // Speed of rotation

    private bool isRotatingInner = false;                                                                                                                                                                                                                                                      
    private bool isRotatingOuter = false;
    private float outerTargetAngle = 0f;
    private float innerTargetAngle = 0f;

    [Header("Sounds")]
    private AudioSource audioS;
    public AudioClip ringMovementSound;
    public AudioClip victorySound;

    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private int CounterOuterLeft = 0, CounterOuterRight = 0, CounterInnerLeft = 0, CounterInnerRight = 0;
    private List<string> buttonPressOrder = new List<string>();
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private int lastIndex = 0;
    private GlobalTimer timer;
    private DateTime currentTime;
    private TimeSpan timespan;

    public GameObject instructionsCanvas;

    public Material[] patterns;
    public MeshRenderer[] objectsToChangeMaterialsOn;
    
    private void Start()
    {
        audioS = GetComponent<AudioSource>();
        RandomizePattern();

        instructionsCanvas.SetActive(false);
        Invoke(nameof(ResetGame), 5);
    }

    public void StartGame()
    {
        enabled = true;
        instructionsCanvas.SetActive(true);
        //analyticsw
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
    }

    public void ResetGame()
    {
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    void Update()
    {
        InputListener();
        
        if (isRotatingInner)
        {
            RotateDial(innerDial, rightKnob, ref innerTargetAngle);
        }
        if (isRotatingOuter)
        {
            RotateDial(outerDial, leftKnob, ref outerTargetAngle);
        }
    }

    private void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateOuterDial(false);
            CounterOuterLeft++;
            //analytics
            timer.GetCurrentTimestamp(out currentTime);
            AddToOrderAndTimeLists("outerDialLeft", ConvertDateTimeToString(currentTime), currentTime);       
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RotateOuterDial(true);
            CounterOuterRight++;
            //analytics
            timer.GetCurrentTimestamp(out currentTime);
            AddToOrderAndTimeLists("outerDialRight", ConvertDateTimeToString(currentTime), currentTime);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateInnerDial(false);
            CounterInnerLeft++;
            //analytics
            timer.GetCurrentTimestamp(out currentTime);
            AddToOrderAndTimeLists("innerDialLeft", ConvertDateTimeToString(currentTime), currentTime);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateInnerDial(true);
            CounterInnerRight++;
            //analytics
            timer.GetCurrentTimestamp(out currentTime);
            AddToOrderAndTimeLists("innerDialRight", ConvertDateTimeToString(currentTime), currentTime);
        }
    }
    // Rotate a specific dial towards a target angle
    void RotateDial(Transform dial, Transform knob, ref float targetAngle)
    {
        float step = rotationSpeed * Time.deltaTime;
        dial.localRotation = Quaternion.RotateTowards(dial.localRotation, Quaternion.Euler(0, targetAngle, 0), step);
        knob.localRotation = Quaternion.RotateTowards(knob.localRotation, Quaternion.Euler(0, targetAngle, 0), step);
        if (!audioS.isPlaying)
        {
            audioS.Play();
        }
        if (Quaternion.Angle(dial.localRotation, Quaternion.Euler(0, targetAngle, 0)) < 0.05f)
        {
            dial.localRotation = Quaternion.Euler(0, targetAngle, 0); // Snap to exact rotation
            knob.localRotation = Quaternion.Euler(0, targetAngle, 0);
            if (dial == innerDial)
                isRotatingInner = false;
            else
                isRotatingOuter = false;

            CheckWinCondition();
        }
    }

    /// <summary>
    /// Use true for forward, false for backwards!
    /// </summary>
    /// <param name="positive"></param>
    public void RotateInnerDial(bool positive)
    {
        if (!audioS.isPlaying)
        {
            PlayInnerSound();
        }
        if (positive)
        {
            innerTargetAngle += rotateStepDegrees;
            innerTargetAngle = innerTargetAngle % 360; // Keep the angle within 0-360 degrees
            isRotatingInner = true;
        }
        else
        {
            innerTargetAngle -= rotateStepDegrees;
            innerTargetAngle = innerTargetAngle % 360; // Keep the angle within 0-360 degrees
            isRotatingInner = true;
        }
    }

    /// <summary>
    /// Use true for forward, false for backwards!
    /// </summary>
    /// <param name="positive"></param>
    public void RotateOuterDial(bool positive)
    {
        if (!audioS.isPlaying)
        {
            PlayOuterSound();
        }
        if (positive)
        {
            outerTargetAngle += rotateStepDegrees;
            outerTargetAngle = outerTargetAngle % 360; // Keep the angle within 0-360 degrees
            isRotatingOuter = true;
        }
        else
        {
            outerTargetAngle -= rotateStepDegrees;
            outerTargetAngle = outerTargetAngle % 360; // Keep the angle within 0-360 degrees
            isRotatingOuter = true;
        }
    }

    private void CheckWinCondition()
    {
        bool[] conditions = new bool[2];
        conditions[0] = OuterCircleCorectlySet();
        conditions[1] = InnerCircleCorectlySet();
        print("Inner circle status " + conditions[1] + "\n  Outer circle status " + conditions[0]);

        if(conditions.All(x => x))
        {
            PlayVictorySound();
            print("You win, you wild beast!");
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.parent.gameObject);
            //analytics
            analyticsMethods.CodeCrackingPuzzleFinished(true);
            analyticsMethods.CodeCrackingPuzzleRotationNavigation(CounterOuterLeft, CounterOuterRight, CounterInnerLeft, CounterInnerRight);
            analyticsMethods.CodeCrackingPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
            ResetCountersAndListsForAnalytics();
            Invoke(nameof(RandomizePattern), 2);
        }
    }
    private bool InnerCircleCorectlySet()
    {
        if (innerTargetAngle >= 0)
        {
            if (innerTargetAngle == innerTargetSet * rotateStepDegrees) //inner circle forward position
            {
                return true;
            }
            else return false;
        }
        else
        {
            float revertedAngle = -180f + Mathf.Abs(innerTargetAngle);
            if (Mathf.Abs(revertedAngle) == innerTargetSet * rotateStepDegrees)
            {
                return true;
            }
            else return false;
        }
    }

    private bool OuterCircleCorectlySet()
    {
        if (outerTargetAngle >= 0)
        {
            if (outerTargetAngle == outerTargetSet * rotateStepDegrees) //inner circle forward position
            {
                return true;
            }
            else return false;
        }
        else
        {
            float revertedAngle = -180f + Mathf.Abs(outerTargetAngle);
            print("revert operation = " + (-180f + Mathf.Abs(outerTargetAngle)).ToString() + " and check target = " + (outerTargetSet * rotateStepDegrees).ToString() + " and last check = " + ((-180f + Mathf.Abs(outerTargetAngle)) == outerTargetSet * rotateStepDegrees));
            if (Mathf.Abs(revertedAngle) == outerTargetSet * rotateStepDegrees)
            {
                return true;
            }
            else return false;
        }
    }

    public void PlayInnerSound()
    {
        audioS.clip = ringMovementSound;
        audioS.pitch = 0.85f;
        audioS.PlayOneShot(ringMovementSound);
    }

    public void PlayOuterSound()
    {
        audioS.clip = ringMovementSound;
        audioS.pitch = 1.15f;
        audioS.PlayOneShot(ringMovementSound);
    }

    public void PlayVictorySound()
    {
        audioS.pitch = 1f;
        audioS.PlayOneShot(victorySound);
    }

    public void RandomizePattern()
    {
        int random = UnityEngine.Random.Range(0, patterns.Length);
        print("Random je " + random.ToString());
        foreach(MeshRenderer mr in objectsToChangeMaterialsOn)
        {
            List<Material> materials = new List<Material>();
            materials.Add(mr.materials[0]);
            materials.Add(patterns[random]);

            mr.SetMaterials(materials);
        }

        RandomizeDialPositions();
    }


    private void RandomizeDialPositions()
    {
        // Randomize initial positions of the dials to avoid 0 position
        outerTargetAngle = RandomizeAngle();
        innerTargetAngle = RandomizeAngle();

        // Set the initial rotations
        isRotatingInner = true;
        isRotatingOuter = true;
        RotateDial(outerDial, rightKnob, ref outerTargetAngle);
        RotateDial(innerDial, rightKnob, ref innerTargetAngle);
        //outerDial.localRotation = Quaternion.Euler(0, outerTargetAngle, 0);
        //innerDial.localRotation = Quaternion.Euler(0, innerTargetAngle, 0);
    }

    private float RandomizeAngle()
    {
        float angle = 0;
        while (angle == 0)
        {
            int steps = UnityEngine.Random.Range(1, 10); // 1 to 9 to avoid zero
            angle = steps * rotateStepDegrees;
        }
        return angle;
    }

    #region Analytics
    private string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }
    private void AddToOrderAndTimeLists(string ring, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + ring);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + ring);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }
    private void ResetCountersAndListsForAnalytics()
    {
        CounterInnerLeft = 0;
        CounterInnerRight = 0;
        CounterOuterLeft = 0;
        CounterOuterRight = 0;
        buttonPressOrder.Clear();
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
    }

    #endregion
}
