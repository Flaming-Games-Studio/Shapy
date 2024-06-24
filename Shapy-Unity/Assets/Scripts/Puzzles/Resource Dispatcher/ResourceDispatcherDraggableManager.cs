using UnityEngine;
using TMPro;

public class ResourceDispatcherDraggableManager : MonoBehaviour
{
    public int batteryConsumption = 10;
    public TextMeshProUGUI batteryConsumptionTxt;
    public int ID;
    public LayerMask draggableLayer;

    [Header("Samo za hard mode")]
    public GameObject[] lowConsumptionDraggables;
    public GameObject[] mediumConsumptionDraggables;
    public GameObject[] highConsumptionDraggables;


    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 posBeforePickedUp;
    [HideInInspector]
    public ResourceDispatcherTile assignedTile;
    [HideInInspector]
    public ResourceDispatcherPuzzle resourceDispatcherPuzzle;
    [HideInInspector]
    public Camera cam;

    private bool isDragging = false;
    private bool isHovering = false;




    private void Update()
    {
        MouseDragg();
    }

    void MouseDragg()
    {
        if (!resourceDispatcherPuzzle.enabled)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 10, draggableLayer);


        foreach (RaycastHit hit in hits)
        {
            print(hit.collider.gameObject);
            if (hit.collider.gameObject == gameObject && !isHovering)
            {
                isHovering = true;
                batteryConsumptionTxt.text = batteryConsumption.ToString();
            }
        }
        if (hits.Length == 0)
        {
            isHovering = false;
            batteryConsumptionTxt.text = "";
        }


        if (Input.GetMouseButtonDown(0))
        {
            foreach (RaycastHit hit in hits)
            {
                print(hit.collider.gameObject);
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;

                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = cam.WorldToScreenPoint(transform.position).z;
                    offset = transform.position - cam.ScreenToWorldPoint(mousePosition);

                    posBeforePickedUp = transform.position;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging)
                return;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cam.WorldToScreenPoint(transform.position).z;
            Vector3 targetPosition = cam.ScreenToWorldPoint(mousePosition) + offset;

            Vector3 localTargetPosition = transform.parent.InverseTransformPoint(targetPosition);
            localTargetPosition.x = Mathf.Clamp(localTargetPosition.x, -0.5f, 0.5f);
            localTargetPosition.z = Mathf.Clamp(localTargetPosition.z, -0.5f, 0.5f);
            targetPosition = transform.parent.TransformPoint(localTargetPosition);

            targetPosition.y = posBeforePickedUp.y + 0.1f;
            transform.position = targetPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
                return;

            isDragging = false;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            resourceDispatcherPuzzle.DroppedDraggable(this, batteryConsumption);
        }
    }







    public void ReturnToPosBeforePickedUp()
    {
        transform.position = posBeforePickedUp;
    }
}

//void OnMouseDown()
//{
//    Vector3 mousePosition = Input.mousePosition;
//    mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
//    offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);

//    posBeforePickedUp = transform.position;
//}

//void OnMouseDrag()
//{
//    Vector3 mousePosition = Input.mousePosition;
//    mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
//    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;

//    targetPosition.y = posBeforePickedUp.y + 0.1f;
//    transform.position = targetPosition;
//}

//private void OnMouseUp()
//{
//    transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
//    ResourceDispatcherPuzzle.instance.DroppedDraggable(this, batteryConsumption);
//}

//private void OnMouseEnter()
//{
//    batteryConsumptionTxt.text = batteryConsumption.ToString();
//}
//private void OnMouseExit()
//{
//    batteryConsumptionTxt.text = "";
//}
