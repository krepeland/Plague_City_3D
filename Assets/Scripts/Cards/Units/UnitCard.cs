using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCard", menuName = "Cards/Unit", order = 1)]
public class UnitCard : ScriptableObject
{
    public string CardName;
    [SerializeField] private string PrefabName;
    public UnitTeam UnitTeam;

    [Range(1, 500)]
    public int Speed = 10;
    [Space(height: 20)]
    public bool GoInHandWhenTired = true;
    public bool UseStamina = true;
    public int StaminaMax = 16;
    public int RestTime = 8;
    [Space(height: 20)]
    public int Health = 3;
    public int Damage = 1;
    [Range(0, 100)]
    public int DodgePercent = 10;
    public float AttacksPerHour = 2;
    [Space(height: 20)]

    public Sprite MarkSprite;

    public List<string> BuildingsOwners = new List<string>();

    [HideInInspector] public Transform prefab_unit;

    public bool LoadPrefab()
    {
        if (PrefabName == "")
        {
            Debug.LogError($"Unit card with name {CardName} has empty Prefab Name");
            return false;
        }
        prefab_unit = PrefabManager.GetPrefab($"Units_{PrefabName}");
        return true;
    }
}
