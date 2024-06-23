using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTile : MonoBehaviour
{
    public bool occupied;
    public Vector2 tileCoordinates;
    public Transform tileTransform;

    

    public InventoryTile(bool occ, Vector2 coords, Transform objT)
    {
        occupied = occ;
        tileCoordinates = coords;
        tileTransform = objT;
    }
}
