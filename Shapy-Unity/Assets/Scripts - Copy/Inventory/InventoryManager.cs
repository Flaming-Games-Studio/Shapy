using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventoryItemData[] test; //adding items for testing
    public InventoryItemData[] test2; //adding items for testing

    public Transform gridSlotsParent;
    public Transform inventoryParent;
    public GameObject emptyItemPrefab;

    public Dictionary<Vector2, InventoryTile> inventoryTiles = new Dictionary<Vector2, InventoryTile>();
    List<BatteryStatus> batterysList = new List<BatteryStatus>();

    List<RectTransform> inventorySlots = new List<RectTransform>();
    List<InventoryItem> inventoryItems = new List<InventoryItem>();
    List<int> collumns = new List<int>();
    List<int> rows = new List<int>();
    int rowSize, collumnSize;

    private void Awake()
    {
        if (Instance == null)
        {
            print("Setting static inventory manager!");
            Instance = this;
        }
    }
    private void Start()
    {
        InitializeEmptySlots();
    }

    private IEnumerator CheckRowsAndCollumns()
    {
        yield return new WaitForEndOfFrame();

        string collumnCoordinates, rowCoordiantes;  
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!collumns.Contains((int)inventorySlots[i].localPosition.y))
            {
                collumns.Add((int)inventorySlots[i].localPosition.y);
                collumnCoordinates = (collumns.Count - 1).ToString();
            }
            else
            {
                collumnCoordinates = collumns.IndexOf((int)inventorySlots[i].localPosition.y).ToString();
            }
            if (!rows.Contains((int)inventorySlots[i].localPosition.x))
            {
                rows.Add((int)inventorySlots[i].localPosition.x);
                rowCoordiantes = (rows.Count - 1).ToString();
            }
            else
            {
                rowCoordiantes = rows.IndexOf((int)inventorySlots[i].localPosition.x).ToString();
            }
            inventorySlots[i].GetComponentInChildren<TextMeshProUGUI>().text = rowCoordiantes + " , " + collumnCoordinates;

            int x, y;
            int.TryParse(rowCoordiantes, out x);
            int.TryParse(collumnCoordinates, out y);
            collumnSize = x;
            rowSize = y;
            //print(x + " , " + y);
            InventoryTile tileData = inventorySlots[i].AddComponent<InventoryTile>();
            tileData.occupied = false;
            tileData.tileCoordinates = new Vector2(x, y);
            tileData.tileTransform = inventorySlots[i].transform;

            if (!inventoryTiles.ContainsKey(new Vector2(x, y)))
            {
                inventoryTiles.Add(new Vector2(x, y), tileData);
            }
            else
            {
                print(new Vector2(x, y) + "  " + tileData);
            }
        }

        for (int i = 0; i < test.Length; i++)
        {
            AddItemToInventory(CreateNewInventoryItemDataInstanceWithRandomQuantity(test[i], 30, 50));
        }

        for (int i = 0; i < test2.Length; i++)
        {
            AddItemToInventory(CreateNewInventoryItemDataInstance(test2[i]));
        }
        UIController.Instance.ToggleInventoryCanvas(new InputAction.CallbackContext());
    }

    private void InitializeEmptySlots()
    {
        for (int i = 0; i < gridSlotsParent.childCount; i++)
        {
            inventorySlots.Add(gridSlotsParent.GetChild(i).gameObject.GetComponent<RectTransform>());
        }
        StartCoroutine(CheckRowsAndCollumns());
    }

    /// <summary>
    /// InventoryManager.Instance.AddItemToInventory(InventoryManager.Instance.CreateNewInventoryItemDataInstance(itemData));
    /// --
    /// Call with CreateNewInventoryItemDataInstance changes on SO are saved on the fly and can be altered realtime. 
    /// --
    /// </summary>
    /// <param name="item"></param>
    public void AddItemToInventory(InventoryItemData item)
    {
        if (!CheckIfItemAlreadyExistInInventory(item))
        {
            SpawnNewItem(item);
        }
        UpdateQuantityTexts();
    }

    public void AddBatteryToInventory(InventoryItemData item, GameObject batteryObject)
    {
        if (!CheckIfBatteryAlreadyExistInInventory(item))
        {
            SpawnNewBattery(item, batteryObject);
            AddNewBatteryToInventory(batteryObject);
        }
        else
        {
            AddNewBatteryToInventory(batteryObject);
        }
    }

    public InventoryItemData CreateNewInventoryItemDataInstance(InventoryItemData item)
    {
        InventoryItemData iid = ScriptableObject.Instantiate(item);
        iid.itemName = item.itemName;
        iid.itemImage = item.itemImage;
        iid.itemWidth = item.itemWidth;
        iid.itemHeight = item.itemHeight;
        iid.itemQuantity = 1;
        iid.itemPrefab = item.itemPrefab;

        return iid;
    }
    public InventoryItemData CreateNewInventoryItemDataInstanceWithDefinedQuantity(InventoryItemData item, int q)
    {
        InventoryItemData iid = ScriptableObject.Instantiate(item);
        iid.itemName = item.itemName;
        iid.itemImage = item.itemImage;
        iid.itemWidth = item.itemWidth;
        iid.itemHeight = item.itemHeight;
        iid.itemQuantity = q;
        iid.itemPrefab = item.itemPrefab;

        return iid;
    }
    public InventoryItemData CreateNewInventoryItemDataInstanceWithRandomQuantity(InventoryItemData item, int r1, int r2)
    {
        InventoryItemData iid = ScriptableObject.Instantiate(item);
        iid.itemName = item.itemName;
        iid.itemImage = item.itemImage;
        iid.itemWidth = item.itemWidth;
        iid.itemHeight = item.itemHeight;
        iid.itemQuantity = Random.Range(r1, r2);
        iid.itemPrefab = item.itemPrefab;

        return iid;
    }

    public BatteryStatus ReturnFirstAvailableBattery()
    {
        if (batterysList.Count > 0)
        {
            BatteryStatus battery = batterysList[0];
            batterysList.RemoveAt(0);

            return battery;
        }
        else
        {
            return null;
        }
    }

    public void RemoveItemFromInventory(InventoryItemData item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].itemName == item.itemName)
            {
                //print("Removing " + item.itemQuantity.ToString() + " from " + inventoryItems[i].itemName + " stack which has " + inventoryItems[i].quantity.ToString());
                inventoryItems[i].quantity -= item.itemQuantity;
                UpdateQuantityTexts();
                if (inventoryItems[i].quantity <= 0)
                {
                    for (int j = 0; j < inventoryItems[i].occupiedTiles.Count; j++)
                    {
                        inventoryItems[i].occupiedTiles[j].occupied = false;
                    }
                    if (inventoryItems[i].gameObject != null)
                    {
                        GameObject go = inventoryItems[i].gameObject;
                        inventoryItems.RemoveAt(i);
                        Destroy(go);
                    }
                }
                else return;
            }
        }
    }

    private bool CheckIfItemAlreadyExistInInventory(InventoryItemData item)
    {
        if (inventoryItems.Count == 0)
            return false;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].itemName == item.itemName)
            {
                inventoryItems[i].quantity += item.itemQuantity;
                UpdateQuantityTexts();
                return true;
            }
        }
        return false;
    }

    private bool CheckIfBatteryAlreadyExistInInventory(InventoryItemData item)
    {
        if (inventoryItems.Count == 0)
            return false;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].itemName == item.itemName)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateQuantityTexts()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItems[i].quantityText.text = inventoryItems[i].quantity.ToString();
        }
    }
    public void UpdateBatteryQuantityTexts()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].itemName == "Battery")
            {
                inventoryItems[i].quantityText.text = batterysList.Count.ToString();
            }
        }
    }
    private void SpawnNewItem(InventoryItemData item)
    {
        GameObject i = Instantiate(emptyItemPrefab, inventoryParent);
        InventoryItem ii = i.GetComponent<InventoryItem>();

        ii.SetItemDataVariable(item);
        ii.SetInventoryManagerVariable(this);
        inventoryItems.Add(ii);

        if (ii.CheckIfPlacingIsValid(inventoryTiles[Vector2.zero]))
        {
            ii.gameObject.transform.position = inventoryTiles[Vector2.zero].tileTransform.position;
        }
        else
        {
            for (int l = 0; l < rowSize; l++)
            {
                for (int k = 0; k < collumnSize; k++)
                {
                    ii.occupiedTiles.Clear();
                    if (ii.CheckIfPlacingIsValid(inventoryTiles[new Vector2(k, l)]))
                    {
                        return;
                    }
                    else
                    {
                        //print("Tile " + new Vector2(k, l).ToString() + " occupied or bounding tile occupied");
                    }
                }
            }
            inventoryItems.Remove(ii);
            Destroy(i);
        }
    }

    public void AddNewBatteryToInventory(GameObject batteryObject)
    {
        batterysList.Add(batteryObject.GetComponent<BatteryStatus>());
        UpdateBatteryQuantityTexts();
    }

    public void AddNewBatteryToInventory(BatteryStatus batteryObject)
    {
        batterysList.Add(batteryObject);
        UpdateBatteryQuantityTexts();
    }

    private void SpawnNewBattery(InventoryItemData item, GameObject batteryObject)
    {
        GameObject i = Instantiate(emptyItemPrefab, inventoryParent);
        InventoryItem ii = i.GetComponent<InventoryItem>();

        ii.SetItemDataVariable(item);
        ii.SetItemObjectReference(batteryObject);
        ii.SetInventoryManagerVariable(this);
        inventoryItems.Add(ii);
        

        if (ii.CheckIfPlacingIsValid(inventoryTiles[Vector2.zero]))
        {
            ii.gameObject.transform.position = inventoryTiles[Vector2.zero].tileTransform.position;
        }
        else
        {
            for (int l = 0; l < rowSize; l++)
            {
                for (int k = 0; k < collumnSize; k++)
                {
                    //tu negdje upalit batery objekte nazad i javit da je inventory pun ili da nema mjesta
                    ii.occupiedTiles.Clear();
                    if (ii.CheckIfPlacingIsValid(inventoryTiles[new Vector2(k, l)]))
                    {
                        return;
                    }
                    else
                    {
                        //print("Tile " + new Vector2(k, l).ToString() + " occupied or bounding tile occupied");
                    }
                }
            }
            inventoryItems.Remove(ii);
            Destroy(i);
        }
    }

    public (int, int) ReturnGridSize()
    {
        return (collumnSize, rowSize);
    }

    public int ReturnItemQuantity(InventoryItemData item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].itemName == item.itemName)
            {
                return inventoryItems[i].quantity;
            }
        }
        return 0;
    }
}
