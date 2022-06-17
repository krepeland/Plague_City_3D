using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectionManager : MonoBehaviour, ITickable
{
    public static InfectionManager singleton;

    public Infection_Settings Infection_Settings;

    public float InfectionPower;
    public HashSet<IInfectable> InfectedObjects;

    int powerPerTick;
    float powerPerInfectedHouse;

    HashSet<IInfectable> objectsToAdd;
    HashSet<IInfectable> objectsToRemove;

    List<InfectionSkill> infectionSkillBase;

    public List<InfectionSkillBase> InfectionSkills;

    public static Texture2D InfectionTexture;

    public Material mat;

    public static int InfectionEnemyCount;

    private void Awake()
    {
        singleton = this;
        InfectionEnemyCount = 0;

        objectsToAdd = new HashSet<IInfectable>();
        InfectedObjects = new HashSet<IInfectable>();
        objectsToRemove = new HashSet<IInfectable>();

        InfectionTexture = new Texture2D(80, 80);
        InfectionTexture.filterMode = FilterMode.Point;
        InfectionTexture.wrapMode = TextureWrapMode.Clamp;
        for (var x = 0; x < 80; x++) {
            for (var y = 0; y < 80; y++)
            {
                SetInfectionTextureState(x, y, CellInfectionState.None);
            }
        }

        mat.mainTexture = InfectionTexture;
        //Shader.SetGlobalTexture("Texture2D_0f712be9b4cf48069a6642e8cd337aa9", InfectionTexture);
    }

    public void LoadConfig(Infection_Settings config)
    {
        Infection_Settings = config;

        B_House.TimeToDeathRandom = config.TimeToDeathRandom;
        B_House.TimeToDeathSample = config.TimeToDeathSample;
        powerPerTick = config.PowerPerTick;
        powerPerInfectedHouse = config.PowerPerInfectedHouse;

        //Load skills
        infectionSkillBase = config.InfectionSkillBase;
        InfectionSkills = new List<InfectionSkillBase>();
        foreach (var skillData in infectionSkillBase) {
            var infectionSkill = Instantiate(skillData.InfectionSkillBase.gameObject, transform).GetComponent<InfectionSkillBase>();
            infectionSkill.TicksToUse = skillData.TicksTillUnlockingSKill;
            InfectionSkills.Add(infectionSkill);
        }

        InfectionSkills.Sort(new InfectionSkillBaseComparer());
    }

    private void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Medium);
    }

    public void AddInfectedHouse(B_House house) {
        if (InfectedObjects.Contains(house) || objectsToAdd.Contains(house))
            return;
        objectsToAdd.Add(house);
    }

    public void RemoveInfectedHouse(B_House house)
    {
        if (!InfectedObjects.Contains(house))
            return;
        objectsToRemove.Add(house);
    }

    public List<Vector2Int> FindPathToInfectClosestHouse(List<Vector2Int> startPoints, int maxDepth, HashSet<Vector2Int> exceptPoints) {
        Dictionary<Vector2Int, (int, Vector2Int)> dict = new Dictionary<Vector2Int, (int, Vector2Int)>();

        Queue<(Vector2Int, (int, Vector2Int))> queue = new Queue<(Vector2Int, (int, Vector2Int))>();

        foreach (var pos in startPoints)
            queue.Enqueue((pos, (maxDepth, pos)));

        Vector2Int endPoint = new Vector2Int(-1, -1);
        int flag = startPoints.Count;
        while (queue.Count > 0)
        {
            var itemNow = queue.Dequeue();
            var posNow = itemNow.Item1;
            var valueNow = itemNow.Item2.Item1;
            if (!dict.ContainsKey(posNow))
                dict[posNow] = itemNow.Item2;

            var nextValue = valueNow - 1;
            if (nextValue < 0)
                continue;

            var townManager = TownManager.singleton;
            foreach (var offset in GetOffsets())
            {
                var nextPos = posNow + offset;

                var buildingAt = townManager.GetBuildingAt(nextPos);
                IInfectable infectable = null;
                var isInfectable = buildingAt == null ? false : buildingAt.TryGetComponent<IInfectable>(out infectable);

                if (flag > 0) {
                    if (buildingAt != null && isInfectable)
                        continue;
                }

                if (buildingAt != null) {
                    if (!isInfectable)
                        continue;
                    if (InfectedObjects.Contains(infectable) || !infectable.CheckIsCanBeInfected())
                        continue;

                    dict[nextPos] = (nextValue, posNow);
                    queue = new Queue<(Vector2Int, (int, Vector2Int))>();
                    endPoint = nextPos;
                    break;
                }

                if (dict.ContainsKey(nextPos) || 
                    !TownManager.singleton.Roads.ContainsKey(nextPos) || 
                    exceptPoints.Contains(nextPos)
                    )
                    continue;

                queue.Enqueue((nextPos, (nextValue, posNow)));
            }
            flag -= 1;
        }
        if (endPoint == new Vector2Int(-1, -1))
            return new List<Vector2Int>();

        var result = new List<Vector2Int>();
        var itemPath = dict[endPoint];
        int depth = maxDepth * 2;
        while (itemPath.Item1 != maxDepth) {
            depth -= 1;
            if (depth <= 0)
                return new List<Vector2Int>();
            result.Add(endPoint);
            endPoint = itemPath.Item2;
            itemPath = dict[endPoint];
        }
        result.Add(endPoint);

        return result;
    }

    public IInfectable GetRandomInfectedObject() {
        var list = InfectedObjects.ToList();
        if (list.Count == 0)
            return null;
        return list[Random.Range(0, list.Count)];
    }

    public void Tick(int luck)
    {
        foreach (var infectedObject in InfectedObjects) {
            infectedObject.CalculateInfection();

            if (!infectedObject.CheckIsCanBeInfected())
                objectsToRemove.Add(infectedObject);
        }

        if (objectsToRemove.Count > 0)
        {
            foreach (var house in objectsToRemove)
                InfectedObjects.Remove(house);
            objectsToRemove = new HashSet<IInfectable>();
        }

        if (objectsToAdd.Count > 0)
        {
            foreach (var house in objectsToAdd)
                InfectedObjects.Add(house);
            objectsToAdd = new HashSet<IInfectable>();
        }

        InfectionPower += powerPerTick;
        InfectionPower += powerPerInfectedHouse * InfectedObjects.Count;

        TryMakeInfectionTurn();
    }

    bool TryMakeInfectionTurn() {
        foreach (var skill in InfectionSkills) {
            if (skill.TicksToUse > 0 || InfectionPower < skill.PowerCost)
                continue;

            if (skill.TryCastSkill(this))
            {
                InfectionPower -= skill.PowerCost;
                skill.TicksToUse = skill.TicksReload + Random.Range(-skill.TicksReloadRandom, skill.TicksReloadRandom);
                //Debug.Log("Plague skill: " + skill.gameObject.name);
            }
        }
        return false;
    }

    IEnumerable<Vector2Int> GetOffsets()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.right;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
    }

    public static void SetInfectionTextureState(int x, int y, CellInfectionState cellInfectionState) {
        var color = new Color(0.8f, 0.8f, 0.8f);
        switch (cellInfectionState) {
            case CellInfectionState.None:
                color = new Color(0.8f, 0.8f, 0.8f);
                break;
            case CellInfectionState.NotInfected:
                color = Color.green;
                break;
            case CellInfectionState.Infected:
                color = Color.red;
                break;
            case CellInfectionState.Dead:
                color = Color.black;
                break;
            case CellInfectionState.Defensed:
                color = Color.blue;
                break;
        }
        InfectionTexture.SetPixel(x, y, color);
        ApplyInfectionTexture();
    }

    public static void SetInfectionTextureColor(int x, int y, Color color)
    {
        InfectionTexture.SetPixel(x, y, color);
        ApplyInfectionTexture();
    }

    public static void ApplyInfectionTexture() {
        InfectionTexture.Apply();
    }
}
