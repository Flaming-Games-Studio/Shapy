using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SimonSays : MonoBehaviour
{
    public static SimonSays Instance;
    private List<int> simonSays = new List<int>();
    private List<int> playerSays = new List<int>();
    private List<BlockMovement> blocks = new List<BlockMovement>();
    private bool simon = false;

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
    private int levelReached = 0;
    [Header("5-7-9 (easy, mid, hard)")]
    public int leverRequiredToFinish = 5; //5-7-9 (easy, mid, hard)

    public GameObject instructionsCanvas;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       

        //napuni listu blokovima i dodaj im movement
        for (int i = 0; i < transform.childCount; i++)
        {
            BlockMovement bm = transform.GetChild(i).gameObject.AddComponent<BlockMovement>();
            blocks.Add(bm);
            bm.Id = i;
        }
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        enabled = true;
        instructionsCanvas.SetActive(true);
        DisableMouse();

        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        //analytics end
        Invoke(nameof(RandomizeNextStep), 1);
    }

    public void ResetGame()
    {
        playerSays.Clear();
        simonSays.Clear();
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !simon) // Check if the left mouse button was clicked
        {
            RaycastFromMouse();
        }
        if (simonSays.Count == 1) EnableMouse();
    }

    public void RandomizeNextStep()
    {
        simon = true;
        int randomStep = UnityEngine.Random.Range(0, blocks.Count - 1);
        simonSays.Add(randomStep);
        blocks[randomStep].MoveBlock();
        print(randomStep);
    }
    void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void EnableMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void RaycastFromMouse()
    {
        if (!Cursor.visible) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera to the mouse position
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Perform the raycast
        {
            Debug.Log("You clicked on: " + hit.transform.name);
            BlockMovement clickedBlock;
            hit.collider.TryGetComponent<BlockMovement>(out clickedBlock);

            if (clickedBlock == null)
            {
                Debug.Log("And it doesn't have BlockMovement component on itself.");
                return;
            }
            DisableMouse();
            clickedBlock.MoveBlock();
            playerSays.Add(clickedBlock.ReturnBlockId());
            timer.GetCurrentTimestamp(out currentTime);
            AddToOrderAndTimeLists(clickedBlock.name, ConvertDateTimeToString(currentTime), currentTime);
            //check results
            StartCoroutine(CheckWhatSimonTold());
        }
    }

    IEnumerator CheckWhatSimonTold()
    {
        bool[] result = new bool[simonSays.Count];
        for (int i = 0; i < playerSays.Count; i++)
        {
            if (simonSays.Count == 0)
            {
                playerSays.Clear();
                break;
            }
            if (simonSays[i] == playerSays[i])
            {
                result[i] = true;
                print("Good move!");
                EnableMouse();
            }
            else
            {
                result[i] = false; print("Wrong move, start again!");
                DisableMouse();
                //analytics
                analyticsMethods.SimonSaysPuzzleFinished(true);  
                analyticsMethods.SimonSaysPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], levelReached, buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
                ResetCountersAndListsForAnalytics();
                //analytics end
                yield return new WaitForSeconds(4f);
                playerSays.Clear();
                simonSays.Clear();
                RandomizeNextStep();
                EnableMouse();
                break;
            }
        }
        if (result.All(x => x))
        {
            if (simonSays.Count == 0)
            {
                playerSays.Clear();
                yield return null;
            }
            DisableMouse();
            print("So far so good!");
            levelReached++;
            CheckWinCondition();
            playerSays.Clear();
            yield return new WaitForSeconds(2f);
            StartCoroutine(SimonToldHistory());
        }
    }

    public void CheckWinCondition()
    {
        if(levelReached == leverRequiredToFinish)
        {
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.parent.parent.gameObject);
        }
        //if levelReacher > levelRequiredToFinish give smaller reward.
    }
    IEnumerator SimonToldHistory()
    {
        for (int i = 0; i < simonSays.Count; i++)
        {
            blocks[simonSays[i]].MoveBlock();
            yield return new WaitForSeconds(1.3f);
        }
        RandomizeNextStep();
        EnableMouse();
    }

    public class BlockMovement : MonoBehaviour
    {
        public float duration = 0.5f;
        public float distance = -2f;
        public int Id = 0;
        Animator animator;
        bool moving;
        AudioSource AudioSource;
        ParticleSystem particle;

        private void Start()
        {
            AudioSource = gameObject.GetComponent<AudioSource>();
            particle = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();

        }

        public int ReturnBlockId()
        { return Id; }

        public void MoveBlock()
        {
            if (moving) return;

            StartCoroutine(LerpPosition());
        }
        private IEnumerator LerpPosition()
        {
            moving = true;
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            gameObject.GetComponent<Collider>().enabled = false;

            yield return new WaitForEndOfFrame();

            animator.SetTrigger("Move");
            AudioSource.Play();
            particle.Play();

            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length * 1.1f);

            yield return new WaitForEndOfFrame();
            SimonSays.Instance.simon = false;
            gameObject.GetComponent<Collider>().enabled = true;
            moving = false;        
        }
    }

    #region Analytics
    private void ResetCountersAndListsForAnalytics()
    {
        buttonPressOrder.Clear();
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
        levelReached = 0;
    }

    private string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }

    private void AddToOrderAndTimeLists(string button, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + button);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + button);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }

    #endregion

}

