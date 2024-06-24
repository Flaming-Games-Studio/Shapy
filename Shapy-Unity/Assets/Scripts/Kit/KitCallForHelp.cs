using Inworld;
using Inworld.Sample.Innequin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitCallForHelp : MonoBehaviour
{
    public InworldCharacter3D m_CurrentCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }
}
