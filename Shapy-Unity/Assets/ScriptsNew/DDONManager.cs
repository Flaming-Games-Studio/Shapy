using UnityEngine;

public class DDONManager : MonoBehaviour
{
    public static DDONManager Instance;
    void Awake()
    {
        if (Instance != this || Instance == null)
        {
            print("Setting static dont destroy manager!");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            print("Destroying non Instance");
            Destroy(gameObject);
        }
    }
}
