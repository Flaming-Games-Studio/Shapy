using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReferenceManager : MonoBehaviour
{

    public static ItemReferenceManager Instance;

    public InventoryItemData battery;
    public InventoryItemData wood;
    public InventoryItemData cloth;
    public InventoryItemData glass;

    private void Awake()
    {
        if (Instance != this || Instance == null)
        {
            print("Setting static inventory manager!");
            Instance = this;
        }
        else
        {
            print("Destroying non Instance");
        }
    }
}
