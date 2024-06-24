using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        //save itd.
        Application.Quit();
    }

    public void Pause()
    {
        //gameObject.SetActive(true);
    }

    public void Resume()
    {
        UIController.Instance.CloseUI(new UnityEngine.InputSystem.InputAction.CallbackContext());
        //gameObject.SetActive(false);
    }
}