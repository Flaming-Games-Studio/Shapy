using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public GameObject[] boundaries;

    private void Start()
    {
        for (int i = 0; i < boundaries.Length; i++) 
        {
            boundaries[i].transform.position = PlaceObjectAtBottom(i);
        }
    }

    Vector3 PlaceObjectAtBottom(int x)
    {
        Renderer renderer = boundaries[x].GetComponent<Renderer>();
        Vector3 objectExtents = renderer.bounds.extents;
        Vector3 newPosition = Vector3.zero;
        switch (x)
        {
            default:
                return newPosition;
            case 0:
                Vector3 botCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0));
                newPosition = new Vector3(0f, botCenter.y - objectExtents.y, 0f);
                return newPosition;
            case 1:
                Vector3 topCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, 0));
                newPosition = new Vector3(0f, topCenter.y + objectExtents.y, 0f);
                return newPosition;
            case 2:
                Vector3 rightCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, 0));
                newPosition = new Vector3(rightCenter.x + objectExtents.x, 0f, 0f);
                return newPosition;
            case 3:
                Vector3 leftCenter = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0));
                newPosition = new Vector3(leftCenter.x - objectExtents.x, 0f, 0f);
                return newPosition;
        }
    }
}

   