using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager Instance;

    public GameObject cardPanel;
    public GameObject cardPrefab;
    public CardSet cardSet;

    private Card firstSelected = null, secondSelected = null;
    private List<Card> cards = new List<Card>();
    private GridLayoutGroup grid;

    private int maxPairs = 99;

    int basePadding = 25;
    Vector2 cardSize = new Vector2(100f, 150f);

    private void Start()
    {
        Instance = this;
        //UIController.Instance.ToggleUIControls(true);
        //ContextMenu.Instance.uiMenu.RemoveAllListeners();
        grid = cardPanel.GetComponent<GridLayoutGroup>();
        SetGridLayout();
        SpawnCardsAndInitiateStartingValues(maxPairs);
    }

    public void SetMaxPairs(int max)
    {
        if (cardSet.set.Count <= max)
        {
            maxPairs = cardSet.set.Count;
        }
        else
        {
            maxPairs = max;
        }
        print("Max pairs set to " + maxPairs.ToString());
        
    }
    private void SetGridLayout()
    {
        grid.padding = new RectOffset(basePadding * (Screen.width / Screen.height), basePadding * (Screen.width / Screen.height), basePadding * (Screen.width / Screen.height), basePadding * (Screen.width / Screen.height));
        grid.cellSize = cardSize * (Screen.width / Screen.height);
        grid.spacing = new Vector2(basePadding * (Screen.width/Screen.height), basePadding * (Screen.width / Screen.height));

        SetMaxPairs(maxPairs);
    }

    private void SpawnCardsAndInitiateStartingValues(int maxPairs)
    {
        for (int y = 0; y < 2; y++)
        {
            if (y == 0)
            {
                for (int i = 0; i < maxPairs; i++)
                {
                    GameObject t = Instantiate(cardPrefab, cardPanel.gameObject.transform);
                    cards.Add(t.GetComponent<Card>());
                    cards[i].cardData = cardSet.set[i];
                    cards[i].Initialize();
                    cards[i].cardData = cardSet.set[i];
                }
            }
            if (y == 1)
            {
                for (int i = 0; i < maxPairs; i++)
                {
                    GameObject t = Instantiate(cardPrefab, cardPanel.gameObject.transform);
                    cards.Add(t.GetComponent<Card>());
                    cards[i + maxPairs].cardData = cardSet.set[i];
                    cards[i + maxPairs].Initialize();
                    cards[i + maxPairs].cardData = cardSet.set[i];
                }
            }
        }
        RandomizePositions();
    }

    private void RandomizePositions()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(UnityEngine.Random.Range(0, cardPanel.transform.childCount));
        }
    }

    public void SelectCard(Card card)
    {
        if (firstSelected == null)
        {
            firstSelected = card;
            return;
        }
        if (secondSelected == null && firstSelected != null && firstSelected != card)
        {
            secondSelected = card;
            StartCoroutine(CheckCards());
        }
    }

    public void DeselectCard(Card card)
    {
        if (firstSelected == card)
        {
            firstSelected = null;
            return;
        }
    }
    

    private IEnumerator CheckCards()
    {
        yield return new WaitForSeconds(1.5f);
        if (firstSelected.cardData.face.name == secondSelected.cardData.face.name)
        {
            print("Bravo, you matched a pair!");
            if (grid.enabled)
            {
                grid.enabled = false;
            }
            Destroy(firstSelected.gameObject);
            Destroy(secondSelected.gameObject);
            firstSelected = null;
            secondSelected = null;
        }
        else
        {
            print("No match!");
            firstSelected.FlipCard(true);
            secondSelected.FlipCard(true);
            firstSelected = null;
            secondSelected = null;
        }
        StartCoroutine(CheckWinCondition());
    }

    private IEnumerator CheckWinCondition()
    {
        yield return new WaitForSeconds(0.5f);
        cards.RemoveAll(item => item == null);

        if (cards.Count == 0)
        {
            //insert win logic here! :-)
            //UIController.Instance.ToggleUIControls(false);

            print("You matched all pairs! Good job!");
            //SceneManager.LoadScene("Home");
        }
        else print("More pairs to match!");
    }

    public bool CanSelectCard()
    {
        if (firstSelected != null && secondSelected != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
