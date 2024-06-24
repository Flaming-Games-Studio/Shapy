using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformSetter : MonoBehaviour
{
    [Header("Prazno ako je isti objekt")]
    public Transform setPlayerPositionTransform;

    public void Start()
    {
        if (setPlayerPositionTransform == null)
        {
            setPlayerPositionTransform = transform;
        }
        Transform playerTransform = FPSController.Instance.transform;
        playerTransform.position = setPlayerPositionTransform.position;
        playerTransform.rotation = setPlayerPositionTransform.rotation;
    }
}
