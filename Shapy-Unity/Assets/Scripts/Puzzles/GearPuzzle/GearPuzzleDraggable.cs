using UnityEngine;

public class GearPuzzleDraggable : MonoBehaviour
{
    public LayerMask draggableLayer;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 posBeforePickedUp;

    [HideInInspector]
    public GearPuzzleGearSlot assignedGearSlot;
    [HideInInspector]
    public GearPuzzleGear gearScript;
    [HideInInspector]
    public Camera cam;

    private bool isDragging = false;


    private void Start()
    {
        gearScript = GetComponent<GearPuzzleGear>();
        //cam = Camera.main;
    }


    private void Update()
    {
        MouseDragg();
    }

    void MouseDragg()
    {
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

            localCursorPosition.x = Mathf.Clamp(localCursorPosition.x, -1.3f, 1.3f);
            localCursorPosition.y = Mathf.Clamp(localCursorPosition.y, -1.5f, 1f);
            transform.localPosition = new Vector3(localCursorPosition.x, localCursorPosition.y, transform.localPosition.z);

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
                return;

            isDragging = false;
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, GearPuzzleManager.instance.transform.position.z);
            transform.position = targetPosition;
            GearPuzzleManager.instance.DroppedDraggable(this);
        }

    }

    public void ReturnToPosBeforePickedUp()
    {
        transform.position = posBeforePickedUp;
    }
}

//void OnMouseDown()
//{
//    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//    RaycastHit[] hits = Physics.RaycastAll(ray, 10);

//    //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
//    //RaycastHit hit;

//    foreach (RaycastHit hit in hits)
//    {
//        if (hit.collider.gameObject == gameObject /*hit.collider.gameObject.layer == draggableLayer*/)
//        {
//            screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
//            posBeforePickedUp = transform.position;
//            isDragging = true;
//        }
//    }


//    //if (Physics.Raycast(ray, out hit, Mathf.Infinity, draggableLayer))
//    //{
//    //    if (hit.collider.gameObject == gameObject)
//    //    {
//    //        screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
//    //        posBeforePickedUp = transform.position;
//    //        isDragging = true;
//    //    }
//    //}
//}

//void OnMouseDrag()
//{
//    if (!isDragging) return;

//    Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
//    Vector3 cursorPosition = cam.ScreenToWorldPoint(cursorPoint);

//    Vector3 localCursorPosition = transform.parent.InverseTransformPoint(cursorPosition);

//    localCursorPosition.x = Mathf.Clamp(localCursorPosition.x, -1.3f, 1.3f);
//    localCursorPosition.y = Mathf.Clamp(localCursorPosition.y, -1.5f, 1f);
//    transform.localPosition = new Vector3(localCursorPosition.x, localCursorPosition.y, transform.localPosition.z);
//}

//private void OnMouseUp()
//{
//    if (!isDragging) return;

//    isDragging = false;
//    Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, GearPuzzleManager.instance.transform.position.z);
//    transform.position = targetPosition;
//    GearPuzzleManager.instance.DroppedDraggable(this);
//}




//void OnMouseDown()
//{
//    screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
//    //offset = gameObject.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z - 0.1f));
//    posBeforePickedUp = transform.position;
//}

//void OnMouseDrag()
//{
//    Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
//    Vector3 cursorPosition = cam.ScreenToWorldPoint(cursorPoint) /*+ offset*/;

//    Vector3 localCursorPosition = transform.parent.InverseTransformPoint(cursorPosition);

//    localCursorPosition.x = Mathf.Clamp(localCursorPosition.x, -1.3f, 1.3f);
//    localCursorPosition.y = Mathf.Clamp(localCursorPosition.y, -1.5f, 1f);
//    transform.localPosition = new Vector3(localCursorPosition.x, localCursorPosition.y, transform.localPosition.z);
//}


//private void OnMouseUp()
//{
//    Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, GearPuzzleManager.instance.transform.position.z);
//    transform.position = targetPosition;
//    GearPuzzleManager.instance.DroppedDraggable(this);
//}


//public void ReturnToPosBeforePickedUp()
//{
//    transform.position = posBeforePickedUp;
//}