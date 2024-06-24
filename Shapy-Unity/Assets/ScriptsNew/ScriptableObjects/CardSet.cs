using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemorySet", menuName = "ScriptableObjects/Memory/MemorySet", order = 2)]
public class CardSet : ScriptableObject
{
    public List<CardData> set;
}
