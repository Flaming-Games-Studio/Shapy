using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    #region variables
    [Space(10)]
    [Header("Buttons")]
    public Button craftButton;
    public Button furnitureButton;
    public Button decorationsButton;
    public Button toolsButton;
    public Button accessoriesButton;
    public Slider craftingProcessSlider;

    [Space(10)][Header("Panels")]
    public GameObject furniturePanel;
    public GameObject decorationsPanel;
    public GameObject toolsPanel;
    public GameObject accessoriesPanel;
    public GameObject craftingRequirementPanel;
    private List<ItemRequirementPrefab> craftingRequirementsPrefabs = new List<ItemRequirementPrefab>();

    [Space(10)][Header("DescriptionTexts")]
    public TextMeshProUGUI descNameText;
    public TextMeshProUGUI descDescriptionText;
    public TextMeshProUGUI descCUText;
    public TextMeshProUGUI descT1Text;
    public TextMeshProUGUI descT2Text;

    [Space(10)][Header("Prefabs")]
    public GameObject requiredComponentVisualPrefab;
    public GameObject blueprintVisualPrefab;
    [Header(null)]

    public ScrollRect blueprintsScrollRect;

    [Space(10)][Header("Blueprints")]
    private List<List<BlueprintData>> blueprintDataList = new List<List<BlueprintData>>();
    public List<BlueprintData> furnitureBlueprintData = new List<BlueprintData>();
    public List<BlueprintData> decorationsBlueprintData = new List<BlueprintData>();
    public List<BlueprintData> toolsBlueprintData = new List<BlueprintData>();
    public List<BlueprintData> accessoriesBlueprintData = new List<BlueprintData>();
    private BlueprintData currentSelectedBlueprint;
    #endregion

    private void Start()
    {
        craftButton.interactable = false;
        SetAllListeners();
        LoadRecipesData();
        UIController.Instance.ToggleCraftingCanvas(new InputAction.CallbackContext());
    }

    private void SetAllListeners()
    {
        craftButton.onClick.AddListener(StartCraftingButtonCallback);
        furnitureButton.onClick.AddListener(delegate { SwitchBlueprintPanel(0); });
        decorationsButton.onClick.AddListener(delegate { SwitchBlueprintPanel(1); });
        toolsButton.onClick.AddListener(delegate { SwitchBlueprintPanel(2); });
        accessoriesButton.onClick.AddListener(delegate { SwitchBlueprintPanel(3); });
    }

    public void LoadRecipesData()
    {
        blueprintDataList.Add(furnitureBlueprintData);
        blueprintDataList.Add(decorationsBlueprintData);
        blueprintDataList.Add(toolsBlueprintData);
        blueprintDataList.Add(accessoriesBlueprintData);
        for (int i = 0; i < blueprintDataList.Count; i++)
        {
            switch (i)
            {
                default:
                    print("Eternal void in universe " + i.ToString());
                    break;
                case 0:
                    for (int j = 0; j < blueprintDataList[i].Count; j++)
                    {
                        GameObject go = Instantiate(blueprintVisualPrefab, furniturePanel.transform.GetChild(0).transform);
                        BlueprintVisualPrefab bvp = go.GetComponent<BlueprintVisualPrefab>();
                        bvp.blueprintImage.sprite = furnitureBlueprintData[j].recipeImage;
                        bvp.blueprintName.text = furnitureBlueprintData[j].recipeName;
                        bvp.blueprintData = CreateNewBlueprintDataInstance(furnitureBlueprintData[j]);
                        bvp.blueprintButton.onClick.RemoveAllListeners();
                        bvp.blueprintButton.onClick.AddListener(() => this.LoadSelectedBlueprintData(bvp.blueprintData));
                        bvp.CM = this;
                    }
                    break;
                case 1:
                    for (int j = 0; j < blueprintDataList[i].Count; j++)
                    {
                        GameObject go = Instantiate(blueprintVisualPrefab, decorationsPanel.transform.GetChild(0).transform);
                        BlueprintVisualPrefab bvp = go.GetComponent<BlueprintVisualPrefab>();
                        bvp.blueprintImage.sprite = decorationsBlueprintData[j].recipeImage;
                        bvp.blueprintName.text = decorationsBlueprintData[j].recipeName;
                        BlueprintData bd = CreateNewBlueprintDataInstance(decorationsBlueprintData[j]);
                        bvp.blueprintButton.onClick.RemoveAllListeners();
                        bvp.blueprintButton.onClick.AddListener(() => this.LoadSelectedBlueprintData(bd));
                        bvp.CM = this;
                    }
                    break;
                case 2:
                    for (int j = 0; j < blueprintDataList[i].Count; j++)
                    {
                        GameObject go = Instantiate(blueprintVisualPrefab, toolsPanel.transform.GetChild(0).transform);
                        BlueprintVisualPrefab bvp = go.GetComponent<BlueprintVisualPrefab>();
                        bvp.blueprintImage.sprite = toolsBlueprintData[j].recipeImage;
                        bvp.blueprintName.text = toolsBlueprintData[j].recipeName;
                        bvp.blueprintData = CreateNewBlueprintDataInstance(toolsBlueprintData[j]);
                        bvp.blueprintButton.onClick.RemoveAllListeners();
                        bvp.blueprintButton.onClick.AddListener(() => this.LoadSelectedBlueprintData(bvp.blueprintData));
                        bvp.CM = this;
                    }
                    break;
                case 3:
                    for (int j = 0; j < blueprintDataList[i].Count; j++)
                    {
                        GameObject go = Instantiate(blueprintVisualPrefab, accessoriesPanel.transform.GetChild(0).transform);
                        BlueprintVisualPrefab bvp = go.GetComponent<BlueprintVisualPrefab>();
                        bvp.blueprintImage.sprite = accessoriesBlueprintData[j].recipeImage;
                        bvp.blueprintName.text = accessoriesBlueprintData[j].recipeName;
                        BlueprintData bd = CreateNewBlueprintDataInstance(accessoriesBlueprintData[j]);
                        bvp.blueprintButton.onClick.RemoveAllListeners();
                        bvp.blueprintButton.onClick.AddListener(() => this.LoadSelectedBlueprintData(bd));
                        bvp.CM = this;
                    }
                    break;
            }
        }
    }

    public void LoadSelectedBlueprintData(BlueprintData blueprintData)
    {
        CleanCurrentRequirements();

        currentSelectedBlueprint = blueprintData;
        descNameText.text = currentSelectedBlueprint.recipeName;
        descDescriptionText.text = currentSelectedBlueprint.recipeDescription;
        descCUText.text = currentSelectedBlueprint.CU;
        descT1Text.text = currentSelectedBlueprint.T1;
        descT2Text.text = currentSelectedBlueprint.T2;

        for (int i = 0; i < currentSelectedBlueprint.requiredMaterials.Count; i++)
        {
            GameObject go = Instantiate(requiredComponentVisualPrefab, craftingRequirementPanel.transform);
            ItemRequirementPrefab irp = go.GetComponent<ItemRequirementPrefab>();
            craftingRequirementsPrefabs.Add(irp);
            irp.requirementImage.sprite = currentSelectedBlueprint.requiredMaterials[i].item.itemImage;
            irp.requirementText.text = InventoryManager.Instance.ReturnItemQuantity(currentSelectedBlueprint.requiredMaterials[i].item).ToString() + "/" + currentSelectedBlueprint.requiredMaterials[i].quantity.ToString();
        }

        if (CheckIfRequiredQuantitiesAreSatisfied())
        {
            craftButton.interactable = true;
        }
    }

    public BlueprintData CreateNewBlueprintDataInstance(BlueprintData data)
    {
        BlueprintData bd = ScriptableObject.Instantiate(data);
        bd.ItemType = data.ItemType;
        bd.recipeName = data.recipeName;
        bd.recipeDescription = data.recipeDescription;
        bd.CU = data.CU;
        bd.T1 = data.T1;
        bd.T2 = data.T2;
        bd.recipeImage = data.recipeImage;
        bd.requiredMaterials = new List<MaterialComponent>();
        bd.finalProduct = data.finalProduct;

        for (int i = 0; i < data.requiredMaterials.Count; i++)
        {
            MaterialComponent mc = new MaterialComponent()
            {
                item = InventoryManager.Instance.CreateNewInventoryItemDataInstanceWithDefinedQuantity(data.requiredMaterials[i].item, data.requiredMaterials[i].quantity),
                quantity = data.requiredMaterials[i].quantity
            };
            bd.requiredMaterials.Add(mc);
        }
        return bd;
    }

    public void StartCraftingButtonCallback()
    {
        StartCoroutine(StartCraftingProcess());
    }

    public IEnumerator StartCraftingProcess()
    {
        craftButton.interactable = false;
        craftButton.gameObject.SetActive(false);
        craftingProcessSlider.gameObject.SetActive(true);
        craftingProcessSlider.value = 0;

        yield return new WaitForEndOfFrame();

        while (craftingProcessSlider.value < craftingProcessSlider.maxValue)
        {
            craftingProcessSlider.value += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        InventoryManager.Instance.AddItemToInventory(InventoryManager.Instance.CreateNewInventoryItemDataInstance(currentSelectedBlueprint.finalProduct));
        CleanCurrentRequirements();

        for (int i = 0; i < currentSelectedBlueprint.requiredMaterials.Count; i++)
        {
            InventoryManager.Instance.RemoveItemFromInventory(currentSelectedBlueprint.requiredMaterials[i].item);
        }

        craftingProcessSlider.value = 0;
        craftingProcessSlider.gameObject.SetActive(false);
        craftButton.gameObject.SetActive(true);
    }

    private bool CheckIfRequiredQuantitiesAreSatisfied()
    {
        for (int i = 0; i < craftingRequirementsPrefabs.Count; i++) 
        {
            string[] quantities = craftingRequirementsPrefabs[i].requirementText.text.Split("/");
            if (int.Parse(quantities[0]) < int.Parse(quantities[1]))
            {
                return false;
            }
        }
        return true;
    }

    private void CleanCurrentRequirements()
    {
        craftingRequirementsPrefabs.Clear();
        foreach (Transform item in craftingRequirementPanel.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void UnsubscribeBlueprintOnClickEvents(BlueprintVisualPrefab bvp)
    {
        bvp.blueprintButton.onClick.RemoveAllListeners();
    }

    public void SwitchBlueprintPanel(int panelCase)
    {
        switch(panelCase)
        {
            default:
                print("Nothing");
                break;
            case 0:
                furniturePanel.gameObject.SetActive(true);
                decorationsPanel.gameObject.SetActive(false);
                toolsPanel.gameObject.SetActive(false);
                accessoriesPanel.gameObject.SetActive(false);
                break;
            case 1:
                furniturePanel.gameObject.SetActive(false);
                decorationsPanel.gameObject.SetActive(true);
                toolsPanel.gameObject.SetActive(false);
                accessoriesPanel.gameObject.SetActive(false);
                break;
            case 2:
                furniturePanel.gameObject.SetActive(false);
                decorationsPanel.gameObject.SetActive(false);
                toolsPanel.gameObject.SetActive(true);
                accessoriesPanel.gameObject.SetActive(false);
                break;
            case 3:
                furniturePanel.gameObject.SetActive(false);
                decorationsPanel.gameObject.SetActive(false);
                toolsPanel.gameObject.SetActive(false);
                accessoriesPanel.gameObject.SetActive(true);
                break;
        }

    }
}
