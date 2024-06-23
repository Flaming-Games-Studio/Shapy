using Inworld;
using Inworld.Sample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InworldCanvasControl : PlayerController3D
{
    public Camera overrideCam;
    public GameObject theKit;
    protected override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(optionKey) || Input.GetKeyDown(uiKey))
        {
            if (!IsAnyCanvasOpen)
            {
                UIController.Instance.UnsubscribeUIActionsForKit();
                UIController.Instance.ToggleUIControls(true);
                overrideCam.transform.position = FPSController.Instance.playerCamera.transform.position;
                overrideCam.transform.rotation = FPSController.Instance.playerCamera.transform.rotation;
                overrideCam.gameObject.SetActive(true);
                FPSController.Instance.playerCamera.gameObject.SetActive(false);
            }
            else
            {
                UIController.Instance.SubscribeUIActionsForKit();
                UIController.Instance.ToggleUIControls(false);
                overrideCam.gameObject.SetActive(false);
                FPSController.Instance.playerCamera.gameObject.SetActive(true);
            }
        }
    }
}
//    if (!m_OptionCanvas.activeSelf)
//    {
//        m_ChatCanvas.SetActive(false);
//        print("Disabling UI cuz Kit!");
//        UIController.Instance.UnsubscribeUIActions();
//        UIController.Instance.ToggleUIControls(true);
//        overrideCam.transform.position = FPSController.Instance.playerCamera.transform.position;
//        overrideCam.transform.rotation = FPSController.Instance.playerCamera.transform.rotation;
//        overrideCam.gameObject.SetActive(true);
//        FPSController.Instance.playerCamera.gameObject.SetActive(false);
//    }
//    if (m_OptionCanvas.activeSelf)
//    {
//        print("Enabling UI cuz Kit!");
//        UIController.Instance.SubscribeUIActions();
//        UIController.Instance.ToggleUIControls(false);
//        overrideCam.gameObject.SetActive(false);
//        FPSController.Instance.playerCamera.gameObject.SetActive(true);
//    }
//}
//if (Input.GetKeyDown(uiKey))
//{
//    if (!m_ChatCanvas.activeSelf)
//    {
//        m_OptionCanvas.SetActive(false);
//        print("Disabling UI cuz Kit!");
//        UIController.Instance.UnsubscribeUIActions();
//        UIController.Instance.ToggleUIControls(true);
//        overrideCam.transform.position = FPSController.Instance.playerCamera.transform.position;
//        overrideCam.transform.rotation = FPSController.Instance.playerCamera.transform.rotation;
//        overrideCam.gameObject.SetActive(true);
//        FPSController.Instance.playerCamera.gameObject.SetActive(false);
//    }
//    if (m_ChatCanvas.activeSelf)
//    {
//        print("Enabling UI cuz Kit!");
//        UIController.Instance.SubscribeUIActions();
//        UIController.Instance.ToggleUIControls(false);
//        overrideCam.gameObject.SetActive(false);
//        FPSController.Instance.playerCamera.gameObject.SetActive(true);
//    }