using UnityEngine;
using UnityEngine.UI;

public class ButtonMethodSubscribe : MonoBehaviour
{
    Button button;
    public InventoryItemData inventoryItemData;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { InventoryManager.Instance.AddItemToInventory(InventoryManager.Instance.CreateNewInventoryItemDataInstance(inventoryItemData)); });
        button.onClick.AddListener(delegate { UIController.Instance.ToggleUIControls(false); });
        button.onClick.AddListener(delegate { TogglePanel(); });
        button.onClick.AddListener(delegate { transform.parent.transform.parent.GetComponent<FormInfoData>().CollectAllData();});
    }

    public void TogglePanel()
    {
        gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
    }
}
