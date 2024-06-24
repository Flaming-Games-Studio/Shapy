using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorySet", menuName = "ScriptableObjects/Story/StorySet", order = 3)]
public class StorySet : ScriptableObject
{
    public List<StoryData> set;
}