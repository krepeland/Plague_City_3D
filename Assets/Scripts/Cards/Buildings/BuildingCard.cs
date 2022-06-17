using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingCard", menuName = "Cards/Building", order = 1)]
public class BuildingCard : ScriptableObject
{
    public string CardName;
    [SerializeField] private string PrefabName;

    public List<Vector2Int> BuildingPositions = new List<Vector2Int>() { Vector2Int.zero };
    public bool IsConnectingToRoad;

    public bool IsUnitsCanWalkThrough;
    public bool IsCanBePlacedOnEarth = true;
    public bool IsCanBePlacedOnWater;
    public bool IsCanBePlacedOnlyOnRoad;

    public List<EBuilding> CanReplaceBuildingsTypes = new List<EBuilding>();

    public Sprite BuildingInfoSprite;

    [HideInInspector] public Transform prefab_placedBuilding;
    [HideInInspector] public Transform prefab_preview;

    public bool LoadPrefab() {
        if (PrefabName == "")
        {
            Debug.LogError($"Building card with name {CardName} has empty Prefab Name");
            return false;
        }
        prefab_placedBuilding = PrefabManager.GetPrefab($"Buildings_{PrefabName}");
        prefab_preview = PrefabManager.GetPrefab($"BuildingsPreviews_{PrefabName}");
        return true;
    }
}
