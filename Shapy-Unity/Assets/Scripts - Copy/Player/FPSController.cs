using UnityEngine;

public class FPSController : MonoBehaviour
{
    public static FPSController Instance;

    public Camera playerCamera;  // Assign the Camera object from the child Capsule in the Inspector
    public vp_FPController movement;
    public vp_FPInput input;

    [Header("Transforms")]
    public Transform startingPosition;
    public bool canMove = true;


    public void Awake()
    {
        if (Instance != this || Instance == null)
        {
            print("Setting static player controls!");
            Instance = this;
        }
        else
        {
            print("Destroying non Instance");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ResetPlayerPosition();
    }

    void Update()
    {
        if (canMove)
        {
            if (!movement.enabled)
            {
                movement.enabled = true;
                input.enabled = true;
            }
        }
        else
        {
            if (movement.enabled)
            {  
                movement.enabled = false;
                input.enabled = false;
            }
        }
    }

    public void ResetPlayerPosition()
    {
        transform.position = startingPosition.position;
        transform.rotation = startingPosition.rotation;
    }

    public void ResetPlayerPosition(Vector3 newPos,Quaternion newRot)
    {
        transform.position = newPos;
        transform.rotation = newRot;
        playerCamera.transform.rotation = newRot;
    }

}