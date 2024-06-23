using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeverPullingManager : MonoBehaviour
{
    public GameObject leverPrefab;
    public LayerMask leverLayerMask;
    public int leversCnt = 3;
    public float spacing = 0.5f;

    [Header("Audio")]
    public AudioSource audioS;
    public AudioClip winSound;
    public AudioClip leverPulledSound;
    public AudioClip wrongLeverSound;

    [Header("Stefan visuals")]
    public Transform hiderTransform;
    public Transform baseTransform;

    public Camera cam;
    private List<Vector3> leverSpawnPoints = new List<Vector3>();
    private List<GameObject> correctLeverSequence = new List<GameObject>();
    private int playerTryCnt;
    private bool canPullLevers;

    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private List<string> buttonPressOrder = new List<string>();
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private List<Vector3> leverSpawnPointsOriginals = new List<Vector3>();
    private int lastIndex = 0;
    private GlobalTimer timer;
    private DateTime currentTime;
    private TimeSpan timespan;



    void Start()
    {
        
        CreateLevers();
        enabled = false;
    }

    /// <summary>
    /// called in PuzzleInteractions when E is pressed new puzzle
    /// </summary>
    public void StartGame()
    {
        enabled = true;
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        //analytics end

        canPullLevers = true;
    }

    private void CreateLevers()
    {
        float startPos = -(leversCnt - 1) * spacing / 2;

        for (int i = 0; i < leversCnt; i++)
        {
            float xPos = startPos + i * spacing;
            Vector3 localLeverPosition = new Vector3(xPos, 1.77f, 0.6f);
            Vector3 worldLeverPosition = transform.TransformPoint(localLeverPosition);

            leverSpawnPoints.Add(worldLeverPosition);
            leverSpawnPointsOriginals.Add(worldLeverPosition);
        }

        leverSpawnPoints = leverSpawnPoints.OrderBy(x => System.Guid.NewGuid()).ToList();

        for (int i = 0; i < leversCnt; i++)
        {
            GameObject lever = Instantiate(leverPrefab, leverSpawnPoints[i], Quaternion.identity, transform);
            lever.transform.localEulerAngles = new Vector3(0, 180, 0);
            correctLeverSequence.Add(lever);
        }

        if (leversCnt > 4)
        {
            float lvrcnt = leversCnt - 4;
            float xBaseScale = 1 + lvrcnt * 0.2f;
            baseTransform.localScale = new Vector3(xBaseScale, baseTransform.localScale.y, baseTransform.localScale.z);
        }
    }


    private void Update()
    {
        TileMouseHover();
    }
    void TileMouseHover()
    {
        if (!canPullLevers)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10, leverLayerMask))
        {
            CheckLeverSequence(hit.collider.gameObject);
        }
    }



    private void CheckLeverSequence(GameObject lever)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //analytics
            timer.GetCurrentTimestamp(out currentTime);
            
            for (int i = 0; i < correctLeverSequence.Count; i++)
            {
                if (lever == correctLeverSequence[i])
                {               
                    Transform leverTransform = lever.transform;
                    Vector3 leverPosition = leverTransform.position;
                    for (int j = 0; j < leverSpawnPointsOriginals.Count; j++)
                    {
                        if (leverSpawnPointsOriginals[j]==leverPosition)
                        {
                            int leverNumber = j+1;
                            int orderNumber = i+1;
                            string leverMoved = leverNumber + ". lever " + "Current order: " + orderNumber;
                            
                            AddToOrderAndTimeLists(leverMoved, ConvertDateTimeToString(currentTime), currentTime);
                        }
                    }
                }
            }
            //analytics end



            Animator leverAnimator = lever.GetComponentInChildren<Animator>();

            if (lever == correctLeverSequence[playerTryCnt])
            {
                playerTryCnt++;
                audioS.PlayOneShot(leverPulledSound);
                leverAnimator.SetBool("Pulled", true);

                float hiderScalePercentage = 1 - ((float)playerTryCnt / correctLeverSequence.Count);
                hiderTransform.localScale = new Vector3(hiderScalePercentage, hiderTransform.localScale.y, hiderTransform.localScale.z);
            }
            else
            {
                playerTryCnt = 0;
                audioS.PlayOneShot(wrongLeverSound);

                hiderTransform.localScale = new Vector3(1, hiderTransform.localScale.y, hiderTransform.localScale.z);

                //levers vratit animacije na PulledUp
                for (int i = 0; i < correctLeverSequence.Count; i++)
                {
                    Animator currLeverAnimator = correctLeverSequence[i].GetComponentInChildren<Animator>();
                    currLeverAnimator.SetBool("Pulled", false);
                }

            }        
          
            // Win condition
            if (playerTryCnt >= leversCnt)
            {
                print("You Win!");
                GameManager.Instance.CheckPuzzleCompleteState(transform.parent.gameObject);
                audioS.PlayOneShot(winSound);
                StartCoroutine(Reset());

                //analytics
                analyticsMethods.LeverPullingPuzzleFinished(true);
                analyticsMethods.LeverPullingPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
                ResetCountersAndListsForAnalytics();
            }
        }
    }

    /// <summary>
    /// called from PuzzleInteractions when leaving puzzle
    /// </summary>
    public void ResetGame()
    {
        StartCoroutine(Reset());
        enabled = false;
    }
    IEnumerator Reset()
    {
        canPullLevers = false;
        yield return new WaitForSeconds(3);

        playerTryCnt = 0;
        leverSpawnPoints.Clear();

        for (int i = 0; i < correctLeverSequence.Count; i++)
            Destroy(correctLeverSequence[i]);

        correctLeverSequence.Clear();

        hiderTransform.localScale = new Vector3(1, hiderTransform.localScale.y, hiderTransform.localScale.z);

        CreateLevers();
        canPullLevers = true;
    }



    #region Analytics

    private void ResetCountersAndListsForAnalytics()
    {
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

    private void AddToOrderAndTimeLists(string leverMoved, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + leverMoved);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + leverMoved);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }
    #endregion
}

