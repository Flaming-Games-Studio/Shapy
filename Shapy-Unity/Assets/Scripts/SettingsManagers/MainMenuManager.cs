using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject loadingScreen;

    public void Start()
    {
        loadingScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(ShowMenuWithDelay());
        
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator ShowMenuWithDelay()
    {
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.Confined;
        loadingScreen.SetActive(false);
        optionsMenu.SetActive(false);

    }

    public void Options()
    {  
       mainMenu.SetActive(false);
       optionsMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}