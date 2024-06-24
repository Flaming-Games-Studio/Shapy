using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObjectArray[] PuzzlesIslandGameobjectsArray;
    private List<PuzzlesStatesArray> puzzlesStatesArray = new List<PuzzlesStatesArray>();
    public BridgeControl bridge;
    public GameObject cableCar1, cableCar2, boat;
    private ProgressConfigManager progressConfigManager = new ProgressConfigManager();


    #region testing save
    public bool testSave;
    public bool testLoad;
    public bool resetSaveData;
    public bool finishIslandOne;
    public bool finishIslandTwo;
    public bool finishIslandThree;

    private void Update()
    {
        if (testSave)
        { 
            UpdatePlayerProgress();
            testSave = false;
        }
        if (testLoad)
        {
            LoadPlayerProgress();
            testLoad = false;
        }
        if (resetSaveData)
        {
            ConfigSaveLoadSystem.Save("");
            resetSaveData = false;
        }

        if (finishIslandOne)
        {
            for (int i = 0; i < puzzlesStatesArray[0].puzzlesStatus.Count; i++)
            {
                puzzlesStatesArray[0].puzzlesStatus[i] = true;
            }
            CheckIfIslandOneIsCompleted(puzzlesStatesArray[0].puzzlesStatus, bridge, null);
            finishIslandOne = false;
        }
        if (finishIslandTwo)
        {
            for (int i = 0; i < puzzlesStatesArray[1].puzzlesStatus.Count; i++)
            {
                puzzlesStatesArray[1].puzzlesStatus[i] = true;
            }
            CheckIfIslandTwoIsCompleted(puzzlesStatesArray[1].puzzlesStatus);
            finishIslandTwo = false;
        }
        if (finishIslandThree)
        {
            for (int i = 0; i < puzzlesStatesArray[2].puzzlesStatus.Count; i++)
            {
                puzzlesStatesArray[2].puzzlesStatus[i] = true;
            }
            CheckIfIslandThreeIsCompleted(puzzlesStatesArray[2].puzzlesStatus);
            finishIslandThree = false;
        }
    }
    #endregion




    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < PuzzlesIslandGameobjectsArray.Length; i++)
        {
            PuzzlesStatesArray psa = new PuzzlesStatesArray();
            puzzlesStatesArray.Add(psa);
            for (int j = 0; j < PuzzlesIslandGameobjectsArray[i].gameObjects.Length; j++)
            {
                puzzlesStatesArray[i].puzzlesStatus.Add(false);
            }
        }
        ConfigSaveLoadSystem.Init();
        if (ConfigSaveLoadSystem.configTemp != null)
        {
            LoadPlayerProgress();
        }
        else
        {
            print("Save file empty or null!");
        }
    }

    public void LoadPlayerProgress()
    {
        if (!ConfigSaveLoadSystem.CheckIfLoadExists())
        {
            return;
        }
        ConfigSaveLoadSystem.Load();
        if (ConfigSaveLoadSystem.configTemp == null)
        {
            print("Save file corupted or empty!");
            return;
        }
        progressConfigManager = ConfigSaveLoadSystem.configTemp;

        bridge.SetBridgeState(progressConfigManager.bridgeState);
        puzzlesStatesArray = progressConfigManager.puzzleStatusTracker.puzzlesStatesArrays;

        if (progressConfigManager.bridgeState)
        {
            bridge.ForceBridgeDown();
        }
        if (progressConfigManager.cableCar1State)
        {
            cableCar1.SetActive(true);
        }
        if (progressConfigManager.cableCar2State)
        {
            cableCar2.SetActive(true);
        }
        if (progressConfigManager.boatState)
        {
            boat.SetActive(true);
        }
        if (progressConfigManager.kitState)
        {
            KitController.Instance.ReloadKit();
        }
        if (progressConfigManager.playerPosition != FPSController.Instance.transform.position)
        {
            print("moving player");
            FPSController.Instance.canMove = false;
            FPSController.Instance.ResetPlayerPosition(progressConfigManager.playerPosition, progressConfigManager.playerRotation);
            FPSController.Instance.canMove = true;

            //FPSController.Instance.transform.position = progressConfigManager.playerPosition + (Vector3.up / 4);
            //FPSController.Instance.transform.rotation = progressConfigManager.playerRotation;
            //FPSController.Instance.playerCamera.transform.rotation = progressConfigManager.playerRotation;
        }
        PopupManager.Instance.GenerateNewPopupMessage("Save system", "Game loaded!");
    }

    /// <summary>
    /// update after every puzzle and tracked achievement and when closing the game or leaving the scene
    /// </summary>
    public void UpdatePlayerProgress()
    {
        if (progressConfigManager != null)
        {
            progressConfigManager.UpdateProgress(bridge.AlreadyDown(),cableCar1.activeSelf, cableCar2.activeSelf, boat.activeSelf, puzzlesStatesArray, FPSController.Instance.transform.position, FPSController.Instance.transform.rotation);
            PopupManager.Instance.GenerateNewPopupMessage("Save system", "Game saved!");
        }
    }

    public void UpdateKitStatus(bool kitActivated)
    {
        if (progressConfigManager != null)
        {
            progressConfigManager.UpdateKitStatus(kitActivated);
        }
    }


    /// <summary>
    /// Send the upmost parent of the puzzle, the one slotted into PuzzlesIslandGameobjectsArray.
    /// </summary>
    /// <param name="puzzleGO"></param>
    public void CheckPuzzleCompleteState(GameObject puzzleGO)
    {
        print("Initiating check if puzzle is complete on: " + puzzleGO.name);
        for (int i = 0; i < PuzzlesIslandGameobjectsArray.Length; i++)
        {
            for (int j = 0; j < PuzzlesIslandGameobjectsArray[i].gameObjects.Length; j++)
            {
                if (puzzleGO.GetHashCode() == PuzzlesIslandGameobjectsArray[i].gameObjects[j].GetHashCode())
                {
                    if (!puzzlesStatesArray[i].puzzlesStatus[j])
                    {
                        print("Puzzle complete!");
                        puzzlesStatesArray[i].puzzlesStatus[j] = true;
                        AddFirstTimeReward(i + 1);
                    }
                    else
                    {
                        AddConsecutiveReward(i + 1);
                    }
                }
            }
            if (i == 0)
            {
                CheckIfIslandOneIsCompleted(puzzlesStatesArray[i].puzzlesStatus, bridge, puzzleGO);
            }
            if (i == 1)
            {
                CheckIfIslandTwoIsCompleted(puzzlesStatesArray[i].puzzlesStatus);
            }
            if (i == 1)
            {
                CheckIfIslandThreeIsCompleted(puzzlesStatesArray[i].puzzlesStatus);
            }

        }
        UpdatePlayerProgress();
    }

    public void CheckIfIslandOneIsCompleted(List<bool> puzzleState, BridgeControl bridge, GameObject puzzleGO)
    {
        if (puzzleState.All(x => x))
        {
            if (!bridge.AlreadyDown())
            {
                if (puzzleGO == null)
                {
                    bridge.OpenBridge();
                    return;
                }
                puzzleGO.TryGetComponent<InteractableObject>(out InteractableObject IO);
                if (IO != null)
                {
                    IO.Interact();
                }
                bridge.OpenBridge();
            }
        }
    }
    public void CheckIfIslandTwoIsCompleted(List<bool> puzzleState)
    {
        if (puzzleState.All(x => x))
        {
            boat.SetActive(true);
            cableCar1.SetActive(true);
        }
    }

    public void CheckIfIslandThreeIsCompleted(List<bool> puzzleState)
    {
        if (puzzleState.All(x => x))
        {
            cableCar2.SetActive(true);
        }
    }

    /// <summary>
    /// every island will give more rewards suggestion (1,2,4)
    /// TODO: check incoming int if 3 make it 4 (1,2 works out of the box)
    /// -randomize rewards or give less of all 3 materials??
    /// </summary>
    /// <param name="islandMultiplier"></param>
    public void AddFirstTimeReward(int islandMultiplier)
    {
        //popup
        PopupManager.Instance.GenerateNewPopupMessage("Puzzle Reward!", "You recieved " + (20 * islandMultiplier).ToString() + "x" + ItemReferenceManager.Instance.wood.itemName + " and " + (20 * islandMultiplier).ToString() + "x" + ItemReferenceManager.Instance.glass.itemName);
       
        InventoryManager.Instance.AddItemToInventory(RewardWrapper(ItemReferenceManager.Instance.wood, 20 * islandMultiplier));
        InventoryManager.Instance.AddItemToInventory(RewardWrapper(ItemReferenceManager.Instance.glass, 20 * islandMultiplier));
    }

    /// <summary>
    /// every island will give more rewards suggestion (1,2,4)
    /// </summary>
    /// <param name="islandMultiplier"></param>
    public void AddConsecutiveReward(int islandMultiplier)
    {
        //popup
        PopupManager.Instance.GenerateNewPopupMessage("Puzzle Reward!", "You recieved " + (5 * islandMultiplier).ToString() + "x" + ItemReferenceManager.Instance.wood.itemName + " and " + (5 * islandMultiplier).ToString() + "x" + ItemReferenceManager.Instance.glass.itemName);

        InventoryManager.Instance.AddItemToInventory(RewardWrapper(ItemReferenceManager.Instance.wood, 5 * islandMultiplier));
        InventoryManager.Instance.AddItemToInventory(RewardWrapper(ItemReferenceManager.Instance.glass, 5 * islandMultiplier));
    }
    /// <summary>
    /// Provides a fresh copy of InventoryItemData we dont want to share hash with reference!
    /// </summary>
    /// <param name="rewardRef"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private InventoryItemData RewardWrapper(InventoryItemData rewardRef, int amount)
    {
        InventoryItemData reward = new InventoryItemData()
        {
            itemName = rewardRef.itemName,
            itemImage = rewardRef.itemImage,
            itemWidth = rewardRef.itemWidth,
            itemHeight = rewardRef.itemHeight,
            itemQuantity = amount,
            itemPrefab = rewardRef.itemPrefab
        };

        return reward;
    }
}







[System.Serializable]
public class GameObjectArray
{
    public GameObject[] gameObjects;
}

[System.Serializable]
public class PuzzlesStatesArray
{
    [SerializeField]
    public List<bool> puzzlesStatus = new List<bool>();
}
