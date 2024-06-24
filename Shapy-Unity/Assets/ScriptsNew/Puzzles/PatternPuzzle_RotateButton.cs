using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public float rotateTime = 2;
    public GameObject objectToRotateRow1, objectToRotateRow2, objectToRotateRow3, objectToRotateRow4;
    public GameObject btnRow1L, btnRow1R, btnRow2L, btnRow2R, btnRow3L, btnRow3R, btnRow4L, btnRow4R;
    public AudioSource audioS;
    public AudioClip victoryClip;

    private bool isRotateing;

    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private int counterBtnRow1L = 0, counterBtnRow1R = 0, counterBtnRow2L = 0, counterBtnRow2R = 0, counterBtnRow3L = 0, counterBtnRow3R = 0, counterBtnRow4L = 0, counterBtnRow4R = 0;
    private List<string> buttonPressOrder = new List<string>();
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private int lastIndex = 0;
    private GlobalTimer timer;
    private DateTime currentTime;
    private TimeSpan timespan;

    public GameObject instructionsCanvas;

    private void Start()
    {
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        enabled = true;
        instructionsCanvas.SetActive(true);
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        StartCoroutine(ResetGame(1));
    }

    public void ResetGame()
    {
        enabled = false;
        instructionsCanvas.SetActive(false);
    }
    void Update()
    {
        if (isRotateing)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 5);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == btnRow1L)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow1, 90f));
                    //analytics
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow1L, ConvertDateTimeToString(currentTime),currentTime);
                    counterBtnRow1L++;
                }
                else if (hit.collider.gameObject == btnRow1R)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow1, -90));
                    //analytics
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow1R, ConvertDateTimeToString(currentTime),currentTime);
                    counterBtnRow1R++;
                }
                else if (hit.collider.gameObject == btnRow2L)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow2, 90f));
                    //analytics
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow2L, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow2L++;
                }
                else if (hit.collider.gameObject == btnRow2R)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow2, -90));
                    //analytics
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow2R, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow2R++;
                }
                else if (hit.collider.gameObject == btnRow3L)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow3, 90f));
                    //analytics 
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow3L, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow3L++;
                }
                else if (hit.collider.gameObject == btnRow3R)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow3, -90));
                    //analytics 
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow3R, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow3R++;
                }
                else if (hit.collider.gameObject == btnRow4L)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow4, 90f));
                    //analytics 
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow4L, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow4L++;
                }
                else if (hit.collider.gameObject == btnRow4R)
                {
                    if (hit.collider.gameObject.TryGetComponent<Animator>(out Animator hitAnimator))
                        hitAnimator.SetTrigger("PatternPuzzle_ButtonClick_Trigger");

                    StartCoroutine(RotateOverTime(objectToRotateRow4, -90));
                    //analytics 
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(btnRow4R, ConvertDateTimeToString(currentTime), currentTime);
                    counterBtnRow4R++;
                }
            }
        }
    }

    private IEnumerator RotateOverTime(GameObject objToRotate, float angle)
    {
        isRotateing = true;

        Quaternion startRotation = objToRotate.transform.localRotation;
        //Quaternion endRotation = Quaternion.Euler(0, objToRotate.transform.localEulerAngles.y + angle, 0);
        Quaternion endRotation = Quaternion.Euler(0, NormalizeAngle(objToRotate.transform.localEulerAngles.y + angle), 0);
        float timePassed = 0f;
        audioS.Play();

        while (timePassed < rotateTime)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / rotateTime;
            t = t * t * (3f - 2f * t);

            objToRotate.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        objToRotate.transform.localRotation = endRotation;
        isRotateing = false;

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        print(objectToRotateRow1.transform.localEulerAngles.y + " " + objectToRotateRow2.transform.localEulerAngles.y + " " + objectToRotateRow3.transform.localEulerAngles.y + " " + objectToRotateRow4.transform.localEulerAngles.y);

        if (Mathf.Approximately(NormalizeAngle(objectToRotateRow1.transform.localEulerAngles.y), 0) &&
                    Mathf.Approximately(NormalizeAngle(objectToRotateRow2.transform.localEulerAngles.y), 0) &&
                    Mathf.Approximately(NormalizeAngle(objectToRotateRow3.transform.localEulerAngles.y), 0) &&
                    Mathf.Approximately(NormalizeAngle(objectToRotateRow4.transform.localEulerAngles.y), 0))
        {
            audioS.PlayOneShot(victoryClip);
            print("BRAVO MAJSTORE!");
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.parent.gameObject);

            StartCoroutine(ResetGame(1));

            //analytics
            analyticsMethods.PatternPuzzleFinished(true);
            analyticsMethods.PatternPuzzleBtnPresses(counterBtnRow1L, counterBtnRow1R, counterBtnRow2L, counterBtnRow2R, counterBtnRow3L, counterBtnRow3R, counterBtnRow4L, counterBtnRow4R);
            analyticsMethods.PatternPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count-1],buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
            ResetCountersAndListsForAnalytics();
        }
    }

    IEnumerator ResetGame(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);


        List<int> randomAngleList = new List<int>();
        for (int i = 1; i < 4; i++)
        {
            randomAngleList.Add(i * 90);
        }

        StartCoroutine(RotateAllOverTime(objectToRotateRow1, randomAngleList[UnityEngine.Random.Range(0, randomAngleList.Count)]));
        StartCoroutine(RotateAllOverTime(objectToRotateRow2, randomAngleList[UnityEngine.Random.Range(0, randomAngleList.Count)]));
        StartCoroutine(RotateAllOverTime(objectToRotateRow3, randomAngleList[UnityEngine.Random.Range(0, randomAngleList.Count)]));
        StartCoroutine(RotateAllOverTime(objectToRotateRow4, randomAngleList[UnityEngine.Random.Range(0, randomAngleList.Count)]));
    }



    private IEnumerator RotateAllOverTime(GameObject objToRotate, float angle)
    {
        Quaternion startRotation = objToRotate.transform.localRotation;
        //Quaternion endRotation = Quaternion.Euler(0, objToRotate.transform.localEulerAngles.y + angle, 0);
        Quaternion endRotation = Quaternion.Euler(0, NormalizeAngle(objToRotate.transform.localEulerAngles.y + angle), 0);
        float timePassed = 0f;
        audioS.Play();

        while (timePassed < rotateTime)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / rotateTime;
            t = t * t * (3f - 2f * t);

            objToRotate.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        objToRotate.transform.localRotation = endRotation;
    }


    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
            angle += 360;
        return angle;
    }

    #region Analytics

    private void ResetCountersAndListsForAnalytics()
    {
        counterBtnRow1L = 0;
        counterBtnRow1R = 0;
        counterBtnRow2L = 0;
        counterBtnRow2R = 0;
        counterBtnRow3L = 0;
        counterBtnRow3R = 0;
        counterBtnRow4L = 0;
        counterBtnRow4R = 0;
        buttonPressOrder.Clear();
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
    }
 
    private string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }

    private void AddToOrderAndTimeLists(GameObject gameObject, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + gameObject.name);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + gameObject.name);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }

    #endregion
}
