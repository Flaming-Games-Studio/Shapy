using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour
{
    [HideInInspector]
    public CardData cardData;
    [HideInInspector]
    public Image cardImage;

    public void Initialize()
    {
        cardImage = GetComponent<Image>();
        cardImage.sprite = cardData.back;
    }

    public void FlipCard(bool force)
    {
        if (!MemoryGameManager.Instance.CanSelectCard() && !force)
        {
            print("Can't select another card at the moment!");
            return;
        }
        if (cardImage.sprite == cardData.back) 
        {
            cardImage.sprite = cardData.face;
            MemoryGameManager.Instance.SelectCard(this);
        }
        else
        {
            cardImage.sprite = cardData.back;
            MemoryGameManager.Instance.DeselectCard(this);
        }
    }
}