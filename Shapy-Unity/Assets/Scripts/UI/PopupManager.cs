using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    public Transform PopupMainPanel;
    public GameObject popupMsgPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //public void Start()
    //{
    //    RandomPopup();
    //}

    public void RandomPopup()
    {
        GenerateNewPopupMessage("Reward", "You recieved a sosage, sosage!");
    }
    public void GenerateNewPopupMessage(string title, string message)
    {
        PopupMessage pm = Instantiate(popupMsgPrefab, PopupMainPanel).GetComponent<PopupMessage>();
        pm.title.text = title;
        pm.message.text = message;

        pm.StartFade();
    }
}
