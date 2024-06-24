using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractions : InteractableObject
{
    public Animator anim;
    bool areFrontDoorsClosed = true; // Assuming the initial state is closed
    BoxCollider doorCol;
    private void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        doorCol = GetComponent<BoxCollider>();
    }
    public override void Interact()
    {
        anim.SetBool("DoorOpen", areFrontDoorsClosed ? false : true);
        doorCol.enabled = !areFrontDoorsClosed;
    }

    public void ChangeDoorBool()
    {
        areFrontDoorsClosed = !areFrontDoorsClosed;
    }
}
