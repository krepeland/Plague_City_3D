using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyCardData", menuName = "Cards/BuyCardData", order = 2)]
public class BuyCardData : ScriptableObject
{
    public string CardName;
    public List<ResourceRequired> ResourcesRequired = new List<ResourceRequired>();
    public bool IsCanBeSold = true;
    public List<ResourceRequired> ResourcesFromSold = new List<ResourceRequired>();

    public List<BuildingRequired> BuildingsRequired = new List<BuildingRequired>();

    public Sprite UnlockedSprite;
    public Sprite LockedSprite;

    public bool IsShowInShop = false;
    public int priority;
}
