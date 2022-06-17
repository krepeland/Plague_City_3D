using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public int FoodRequired;

    public static FoodManager singleton;

    void Awake()
    {
        singleton = this;
    }

    public void TickFood()
    {
        FoodRequired = 0;
        List<B_House> NotFeededHouses = new List<B_House>();
        foreach (var e in TownManager.singleton.GetAllHouses())
        {
            if (e.IsEmptyHouse)
                continue;

            if (e.FeedState == EFeedState.Feed)
            {
                FoodRequired += Mathf.Max((e.HealthyPeople + e.InfectedPeople) / 4, 1) * 5;
            }
            else {
                if (e.FeedState == EFeedState.NotFeed) {
                    NotFeededHouses.Add(e);
                }
            }
        }

        if (ResourceManager.singleton.TrySubstractResource(EResource.Food, FoodRequired, out var delta, true))
        {
            Debug.Log("Enough food");
            foreach (var e in TownManager.singleton.GetAllHouses())
            {
                if (e.IsEmptyHouse)
                    continue;

                if (e.FeedState == EFeedState.Feed)
                {
                    e.AddHungerTick(-4);
                }
            }
        }
        else
        {
            Debug.Log("Not enough food");
            var foodUsed = FoodRequired + delta;
            var possibleHouses = new List<(B_House, int)>();
            foreach (var e in TownManager.singleton.GetAllHouses())
            {
                if (!e.IsEmptyHouse && e.FeedState == EFeedState.Feed)
                {
                    possibleHouses.Add((e, Mathf.Max((e.HealthyPeople + e.InfectedPeople) / 4, 1)));
                }
            }

            while (foodUsed > 0 && possibleHouses.Count > 0)
            {
                var index = Random.Range(0, possibleHouses.Count);
                var pair = possibleHouses[index];
                foodUsed -= pair.Item2;
                pair.Item1.AddHungerTick(-4);
                possibleHouses.RemoveAt(index);
            }
            foreach (var e in possibleHouses) {
                NotFeededHouses.Add(e.Item1);
            }

            ResourceManager.singleton.AddResourceCount(EResource.Food, delta, delta != 0);
        }

        foreach (var e in NotFeededHouses)
        {
            e.AddHungerTick(1);
        }
    }
}
