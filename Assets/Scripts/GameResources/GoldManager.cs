using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager singleton;

    private void Awake()
    {
        singleton = this;
    }

    public void TickGold() {
        var gold = 0;
        foreach (var e in TownManager.singleton.GetAllHouses())
        {
            if (e.IsEmptyHouse)
                continue;
            gold += Mathf.Max((e.HealthyPeople + e.InfectedPeople) / 4, 1);
        }
        ResourceManager.singleton.AddResourceCount(EResource.Gold, gold, true);
    }
}
