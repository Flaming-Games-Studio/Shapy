using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StoryboardManager : MonoBehaviour
{
    public GameObject paperPrefab;
    public List<StorySet> storySets;
    private StorySet currStorySet;

    public List<StoryboardSlot> slots;
    private List<StoryboardDraggable> draggableScripts;


    public float draggableLocalXMin, draggableLocalXMax;
    public float draggableLocalYMin, draggableLocalYMax;


    [Header("za raycast na gearSlotove")]
    public LayerMask gearSlotLayerMask;
    public Camera cam;
    private GameObject currHoveredSlot;
    private StoryboardSlot currHoveredSlotScript;

    [Header("Sounds")]
    public AudioSource audioS;
    public AudioClip socketPlacementSound;
    public AudioClip gearSpinSound;
    public AudioClip emptyPlacementSound;
    public AudioClip invalidPlacementSound;
    public AudioClip winSound;




    void Start()
    {
        InitializeStoryPapers();
    }

    public void StartGame()
    {
        enabled = true;
    }

    public void ResetGame()
    {
        enabled = false;
    }


    void InitializeStoryPapers()
    {
        int randomStorySet = UnityEngine.Random.Range(0, storySets.Count);
        currStorySet = storySets[randomStorySet];

        for (int i = 0; i < storySets.Count; i++)
        {
            GameObject storyPaper = Instantiate(paperPrefab, slots[i].transform.position, slots[i].transform.rotation, transform);
            StoryboardDraggable draggable = storyPaper.GetComponent<StoryboardDraggable>();
            draggable.cam = cam;
            draggable.storyboarManager = this;

            draggableScripts.Add(draggable);
            TextMeshPro storyText = storyPaper.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
            storyText.text = currStorySet.set[i].storyText;
        }
    }



    public void DroppedDraggable(StoryboardDraggable draggable)
    {
        if (!enabled)
            return;

        // ak je povuces na prazno
        if (currHoveredSlotScript == null)
        {
            if (draggable.assignedSlot != null)
            {
                draggable.assignedSlot.assignedDraggable = null;
                draggable.assignedSlot = null;
                //CheckAllStoriesPlaced(draggable);
            }

            audioS.clip = emptyPlacementSound;
            audioS.Play();
            return;
        }
        // ak je povuces na polje koje je zauzeto
        if (currHoveredSlotScript.assignedDraggable != null)
        {
            audioS.clip = invalidPlacementSound;
            audioS.Play();
            draggable.ReturnToPosBeforePickedUp();
        }
        else
        {
            draggable.transform.position = currHoveredSlotScript.transform.position;
            if (draggable.assignedSlot != null)
            {
                //ocistit gearSlotScript s kojeg je povucen
                draggable.assignedSlot.assignedDraggable = null;
            }

            currHoveredSlotScript.assignedDraggable = draggable;
            draggable.assignedSlot = currHoveredSlotScript;
            draggable.transform.position = currHoveredSlotScript.transform.position;
            audioS.PlayOneShot(socketPlacementSound);

            //CheckAllStoriesPlaced(draggable);
        }
    }

    //void CheckAllStoriesPlaced(GearPuzzleDraggable draggable)
    //{
    //    //prvo im svima zaustavit vrtnje
    //    for (int i = 0; i < instantiatedSlots.Count; i++)
    //    {
    //        instantiatedGearScripts[i].StopSpinning();
    //    }
    //    instantiatedGearScripts[instantiatedGearScripts.Count - 1].StopSpinning();


    //    //onda ludorije
    //    int filledSlotCnt = 0;
    //    for (int i = 0; i < instantiatedSlots.Count; i++)
    //    {
    //        if (instantiatedSlots[i].assignedGear != null)
    //        {
    //            if (instantiatedSlots[i].assignedGear == instantiatedDraggableGearScripts[i])
    //            {
    //                bool lastGearRotationLeft = false;

    //                if (i == 0)
    //                {
    //                    //izcitat iz starting gear rotaciju
    //                    lastGearRotationLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 2].spinLeft;
    //                    instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);

    //                    if (instantiatedDraggableGearScripts[i] == draggable)
    //                    {
    //                        AudioClip randomSound = gearSpinSound;
    //                        audioS.PlayOneShot(randomSound);
    //                    }
    //                }
    //                else
    //                {
    //                    // za sve ostale draggable gearove
    //                    if (instantiatedGearScripts[i - 1].isSpinning)
    //                    {
    //                        lastGearRotationLeft = !instantiatedGearScripts[i - 1].spinLeft;
    //                        instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);


    //                        if (instantiatedDraggableGearScripts[i] == draggable)
    //                        {
    //                            AudioClip randomSound = gearSpinSound;
    //                            audioS.PlayOneShot(randomSound);
    //                        }
    //                    }
    //                }

    //                filledSlotCnt++;
    //            }
    //            else
    //            {
    //                instantiatedGearScripts[i].StopSpinning();
    //            }
    //        }
    //        else
    //        {
    //            instantiatedGearScripts[i].StopSpinning();
    //        }
    //    }


    //    if (filledSlotCnt < gearsCnt)
    //    {
    //        return;
    //    }
    //    else
    //    {

    //        bool endGearSpinLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 3].spinLeft;
    //        instantiatedGearScripts[instantiatedGearScripts.Count - 1].StartSpinning(endGearSpinLeft);

    //        audioS.PlayOneShot(winSound);
    //        print("bravo winao si");

    //        GameManager.Instance.CheckPuzzleCompleteState(transform.parent.gameObject);

    //        StartCoroutine(ResetTask());

    //        //Analytics
    //        analyticsMethods.GearsPuzzleFinished(true);
    //        analyticsMethods.GearsPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
    //        ResetCountersAndListsForAnalytics();
    //    }
    //}

    //IEnumerator ResetTask()
    //{
    //    yield return new WaitForSeconds(2);

    //    for (int i = 0; i < instantiatedSlots.Count; i++)
    //    {
    //        Destroy(instantiatedSlots[i].gameObject.transform.parent.gameObject);
    //    }
    //    instantiatedSlots.Clear();

    //    for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
    //    {
    //        Destroy(instantiatedDraggableGearScripts[i].gameObject);
    //    }
    //    instantiatedDraggableGearScripts.Clear();

    //    for (int i = 0; i < instantiatedGearScripts.Count; i++)
    //    {
    //        Destroy(instantiatedGearScripts[i].gameObject);
    //    }
    //    instantiatedGearScripts.Clear();

    //    for (int i = 0; i < instantiatedFakeSlots.Count; i++)
    //    {
    //        Destroy(instantiatedFakeSlots[i]);
    //    }
    //    instantiatedFakeSlots.Clear();


    //    Destroy(leftGear.gameObject);
    //    Destroy(rightGear.gameObject);

    //    InitializeStoryPapers();
    //}



    void Update()
    {
        TileMouseHover();
    }
    void TileMouseHover()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10, gearSlotLayerMask))
        {
            GameObject newHoveredSlot = hit.collider.gameObject;

            if (newHoveredSlot != currHoveredSlot)
            {
                currHoveredSlot = newHoveredSlot;
                print(currHoveredSlot);
            }
            currHoveredSlotScript = currHoveredSlot.GetComponent<StoryboardSlot>();
        }
        else
        {
            if (currHoveredSlotScript != null)
            {
                currHoveredSlotScript = null;
            }
        }

    }
}

