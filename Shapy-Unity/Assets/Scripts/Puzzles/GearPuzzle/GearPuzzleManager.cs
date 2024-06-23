using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class GearPuzzleManager : MonoBehaviour
{
    public static GearPuzzleManager instance;

    public int gearsCnt = 3;
    public int wrongSlotCnt = 2;

    [Header("Creation")]
    public List<GameObject> gearPrefabs;
    public GameObject leftGearPrefab;
    public GameObject rightGearPrefab;
    public GameObject gearSlotPrefab;
    public float minY = -1f;
    public float maxY = 1f;

    public float gearMinDiameter = 0.2f;
    public float gearMaxDiameter = 0.6f;


    [Header("za raycast na gearSlotove")]
    public LayerMask gearSlotLayerMask;
    public Camera cam;
    private GameObject currHoveredGearSlot;

    [Header("Sounds")]
    public AudioSource audioS;
    public AudioClip socketPlacementSound;
    public AudioClip gearSpinSound;
    public AudioClip emptyPlacementSound;
    public AudioClip invalidPlacementSound;
    public AudioClip winSound;

    private List<GearPuzzleGearSlot> instantiatedSlots = new List<GearPuzzleGearSlot>();
    private List<GearPuzzleDraggable> instantiatedDraggableGearScripts = new List<GearPuzzleDraggable>();
    private List<GearPuzzleGear> instantiatedGearScripts = new List<GearPuzzleGear>();
    private List<GameObject> instantiatedFakeSlots = new List<GameObject>();

    private GameObject leftGear;
    private GameObject rightGear;
    private GearPuzzleGearSlot gearSlotScript;

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
    private string correctSlot;
    private string currentSlot;
    private List<string> fakeSlots = new List<string>();

    public GameObject instructionsCanvas;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        instructionsCanvas.SetActive(false);
        //cam = Camera.main;
        InitializeGears();
    }

    public void StartGame()
    {
        enabled = true;
        instructionsCanvas.SetActive(true);
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        //analytics end
    }

    public void ResetGame()
    {
        enabled = false;
        instructionsCanvas.SetActive(false);
    }

    void InitializeGears()
    {
        float totalWidth = 0;
        List<float> gearDiameters = new List<float>();


        //stvaranje draggable gears
        List<float> randomDiameters = new List<float>(); // ovo rjesava da su svi gearovi vidno veci il manji jedni od drugih
        for (int i = 0; i < gearsCnt; i++)
        {
            randomDiameters.Add(gearMinDiameter + i * 0.1f);
        }
        randomDiameters = randomDiameters.OrderBy(x => UnityEngine.Random.value).ToList();


        for (int i = 0; i < gearsCnt; i++)
        {
            GameObject gear = Instantiate(gearPrefabs[UnityEngine.Random.Range(0, gearPrefabs.Count)], Vector3.zero, transform.rotation, transform);

            int randomIndex = UnityEngine.Random.Range(0, randomDiameters.Count);
            float rndDiameterGears = randomDiameters[randomIndex];
            randomDiameters.RemoveAt(randomIndex);

            gear.transform.localScale = new Vector3(rndDiameterGears, rndDiameterGears, gear.transform.localScale.z);

            GearPuzzleDraggable gearDraggableScript = gear.GetComponent<GearPuzzleDraggable>();
            gearDraggableScript.cam = cam;
            instantiatedDraggableGearScripts.Add(gearDraggableScript);
            GearPuzzleGear gearScript = gear.GetComponent<GearPuzzleGear>();
            instantiatedGearScripts.Add(gearScript);

            gearDiameters.Add(rndDiameterGears);
            totalWidth += rndDiameterGears;
        }


        //shuffle i pozicioniranje draggable gearsa
        float cumulativeWidth = 0;
        List<GearPuzzleDraggable> shuffeledGearDraggables = new List<GearPuzzleDraggable>();
        shuffeledGearDraggables = instantiatedDraggableGearScripts;
        shuffeledGearDraggables = shuffeledGearDraggables.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < shuffeledGearDraggables.Count; i++)
        {
            float gearDiameter = shuffeledGearDraggables[i].transform.localScale.x;
            float gearRadius = shuffeledGearDraggables[i].transform.localScale.x / 2;
            float gearPosX = cumulativeWidth + gearRadius;

            Vector3 gearPos = new Vector3(gearPosX - (totalWidth / 2), -1, 0) + transform.position;
            shuffeledGearDraggables[i].transform.position = gearPos;
            cumulativeWidth += gearDiameter + 0.1f;//offset
        }



        //stvaranje start geara
        float startX = totalWidth / 2;
        leftGear = Instantiate(leftGearPrefab, new Vector3(startX, 0, 0) + transform.position, transform.rotation, transform);
        GearPuzzleGear startGearScript = leftGear.GetComponent<GearPuzzleGear>();
        instantiatedGearScripts.Add(startGearScript);
        startGearScript.StartSpinning(false);
        float rndDiameterStartGear = UnityEngine.Random.Range(0.3f, 0.5f);
        leftGear.transform.localScale = new Vector3(rndDiameterStartGear, rndDiameterStartGear, leftGear.transform.localScale.z);


        //stvaranje gear slots
        Vector3 previousPosition = leftGear.transform.position;
        for (int i = 0; i < gearsCnt; i++)
        {
            float distanceBtwGears = 0;

            if (i == 0)
                distanceBtwGears = leftGear.transform.localScale.x / 2 + gearDiameters[0] / 2;
            else
                distanceBtwGears = gearDiameters[i - 1] / 2 + gearDiameters[i] / 2;

            Vector3 rndDir = new Vector3(UnityEngine.Random.Range(-0.7f, -2), UnityEngine.Random.Range(minY, maxY), 0);
            Vector3 randomDirectionGear = rndDir.normalized;
            Vector3 randomVector = randomDirectionGear * distanceBtwGears;

            GameObject slot = Instantiate(gearSlotPrefab, previousPosition + randomVector, transform.rotation, transform);
            GearPuzzleGearSlot gearSlot = slot.GetComponentInChildren<GearPuzzleGearSlot>();
            instantiatedSlots.Add(gearSlot);
            previousPosition += randomVector;
        }



        //stvaranje end geara
        float distanceBtwGearsEndGear = 0;
        float rndDiameterEndGear = UnityEngine.Random.Range(0.3f, 0.5f);
        distanceBtwGearsEndGear = rndDiameterEndGear / 2 + gearDiameters[gearsCnt - 1] / 2;

        Vector3 rndDirRGear = new Vector3(UnityEngine.Random.Range(-0.7f, -2), UnityEngine.Random.Range(minY, maxY), 0);
        Vector3 randomDirectionRGear = rndDirRGear.normalized;
        Vector3 randomVectorRGear = randomDirectionRGear * distanceBtwGearsEndGear;

        rightGear = Instantiate(rightGearPrefab, previousPosition + randomVectorRGear, transform.rotation, transform);
        rightGear.transform.localScale = new Vector3(rndDiameterEndGear, rndDiameterEndGear, rightGear.transform.localScale.z);

        GearPuzzleGear endGearScript = rightGear.GetComponent<GearPuzzleGear>();
        instantiatedGearScripts.Add(endGearScript);


        //stvaranje pogresnih gear slotova
        for (int i = 0; i < wrongSlotCnt; i++)
        {
            Vector3 tryPos = Vector3.zero;
            for (int j = 0; j < 100; j++)
            {
                Vector2 rndDir = new Vector2(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(minY, maxY));
                Vector2 randomDirectionGear = rndDir.normalized;
                Vector2 randomVector = randomDirectionGear * UnityEngine.Random.Range(0.2f, 0.5f);
                tryPos = instantiatedSlots[UnityEngine.Random.Range(1, instantiatedSlots.Count - 1)].transform.position + (Vector3)randomVector;

                Collider[] hitColliders = Physics.OverlapSphere(tryPos, 0.3f, gearSlotLayerMask);
                if (hitColliders.Length == 0)
                {
                    GameObject wrongSlot = Instantiate(gearSlotPrefab, new Vector3(0, 500, 0), Quaternion.identity);
                    instantiatedFakeSlots.Add(wrongSlot);
                    wrongSlot.name = "WrongSlot" + i;
                    wrongSlot.transform.position = tryPos;
                    //list for analytics
                    fakeSlots.Add(wrongSlot.transform.position.ToString());
                    print("opa!! valid pose " + i + " try cnt " + j);
                    break;
                }
            }
        }


        //pozicioniranje draggable gearsa (opet)
        if (rightGear.transform.position.y > leftGear.transform.position.y)
        {
            for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
            {
                shuffeledGearDraggables[i].transform.position = new Vector3(shuffeledGearDraggables[i].transform.position.x, -0.5f + transform.position.y, shuffeledGearDraggables[i].transform.position.z);
            }
        }
        else
        {
            for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
            {
                shuffeledGearDraggables[i].transform.position = new Vector3(shuffeledGearDraggables[i].transform.position.x, 0.5f + transform.position.y, shuffeledGearDraggables[i].transform.position.z);
            }
        }
        enabled = false;
    }



    //public void PickUpDraggable(GearPuzzleDraggable gearDraggable)
    //{
        
    //}

    public void DroppedDraggable(GearPuzzleDraggable draggableGear)
    {
        if (!enabled)
        {
            return;
        }
        //Analytics
        for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
        {
            if (instantiatedDraggableGearScripts[i] == draggableGear)
            {
                correctSlot = "Correct slot: " + i.ToString();
            }
        }
     
        for (int i = 0; i < instantiatedSlots.Count; i++)
        {       
            if (instantiatedSlots[i] == gearSlotScript)
            {             
                currentSlot = "Current slot: " + i.ToString();
            }    
        }

        if (gearSlotScript == null) {
            currentSlot = "Current slot: NoSlot";
        }
        else {
            for (int i = 0; i < fakeSlots.Count; i++)
            {
                if (fakeSlots[i] == gearSlotScript.transform.position.ToString())
                {
                    currentSlot = "Current slot: Fake";
                }        
            }
        }
        GearMovedAnalytics(correctSlot, currentSlot);
        //Analytics end


        // ak je povuces na prazno
        if (gearSlotScript == null)
        {
            if (draggableGear.assignedGearSlot != null)
            {
                draggableGear.assignedGearSlot.assignedGear = null;
                draggableGear.assignedGearSlot = null;
                CheckAllGearsPlaced(draggableGear);
            }
            draggableGear.gearScript.StopSpinning();

            audioS.clip = emptyPlacementSound;
            audioS.Play();
            return;
        }
        // ak je povuces na polje koje je zauzeto
        if (gearSlotScript.assignedGear != null)
        {
            audioS.clip = invalidPlacementSound;
            audioS.Play();
            draggableGear.ReturnToPosBeforePickedUp();
        }
        else
        {
            draggableGear.transform.position = gearSlotScript.transform.position;
            if (draggableGear.assignedGearSlot != null)
            {
                //ocistit gearSlotScript s kojeg je povucen
                draggableGear.assignedGearSlot.assignedGear = null;
            }

            gearSlotScript.assignedGear = draggableGear;
            draggableGear.assignedGearSlot = gearSlotScript;
            draggableGear.transform.position = gearSlotScript.transform.position;
            audioS.PlayOneShot(socketPlacementSound);

            CheckAllGearsPlaced(draggableGear);
        }
    }

    void CheckAllGearsPlaced(GearPuzzleDraggable draggableGear)
    {
        //prvo im svima zaustavit vrtnje
        for (int i = 0; i < instantiatedSlots.Count; i++)
        {
            instantiatedGearScripts[i].StopSpinning();
        }
        instantiatedGearScripts[instantiatedGearScripts.Count - 1].StopSpinning();


        //onda ludorije
        int filledSlotCnt = 0;
        for (int i = 0; i < instantiatedSlots.Count; i++)
        {
            if (instantiatedSlots[i].assignedGear != null)
            {
                if (instantiatedSlots[i].assignedGear == instantiatedDraggableGearScripts[i])
                {
                    bool lastGearRotationLeft = false;

                    if (i == 0)
                    {
                        //izcitat iz starting gear rotaciju
                        lastGearRotationLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 2].spinLeft;
                        instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);

                        if (instantiatedDraggableGearScripts[i] == draggableGear)
                        {
                            AudioClip randomSound = gearSpinSound;
                            audioS.PlayOneShot(randomSound);
                        }
                    }
                    else
                    {
                        // za sve ostale draggable gearove
                        if (instantiatedGearScripts[i - 1].isSpinning)
                        {
                            lastGearRotationLeft = !instantiatedGearScripts[i - 1].spinLeft;
                            instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);
                            

                            if (instantiatedDraggableGearScripts[i] == draggableGear)
                            {
                                AudioClip randomSound = gearSpinSound;
                                audioS.PlayOneShot(randomSound);
                            }
                        }
                    }

                    filledSlotCnt++;
                }
                else
                {
                    instantiatedGearScripts[i].StopSpinning();
                }
            }
            else
            {
                instantiatedGearScripts[i].StopSpinning();
            }
        }


        if (filledSlotCnt < gearsCnt)
        {
            return;
        }
        else
        {

            bool endGearSpinLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 3].spinLeft;
            instantiatedGearScripts[instantiatedGearScripts.Count - 1].StartSpinning(endGearSpinLeft);

            audioS.PlayOneShot(winSound);
            print("bravo winao si");

            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.gameObject);

            StartCoroutine(ResetTask());

            //Analytics
            analyticsMethods.GearsPuzzleFinished(true);
            analyticsMethods.GearsPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
            ResetCountersAndListsForAnalytics();
        }
    }

    IEnumerator ResetTask()
    {
        yield return new WaitForSeconds(2);

        for (int i = 0; i < instantiatedSlots.Count; i++)
        {
            Destroy(instantiatedSlots[i].gameObject.transform.parent.gameObject);
        }
        instantiatedSlots.Clear();

        for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
        {
            Destroy(instantiatedDraggableGearScripts[i].gameObject);
        }
        instantiatedDraggableGearScripts.Clear();

        for (int i = 0; i < instantiatedGearScripts.Count; i++)
        {
            Destroy(instantiatedGearScripts[i].gameObject);
        }
        instantiatedGearScripts.Clear();

        for (int i = 0; i < instantiatedFakeSlots.Count; i++)
        {
            Destroy(instantiatedFakeSlots[i]);
        }
        instantiatedFakeSlots.Clear();


        Destroy(leftGear.gameObject);
        Destroy(rightGear.gameObject);

        InitializeGears();
    }

    #region Raycast na gearSlotove
    void Update()
    {
        TileMouseHover();
    }
    void TileMouseHover()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15, gearSlotLayerMask))
        {
            GameObject newHoveredGearSlot = hit.collider.gameObject;

            if (newHoveredGearSlot != currHoveredGearSlot)
            {
                currHoveredGearSlot = newHoveredGearSlot;
            }
            gearSlotScript = currHoveredGearSlot.GetComponent<GearPuzzleGearSlot>();
        }
        else
        {
            if (gearSlotScript != null)
            {
                gearSlotScript = null;
            }
        }

    }
    #endregion


    #region Analytics


    private void GearMovedAnalytics(string correctSlot, string currentSlot)
    {
        string gearMoved = correctSlot + " " + currentSlot;
        timer.GetCurrentTimestamp(out currentTime);
        AddToOrderAndTimeLists(gearMoved, ConvertDateTimeToString(currentTime), currentTime);     
    }
    private void ResetCountersAndListsForAnalytics()
    {
        buttonPressOrder.Clear();
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
        //fakeSlots.Clear();
        //correctSlot=null;
        //currentSlot=null;
}

    private string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }

    private void AddToOrderAndTimeLists(string gearMoved, string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressOrder.Count;
        if (lastIndex == 0)
        {
            buttonPressOrder.Add("1. " + gearMoved);
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressOrder.Add((lastIndex + 1).ToString() + ". " + gearMoved);
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }
    #endregion
}
