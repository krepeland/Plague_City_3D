using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelMapManager
{
    public static List<LevelData> LevelsData;

    public static Dictionary<Vector2Int, LevelData> LevelsDataPoses;
    public static Dictionary<Vector2Int, List<Vector2Int>> LevelsConnections;

    public static Dictionary<Vector2Int, Vector3> PosOnCoords;

    public static List<LevelData> LevelsTutorialData = new List<LevelData>() { 
        new LevelData(1, false, ELevelData_FoodProblems.NoFood, ELevelData_DiscontentProblems.NoDiscontentProblems),
    };

    public static Vector2Int SelectedLevel;

    public static PacifistMode PacifistModeNow = PacifistMode.No;
    public static bool IsLevelStarted;

    public static void CreateOrLoadMap()
    {
        if (LevelsData == null)
        {
            //new List<int>() { -2, -1, 0, 1, 2 },
            //new List<int>() { -1, 0, 1, 2 },
            //new List<int>() { -1, 0, 1 },
            CreateNewMap(
                7, 
                14,
                100,
                new List<int>() { -1, 0, 1 },
                new List<(Vector2Int, int)>() 
                    {
                    (new Vector2Int(1, 0), 0),
                    (new Vector2Int(1, 1), 1),
                    (new Vector2Int(1, -1), 1),
                    (new Vector2Int(0, 1), 4),
                    (new Vector2Int(0, -1), 4),
                    (new Vector2Int(2, 0), 3),
                    (new Vector2Int(0, 2), 6),
                    (new Vector2Int(0, -2), 6)
                    }
                );
        }
    }

    public static void CheckForLevelEnded() {
        if (!IsLevelStarted)
            return;

        var levelObject = LevelMap.singleton.LevelsOnCoords[SelectedLevel].transform;
        LevelMap.singleton.MapTransform.localPosition = new Vector3(-levelObject.localPosition.x, -levelObject.localPosition.y, 0);

        IsLevelStarted = false;
        ChangeLevelStatus(SelectedLevel, GameManager.IsWin ? LevelStatus.Completed : LevelStatus.Failed);
        UnlockLevels(SelectedLevel);

        if (GameRoundEnd.CoinsCount > 0) {
            var gettedCoint = GameRoundEnd.CoinsCount;
            GameRoundEnd.CoinsCount = 0;
        }
    }

    public static void ChangeLevelStatus(Vector2Int pos, LevelStatus status) {
        if (!LevelsDataPoses.ContainsKey(pos))
            return;
        var levelData = LevelsDataPoses[pos];
        LevelMap.singleton.LevelsOnCoords[pos].UpdatedLevelStatus(status);
    }

    public static void UnlockLevels(Vector2Int connectionStart) {
        foreach (var e in LevelsConnections.Where((x) => x.Value.Any((f) => f == connectionStart)))
        {
            var pos = e.Key;
            if (!LevelsDataPoses.ContainsKey(pos))
                continue;
            var levelData = LevelsDataPoses[pos];
            if (levelData.LevelStatus == LevelStatus.Locked)
            {
                ChangeLevelStatus(pos, LevelStatus.Unlocked);
            }
        }

        foreach (var e in LevelsConnections.Where((x) => x.Key == connectionStart))
        {
            foreach (var f in e.Value)
            {
                if (!LevelsDataPoses.ContainsKey(f))
                    continue;
                var levelData = LevelsDataPoses[f];
                if (levelData.LevelStatus == LevelStatus.Locked)
                {
                    ChangeLevelStatus(f, LevelStatus.Unlocked);
                }
            }
        }
    }

    public static Vector3 ConvertCoordsToPosition(Vector2Int pos, bool useRandom)
    {
        if (!useRandom)
            return new Vector3Int((pos.x) * 400, (pos.y) * 300, 0);

        if (!PosOnCoords.ContainsKey(pos))
            PosOnCoords[pos] = ConvertCoordsToPosition(pos, false) + new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);

        return PosOnCoords[pos];
    }

    public static void CreateNewMap(int LevelsLenght, int LevelsCount, float ProblemsPower, List<int> yPosesSample, List<(Vector2Int, int)> connectionsOffsetsSample) {
        LevelsData = new List<LevelData>();
        LevelsDataPoses = new Dictionary<Vector2Int, LevelData>();
        LevelsConnections = new Dictionary<Vector2Int, List<Vector2Int>>();
        PosOnCoords = new Dictionary<Vector2Int, Vector3>();

        var possibleYPoses = new List<(int, List<int>)>();
        //var possibleConnections = new Dictionary<Vector2Int, List<Vector2Int>>();

        for (var i = 0; i < LevelsLenght; i++)
        {
            var posesCopy = new List<int>();
            foreach (var e in yPosesSample)
            {
                //possibleConnections[new Vector2Int(i, e)] = new List<Vector2Int>();
                posesCopy.Add(e);
                //foreach (var offset in connectionsOffsetsSample) {
                //    possibleConnections[new Vector2Int(i, e)].Add(new Vector2Int(i + offset.x, e + offset.y));
                //}
            }
            possibleYPoses.Add((i, posesCopy));
        }

        int placedLevelsCount = LevelsCount;

        var percentPerSizeLevel = 8f / ((float)LevelsLenght);
        var difficultyPointPerLevel = ProblemsPower / ((float)LevelsLenght);
        float difficultyPoints = 10;

        for (var x = 0; x < LevelsLenght; x++)
        {
            int difficultyPointsNow = Mathf.RoundToInt((difficultyPoints + (difficultyPointPerLevel * x)) * Random.Range(0.6f, 1.1f));
            int sizeNow = Mathf.Clamp(Mathf.RoundToInt(percentPerSizeLevel * x) + (Random.Range(0, 3)/2), 0, 8);
            var level = CreateNewLevelData(sizeNow, difficultyPointsNow, true);

            var possibleYPosesNow = possibleYPoses.Where(e => e.Item1 == x).First().Item2;
            while (possibleYPosesNow.Count > 0)
            {
                var index = Random.Range(0, possibleYPosesNow.Count);
                var y = possibleYPosesNow[index];
                var levelPos = new Vector2Int(x, y);
                if (RegisterLevelData(level, levelPos))
                {
                    if (x > 0) {
                        CreateConnectionAt(LevelsDataPoses.First(e => (e.Key.x == (x - 1))).Key , levelPos);
                    }

                    placedLevelsCount -= 1;
                    break;
                }
                possibleYPosesNow.RemoveAt(index);
                if (possibleYPosesNow.Count == 0) {
                    possibleYPoses.RemoveAt(x);
                }
            }
        }

        while (placedLevelsCount > 0) {
            if (possibleYPoses.Count == 0)
                break;

            var pair = possibleYPoses[Random.Range(0, possibleYPoses.Count)];
            var x = pair.Item1;
            if (x == 0 || x == LevelsLenght - 1) {
                possibleYPoses.Remove(pair);
                continue;
            }
            var possibleYPosesNow = pair.Item2;

            int difficultyPointsNow = Mathf.RoundToInt((difficultyPoints + (difficultyPointPerLevel * x)) * Random.Range(0.6f, 1.1f));
            int sizeNow = Mathf.Clamp(Mathf.RoundToInt(percentPerSizeLevel * x) + (Random.Range(0, 3) / 2), 0, 8);
            var level = CreateNewLevelData(sizeNow, difficultyPointsNow, true);

            while (possibleYPosesNow.Count > 0)
            {
                var index = Random.Range(0, possibleYPosesNow.Count);
                var y = possibleYPosesNow[index];
                if (RegisterLevelData(level, new Vector2Int(x, y)))
                {
                    placedLevelsCount -= 1;
                    break;
                }
                possibleYPosesNow.RemoveAt(index);
                if (possibleYPosesNow.Count == 0)
                {
                    possibleYPoses.Remove(possibleYPoses.Where(e => e.Item1 == x).First());
                }
            }
        }

        for (var x = 0; x < LevelsLenght; x++) {
            foreach (var level in LevelsDataPoses.Where(e => e.Key.x == x)) {
                int connectionCount = Random.Range(1, 3);
                for (var i = 0; i < connectionCount; i++)
                {
                    var result = TryCreateConnectionAt(level.Key, connectionsOffsetsSample);
                }
            }
        }

        foreach (var level in LevelsDataPoses)
        {
            if (level.Key.x == 0)
            {
                level.Value.LevelStatus = LevelStatus.Unlocked;
            }
            else
            {
                level.Value.LevelStatus = LevelStatus.Locked;
            }
        }
    }

    public static (bool, Vector2Int, Vector2Int) TryCreateConnectionAt(Vector2Int pos, List<(Vector2Int, int)> connectionsOffsetsSample)
    {
        var possibleConnections = new List<(Vector2Int, int)>();
        foreach (var pair in connectionsOffsetsSample) {
            var newPos = pos + pair.Item1;

            if (LevelsDataPoses.ContainsKey(newPos))
            {
                if (Vector2Int.Distance(pos, newPos) == 2) {
                    if (pair.Item1.y == 0 && LevelsDataPoses.ContainsKey(pos + new Vector2Int((pair.Item1.x / 2), 0)))
                            continue;
                    if (pair.Item1.x == 0 && LevelsDataPoses.ContainsKey(pos + new Vector2Int(0, (pair.Item1.y / 2))))
                            continue;
                }

                possibleConnections.Add((newPos, pair.Item2));
            }

            for (var i = 0; i < possibleConnections.Count; i++) {
                var target = possibleConnections[i].Item1;
                if (LevelsConnections.ContainsKey(target)) {
                    possibleConnections[i] = (target, possibleConnections[i].Item2 + LevelsConnections[target].Count);
                }
            }
        }

        if (possibleConnections.Count != 0)
        {
            possibleConnections.Sort((x, y) => { return x.Item2 - y.Item2; });

            while (possibleConnections.Count > 0)
            {
                var bestValue = possibleConnections[0].Item2;
                var bestConnections = possibleConnections.Where(x => x.Item2 == bestValue).ToList();
                var res = bestConnections[Random.Range(0, bestConnections.Count)];

                if (!CreateConnectionAt(pos, res.Item1))
                {
                    bestConnections.Remove(res);
                    possibleConnections.Remove(res);
                    continue;
                }
                return (true, pos, res.Item1);
            }
        }
        return (false, Vector2Int.zero, Vector2Int.zero);
    }

    public static bool CreateConnectionAt(Vector2Int start, Vector2Int target)
    {
        if (!LevelsConnections.ContainsKey(target))
            LevelsConnections[target] = new List<Vector2Int>();

        if (LevelsConnections[target].Contains(start) || 
            (LevelsConnections.ContainsKey(start) && LevelsConnections[start].Contains(target)))
            return false;

        LevelsConnections[target].Add(start);
        return true;
    }

    public static bool RegisterLevelData(LevelData level, Vector2Int pos) {
        if (LevelsDataPoses.ContainsKey(pos))
        {
            return false;
        }
        
        LevelsData.Add(level);
        LevelsDataPoses[pos] = level;
        return true;
    }

    public static void LoadLevel(Vector2Int coords)
    {
        if (!LevelsDataPoses.ContainsKey(coords))
            return;
        SelectedLevel = coords;

        var levelData = LevelsDataPoses[coords];
        LoadLevel(levelData);
    }

    public static void LoadLevel(LevelData levelData)
    {
        GameManager.SetLevelData(levelData);
        IsLevelStarted = true;
        var e = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    static LevelData CreateNewLevelData(int size, int difficultyPoints, bool infectionProblems)
    {
        var randomProblems = Random.Range(0f, 1f);
        var foodPoints = difficultyPoints * randomProblems;
        var discontentPoints = difficultyPoints * (1 - randomProblems);

        if (discontentPoints > 60)
        {
            foodPoints += discontentPoints - 60;
            discontentPoints = 60;
        }
        else {
            if (foodPoints > 40) {
                discontentPoints += foodPoints - 40;
                foodPoints = 40;
            }
        }
        foodPoints = Mathf.RoundToInt(foodPoints);
        discontentPoints = Mathf.RoundToInt(discontentPoints);

        ELevelData_FoodProblems foodProblems;
        if (foodPoints > 10f)
        {
            if (foodPoints < 25f)
            {
                foodProblems = ELevelData_FoodProblems.WeakFoodProblems;
            }
            else
            {
                if (foodPoints < 40f)
                {
                    foodProblems = ELevelData_FoodProblems.StrongFoodProblems;
                }
                else {
                    foodProblems = ELevelData_FoodProblems.NoFood;
                }
            }
        }
        else
        {
            foodProblems = ELevelData_FoodProblems.NoFoodProblems;
        }

        ELevelData_DiscontentProblems discontentProblems;
        if (discontentPoints > 25f)
        {
            if (discontentPoints < 60f)
            {
                discontentProblems = ELevelData_DiscontentProblems.WeakDiscontentProblems;
            }
            else
            {
                discontentProblems = ELevelData_DiscontentProblems.StrongDiscontentProblems;
            }
        }
        else
        {
            discontentProblems = ELevelData_DiscontentProblems.NoDiscontentProblems;
        }

        //Debug.Log($"{foodPoints}-{foodProblems}; {discontentPoints}-{discontentProblems}");
        //Debug.Log($"{foodProblems}; {discontentProblems}");

        var level = new LevelData(size, infectionProblems, foodProblems, discontentProblems);
        var gameTasks = CreateGameTasks(level);
        level.SetTasks(gameTasks.Item1, gameTasks.Item2);

        return level;
    }

    public static (int, int) GetTownSize(int size) {
        if (size > 0)
            return (100 * (int)Mathf.Pow(1.5f, size), 120 * (int)Mathf.Pow(1.5f, size));
        return (100, 120);
    }

    public static (List<GameTask>, List<GameTask>) CreateGameTasks(LevelData levelData) {
        var mainTasks = GetMainTasks(levelData);
        var extraTasks = GetExtraTasks(levelData, mainTasks.Count);

        return (mainTasks, extraTasks);
    }

    static List<GameTask> GetMainTasks(LevelData levelData) {
        var mainTasks = new List<GameTask>();
        int tasksCount = 0;

        //Infected percent
        if (levelData.InfectionProblems)
        {
            mainTasks.Add(new Task_InfectedPercent(TaskCountComparison.LessThan, 0));
            tasksCount += 1;
        }

        //Dead precent
        switch (PacifistModeNow) {
            case PacifistMode.No:
                break;
            case PacifistMode.Low:
                mainTasks.Add(new Task_DeadPercent(TaskCountComparison.LessThan, 75));
                tasksCount += 1;
                break;
            case PacifistMode.Max:
                mainTasks.Add(new Task_DeadPercent(TaskCountComparison.LessThan, 50));
                tasksCount += 1;
                break;
        }

        //Food count
        if (levelData.FoodProblems != ELevelData_FoodProblems.NoFoodProblems && PacifistModeNow != PacifistMode.No)
        {
            var foodCount = 1000 * Mathf.RoundToInt(((int)(levelData.FoodProblems) * 0.5f * (int)Mathf.Pow(1.5f, levelData.Size)));
            if (foodCount < 1000)
                foodCount = 1000;

            mainTasks.Add(new Task_ResourceCount(EResource.Food, TaskCountComparison.MoreOrEqualTo, foodCount));
            tasksCount += 1;
        }

        //Discontent percent
        if (levelData.DiscontentProblems != ELevelData_DiscontentProblems.NoDiscontentProblems)
        {
            mainTasks.Add(new Task_DiscontentMedium(TaskCountComparison.LessThan, 10));
            tasksCount += 1;
        }

        return mainTasks;
    }

    static List<GameTask> GetExtraTasks(LevelData levelData, int tasksCount)
    {
        var extraTasks = new List<GameTask>();


        //Dead precent
        switch (PacifistModeNow)
        {
            case PacifistMode.No:
                extraTasks.Add(new Task_DeadPercent(TaskCountComparison.LessThan, 75));
                tasksCount += 1;
                break;
            case PacifistMode.Low:
                extraTasks.Add(new Task_DeadPercent(TaskCountComparison.LessThan, 50));
                tasksCount += 1;
                break;
            case PacifistMode.Max:
                extraTasks.Add(new Task_DeadPercent(TaskCountComparison.LessThan, 25));
                tasksCount += 1;
                break;
        }

        if (tasksCount < 5)
        {
            extraTasks.Add(new Task_Days(TaskCountComparison.LessThan, ((levelData.Size + 2) / 2) * 5));
        }

        return extraTasks;
    }
}