//void InitializeStoryPapers()
//{
//    for (int i = 0; i < gearsCnt; i++)
//    {
//        GameObject gear = Instantiate(gearPrefabs[UnityEngine.Random.Range(0, gearPrefabs.Count)], Vector3.zero, transform.rotation, transform);

//        int randomIndex = UnityEngine.Random.Range(0, randomDiameters.Count);
//        float rndDiameterGears = randomDiameters[randomIndex];
//        randomDiameters.RemoveAt(randomIndex);

//        gear.transform.localScale = new Vector3(rndDiameterGears, rndDiameterGears, gear.transform.localScale.z);

//        GearPuzzleDraggable gearDraggableScript = gear.GetComponent<GearPuzzleDraggable>();
//        gearDraggableScript.gearPuzzleManager = this;
//        gearDraggableScript.cam = cam;
//        gearDraggableScript.draggableLocalXMin = draggableLocalXMin;
//        gearDraggableScript.draggableLocalXMax = draggableLocalXMax;
//        gearDraggableScript.draggableLocalYMin = draggableLocalYMin;
//        gearDraggableScript.draggableLocalYMax = draggableLocalYMax;

//        instantiatedDraggableGearScripts.Add(gearDraggableScript);
//        GearPuzzleGear gearScript = gear.GetComponent<GearPuzzleGear>();
//        instantiatedGearScripts.Add(gearScript);

