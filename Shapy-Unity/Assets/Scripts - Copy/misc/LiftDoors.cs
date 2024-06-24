using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LiftDoors : MonoBehaviour
{
    public Animator doorsAC;

    private AudioSource audioS;
    public AudioClip openDoorSound;

    [HideInInspector]
    public bool canOpen;


    //private void Start()
    //{
    //    audioS.GetComponent<AudioSource>();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canOpen)
        {
            doorsAC.SetBool("Open", true);
            //audioS.PlayOneShot(openDoorSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && canOpen)
        {
            doorsAC.SetBool("Open", false);
            //audioS.PlayOneShot(openDoorSound);
        }
    }
}
