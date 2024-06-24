using Inworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitTutorialInteractions : InteractableObject
{
    public GameObject realKit;
    private void Start()
    {
        base.Start();
        realKit.GetComponent<KitController>().fakeKit = gameObject;
    }
    public override void Interact()
    {
        gameObject.SetActive(false);
        realKit.SetActive(true);
        InworldController.Instance.Reconnect();
    }
}
