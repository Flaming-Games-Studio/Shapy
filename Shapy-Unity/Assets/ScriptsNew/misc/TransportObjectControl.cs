using UnityEngine;

// Ovo pokrece boat i cable car (i bilo sta drugo)
public class TransportObjectControl : InteractableObject
{
    [Header("objekt koji krecemo")]
    public Transform transportObjectTransform;
    public Camera cam;

    [Header("ubrzavanje na pocetku i usporavanje na kraju")]
    public float maxSpeed = 10f;
    public float accelerationDistance = 20f;
    public Transform startDestination;
    public Transform endDestination;

    [Tooltip("moze ostat prazno")]
    public Animator doorAnimator;

    private bool isMoveing;
    private bool moveTowardsEnd;




    public override void Interact()
    {
        ActivateLift();
    }

    void ActivateLift()
    {
        if (isMoveing)
            return;

        isMoveing = true;
        KitController.Instance.canOpenChat = false;
        moveTowardsEnd = transportObjectTransform.transform.position != endDestination.position;
        if(doorAnimator != null)
            doorAnimator.SetBool("Open", false);

        FPSController.Instance.gameObject.SetActive(false);
        cam.gameObject.transform.rotation = FPSController.Instance.transform.rotation;
        cam.gameObject.SetActive(true);
        print(cam.gameObject.transform.eulerAngles + " CAM ::::: " + FPSController.Instance.transform.eulerAngles + " PLAYER");
    }

    void FixedUpdate()
    {
        MoveLift(moveTowardsEnd);
    }
    void MoveLift(bool moveTowardsEnd)
    {
        if (!isMoveing)
            return;

        float distanceToEnd = Vector3.Distance(transportObjectTransform.position, endDestination.position);
        float distanceToStart = Vector3.Distance(transportObjectTransform.position, startDestination.position);
        float speed = CalculateSpeed(distanceToStart, distanceToEnd);


        if (moveTowardsEnd)
        {
            transportObjectTransform.transform.position = Vector3.MoveTowards(transportObjectTransform.transform.position, endDestination.position, speed * Time.deltaTime);

            if (distanceToEnd < 0.05f)
            {
                isMoveing = false;
                KitController.Instance.canOpenChat = true;
                if (doorAnimator != null)
                    doorAnimator.SetBool("Open", true);

                cam.gameObject.SetActive(false);
                FPSController.Instance.gameObject.SetActive(true);
                FPSController.Instance.movement.SetPosition(transportObjectTransform.transform.position);
                FPSController.Instance.movement.transform.eulerAngles = new Vector3(FPSController.Instance.movement.transform.eulerAngles.x, cam.gameObject.transform.eulerAngles.y, FPSController.Instance.movement.transform.eulerAngles.z);
            }
        }
        else
        {
            transportObjectTransform.transform.position = Vector3.MoveTowards(transportObjectTransform.transform.position, startDestination.position, speed * Time.deltaTime);

            if (distanceToStart < 0.05f)
            {
                isMoveing = false;
                KitController.Instance.canOpenChat = true;
                if (doorAnimator != null)
                    doorAnimator.SetBool("Open", true);


                cam.gameObject.SetActive(false);
                FPSController.Instance.gameObject.SetActive(true);
                FPSController.Instance.movement.SetPosition(transportObjectTransform.transform.position);
                FPSController.Instance.movement.transform.eulerAngles = new Vector3(FPSController.Instance.movement.transform.eulerAngles.x, cam.gameObject.transform.eulerAngles.y, FPSController.Instance.movement.transform.eulerAngles.z);
            }
        }
    }

    private float CalculateSpeed(float distanceToStart, float distanceToEnd)
    {
        float speed = maxSpeed;

        if (distanceToStart < accelerationDistance)
        {
            speed = Mathf.Lerp(1f, maxSpeed, distanceToStart / accelerationDistance);
        }
        else if (distanceToEnd < accelerationDistance)
        {
            speed = Mathf.Lerp(maxSpeed, 1f, (accelerationDistance - distanceToEnd) / accelerationDistance);
        }

        return speed;
    }
}
