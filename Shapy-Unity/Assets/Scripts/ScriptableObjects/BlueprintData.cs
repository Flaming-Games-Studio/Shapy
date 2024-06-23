using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintRecipe", menuName = "ScriptableObjects/Crafting/CraftingRecipe", order = 0)]
public class BlueprintData : ScriptableObject
{
    public ItemType ItemType;
    public string recipeName;
    public string recipeDescription;
    public string CU;
    public string T1;
    public string T2;

    public Sprite recipeImage;
    [SerializeField]
    public List<MaterialComponent> requiredMaterials = new List<MaterialComponent>();

    public InventoryItemData finalProduct;
}

public enum ItemType
{
    NA,
    Furniture, 
    Decoration,
    Tool,
    Accessorie
}


[Serializable]
public struct MaterialComponent
{
    public InventoryItemData item;
    public int quantity;
}