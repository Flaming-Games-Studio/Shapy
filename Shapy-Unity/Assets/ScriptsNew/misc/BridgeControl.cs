using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeControl : MonoBehaviour
{
    public Transform bridgeLeftSide, bridgeRightSide;
    private Vector3 camStartingPos;
    private Quaternion camStartingRot;
    public Camera cam;
    public Transform bridgeFocusPoint; // The point where the camera should focus before lowering the bridge
    public float camMoveDuration = 2.0f; // Duration to move the camera to the focus point
    public float bridgeLowerDuration = 3.0f; // Duration to lower the bridge

    bool bridgeDown = false;

    private void Start()
    {
        camStartingPos = cam.transform.position;
        camStartingRot = cam.transform.rotation;
    }

    public void OpenBridge()
    {
        StartCoroutine(LowerBridgeSequence());
    }

    public void ForceBridgeDown()
    {
        Quaternion leftTargetRotation = Quaternion.Euler(0, bridgeLeftSide.localEulerAngles.y, bridgeLeftSide.localEulerAngles.z);
        Quaternion rightTargetRotation = Quaternion.Euler(0, bridgeRightSide.localEulerAngles.y, bridgeRightSide.localEulerAngles.z);
        bridgeLeftSide.localRotation = leftTargetRotation;
        bridgeRightSide.localRotation = rightTargetRotation;
    }

    public void SetBridgeState(bool state)
    {
        bridgeDown = state;
    }
    public bool AlreadyDown()
    { return bridgeDown; }

    private IEnumerator LowerBridgeSequence()
    {
        bridgeDown = true;
        yield return StartCoroutine(ReturnCameraToStart());

        yield return StartCoroutine(LowerBridge(bridgeLeftSide, -90f, 0f, bridgeLowerDuration));
        yield return StartCoroutine(LowerBridge(bridgeRightSide, -90f, 0f, bridgeLowerDuration));

        yield return new WaitForSeconds(1.5f);

        cam.gameObject.SetActive(false);
        FPSController.Instance.gameObject.SetActive(true);
    }

    private IEnumerator LowerBridge(Transform bridgePart, float fromAngle, float toAngle, float duration)
    {
        float elapsed = 0.0f;
        Quaternion initialRotation = bridgePart.localRotation;
        Quaternion targetRotation = Quaternion.Euler(toAngle, bridgePart.localEulerAngles.y, bridgePart.localEulerAngles.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float angle = Mathf.Lerp(fromAngle, toAngle, t);
            bridgePart.localRotation = Quaternion.Euler(angle, bridgePart.localEulerAngles.y, bridgePart.localEulerAngles.z);
            yield return null;
        }

        bridgePart.localRotation = targetRotation;
    }

    private IEnumerator ReturnCameraToStart()
    {
        cam.gameObject.SetActive(true);
        cam.transform.position = FPSController.Instance.playerCamera.transform.position;
        FPSController.Instance.gameObject.SetActive(false);

        float elapsed = 0.0f;
        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;

        while (elapsed < camMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / camMoveDuration);
            cam.transform.position = Vector3.Lerp(startPos, camStartingPos, t);
            cam.transform.rotation = Quaternion.Lerp(startRot, camStartingRot, t);
            yield return null;
        }

        cam.transform.position = camStartingPos;
        cam.transform.rotation = camStartingRot;
    }
}
