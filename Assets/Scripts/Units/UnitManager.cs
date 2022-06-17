using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public bool IsUnitSelected;
    public Dictionary<Vector2Int, RoadType> Roads;

    public UnitBase SelectedUnit;
    public ISelectable Target;
    public SelectedType TargetType;
    public HashSet<Vector2Int> startPoints;
    public HashSet<Vector2Int> targets;

    public Dictionary<Vector2Int, HashSet<UnitBase>> UnitsPoses;
    private Dictionary<Vector2Int, Fight> fightPoses;
    private Dictionary<UnitBase, Vector2Int> unitPosNow;

    Transform wayPrefab;

    public List<Vector2Int> Way;
    public List<GameObject> WayObjects;

    public static UnitManager singleton;

    public int EnemyCount;
    public static int EnemyMaxCount = 50;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        Init(TownManager.singleton.Roads);

        UnitsPoses = new Dictionary<Vector2Int, HashSet<UnitBase>>();
        unitPosNow = new Dictionary<UnitBase, Vector2Int>();
        fightPoses = new Dictionary<Vector2Int, Fight>();
    }

    public void Init(Dictionary<Vector2Int, RoadType> roads) {
        Roads = roads;
        wayPrefab = PrefabManager.GetPrefab("Units_Way");
        startPoints = new HashSet<Vector2Int>();
        targets = new HashSet<Vector2Int>();
    }

    public UnitBase SpawnUnit(UnitCard unitCard) {
        var newUnit = Instantiate(unitCard.prefab_unit.gameObject);
        return newUnit.GetComponent<UnitBase>();
    }

    public UnitBase SpawnAndInitEnemy(string name, Vector2Int pos, ISelectable target, bool isForceCreate) {
        if (EnemyCount >= EnemyMaxCount && !isForceCreate)
        {
            return null;
        }
        EnemyCount += 1;
        var unitCard = CardsManager.singleton.GetUnitCard(name);
        var unit = SpawnUnit(unitCard);
        unit.InitEnemy(unitCard, pos);
        return unit;
    }

    private void Update()
    {
        UnitCard cardData = null;
        if (Selector.singleton.SelectedObject != null)
        {
            if (SelectedUnit != null)
            {
                cardData = SelectedUnit.UnitCard;
            }
            else
            {
                if (CardsManager.singleton.SelectedCard != null)
                {
                    if (CardsManager.singleton.SelectedCard.CardClassType != CardClassType.Building)
                    {
                        cardData = CardsManager.singleton.GetUnitCard(CardsManager.singleton.SelectedCard.CardData);
                    }
                }
            }

            if (cardData != null)
            {
                SetUnitActionInfo(UnitBase.GetActionTypeOnSelectedType(cardData, Target));
            }
        }
        else
        {
            UnitActionInfo.singleton.SetAction(0, UnitActionType.None);
            UnitActionInfo.singleton.SetAction(1, UnitActionType.None);
        }

    }

    private void SetUnitActionInfo((UnitActionType, string)[] actionType) {
        for (var i = 0; i < actionType.Length; i++)
        {
            UnitActionInfo.singleton.SetAction(i, actionType[i].Item1);
            UnitActionInfo.singleton.SetActionText(i, actionType[i].Item2);
        }
    }

    public void UpdateUnitPose(UnitBase unit, Vector2Int newPos)
    {
        if (unitPosNow.ContainsKey(unit))
        {
            var oldPos = unitPosNow[unit];

            if (UnitsPoses[oldPos] != null)
            {
                if (UnitsPoses[oldPos].Contains(unit))
                    UnitsPoses[oldPos].Remove(unit);

                if (UnitsPoses[oldPos].Count <= 0)
                    UnitsPoses.Remove(oldPos);
            }
        }

        unitPosNow[unit] = newPos;
        if (!UnitsPoses.ContainsKey(newPos))
            UnitsPoses[newPos] = new HashSet<UnitBase>();

        UnitsPoses[newPos].Add(unit);
    }

    public void UnitDeletedFromBoard(UnitBase unit) {
        if (unitPosNow.ContainsKey(unit))
        {
            var oldPos = unitPosNow[unit];

            if (UnitsPoses[oldPos] != null)
            {
                if (UnitsPoses[oldPos].Contains(unit))
                    UnitsPoses[oldPos].Remove(unit);

                if (UnitsPoses[oldPos].Count <= 0)
                    UnitsPoses.Remove(oldPos);
            }

            unitPosNow.Remove(unit);
        }
    }

    public bool IsUnitCollideWithAnother(UnitBase unit, out List<UnitBase> others) {
        var unitPos = unitPosNow[unit];
        others = new List<UnitBase>();
        foreach (var other in UnitsPoses[unitPos]) {
            if (other == unit)
                continue;
            others.Add(other);
        }
        return others.Count > 0;
    }

    public void UnitMoved(UnitBase unit)
    {
        if (unit.IsDied)
            return;

        if (SelectedUnit == unit)
            UnitSelected(unit);

        var pos = unit.GetPosNow();
        UpdateUnitPose(unit, pos);

        if (IsFightAt(pos)) {
            var fight = GetFightAt(pos);
            fight.AddUnitToFight(unit);
            return;
        }

        if (UnitsPoses[pos].Count > 0 && IsFightShouldStartAt(pos, unit.UnitTeam))
        {
            StartFightAt(pos);
        }
        else
        {
            unit.DoActionUnitMoved(pos);
        }
    }

    void StartFightAt(Vector2Int pos) {
        if (!UnitsPoses.ContainsKey(pos))
            return;
        var fight = Instantiate(PrefabManager.GetPrefab("Fight")).GetComponent<Fight>();
        fight.Init(pos);
        fightPoses[pos] = fight;
        foreach (var unit in UnitsPoses[pos]) {
            fight.AddUnitToFight(unit);
        }
    }

    public void FightEndedAt(Vector2Int pos) {
        if (fightPoses.ContainsKey(pos)) {
            fightPoses.Remove(pos);
        }
    }

    bool IsFightShouldStartAt(Vector2Int pos, UnitTeam unitTeam) {
        if (unitTeam == UnitTeam.EnemyToAll)
            return true;
        foreach (var e in UnitsPoses[pos])
            if (e.UnitTeam != unitTeam)
                return true;
        return false;
    }

    bool IsFightAt(Vector2Int pos) {
        return fightPoses.ContainsKey(pos);
    }

    Fight GetFightAt(Vector2Int pos)
    {
        if (!IsFightAt(pos))
            return null;
        return fightPoses[pos];
    }

    public void SetTarget(ISelectable selectable) {
        if (Target == selectable)
            return;

        Target = selectable;
        if (Target == null)
        {
            Unselected();
            return;
        }

        targets = new HashSet<Vector2Int>();
        TargetType = Target.GetSelectedType();
        switch (TargetType)
        {
            case SelectedType.Building:
                foreach (var pos in ((PlacedBuilding)Target).buildingData.poses)
                {
                    targets.Add(pos);
                }
                break;
            case SelectedType.Road:
                var targetPos = Selector.singleton.SelectedCoords;
                targets.Add(new Vector2Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.z)));
                break;
            default:
                return;
        }

        DrawWay();
    }

    public void RemoveWay()
    {
        for (var i = 0; i < WayObjects.Count; i++)
        {
            Destroy(WayObjects[i]);
        }
        WayObjects = new List<GameObject>();
        Way = new List<Vector2Int>();

        UnitActionInfo.singleton.SetAction(0, UnitActionType.None);
        UnitActionInfo.singleton.SetAction(1, UnitActionType.None);
    }

    List<Vector2Int> DrawWay() {
        RemoveWay();

        if (!IsUnitSelected)
        {
            return null;
        }

        if (startPoints.Count == 0 || targets.Count == 0)
            return null;

        Way = FindWay(startPoints, targets);
        if (Way == null)
            return null;

        foreach (var e in Way)
        {
            var pos = new Vector3(e.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(e.x, e.y)) + 0.5f, e.y);
            var wayObject = Instantiate(wayPrefab, pos, Quaternion.Euler(90, 0, 0));
            WayObjects.Add(wayObject.gameObject);
        }

        return Way;
    }

    public void UnitSelected(ISelectable selectable) {
        if (selectable == null) 
            return;
        UnitSelected(selectable.GetSelectedObject().GetComponent<UnitBase>());
    }

    public void UnitSelected(UnitBase unit)
    {
        if (CardsManager.singleton.IsCardInHand)
            return;

        SelectedUnit = unit;
        IsUnitSelected = true;
        startPoints = new HashSet<Vector2Int>() {
            new Vector2Int(Mathf.RoundToInt(SelectedUnit.startPoint.x), Mathf.RoundToInt(SelectedUnit.startPoint.z))
        };
        DrawWay();
    }

    public void SetStartPoints(HashSet<Vector2Int> startPoints)
    {
        this.startPoints = startPoints;
        DrawWay();
    }

    public void UpdatedUnitCardInHand(GameCard selectedCard) {
        //if (startPoints == null || startPoints.Count == 0)
        //    return;

        var points = new HashSet<Vector2Int>();
        foreach (var buildingName in CardsManager.singleton.GetUnitCard(selectedCard.CardData).BuildingsOwners) {
            foreach (var building in CardsManager.singleton.GetAllBuildingsWithName(buildingName)) {
                foreach (var p in building.buildingData.poses) {
                    points.Add(p);
                }
            }
        }
        SetStartPoints(points);
        IsUnitSelected = true;
    }

    public void SelectedTarget()
    {
        var way = FindWay(
            startPoints,
            targets
            );
        if (way == null)
        {
            Unselected();
            return;
        }

        var resultWay = new List<Vector3>();
        foreach (var e in way) {
            resultWay.Add(new Vector3(e.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(e.x, e.y)), e.y));
        }
        SelectedUnit.SetWay(resultWay, Target, TargetType);

        Unselected();
    }

    public void Unselected()
    {
        TargetType = SelectedType.None;
        IsUnitSelected = false;
        SelectedUnit = null;
        RemoveWay();
    }

    public List<Vector2Int> FindWay(HashSet<Vector2Int> startPoints, HashSet<Vector2Int> targets) {
        var allWays = GetAllWays(startPoints, targets);
        if (allWays.Item1 == null)
            return null;

        var result = new List<Vector2Int>();

        var itemNow = allWays.Item2;
        int depth = 0;

        while (true)
        {
            depth += 1;
            if (depth >= 1000)
            {
                return new List<Vector2Int>();
            }

            if (startPoints.Contains(itemNow))
                break;

            result.Add(itemNow);
            itemNow = allWays.Item1[itemNow];

        }
        result.Add(itemNow);
        result.Reverse();
        return result;
    }

    (Dictionary<Vector2Int, Vector2Int>, Vector2Int) GetAllWays(HashSet<Vector2Int> startPoints, HashSet<Vector2Int> targets) {
        Dictionary<Vector2Int, Vector2Int> dict = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        var fix = 0;
        foreach (var pos in startPoints)
        {
            queue.Enqueue(pos);
            dict[pos] = pos;

            if (TownManager.singleton.GetBuildingAt(pos))
            {
                fix += 1;
            }
        }


        var debugDepth = 100000;
        while (queue.Count > 0)
        {
            debugDepth -= 1;
            if (debugDepth <= 0)
            {
                break;
            }


            var current = queue.Dequeue();

            if (visited.Contains(current)) {
                continue;
            }
            visited.Add(current);

            foreach (var offset in GetOffsets())
            {
                var next = current + offset;
                var buildingNext = TownManager.singleton.GetBuildingAt(next);

                if (visited.Contains(next))
                    continue;

                if (targets.Contains(next) && fix > 0 && buildingNext != null) {
                    continue;
                }

                if (targets.Contains(next))
                {
                    dict[next] = current;
                    return (dict, next);
                }

                if (!Roads.ContainsKey(next) || (Roads.ContainsKey(next) && Roads[next] != RoadType.BuildingRoad))
                {
                    if (buildingNext != null && !buildingNext.IsUnitCanWalkThrough)
                    {
                        continue;
                    }
                }

                if (!Roads.ContainsKey(next) || (Roads.ContainsKey(next) && Roads[next] == RoadType.None))
                    continue;

                dict[next] = current;
                queue.Enqueue(next);
            }

            fix -= 1;
        }

        return (null, Vector2Int.zero);
    }


    public List<Vector2Int> FindRandomWay(Vector2Int startPoint, int maxDepth, bool isCanVisitHouses, bool isCanWalkThroughBarricades)
    {
        var allWays = GetAllWays(startPoint, maxDepth, isCanVisitHouses, isCanWalkThroughBarricades);

        int RandomIndex = Random.Range(0, allWays.Count);
        Vector2Int itemNow = Vector2Int.zero;
        foreach (var e in allWays) {
            RandomIndex -= 1;
            if (RandomIndex <= 0) {
                itemNow = e.Key;
                break;
            }
        }

        var result = new List<Vector2Int>();
        int depth = -1;
        while (true)
        {
            depth += 1;
            if (depth > maxDepth)
            {
                return new List<Vector2Int>();
            }

            if (startPoint == itemNow)
                break;

            result.Add(itemNow);
            itemNow = allWays[itemNow];

        }
        result.Add(itemNow);
        result.Reverse();
        return result;
    }

    Dictionary<Vector2Int, Vector2Int> GetAllWays(Vector2Int startPoint, int maxDepth, bool isCanVisitHouses, bool isCanWalkThroughBarricades)
    {
        Dictionary<Vector2Int, Vector2Int> dict = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<(Vector2Int, int)> queue = new Queue<(Vector2Int, int)>();

        queue.Enqueue((startPoint, maxDepth));
        dict[startPoint] = startPoint;


        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Item2 <= 0) {
                continue;
            }

            if (visited.Contains(current.Item1))
            {
                continue;
            }
            visited.Add(current.Item1);

            foreach (var offset in GetOffsets())
            {
                var next = current.Item1 + offset;
                var buildingNext = TownManager.singleton.GetBuildingAt(next);

                if (visited.Contains(next))
                    continue;

                var flag = false;
                if (!Roads.ContainsKey(next) && isCanVisitHouses && buildingNext != null)
                {
                    if (buildingNext.GetBuildingName() != "House")
                    {
                        continue;
                    }
                    else {
                        flag = true;
                    }
                }
                else
                {
                    if (buildingNext != null && buildingNext.GetBuildingName() == "Barricade") {
                        if (!isCanWalkThroughBarricades)
                            continue;
                    }

                    if (!Roads.ContainsKey(next) || (Roads.ContainsKey(next) && Roads[next] == RoadType.None))
                        continue;
                }

                dict[next] = current.Item1;
                if(!flag)
                    queue.Enqueue((next, current.Item2 - 1));
            }
        }

        return dict;
    }

    IEnumerable<Vector2Int> GetOffsets()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.right;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
    }
}
