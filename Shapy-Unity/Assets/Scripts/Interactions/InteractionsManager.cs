using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionsManager : MonoBehaviour
{
    public Vector3 size; //0.7,1,1
    private GameObject lastInteractedGO = null;

    private void Start()
    {
        UIController.Instance.inputModule.actionsAsset.actionMaps[0].FindAction("Interact").performed += CheckForInteractableObjects;
        
    }
  
    private void CheckForInteractableObjects(InputAction.CallbackContext context)
    {
        if (transform == null) { return; }
        Vector3 boxPos = transform.position + transform.forward * 1f + transform.up * (size.y / 2);
        Quaternion offset = Quaternion.LookRotation(transform.forward);
        StartCoroutine(CheckBoundingBox(boxPos, offset));
    }

    public IEnumerator CheckBoundingBox(Vector3 boxPos, Quaternion offset)
    {
        Collider[] hitColliders = Physics.OverlapBox(boxPos, size, offset);
        List<GameObject> ogObjects = new List<GameObject>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (!ogObjects.Contains(hitColliders[i].gameObject))
            {
                ogObjects.Add(hitColliders[i].gameObject);
            }
        }
   
        foreach (var interactionObject in ogObjects)
        {
            interactionObject.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
        }
        yield return new WaitForEndOfFrame();
    }
}