//        gearDiameters.Add(rndDiameterGears);
//        totalWidth += rndDiameterGears;
//    }


//    //shuffle i pozicioniranje draggable gearsa
//    float cumulativeWidth = 0;
//    List<GearPuzzleDraggable> shuffeledGearDraggables = new List<GearPuzzleDraggable>();
//    shuffeledGearDraggables = instantiatedDraggableGearScripts;
//    shuffeledGearDraggables = shuffeledGearDraggables.OrderBy(x => UnityEngine.Random.value).ToList();

//    for (int i = 0; i < shuffeledGearDraggables.Count; i++)
//    {
//        float gearDiameter = shuffeledGearDraggables[i].transform.localScale.x;
//        float gearRadius = shuffeledGearDraggables[i].transform.localScale.x / 2;
//        float gearPosX = cumulativeWidth + gearRadius;

//        Vector3 gearPos = new Vector3(gearPosX - (totalWidth / 2), -1, 0) + transform.position;
//        shuffeledGearDraggables[i].transform.position = gearPos;
//        cumulativeWidth += gearDiameter + 0.1f;//offset
//    }



//    //stvaranje start geara
//    float startX = totalWidth / 2;
//    leftGear = Instantiate(leftGearPrefab, new Vector3(startX, 0, 0) + transform.position, transform.rotation, transform);
//    GearPuzzleGear startGearScript = leftGear.GetComponent<GearPuzzleGear>();
//    instantiatedGearScripts.Add(startGearScript);
//    startGearScript.StartSpinning(false);
//    float rndDiameterStartGear = UnityEngine.Random.Range(0.3f, 0.5f);
//    leftGear.transform.localScale = new Vector3(rndDiameterStartGear, rndDiameterStartGear, leftGear.transform.localScale.z);


//    //stvaranje gear slots
//    Vector3 previousPosition = leftGear.transform.position;
//    for (int i = 0; i < gearsCnt; i++)
//    {
//        float distanceBtwGears = 0;

//        if (i == 0)
//            distanceBtwGears = leftGear.transform.localScale.x / 2 + gearDiameters[0] / 2;
//        else
//            distanceBtwGears = gearDiameters[i - 1] / 2 + gearDiameters[i] / 2;

//        Vector3 rndDir = new Vector3(UnityEngine.Random.Range(-0.7f, -2), UnityEngine.Random.Range(minY, maxY), 0);
//        Vector3 randomDirectionGear = rndDir.normalized;
//        Vector3 randomVector = randomDirectionGear * distanceBtwGears;

//        GameObject slot = Instantiate(gearSlotPrefab, previousPosition + randomVector, transform.rotation, transform);
//        GearPuzzleGearSlot gearSlot = slot.GetComponentInChildren<GearPuzzleGearSlot>();
//        instantiatedSlots.Add(gearSlot);
//        previousPosition += randomVector;
//    }



//    //stvaranje end geara
//    float distanceBtwGearsEndGear = 0;
//    float rndDiameterEndGear = UnityEngine.Random.Range(0.3f, 0.5f);
//    distanceBtwGearsEndGear = rndDiameterEndGear / 2 + gearDiameters[gearsCnt - 1] / 2;

