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
    private ProgressConfigManager progressConfigManager = new ProgressConfigManager();



    #region testing save
    public bool testSave;
    public bool testLoad;
    public bool resetSaveData;

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
        if (progressConfigManager.playerPosition != FPSController.Instance.transform.position)
        {
            print("moving player");
            FPSController.Instance.ResetPlayerPosition(progressConfigManager.playerPosition + (Vector3.up / 4), progressConfigManager.playerRotation);
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
            progressConfigManager.UpdateProgress(bridge.AlreadyDown(), puzzlesStatesArray, FPSController.Instance.transform.position, FPSController.Instance.transform.rotation);
            PopupManager.Instance.GenerateNewPopupMessage("Save system", "Game saved!");
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
                    CheckIfIslandIsCompleted(puzzlesStatesArray[i].puzzlesStatus, bridge, puzzleGO);
                }
            }
        }
        UpdatePlayerProgress();
    }

    public void CheckIfIslandIsCompleted(List<bool> puzzleState, BridgeControl bridge, GameObject puzzleGO)
    {
        if (puzzleState.All(x => x))
        {
            if (!bridge.AlreadyDown())
            {
                puzzleGO.TryGetComponent<InteractableObject>(out InteractableObject IO);
                if (IO != null)
                {
                    IO.Interact();
                }
                bridge.OpenBridge();
            }
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
