using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ResourceDispatcherPuzzle : MonoBehaviour
{
    public static ResourceDispatcherPuzzle instance;
    public enum Difficulty { Easy, Hard }
    public Difficulty difficulty;

    [Header("TileCreation")]
    public int width = 3;
    public int height = 3;

    public GameObject tilePrefab;
    public GameObject batteryPrefab;
    public GameObject[] electronicsPrefabs;
    public GameObject electronicsPrefabHardMode;
    public Transform[] electronicsSpawnPoints;

    [Header("TileMouseHover")]
    public Camera cam;
    public LayerMask puzzleTileLayerMask;

    private GameObject currHoveredTile;
    private Renderer currTileRenderer;

    [Header("Sounds")]
    public AudioClip validPlacementSound;
    public AudioClip invalidPlacementSound;
    public AudioClip winSound;
    private AudioSource audioS;

    private GameObject[,] tilesGO;
    private ResourceDispatcherTile tileScript;
    private List<ResourceDispatcherBattery> batteryScripts = new List<ResourceDispatcherBattery>();
    private List<ResourceDispatcherDraggableManager> draggables = new List<ResourceDispatcherDraggableManager>();
    private List<List<int>> rndDraggablePowerUsageBatteries = new List<List<int>>();


    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private List<string> buttonPressOrder = new List<string>();
    private List<string> batteryColumnsMax = new List<string>();
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private int lastIndex = 0;
    private GlobalTimer timer;
    private DateTime currentTime;
    private TimeSpan timespan;

    public GameObject instructionCanvas;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        tilesGO = new GameObject[width, height];
        StartCreating(difficulty);//na battery metodama disableamo skriptu

        instructionCanvas.SetActive(false);

    }

    public void StartGame()
    {
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);

        enabled = true;
        instructionCanvas.SetActive(true);

    }

    public void ResetGame()
    {
        enabled = false;
        instructionCanvas.SetActive(false);
    }
    public void StartCreating(Difficulty difficulty)
    {
        CreateGrid();//ovo valjda treba u StartGame ili? probo sam al i dalje je sjebano kartice neke primit

        if (difficulty == Difficulty.Easy)
        {
            CreateElectronics();
            CreateBatteries();
        }
        else if (difficulty == Difficulty.Hard)
        {
            CreateElectronicsHardMode();
            CreateBatteriesHardMode();
        }
    }
    void CreateGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3((i - width / 2) * tilePrefab.transform.localScale.x, 0, (j - height / 2) * tilePrefab.transform.localScale.z);
                position += transform.position;
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.parent = this.transform;

                tilesGO[i, j] = tile;
                tile.name = tile.name + i + j;
            }
        }
    }
    void CreateElectronics()
    {
        int numOfElectronics = width * height;

        for (int i = 0; i < numOfElectronics; i++)
        {
            Vector3 rndYoffset = new Vector3(0, UnityEngine.Random.Range(-0.0001f, 0.0001f), 0);
            GameObject newElectronic = Instantiate(electronicsPrefabs[UnityEngine.Random.Range(0, electronicsPrefabs.Length)], electronicsSpawnPoints[i].position + rndYoffset, Quaternion.identity, transform);
            ResourceDispatcherDraggableManager draggable = newElectronic.GetComponent<ResourceDispatcherDraggableManager>();
            draggable.cam = cam;
            draggables.Add(draggable);
        }
    }
    void CreateBatteries()
    {
        for (int i = 0; i < width; i++)
        {
            float xPosition = (i - width / 2) * tilePrefab.transform.localScale.x;
            float zPosition = -(height / 2) * tilePrefab.transform.localScale.z - tilePrefab.transform.localScale.z;
            Vector3 position = new Vector3(xPosition, 0, zPosition);
            position += transform.position;

            GameObject battery = Instantiate(batteryPrefab, position, Quaternion.identity, transform);
            battery.name = battery.name + i;
            batteryScripts.Add(battery.GetComponent<ResourceDispatcherBattery>());
        }

        for (int i = 0; i < width; i++)
        {
            int calculatedBatteryCharge = 0;

            for (int j = 0; j < height; j++)
            {
                int index = i * height + j;
                calculatedBatteryCharge += draggables[index].batteryConsumption;
            }

            batteryScripts[i].batteryCharge = calculatedBatteryCharge;
            batteryScripts[i].batteryChargeSlider.value = (float)calculatedBatteryCharge / 100;
            batteryScripts[i].batteryUsageSlider.value = 0;
            batteryScripts[i].batteryChargeTxt.text = calculatedBatteryCharge.ToString();
            batteryScripts[i].batteryUsageTxt.text = 0.ToString();
        }
        enabled = false;
       
    }


    #region hard mode creation
    void CreateBatteriesHardMode()
    {
        for (int i = 0; i < width; i++)
        {
            float xPosition = (i - width / 2) * tilePrefab.transform.localScale.x;
            float zPosition = -(height / 2) * tilePrefab.transform.localScale.z - tilePrefab.transform.localScale.z;

            Vector3 position = new Vector3(xPosition, 0, zPosition);
            position += transform.position;
            GameObject battery = Instantiate(batteryPrefab, position, Quaternion.identity);
            battery.transform.parent = this.transform;
            battery.name = battery.name + i;
            batteryScripts.Add(battery.GetComponent<ResourceDispatcherBattery>());
        }

        for (int i = 0; i < batteryScripts.Count; i++)
        {
            int rndBatteryCharge = UnityEngine.Random.Range(20, 100);
            batteryScripts[i].batteryCharge = rndBatteryCharge;
            batteryScripts[i].batteryChargeSlider.value = (float)rndBatteryCharge / 100;
            batteryScripts[i].batteryUsageSlider.value = 0;
            batteryScripts[i].batteryChargeTxt.text = rndBatteryCharge.ToString();
            batteryScripts[i].batteryUsageTxt.text = 0.ToString();
        }
        enabled = false;
    }
    void CreateElectronicsHardMode()
    {
        int numOfElectronics = width * height;

        for (int i = 0; i < width; i++)
        {
            rndDraggablePowerUsageBatteries.Add(new List<int>());
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                rndDraggablePowerUsageBatteries[j].Add(UnityEngine.Random.Range(1, 50));
            }
        }

        List<int> tempList = new List<int>();
        for (int i = 0; i < width; i++)
        {
            tempList = NormalizeDraggablesConsumption(rndDraggablePowerUsageBatteries[i], batteryScripts[i].batteryCharge);
            rndDraggablePowerUsageBatteries[i] = tempList;
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 rndYoffset = new Vector3(0, UnityEngine.Random.Range(-0.0001f, 0.0001f), 0);
                GameObject newElectronic = Instantiate(electronicsPrefabHardMode, electronicsSpawnPoints[i].position + rndYoffset, Quaternion.identity, transform);
                ResourceDispatcherDraggableManager draggable = newElectronic.GetComponent<ResourceDispatcherDraggableManager>();
                draggable.cam = cam;

                draggables.Add(draggable);
                draggable.batteryConsumption = rndDraggablePowerUsageBatteries[i][j];
            }
        }

        //for (int i = 0; i < draggables.Count; i++)
        //{
        //    Vector3 rndYoffset = new Vector3(0, UnityEngine.Random.Range(-0.01f, 0.01f), 0);
        //    draggables[i].transform.position = electronicsSpawnPoints[i].position + rndYoffset;
        //}

        // palimo gameobject elek. aparata
        for (int i = 0; i < numOfElectronics; i++)
        {
            if (draggables[i].batteryConsumption <= 15)
            {
                int rndDraggableImg = UnityEngine.Random.Range(0, draggables[i].lowConsumptionDraggables.Length);
                draggables[i].lowConsumptionDraggables[rndDraggableImg].SetActive(true);
            }
            else if (draggables[i].batteryConsumption > 15 && draggables[i].batteryConsumption <= 30)
            {
                int rndDraggableImg = UnityEngine.Random.Range(0, draggables[i].mediumConsumptionDraggables.Length);
                draggables[i].mediumConsumptionDraggables[rndDraggableImg].SetActive(true);
            }
            else if (draggables[i].batteryConsumption > 30)
            {
                int rndDraggableImg = UnityEngine.Random.Range(0, draggables[i].highConsumptionDraggables.Length);
                draggables[i].highConsumptionDraggables[rndDraggableImg].SetActive(true);
            }
        }
        
    }

    List<int> NormalizeDraggablesConsumption(List<int> rndDraggablePowerUsage, int targetSum)
    {
        List<int> adjustedDraggablePowerUsage = new List<int>();

        int sum = 0;
        foreach (int num in rndDraggablePowerUsage)
        {
            sum += num;
        }

        float factor = (float)targetSum / sum;
        int adjustedSum = 0;
        foreach (int num in rndDraggablePowerUsage)
        {
            int adjustedNum = Mathf.RoundToInt(num * factor);
            adjustedDraggablePowerUsage.Add(adjustedNum);
            adjustedSum += adjustedNum;
        }

        // Correct rounding errors by adjusting the largest number
        int difference = targetSum - adjustedSum;
        if (difference != 0)
        {
            int maxIndex = 0;
            for (int i = 1; i < adjustedDraggablePowerUsage.Count; i++)
            {
                if (adjustedDraggablePowerUsage[i] > adjustedDraggablePowerUsage[maxIndex])
                {
                    maxIndex = i;
                }
            }
            adjustedDraggablePowerUsage[maxIndex] += difference;
        }

        return adjustedDraggablePowerUsage;

        //// Output the results
        //Debug.Log("Original numbers: " + string.Join(", ", rndDraggablePowerUsage));
        //Debug.Log("Adjusted numbers: " + string.Join(", ", adjustedDraggablePowerUsage));
        //Debug.Log("Sum: " + adjustedSum);
        //Debug.Log("difference: " + difference);
    }
    #endregion 

    void Update()
    {
        TileMouseHover();
    }

    void TileMouseHover()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10, puzzleTileLayerMask))
        {
            GameObject newHoveredTile = hit.collider.gameObject;

            if (newHoveredTile != currHoveredTile)
            {
                if (currTileRenderer != null)
                    currTileRenderer.material.color = Color.white;

                currHoveredTile = newHoveredTile;
                currTileRenderer = currHoveredTile.GetComponent<Renderer>();
                currTileRenderer.material.color = Color.green;

                tileScript = currHoveredTile.GetComponent<ResourceDispatcherTile>();
            }
        }
        else
        {
            if (currTileRenderer != null)
            {
                currTileRenderer.material.color = Color.white;
                currTileRenderer = null;
                currHoveredTile = null;
                tileScript = null;
            }
        }
    }


    #region moveing pieces

    public void DroppedDraggable(ResourceDispatcherDraggableManager dragg, int batteryConsumtion)
    {
        // ak ga ne povuces na polje neg na prazno sastrane
        if (tileScript == null)
        {
            if (dragg.assignedTile != null)
            {
                //smanjit batteryConsumption baterije sa koje si povucen
                int lastBatteryGridX = CheckHoveredTileCoordX(dragg.assignedTile);
                batteryScripts[lastBatteryGridX].batteryUsage -= dragg.batteryConsumption;
                batteryScripts[lastBatteryGridX].batteryUsageSlider.value = (float)batteryScripts[lastBatteryGridX].batteryUsage / 100;
                batteryScripts[lastBatteryGridX].batteryUsageTxt.text = batteryScripts[lastBatteryGridX].batteryUsage.ToString();

                dragg.assignedTile.assignedDraggable = null;
                dragg.assignedTile = null;
            }

            return;
        }
        // ak je povuces na polje koje je zauzeto
        if (tileScript.assignedDraggable != null)
        {
            audioS.clip = invalidPlacementSound;
            audioS.Play();
            dragg.ReturnToPosBeforePickedUp();
        }
        else
        {
            // ak je uzet sa proslog polja i ide na drugo polje
            if (dragg.assignedTile != null)
            {
                //ocistit tileScript s kojeg je povucen
                dragg.assignedTile.assignedDraggable = null;


                //smanjit batteryConsumption baterije sa koje si povucen
                int lastBatteryGridX = CheckHoveredTileCoordX(dragg.assignedTile);
                batteryScripts[lastBatteryGridX].batteryUsage -= dragg.batteryConsumption;
                batteryScripts[lastBatteryGridX].batteryUsageSlider.value = (float)batteryScripts[lastBatteryGridX].batteryUsage / 100;
                batteryScripts[lastBatteryGridX].batteryUsageTxt.text = batteryScripts[lastBatteryGridX].batteryUsage.ToString();
            }

            tileScript.assignedDraggable = dragg;
            dragg.assignedTile = tileScript;
            dragg.transform.position = tileScript.transform.position;
            audioS.clip = validPlacementSound;
            audioS.Play();
            //analytics    

            for (int i = 0; i < draggables.Count; i++) {
                draggables[i].ID = i;
                if (dragg == draggables[i])
                { 
                    int lastBatteryGridXMove = CheckHoveredTileCoordX(dragg.assignedTile);
                    string batteryMax = batteryScripts[lastBatteryGridXMove].batteryCharge.ToString();
                    if (batteryColumnsMax.Count == 0) 
                    { 
                        batteryColumnsMax.Add(batteryMax);
                    }
                    if(batteryColumnsMax.Count == 1 && batteryColumnsMax[0] != batteryMax)
                    {
                        batteryColumnsMax.Add(batteryMax);
                    }
                    if (batteryColumnsMax.Count == 2 && batteryColumnsMax[0] != batteryMax && batteryColumnsMax[1] != batteryMax)
                    {
                        batteryColumnsMax.Add(batteryMax);
                    }
                    string applianceMoved = "ID=" + draggables[i].ID.ToString() + " " + "BatteryConsumption=" + dragg.batteryConsumption.ToString() + " " + "BatteryMax=" + batteryMax;
                    timer.GetCurrentTimestamp(out currentTime);
                    AddToOrderAndTimeLists(applianceMoved, ConvertDateTimeToString(currentTime), currentTime);
                }
             }
            //analytics end
            BatteryUsageCheck(dragg, tileScript);
         }
     }  
    int CheckHoveredTileCoordX(ResourceDispatcherTile tile)
    {
        int coordX = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilesGO[i, j] == tile.gameObject)
                {
                    coordX = i;
                }
            }
        }

        return coordX;
    }

    private void BatteryUsageCheck(ResourceDispatcherDraggableManager dragg, ResourceDispatcherTile tile)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilesGO[i, j] == tile.gameObject)
                {
                    batteryScripts[i].batteryUsage += dragg.batteryConsumption;
                    batteryScripts[i].batteryUsageSlider.value = (float)batteryScripts[i].batteryUsage / 100;
                    batteryScripts[i].batteryUsageTxt.text = batteryScripts[i].batteryUsage.ToString();

                    CheckWinCondition();
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        int batteryFullCounter = 0;
        for (int i = 0; i < batteryScripts.Count; i++)
        {
            if (batteryScripts[i].batteryUsage == batteryScripts[i].batteryCharge)
            {
                batteryFullCounter++;
            }
        }

        if (batteryFullCounter == batteryScripts.Count)
        {
            audioS.clip = winSound;
            audioS.Play();
            print("you win!");
            GameManager.Instance.CheckPuzzleCompleteState(transform.parent.parent.gameObject);
            StartCoroutine(ResetTask());

            //analytics
            analyticsMethods.ResourceDispatcherPuzzleFinished(true);
            analyticsMethods.ResourceDispatcherPuzzleTimesAndOrder(buttonPressTime[buttonPressTime.Count - 1], batteryColumnsMax, buttonPressTimeRealTime, buttonPressOrder, buttonPressTime);
            ResetCountersAndListsForAnalytics();
        }
    }

    public IEnumerator ResetTask()
    {
        yield return new WaitForSeconds(2);

        //tiles
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tilesGO[i, j].GetComponent<ResourceDispatcherTile>().assignedDraggable = null;
                Destroy(tilesGO[i, j]);
            }
        }

        //batteries
        for (int i = 0; i < batteryScripts.Count; i++)
        {
            Destroy(batteryScripts[i].gameObject);
        }
        batteryScripts.Clear();

        //draggables
        for (int i = 0; i < draggables.Count; i++)
        {
            Destroy(draggables[i].gameObject);
        }
        draggables.Clear();
        rndDraggablePowerUsageBatteries.Clear();

        StartCreating(difficulty);
    }

    #endregion


    #region Analytics

    private void ResetCountersAndListsForAnalytics()
    {
    buttonPressOrder.Clear();
    lastIndex = 0;
    buttonPressTime.Clear();
    buttonPressTimeRealTime.Clear();
    batteryColumnsMax.Clear();
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