using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    InventoryManager inventoryManager;

    public TextMeshProUGUI quantityText;
    public Image image;

    [HideInInspector] public string itemName;
    [HideInInspector] public int width, height, quantity = 0;
    public List<InventoryTile> occupiedTiles = new List<InventoryTile>();
    [SerializeField]
    public InventoryItemData itemData;
    private GameObject itemObjectReference;

    int x, y = 0;
    bool selected = false;

    public void SetItemAsSelected()
    {
        selected = true;
    }
    public void SetItemAsDeselected()
    {
        selected = false;
    }
    public void SetItemDataVariable(InventoryItemData data)
    {
        itemData = data;
        InitializeItem(data);
    }
    public void SetInventoryManagerVariable(InventoryManager IM)
    {
        inventoryManager = IM;
    }

    public void SetItemObjectReference(GameObject itemObjectReference)
    { this.itemObjectReference = itemObjectReference; }

    public GameObject GetItemObjectReference()
    { return itemObjectReference; }
    public void InitializeItem(InventoryItemData itemData)
    {
        gameObject.name = itemData.itemName;
        (x,y) = InventoryManager.Instance.ReturnGridSize();

        quantityText.text = itemData.itemQuantity.ToString();
        itemName = itemData.itemName;
        image.sprite = itemData.itemImage;
        width = itemData.itemWidth; 
        height = itemData.itemHeight;
        quantity = itemData.itemQuantity;

        RectTransform uitransform = GetComponent<RectTransform>();

        uitransform.sizeDelta = new Vector2(width * 100, height * 100);
        uitransform.anchorMin = new Vector2(0, 1);
        uitransform.anchorMax = new Vector2(0, 1);
        uitransform.pivot = new Vector2(0, 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                RectTransform tran = transform.GetChild(i).GetComponent<RectTransform>();

                tran.sizeDelta = new Vector2(width * 25, height * 25);
                tran.anchorMin = new Vector2(1, 0);
                tran.anchorMax = new Vector2(1, 0);
                tran.pivot = new Vector2(1, 0);
            }
            else
            {
                RectTransform tran = transform.GetChild(i).GetComponent<RectTransform>();

                tran.sizeDelta = new Vector2(width * 100, height * 100);
                tran.anchorMin = new Vector2(0.5f, 0.5f);
                tran.anchorMax = new Vector2(0.5f, 0.5f);
                tran.pivot = new Vector2(0.5f, 0.5f);
            }
        }
    }
    public void SelectItem(InputAction.CallbackContext context)
    {
        selected = true;
        transform.SetAsLastSibling();  //very important go figure it out!
    }

    public IEnumerator Dragging()
    {
        while(selected) 
        {
            Vector3 correctedMousePosition = new Vector3(Input.mousePosition.x - 15, Input.mousePosition.y + 15, 0);
            transform.position = correctedMousePosition;
            yield return null;
        }
    }

    public void Dropping(InputAction.CallbackContext context, InventoryTile tile)
    {
        if (CheckIfPlacingIsValid(tile))
        {
            transform.position = occupiedTiles[0].tileTransform.position;
            selected = false;
            return;
        }
        ReturnToStartingPosition();
        selected = false;
    }

    public bool CheckIfPlacingIsValid(InventoryTile tile)
    {
        if (tile.occupied && !OwnPosition(tile))
        {
            //print("First selected tile is occupied, no brainer return");
            ReturnToStartingPosition();
            return false;
        }
        if (itemData.itemWidth > ((x + 1) - tile.tileCoordinates.x))
        {
            //print("Item is outside of the width bounding box, reutrning to last valid position!");
            ReturnToStartingPosition();
            return false;
        }
        if (itemData.itemHeight > ((y + 1) - tile.tileCoordinates.y))
        {
            //print("Item is outside of the height bounding box, reutrning to last valid position!");
            ReturnToStartingPosition();
            return false;
        }
        if (CheckBoundingTiles(tile))
        {
            print("Item can be placed on this postition!" + occupiedTiles[0].tileCoordinates);
            transform.position = occupiedTiles[0].tileTransform.position;
            return true;
        }
        else return false;
    }

    private void ReturnToStartingPosition()
    {
        if (occupiedTiles.Count == 0)
        {
            return;
        }
        transform.position = occupiedTiles[0].tileTransform.position;
    }

    public bool CheckBoundingTiles(InventoryTile tile)
    {
        List<InventoryTile> boundingTiles = new List<InventoryTile>();
        bool[,] requiredSpots = new bool[itemData.itemWidth, itemData.itemHeight];

        for (int i = 0; i < itemData.itemWidth; i++)
        {
            for (int j = 0; j < itemData.itemHeight; j++)
            {
                Vector2 boundingBox = new Vector2(tile.tileCoordinates.x + i, tile.tileCoordinates.y + j);

                //check tiles && early exit if tile occupied
                if (inventoryManager.inventoryTiles[boundingBox].occupied)
                {
                    if (OwnPosition(inventoryManager.inventoryTiles[boundingBox]))
                    {
                        requiredSpots[i, j] = true;
                        boundingTiles.Add(inventoryManager.inventoryTiles[boundingBox]);
                    }
                    else
                    {
                        requiredSpots[i, j] = false;
                        //print("One of the spots needed for this item is already occupied!");
                        return false;
                    }
                }
                else
                {
                    requiredSpots[i, j] = true;
                    boundingTiles.Add(inventoryManager.inventoryTiles[boundingBox]);
                }
            }
        }
        //reign supreme
        if (requiredSpots.OfType<bool>().All(b => b))
        {
            for (int i = 0; i < boundingTiles.Count; i++)
            {
                boundingTiles[i].occupied = true;
            }
            for (int i = 0; i < occupiedTiles.Count; i++)
            {
                occupiedTiles[i].occupied = false;
            }
            occupiedTiles.Clear();
            occupiedTiles = boundingTiles;
            
            //print("Updated occupied tiles list!");
            return true;
        }
        else return false;
    }

    private bool OwnPosition(InventoryTile tile)
    {
        for (int i = 0; i < occupiedTiles.Count; i++)
        {
            if (this.occupiedTiles[i].tileCoordinates == tile.tileCoordinates)
            {
                return true;
            }
        }
        return false;
    }
}
