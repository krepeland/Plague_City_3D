using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static Dictionary<string, Transform> PrefabDictionary = new Dictionary<string, Transform>();

    public static List<string> FoldersNames = new List<string>() {
        "LevelMap", "Managers", "Earth", "Houses", "Roads", "Bridges", 
        "Ladders", "Walls", "Cards", "Buildings", "BuildingsPreviews", 
        "UI", "BuildingInfoBlocks", "Units",
    };

    public static PrefabManager singleton;

    void Awake()
    {
        if (singleton != null)
            return;

        singleton = this;

        PrefabDictionary = new Dictionary<string, Transform>();
        foreach (var prefab in Resources.LoadAll<Transform>("Prefabs"))
        {
            PrefabDictionary[prefab.name] = prefab;
        }

        foreach (var folderName in FoldersNames)
        {
            foreach (var prefab in Resources.LoadAll<Transform>($"Prefabs/{folderName}"))
            {
                PrefabDictionary[$"{folderName}_{prefab.name}"] = prefab;
            }
        }

        DontDestroyOnLoad(this);
    }

    public static Transform GetPrefab(string name)
    {
        if (PrefabDictionary.ContainsKey(name))
            return PrefabDictionary[name];

        Debug.LogError($"No prefab with name \"{name}\"");
        return PrefabDictionary["Error"];
    }
}
