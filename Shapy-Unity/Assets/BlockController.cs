using TMPro;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public int baseHp = 1; // Adjust the maximum HP as needed
    private int currentHP;
    private TextMeshProUGUI hpTextElement;

    void Start()
    {
        currentHP = baseHp;
        
        UpdateHP();
    }

    // Method to reduce the block's HP
    public void ReduceHP(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            // Destroy the block if its HP is depleted
            Destroy(gameObject);
        }
        else
        {
            // Add any visual feedback or update the block appearance based on current HP
            // For example, change the color or scale of the block.
            UpdateHP();
        }
    }

    public void UpdateHP()
    {
        if (hpTextElement != null)
        {
            hpTextElement.text = currentHP.ToString();
        }
        else
        {
            hpTextElement = GetComponentInChildren<TextMeshProUGUI>();
            hpTextElement.text = currentHP.ToString();
        }
    }

    public int GetBlockHP()
    {
        return currentHP;
    }

    public void SetBlockHP(int hp)
    {
        currentHP *= hp;
    }
}