//    Vector3 rndDirRGear = new Vector3(UnityEngine.Random.Range(-0.7f, -2), UnityEngine.Random.Range(minY, maxY), 0);
//    Vector3 randomDirectionRGear = rndDirRGear.normalized;
//    Vector3 randomVectorRGear = randomDirectionRGear * distanceBtwGearsEndGear;

//    rightGear = Instantiate(rightGearPrefab, previousPosition + randomVectorRGear, transform.rotation, transform);
//    rightGear.transform.localScale = new Vector3(rndDiameterEndGear, rndDiameterEndGear, rightGear.transform.localScale.z);

//    GearPuzzleGear endGearScript = rightGear.GetComponent<GearPuzzleGear>();
//    instantiatedGearScripts.Add(endGearScript);


//    //stvaranje pogresnih gear slotova
//    for (int i = 0; i < wrongSlotCnt; i++)
//    {
//        Vector3 tryPos = Vector3.zero;
//        for (int j = 0; j < 100; j++)
//        {
//            Vector2 rndDir = new Vector2(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(minY, maxY));
//            Vector2 randomDirectionGear = rndDir.normalized;
//            Vector2 randomVector = randomDirectionGear * UnityEngine.Random.Range(0.2f, 0.5f);
//            tryPos = instantiatedSlots[UnityEngine.Random.Range(1, instantiatedSlots.Count - 1)].transform.position + (Vector3)randomVector;

//            Collider[] hitColliders = Physics.OverlapSphere(tryPos, 0.3f, gearSlotLayerMask);
//            if (hitColliders.Length == 0)
//            {
//                GameObject wrongSlot = Instantiate(gearSlotPrefab, new Vector3(0, 500, 0), transform.rotation /*Quaternion.identity*/, transform);
//                instantiatedFakeSlots.Add(wrongSlot);
//                wrongSlot.name = "WrongSlot" + i;
//                wrongSlot.transform.position = tryPos;
//                //list for analytics
//                fakeSlots.Add(wrongSlot.transform.position.ToString());
//                print("opa!! valid pose " + i + " try cnt " + j);
//                break;
//            }
//        }
//    }


//    //pozicioniranje draggable gearsa (opet)
//    if (rightGear.transform.position.y > leftGear.transform.position.y)
//    {
//        for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
//        {
//            shuffeledGearDraggables[i].transform.position = new Vector3(shuffeledGearDraggables[i].transform.position.x, -0.5f + transform.position.y, shuffeledGearDraggables[i].transform.position.z);
//        }
//    }
//    else
//    {
//        for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
//        {
//            shuffeledGearDraggables[i].transform.position = new Vector3(shuffeledGearDraggables[i].transform.position.x, 0.5f + transform.position.y, shuffeledGearDraggables[i].transform.position.z);
//        }
//    }
//    enabled = false;
//}

//public void DroppedDraggable(GearPuzzleDraggable draggable)
//{
//    if (!enabled)
//        return;

//    // ak je povuces na prazno
//    if (gearSlotScript == null)
//    {
//        if (draggable.assignedGearSlot != null)
//        {
//            draggable.assignedGearSlot.assignedGear = null;
//            draggable.assignedGearSlot = null;
//            CheckAllStoriesPlaced(draggable);
//        }
//        draggable.gearScript.StopSpinning();

//        audioS.clip = emptyPlacementSound;
//        audioS.Play();
//        return;
//    }
//    // ak je povuces na polje koje je zauzeto
//    if (gearSlotScript.assignedGear != null)
//    {
//        audioS.clip = invalidPlacementSound;
//        audioS.Play();
//        draggable.ReturnToPosBeforePickedUp();
//    }
//    else
//    {
//        draggable.transform.position = gearSlotScript.transform.position;
//        if (draggable.assignedGearSlot != null)
//        {
//            //ocistit gearSlotScript s kojeg je povucen
//            draggable.assignedGearSlot.assignedGear = null;
//        }

