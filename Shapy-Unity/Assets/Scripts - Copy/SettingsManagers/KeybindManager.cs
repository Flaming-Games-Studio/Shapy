using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeybindManager : MonoBehaviour
{
    [System.Serializable]
    public class Keybind
    {
        public string actionName;
        public KeyCode defaultKey;
        [HideInInspector] public KeyCode currentKey;
        [HideInInspector] public Button button;
        [HideInInspector] public TextMeshProUGUI buttonText;
    }

    public List<Keybind> keybinds;
    private Keybind currentKeybind;

    private Dictionary<KeyCode, string> keySymbolMapping;

    void Start()
    {
        // Initialize key-symbol mappings
        keySymbolMapping = new Dictionary<KeyCode, string>
        {
            { KeyCode.BackQuote, "¸" },
            { KeyCode.Escape, "Esc" },
            { KeyCode.Alpha1, "1" },
            { KeyCode.Alpha2, "2" },
            { KeyCode.Alpha3, "3" },
            { KeyCode.Alpha4, "4" },
            { KeyCode.Alpha5, "5" },
            { KeyCode.Alpha6, "6" },
            { KeyCode.Alpha7, "7" },
            { KeyCode.Alpha8, "8" },
            { KeyCode.Alpha9, "9" },
            { KeyCode.Alpha0, "0" },
            { KeyCode.Tab, "Tab" },
            { KeyCode.Mouse0, "Left Click" },
            { KeyCode.Mouse1, "Right Click" },
           
        };


        // Initialize keybinds
        foreach (Keybind keybind in keybinds)
        {

            if (keybind.buttonText == null || keybind.button == null)
            {
                //Debug.LogError($"Keybind {keybind.actionName} is missing its button or buttonText component.");
                continue;
            }
            keybind.currentKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keybind.actionName, keybind.defaultKey.ToString()));
           // keybind.buttonText.text = keybind.currentKey.ToString();
            keybind.buttonText.text = GetKeySymbol(keybind.currentKey);
            keybind.buttonText.autoSizeTextContainer = true;
        }
    }

    public void StartRebind(Keybind keybind)
    {
        currentKeybind = keybind;
        keybind.buttonText.text = "";
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress()
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                currentKeybind.currentKey = keyCode;
                currentKeybind.buttonText.text = GetKeySymbol(keyCode);
                //currentKeybind.buttonText.text = keyCode.ToString();
                PlayerPrefs.SetString(currentKeybind.actionName, keyCode.ToString());
                PlayerPrefs.Save();
                currentKeybind = null;
                yield break;
            }
        }
    }


    private string GetKeySymbol(KeyCode keyCode)
    {
        // Return the symbol if it exists, otherwise return the key's name
        if (keySymbolMapping.TryGetValue(keyCode, out string symbol))
        {
            return symbol;
        }
        return keyCode.ToString();
    }
}
