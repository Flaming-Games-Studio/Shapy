using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseRotation : MonoBehaviour
{
    public Transform transformToFollow;
    public float mouseSensitivity = 100f; 
    private float xRotation = 0f;
    private float yRotation = 0f;
    public Vector3 offset;

    [Header("Sine wobble")]
    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    private Vector3 startPosition;


    void Start()
    {
        startPosition = transform.localPosition;
    }
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 followPosition = transformToFollow.position + offset;
        transform.position = new Vector3(followPosition.x, followPosition.y + amplitude * Mathf.Sin(Time.time * frequency), followPosition.z);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
