using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryboardManager : MonoBehaviour
{
    public GameObject paperPrefab;
    public List<StorySet> stories = new List<StorySet>();
    public Transform paperAnchor;
    public GameObject buttonsGO;
    public GameObject panelsGO;


    private StorySet currentStorySet;



    private void Start()
    {
        buttonsGO.SetActive(false);
        panelsGO.SetActive(true);
        GetNextStory();
        GenerateStoryPapers();
        enabled = true;
    }

    public void StartInteraction()
    {
        enabled = true;
        buttonsGO.SetActive(true);
        panelsGO.SetActive(false);
    }

    public void EndInteraction()
    {

        buttonsGO.SetActive(false);
        panelsGO.SetActive(true);
        enabled = false;
    }

    public void GetNextStory()
    {
        //check complete stories with game manager
        currentStorySet = stories[(Random.Range(0, stories.Count))];
    }

    public void GenerateStoryPapers()
    {
        for (int i = 0; i < currentStorySet.set.Count; i++) 
        {
            TextMeshPro storyText = Instantiate(paperPrefab, paperAnchor).transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
            storyText.text = currentStorySet.set[i].storyText;
        }
    }
}
