using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalActivator : MonoBehaviour
{
    public string sceneToLoad;
    public PortalCamera pc;
    public GameObject portalUI;
    private Animator animator;

    private void Start()
    {
        if (portalUI != null ) 
        {
            portalUI.SetActive(false);
        }
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("OpenDoors");
            if (portalUI != null)
            {
                if (!portalUI.activeSelf)
                {
                    portalUI.SetActive(true);
                }
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pc != null)
            {
                pc.AdjustPortalCamera();
            }
        }
    }

    private void Interact()
    {
        print("Activating magic portal!");
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("CloseDoors");
            if (portalUI != null)
            {
                if (portalUI.activeSelf)
                {
                    portalUI.SetActive(false);
                }
            }
        }
    }
}
