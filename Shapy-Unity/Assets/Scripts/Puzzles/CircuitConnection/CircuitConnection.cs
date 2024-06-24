using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class CircuitConnection : MonoBehaviour
{
    public static CircuitConnection Instance;
    public TextAsset[] patterns;

    private int[,] gridArray;
    private List<ConnectorType> gridElements = new List<ConnectorType>();
    private List<Vector3> freeCells = new List<Vector3>();
    private List<Vector3> patternCells = new List<Vector3>();
    public List<CellInfo> cellsInfo = new List<CellInfo>();

    public GameObject startMarker;
    public GameObject finishMarker;
    public GameObject connectorMarker;
    public GameObject cornerMarker_UP_LEFT;
    public GameObject cornerMarker_UP_RIGHT;
    public GameObject cornerMarker_DOWN_LEFT;
    public GameObject cornerMarker_DOWN_RIGHT;
    public GameObject straightMarker_HOR;
    public GameObject straightMarker_VER;
    public GameObject block2x2Marker;
    public GameObject block1x1Marker;
    public GameObject block2x1Marker;

    private List<Transform> patternTransforms = new List<Transform>();
    public List<Transform> patternAnchors;
    public LayerMask puzzleLayerMask;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip correctMove;
    public AudioClip wrongMove;
    public AudioClip circuitComplete;

    //analytics
    [Header("Analytics")]
    public AnalyticsMethods analyticsMethods;
    private List<string> buttonPressTime = new List<string>();
    private List<DateTime> buttonPressTimeRealTime = new List<DateTime>();
    private int lastIndex = 0;
    public GlobalTimer timer;
    public DateTime currentTime;
    private TimeSpan timespan;
    [HideInInspector]
    public int analyticsCorrectMoves = 0;
    [HideInInspector]
    public int analyticsIncorrectMoves= 0;
    [HideInInspector]
    public bool isCorrect = false;
 


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else return;
    }

    void Start()
    {
        //analytics
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
        //analytics end
        audioSource = GetComponent<AudioSource>();
        LoadCSV();
    }

    void LoadCSV()
    {
        if (patterns != null)
        {
            int ran = Random.Range(0, patterns.Length);
            string csvText = patterns[ran].text;
            print(patterns[ran].name);

            string[] rowsDirty = csvText.Split('\n');
            string[] columns = rowsDirty[0].Split(',');
            List<string> cc = rowsDirty.ToList<string>();
            cc.Remove("");
            string[] rows = cc.ToArray();
            gridArray = new int[rows.Length, columns.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] values = rows[i].Split(',');

                for (int j = 0; j < values.Length; j++)
                {
                    if (int.TryParse(values[j], out int result))
                    {
                        gridArray[i, j] = result;
                        print("Loading cell " + i.ToString() + "," + j.ToString() + " - with value of " + result.ToString());
                    }
                    else
                    {
                        Debug.LogWarning("Error parsing CSV value at position (" + i + ", " + j + ")");
                    }
                }
            }
            Debug.Log("CSV Loaded Successfully");

            CreateGrid();
        }
    }

    void CreateGrid()
    {
        float xCoord = 3.5f;
        float zCoord = -3.5f;
        for (int j = 0; j < gridArray.GetLength(1); j++)
        {
            for (int i = gridArray.GetLength(0) - 1; i >= 0; i--)
            {
                Vector3 newCell = new Vector3(xCoord - i, 0, zCoord + j);
                freeCells.Add(newCell);
                print("Creating new cell at:" + newCell.x.ToString() + "," + newCell.y.ToString());
            }
        }
        SpawnCircuitElements();
    }

    private void SpawnCircuitElements()
    {
        int counter = 0;
        for (int i = gridArray.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                switch (gridArray[i, j])
                {
                    default:
                        print("Ended in default gateway, find your way out!");
                        break;
                    //0 - START
                    case 0:
                        StartCoroutine(CreateElement(counter, startMarker, ConnectorType.START));
                        break;
                    //1 - FINISH
                    case 1:
                        StartCoroutine(CreateElement(counter, finishMarker, ConnectorType.FINISH));
                        break;
                    //2 - STRAIGHT_VERTICAL
                    case 2:
                        gridElements.Add(ConnectorType.STRAIGHT_VERTICAL);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, straightMarker, ConnectorType.STRAIGHT));
                        break;
                    //3 - STRAIGHT_HORIZONTAL
                    case 3:
                        gridElements.Add(ConnectorType.STRAIGHT_HORIZONTAL);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, straightMarker, ConnectorType.STRAIGHT));
                        break;
                    //4 - ONE_BY_ONE
                    case 4:
                        StartCoroutine(CreateElement(counter, block1x1Marker, ConnectorType.ONE_BY_ONE));
                        break;
                    //5 - CORNER_UP_LEFT
                    case 5:
                        gridElements.Add(ConnectorType.CORNER_UP_LEFT);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, cornerMarker, ConnectorType.CORNER));
                        break;
                    //6 - CORNER_UP_RIGHT
                    case 6:
                        gridElements.Add(ConnectorType.CORNER_UP_RIGHT);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, cornerMarker, ConnectorType.CORNER));
                        break;
                    //7 - CORNER_DOWN_LEFT
                    case 7:
                        gridElements.Add(ConnectorType.CORNER_DOWN_LEFT);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, cornerMarker, ConnectorType.CORNER));
                        break;
                    //8 - CORNER_DOWN_RIGHT
                    case 8:
                        gridElements.Add(ConnectorType.CORNER_DOWN_RIGHT);
                        patternCells.Add(freeCells[counter]);
                        //StartCoroutine(CreateElement(counter, cornerMarker, ConnectorType.CORNER));
                        break;
                    //99 - EMPTY
                    case 99:
                        print("Execute order 66!");
                        break;
                }
                counter++;
            }
        }

        CreateSolutionPatterns();

    }

    public IEnumerator CreateElement(int slot, GameObject element, ConnectorType type)
    {
        if (freeCells[slot] == null) 
        {
            print("freeCells slot " + slot.ToString() + " is already filled or invalid?");
            yield break; 
        }
        GameObject obj = Instantiate(element, this.transform);
        yield return new WaitForEndOfFrame();
        obj.transform.localPosition = freeCells[slot];
    }

    public enum ConnectorType
    {
        START = 0,
        FINISH = 1,
        STRAIGHT_VERTICAL = 2,
        STRAIGHT_HORIZONTAL = 3,
        ONE_BY_ONE = 4,
        CORNER_UP_LEFT = 5,
        CORNER_UP_RIGHT = 6,
        CORNER_DOWN_LEFT = 7,
        CORNER_DOWN_RIGHT = 8,
        EMPTY = 99

    }

    public void CreateSolutionPatterns()
    {
        for (int i = 0; i < 10; i++)
        {
            if (gridElements.Count == 0)
            {
                break;
            }
            GameObject tmp = new GameObject("PatternSolutionPiece");
            tmp.transform.SetParent(this.transform);
            //tmp.transform.localScale = new Vector3(0.065532f, 0.065532f, 0.065532f);

            int minHelper = gridElements.Count < 2 ? gridElements.Count : 2;
            int maxHelper = Mathf.Min(gridElements.Count, 3) + 1;
            int helper = Random.Range(minHelper, maxHelper);

            List<Vector3> currentPatternCells = new List<Vector3>();

            for (int x = 0; x < helper; x++)
            {
                if (x == 0 || AreAdjacent(currentPatternCells.Last(), patternCells[0]))
                {
                    GameObject subGo = null;
                    switch (gridElements[0])
                    {
                        default:
                            print("Switch default=?!");
                            break;
                        case ConnectorType.CORNER_UP_LEFT:
                            subGo = Instantiate(cornerMarker_UP_LEFT, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                        case ConnectorType.CORNER_UP_RIGHT:
                            subGo = Instantiate(cornerMarker_UP_RIGHT, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                        case ConnectorType.CORNER_DOWN_LEFT:
                            subGo = Instantiate(cornerMarker_DOWN_LEFT, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                        case ConnectorType.CORNER_DOWN_RIGHT:
                            subGo = Instantiate(cornerMarker_DOWN_RIGHT, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                        case ConnectorType.STRAIGHT_VERTICAL:
                            subGo = Instantiate(straightMarker_VER, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                        case ConnectorType.STRAIGHT_HORIZONTAL:
                            subGo = Instantiate(straightMarker_HOR, tmp.transform);
                            subGo.transform.position = patternCells[0];
                            break;
                    }
                    CellInfo ci = subGo.AddComponent<CellInfo>();
                    cellsInfo.Add(ci);
                    ci.designatedPosition = patternCells[0];
                    currentPatternCells.Add(patternCells[0]);
                    gridElements.RemoveAt(0);
                    patternCells.RemoveAt(0);
                }
                else
                {
                    break; // Stop if the next tile is not adjacent
                }
            }
            if (tmp.transform.childCount == 0)
            {
                Destroy(tmp);
            }
            RepositionParentPivot(tmp);
        }
        RandomizePatternPositions();
    }

    bool AreAdjacent(Vector3 a, Vector3 b)
    {
        return (Mathf.Abs(a.x - b.x) == 1 && a.z == b.z) || (Mathf.Abs(a.z - b.z) == 1 && a.x == b.x);
    }

    public void RandomizePatternPositions()
    {
        for (int i = 0; i < patternTransforms.Count; i++)
        {
            int r = Random.Range(0, patternAnchors.Count);
            patternTransforms[i].position = patternAnchors[r].position;
            patternAnchors.RemoveAt(r);

            foreach (Transform t in patternTransforms[i].GetChild(0))
            {
                BoxCollider bc = t.AddComponent<BoxCollider>();
                bc.size = new Vector3(1, 0.5f, 1);
            }

            patternTransforms[i].GetChild(0).AddComponent<Draggable>();

            int layerIndex = LayerMask.NameToLayer("PuzzleTile");

            if (layerIndex == -1)
            {
                Debug.LogError("Layer \"" + "PuzzleTile" + "\" does not exist.");
                return;
            }

            foreach (Transform t in patternTransforms[i].GetChild(0))
            {
                t.gameObject.layer = layerIndex;
            }
        }
    }

    void RepositionParentPivot(GameObject parentObject)
    {
        if (parentObject == null || parentObject.transform.childCount == 0)
        {
            Debug.LogWarning("Parent object is not set or has no children.");
            return;
        }
        Vector3 newPivotPosition = Vector3.zero;
        foreach (Transform child in parentObject.transform)
        {
            newPivotPosition += child.position;
        }
        newPivotPosition /= parentObject.transform.childCount;

        GameObject newParent = new GameObject("WirePiece");
        newParent.transform.position = newPivotPosition;
        newParent.transform.SetParent(this.transform);

        parentObject.transform.SetParent(newParent.transform);
        
        
        patternTransforms.Add(newParent.transform);
    }

    public void CheckWinCondition()
    {
        bool[] results = new bool[cellsInfo.Count];
        for (int i = 0; i < cellsInfo.Count; i++)
        {
            results[i] = cellsInfo[i].inPlace;
        }
        if (results.All(x=>x))
        {
            audioSource.PlayOneShot(circuitComplete);
            transform.Find("Power_End(Clone)").GetComponent<Light>().enabled = true;
            print("You win!");
            //Analytics
            analyticsMethods.WiresPuzzleFinished(true);
            analyticsMethods.WiresPuzzleTimesAndOrder(CircuitConnection.Instance.analyticsCorrectMoves, CircuitConnection.Instance.analyticsIncorrectMoves, buttonPressTime[buttonPressTime.Count - 1], buttonPressTimeRealTime, buttonPressTime);
            //print("tocan " + analyticsCorrectMoves);
            //print("netocan " + analyticsIncorrectMoves);
            ResetCountersAndListsForAnalytics();
            // analytics end
            Invoke(nameof(RestartScene), 2);
        }
        else
        {
            print("Circuit is still not complete!");
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #region Analytics

    private void ResetCountersAndListsForAnalytics()
    {
        lastIndex = 0;
        buttonPressTime.Clear();
        buttonPressTimeRealTime.Clear();
        analyticsCorrectMoves = 0;
        analyticsIncorrectMoves = 0;
    }

    public string ConvertDateTimeToString(DateTime timeToConvert)
    {
        //calculating time difference so that time show time passed after instance started instead of realtime
        timer.CalculateTimeDifference(timer.startTime, currentTime, out timespan);
        string convertedTime = timespan.Hours.ToString() + ":" + timespan.Minutes.ToString() + ":" + timespan.Seconds.ToString() + ":" + timespan.Milliseconds.ToString();
        return convertedTime;
    }

    public void AddToOrderAndTimeLists(string timeOfPress, DateTime currentTimeSent)
    {
        lastIndex = buttonPressTime.Count;
        if (lastIndex == 0)
        {
            buttonPressTime.Add("1. " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
        else
        {
            buttonPressTime.Add((lastIndex + 1).ToString() + ". " + timeOfPress);
            buttonPressTimeRealTime.Add(currentTimeSent);
        }
    }
    #endregion
}




public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector3 startPosition;
    private Transform originalParent;
    private Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("Begin drag");
        startPosition = transform.localPosition;
        originalParent = transform.parent;

        // Calculate the offset between the object's position and the mouse position
        Plane plane = new Plane(Vector3.up, startPosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        plane.Raycast(ray, out distance);
        offset = startPosition - ray.GetPoint(distance);
        // Optionally, set the parent to the root to avoid nested dragging issues
        transform.SetParent(transform.root, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("Dragging...");
        Plane plane = new Plane(Vector3.up, startPosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            transform.localPosition = ray.GetPoint(distance) + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("Released grab!");
        SnapToGrid();
        transform.SetParent(originalParent, true);

        // Check if within grid bounds
        if (IsWithinGridBounds(transform.localPosition))
        {
            gameObject.BroadcastMessage("CheckCellPosition", SendMessageOptions.DontRequireReceiver);
            //analytics
            if (CircuitConnection.Instance.isCorrect == true)
            {
                CircuitConnection.Instance.analyticsCorrectMoves++;
            }
            else
            {
                CircuitConnection.Instance.analyticsIncorrectMoves++;
            }      
            CircuitConnection.Instance.timer.GetCurrentTimestamp(out CircuitConnection.Instance.currentTime);
            CircuitConnection.Instance.AddToOrderAndTimeLists(CircuitConnection.Instance.ConvertDateTimeToString(CircuitConnection.Instance.currentTime), CircuitConnection.Instance.currentTime);
            //end analytics
            CircuitConnection.Instance.gameObject.BroadcastMessage("CheckWinCondition", SendMessageOptions.DontRequireReceiver);

        }
    }

    private void SnapToGrid()
    {
        float gridSize = 0.065532f; // Assuming grid size is 1 unit
        Vector3 gridPosition = new Vector3(
            Mathf.Round(transform.localPosition.x / gridSize) * gridSize,
            0,
            Mathf.Round(transform.localPosition.z / gridSize) * gridSize
        );
        transform.localPosition = gridPosition;
    }

    private bool IsWithinGridBounds(Vector3 position)
    {
        // Adjust these bounds based on your grid size and position
        float minX = -4;
        float maxX = 4;
        float minZ = -4;
        float maxZ = 4;

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    //private void SnapToGrid()
    //{
    //    float gridSize = 1f; // Assuming grid size is 1 unit
    //    Vector3 gridPosition = new Vector3(
    //        Mathf.Round(transform.position.x / gridSize) * gridSize,
    //        0,
    //        Mathf.Round(transform.position.z / gridSize) * gridSize
    //    );
    //    print(gridPosition);
    //    transform.position = new Vector3(Mathf.Round((gridPosition.x * 2f) / 2f), 0f, Mathf.Round((gridPosition.z * 2f) / 2f));
    //    //transform.position = gridPosition;


    //    // Optionally, set back to the original parent
    //    transform.SetParent(originalParent, true);
    //    gameObject.BroadcastMessage("CheckCellPosition", SendMessageOptions.DontRequireReceiver);
    //    CircuitConnection.Instance.gameObject.BroadcastMessage("CheckWinCondition", SendMessageOptions.DontRequireReceiver);
    //}


}

[Serializable]
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class CellInfo : MonoBehaviour
{
    public Vector3 designatedPosition;
    public bool inPlace;
    public Light light;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        light = GetComponent<Light>();
        light.intensity = 0.05f;
        //light.lightmapBakeType = LightmapBakeType.Mixed;
        light.color = new Color(0.15f, 0.75f, 0.65f, 1);
        light.enabled = false;
    }
    private void Update()
    {
        if (inPlace && !light.enabled)
        {
            light.enabled = true;
        }
        if (!inPlace && light.enabled)
        {
            light.enabled = false;
        }
    }
    public void CheckCellPosition()
    {
       

        if (IsWithinGridBounds(transform.localPosition) && Vector3.Distance(transform.localPosition, designatedPosition) < 0.25f)
        {
            print("Cell " + gameObject.name + " is on the right spot!");
            audioSource.PlayOneShot(CircuitConnection.Instance.correctMove);
            transform.localPosition = designatedPosition;
            inPlace = true;
            light.enabled = true;
            //analytics
            CircuitConnection.Instance.isCorrect = true;
            //end
        }
        else
        {
            //analytics
            CircuitConnection.Instance.isCorrect = false;
            //end
            inPlace = false;
            audioSource.PlayOneShot(CircuitConnection.Instance.wrongMove);
            // Reset the pattern to the start position only if it is within the grid bounds
            if (!IsWithinGridBounds(transform.localPosition))
            {
                transform.parent.localPosition = transform.parent.GetComponent<Draggable>().startPosition;
            }
        }
    }

    private bool IsWithinGridBounds(Vector3 position)
    {
        // Adjust these bounds based on your grid size and position
        float minX = -4;
        float maxX = 4;
        float minZ = -4;
        float maxZ = 4;

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    } 
}