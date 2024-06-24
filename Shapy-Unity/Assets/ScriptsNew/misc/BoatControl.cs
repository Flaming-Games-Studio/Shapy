using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControl : InteractableObject
{
    public Transform boatTransf;
    public Camera cam;

    public float speed = 5f;
    public Transform startDestination;
    public Transform endDestination;

    private bool isMoveing;
    private bool moveTowardsEnd;



    public override void Interact()
    {
        ActivateBoat();
    }

    void ActivateBoat()
    {
        if (isMoveing)
            return;

        isMoveing = true;
        moveTowardsEnd = boatTransf.position != endDestination.position;

        FPSController.Instance.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        MoveLift(moveTowardsEnd);
    }
    void MoveLift(bool moveTowardsEnd)
    {
        if (!isMoveing)
            return;


        if (moveTowardsEnd)
        {
            boatTransf.position = Vector3.MoveTowards(boatTransf.position, endDestination.position, speed * Time.deltaTime);

            if (boatTransf.position == endDestination.position)
            {
                isMoveing = false;

                cam.gameObject.SetActive(false);
                FPSController.Instance.gameObject.SetActive(true);
                FPSController.Instance.movement.SetPosition(boatTransf.position);
            }
        }
        else
        {
            boatTransf.position = Vector3.MoveTowards(boatTransf.position, startDestination.position, speed * Time.deltaTime);

            if (boatTransf.position == startDestination.position)
            {
                isMoveing = false;

                cam.gameObject.SetActive(false);
                FPSController.Instance.gameObject.SetActive(true);
                FPSController.Instance.movement.SetPosition(boatTransf.position);
            }
        }
    }
}