//        gearSlotScript.assignedGear = draggable;
//        draggable.assignedGearSlot = gearSlotScript;
//        draggable.transform.position = gearSlotScript.transform.position;
//        audioS.PlayOneShot(socketPlacementSound);

//        CheckAllStoriesPlaced(draggable);
//    }



//}

//void CheckAllStoriesPlaced(GearPuzzleDraggable draggable)
//{
//    //prvo im svima zaustavit vrtnje
//    for (int i = 0; i < instantiatedSlots.Count; i++)
//    {
//        instantiatedGearScripts[i].StopSpinning();
//    }
//    instantiatedGearScripts[instantiatedGearScripts.Count - 1].StopSpinning();


//    //onda ludorije
//    int filledSlotCnt = 0;
//    for (int i = 0; i < instantiatedSlots.Count; i++)
//    {
//        if (instantiatedSlots[i].assignedGear != null)
//        {
//            if (instantiatedSlots[i].assignedGear == instantiatedDraggableGearScripts[i])
//            {
//                bool lastGearRotationLeft = false;

//                if (i == 0)
//                {
//                    //izcitat iz starting gear rotaciju
//                    lastGearRotationLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 2].spinLeft;
//                    instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);

//                    if (instantiatedDraggableGearScripts[i] == draggable)
//                    {
//                        AudioClip randomSound = gearSpinSound;
//                        audioS.PlayOneShot(randomSound);
//                    }
//                }
//                else
//                {
//                    // za sve ostale draggable gearove
//                    if (instantiatedGearScripts[i - 1].isSpinning)
//                    {
//                        lastGearRotationLeft = !instantiatedGearScripts[i - 1].spinLeft;
//                        instantiatedGearScripts[i].StartSpinning(lastGearRotationLeft);


//                        if (instantiatedDraggableGearScripts[i] == draggable)
//                        {
//                            AudioClip randomSound = gearSpinSound;
//                            audioS.PlayOneShot(randomSound);
//                        }
//                    }
//                }

//                filledSlotCnt++;
//            }
//            else
//            {
//                instantiatedGearScripts[i].StopSpinning();
//            }
//        }
//        else
//        {
//            instantiatedGearScripts[i].StopSpinning();
//        }
//    }


//    if (filledSlotCnt < gearsCnt)
//    {
//        return;
//    }
//    else
//    {

//        bool endGearSpinLeft = !instantiatedGearScripts[instantiatedGearScripts.Count - 3].spinLeft;
//        instantiatedGearScripts[instantiatedGearScripts.Count - 1].StartSpinning(endGearSpinLeft);

//        audioS.PlayOneShot(winSound);
//        print("bravo winao si");

//        GameManager.Instance.CheckPuzzleCompleteState(transform.parent.gameObject);

//        StartCoroutine(ResetTask());

//        //Analytics
//        analyticsMethods.GearsPuzzleFinished(true);
//        analyticsMethods.GearsPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
//        ResetCountersAndListsForAnalytics();
//    }
//}

//IEnumerator ResetTask()
//{
//    yield return new WaitForSeconds(2);

//    for (int i = 0; i < instantiatedSlots.Count; i++)
//    {
//        Destroy(instantiatedSlots[i].gameObject.transform.parent.gameObject);
//    }
//    instantiatedSlots.Clear();

//    for (int i = 0; i < instantiatedDraggableGearScripts.Count; i++)
//    {
//        Destroy(instantiatedDraggableGearScripts[i].gameObject);
//    }
//    instantiatedDraggableGearScripts.Clear();

//    for (int i = 0; i < instantiatedGearScripts.Count; i++)
//    {
//        Destroy(instantiatedGearScripts[i].gameObject);
//    }
//    instantiatedGearScripts.Clear();

//    for (int i = 0; i < instantiatedFakeSlots.Count; i++)
//    {
//        Destroy(instantiatedFakeSlots[i]);
//    }
//    instantiatedFakeSlots.Clear();


//    Destroy(leftGear.gameObject);
//    Destroy(rightGear.gameObject);

//    InitializeStoryPapers();
//}