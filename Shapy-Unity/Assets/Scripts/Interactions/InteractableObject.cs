using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("InteractableObject")]
    public string sceneToLoad;
    public GameObject interactionWorldCanvas;
    public GameObject canvasToLoad;
    public SphereCollider col;
    public bool puzzle = false;

    protected virtual void Start()
    {
        TryGetComponent<SphereCollider>(out col);
        if (interactionWorldCanvas != null)
        {
            interactionWorldCanvas.SetActive(false);
        }
        if (canvasToLoad != null)
        {
            canvasToLoad.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionWorldCanvas != null)
            {
                interactionWorldCanvas.transform.LookAt(FPSController.Instance.playerCamera.transform);
                interactionWorldCanvas.transform.Rotate(0, 180, 0);

                if (!interactionWorldCanvas.activeSelf && !puzzle)
                {
                    interactionWorldCanvas.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionWorldCanvas != null)
            {
                if (!interactionWorldCanvas.activeSelf && !puzzle)
                {
                    interactionWorldCanvas.SetActive(true);
                }
            }
        }
    }

    public virtual void Interact()
    {
        print("Interacting the wrong way!");
        // vidi keypadpasscode za primjer prijave i odjave
    }
   

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionWorldCanvas != null)
            {
                if (interactionWorldCanvas.activeSelf)
                {
                    interactionWorldCanvas.SetActive(false);
                }
            }
        }
    }
}
