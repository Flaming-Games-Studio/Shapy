using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform playerCam;
    public float rotationLerpSpeed = 5.0f;


    public void AdjustPortalCamera()
    {
        playerCam = Camera.main.transform;
        transform.rotation = Quaternion.Lerp(transform.rotation, playerCam.rotation, Time.deltaTime * rotationLerpSpeed);
    }
}
