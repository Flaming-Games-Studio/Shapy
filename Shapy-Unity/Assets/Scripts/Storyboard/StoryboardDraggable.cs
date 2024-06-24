using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryboardDraggable : MonoBehaviour
{
    public LayerMask draggableLayer;

    [HideInInspector]
    public float draggableLocalXMin, draggableLocalXMax;
    [HideInInspector]
    public float draggableLocalYMin, draggableLocalYMax;
    [HideInInspector]
    public StoryboardSlot assignedSlot;
    [HideInInspector]
    public StoryboardManager storyboarManager;
    [HideInInspector]
    public Camera cam;

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 posBeforePickedUp;
    private bool isDragging = false;




    private void Update()
    {
        MouseDragg();
    }

    void MouseDragg()
    {
        //if (!storyboarManager.enabled)
        //    return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 5);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
                    posBeforePickedUp = transform.position;
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging)
                return;

            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = cam.ScreenToWorldPoint(cursorPoint);

            Vector3 localCursorPosition = transform.parent.InverseTransformPoint(cursorPosition);

            localCursorPosition.x = Mathf.Clamp(localCursorPosition.x, draggableLocalXMin, draggableLocalXMax);
            localCursorPosition.y = Mathf.Clamp(localCursorPosition.y, draggableLocalYMin, draggableLocalYMax);
            transform.localPosition = new Vector3(localCursorPosition.x, localCursorPosition.y, transform.localPosition.z);

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
                return;

            isDragging = false;
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z  /*GearPuzzleManager.instance.transform.position.z*/);
            transform.position = targetPosition;
            storyboarManager.DroppedDraggable(this);
        }

    }

    public void ReturnToPosBeforePickedUp()
    {
        transform.position = posBeforePickedUp;
    }

}
