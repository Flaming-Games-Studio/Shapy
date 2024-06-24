using UnityEngine;

public class LiftControl : InteractableObject
{
    public Transform basket;

    public float speed = 2.0f;
    public Transform downDestination;
    public Transform upDestination;

    [Tooltip("pogasit vrata lifta dok se voziš")]
    public LiftDoors liftDoorTriggerDown, liftDoorTriggerUp;

    private bool isMoveing;
    private bool moveUp;

    private void Start()
    {
        //base.Start();
        liftDoorTriggerDown.canOpen = true;
        liftDoorTriggerUp.canOpen = false;
    }

    void FixedUpdate()
    {
        MoveLift(moveUp);
    }

    public override void Interact()
    {
        ActivateLift();
    }

    void ActivateLift()
    {
        if (isMoveing)
            return;

        moveUp = basket.transform.position.y < upDestination.position.y;
        isMoveing = true;
        KitController.Instance.canOpenChat = false;

        liftDoorTriggerDown.canOpen = false;
        liftDoorTriggerUp.canOpen = false;
        liftDoorTriggerDown.doorsAC.SetBool("Open", false);
        liftDoorTriggerUp.doorsAC.SetBool("Open", false);
        FPSController.Instance.transform.parent = basket.transform;
        print(FPSController.Instance.transform.parent);
    }

    void MoveLift(bool moveUp)
    {
        if (moveUp)
        {
            basket.transform.position = Vector3.MoveTowards(basket.transform.position, upDestination.position, speed * Time.deltaTime);

            if (basket.transform.position.y >= upDestination.position.y)
            {
                isMoveing = false;
                KitController.Instance.canOpenChat = true;
                liftDoorTriggerUp.canOpen = true;
                FPSController.Instance.transform.parent = null;
            }
        }
        else
        {
            basket.transform.position = Vector3.MoveTowards(basket.transform.position, downDestination.position, speed * Time.deltaTime);

            if (basket.transform.position.y <= downDestination.position.y)
            {
                isMoveing = false;
                KitController.Instance.canOpenChat = true;
                liftDoorTriggerDown.canOpen = true;
                FPSController.Instance.transform.parent = null;
            }
        }
    }
}
