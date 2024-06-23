using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/Inventory/InventoryItem", order = 1)]
public class InventoryItemData : ScriptableObject
{
    public string itemName = "";
    public Sprite itemImage = null;
    [Header("Example: Golden coind is 1x1, sword is 3x1 etc.")]
    [Range(1, 8)]
    public int itemWidth, itemHeight = 1;
    public int itemQuantity = 0;
    public GameObject itemPrefab;
}
