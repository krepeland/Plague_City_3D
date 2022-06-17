using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELevelData_FoodProblems { 
    NoFoodProblems,
    WeakFoodProblems,
    StrongFoodProblems,
    NoFood,
}

public enum ELevelData_DiscontentProblems
{
    NoDiscontentProblems,
    WeakDiscontentProblems,
    StrongDiscontentProblems,
}

public class LevelData
{
    public int Index;
    [Range(1, 3)]
    public int Difficulty = 1;

    public LevelStatus LevelStatus;

    public int Size;
    public Infection_Settings Infection_Settings;
    public bool InfectionProblems;
    public ELevelData_FoodProblems FoodProblems;
    public ELevelData_DiscontentProblems DiscontentProblems;

    public List<GameTask> MainTasks;
    public List<GameTask> ExtraTasks;

    public LevelData(
        int size,
        bool infectionProblems,
        ELevelData_FoodProblems foodProblems,
        ELevelData_DiscontentProblems discontentProblems)
    {
        Size = Mathf.Clamp(size, 0, 8);
        InfectionProblems = infectionProblems;
        FoodProblems = foodProblems;
        DiscontentProblems = discontentProblems;
    }

    public void SetInfectionSettings(Infection_Settings settings) {
        Infection_Settings = settings;
    }

    public void SetTasks(List<GameTask> mainTasks, List<GameTask> extraTasks) {
        MainTasks = mainTasks;
        ExtraTasks = extraTasks;

        //Debug.Log("-------------");
        //foreach (var e in mainTasks)
        //{
        //    Debug.Log("main: " + e);
        //}
        //foreach (var e in extraTasks)
        //{
        //    Debug.Log("extra: " + e);
        //}
    }
}
