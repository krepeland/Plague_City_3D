using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour, ITickable
{
    public static TownManager singleton;

    public Transform HousesContainer;

    List<B_House> houses;
    Dictionary<Vector2Int, PlacedBuilding> buildingsDictionary;
    Dictionary<Vector2Int, EBuilding> buildingsTypes;

    public Dictionary<Vector2Int, RoadType> Roads;

    public List<CityRegion> Regions;
    public List<CitySubRegion> Subregions;
    private Dictionary<Vector2Int, CitySubRegion> pointToSubRegion;
    private Dictionary<Vector2Int, CityRegion> pointToRegion;

    [SerializeField] private int _totalPeopleCount;
    [SerializeField] private int _healthyPeopleCount;
    [SerializeField] private int _infectedPeopleCount;
    [SerializeField] private int _deadPeopleCount;
    public int TotalPeopleCount 
    {
        get => _totalPeopleCount;
        set
        {
            _totalPeopleCount = value;
            TaskManager.UpdatedETask(ETask.DeadPercent);
            TaskManager.UpdatedETask(ETask.InfectedPercent);
        }
    }

    public int HealthyPeopleCount
    {
        get => _healthyPeopleCount;
        set
        {
            _healthyPeopleCount = value;
        }
    }

    public int InfectedPeopleCount
    {
        get => _infectedPeopleCount;
        set
        {
            _infectedPeopleCount = value;
            TaskManager.UpdatedETask(ETask.InfectedPercent);
        }
    }

    public int DeadPeopleCount
    {
        get => _deadPeopleCount;
        set
        {
            _deadPeopleCount = value;
            TaskManager.UpdatedETask(ETask.DeadPercent);
        }
    }

    private void Awake()
    {
        singleton = this;

        if (HousesContainer == null) {
            HousesContainer = transform;
        }
    }

    public void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Main);
    }

    public void Tick(int luck)
    {

    }

    public void RegisterRegions(List<CityRegion> cityRegions) {
        pointToSubRegion = new Dictionary<Vector2Int, CitySubRegion>();
        pointToRegion = new Dictionary<Vector2Int, CityRegion>();
        Regions = cityRegions;
        Subregions = new List<CitySubRegion>();

        foreach (var region in Regions)
        {
            foreach (var subRegion in region.subRegions)
            {
                Subregions.Add(subRegion.Value);
                for (var x = 0; x < 5; x++)
                {
                    for (var y = 0; y < 5; y++)
                    {
                        var pos = new Vector2Int(subRegion.Value.pivotPos.x + x, subRegion.Value.pivotPos.y + y);
                        pointToSubRegion[pos] = subRegion.Value;
                        pointToRegion[pos] = region;
                    }
                } 
            }
        }
    }

    public bool AddBuildingRoad(List<Vector2Int> buildingPoses)
    {
        var allWays = GetAllWays(buildingPoses);

        if (allWays.Item1 == null)
            return false;

        var result = new List<Vector2Int>();

        var itemNow = allWays.Item2;
        int depth = 0;

        while (true)
        {
            depth += 1;
            if (depth >= 1000)
            {
                return false;
            }

            if (buildingPoses.Contains(itemNow))
                break;

            result.Add(itemNow);
            itemNow = allWays.Item1[itemNow];

        }
        result.Add(itemNow);

        foreach (var e in result) {
            Roads[e] = RoadType.BuildingRoad;
        }

        foreach (var e in buildingPoses)
        {
            Roads[e] = RoadType.BuildingRoad;
        }

        return true;
    }

    (Dictionary<Vector2Int, Vector2Int>, Vector2Int) GetAllWays(List<Vector2Int> startPoints)
    {
        Dictionary<Vector2Int, Vector2Int> dict = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();


        foreach (var pos in startPoints)
        {
            queue.Enqueue(pos);
            dict[pos] = pos;
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);

            foreach (var offset in GetOffsets())
            {
                var next = current + offset;

                if (visited.Contains(next))
                    continue;

                if (Roads.ContainsKey(next))
                {
                    dict[next] = current;
                    return (dict, next);
                }

                if (!GeneratorSpawner.singleton.IsInBounds(next) || 
                    !GeneratorSpawner.singleton.Form[next.x, next.y] ||
                    (GeneratorSpawner.singleton.RiversHashset.Contains(next) && !GeneratorSpawner.singleton.BridgesHeights.ContainsKey(next))
                    )
                    continue;

                dict[next] = current;
                queue.Enqueue(next);
            }
        }

        return (null, Vector2Int.zero);
    }


    public CityRegion GetRegionAt(Vector2Int pos) {
        if (pointToRegion.ContainsKey(pos))
            return pointToRegion[pos];
        return null;
    }

    public CitySubRegion GetSubRegionAt(Vector2Int pos)
    {
        if (pointToSubRegion.ContainsKey(pos))
            return pointToSubRegion[pos];
        return null;
    }

    public void RegisterHouses(List<B_House> list)
    {
        buildingsDictionary = new Dictionary<Vector2Int, PlacedBuilding>();
        buildingsTypes = new Dictionary<Vector2Int, EBuilding>();

        houses = list;
        foreach (var house in houses) {
            RegisterBuilding(house, EBuilding.House);
        }
    }

    public void RegisterRoads(Dictionary<Vector2Int, RoadType> roads) {
        Roads = roads;
    }

    public void RegisterBuilding(PlacedBuilding placedBuilding, EBuilding eBuilding)
    {
        foreach (var pos in placedBuilding.buildingData.poses)
        {
            buildingsDictionary[pos] = placedBuilding;
            buildingsTypes[pos] = eBuilding;
        }
    }

    public bool IsBuildingAt(Vector2Int pos) {
        if (buildingsDictionary.ContainsKey(pos) && buildingsTypes[pos] != EBuilding.None)
            return true;
        return false;
    }

    public PlacedBuilding GetBuildingAt(Vector2Int pos) {
        if (IsBuildingAt(pos))
            return buildingsDictionary[pos];
        return null;
    }

    public void DestroyBuildingAt(Vector2Int pos)
    {
        DestroyBuilding(GetBuildingAt(pos));
    }

    public void DestroyBuilding(PlacedBuilding building)
    {
        if (building != null)
        {
            foreach (var p in building.buildingData.poses)
            {
                buildingsDictionary[p] = null;
                buildingsTypes[p] = EBuilding.None;
                InfectionManager.SetInfectionTextureState(p.x, p.y, CellInfectionState.None);
                if (building.gameObject.TryGetComponent<B_House>(out var house))
                {
                    if (!house.IsHouseDestroyed)
                    {
                        house.IsHouseDestroyed = true;

                        InfectionManager.singleton.RemoveInfectedHouse(house);
                        house.DeadPeople = 0;
                        house.InfectedPeople = 0;
                        house.HealthyPeople = 0;

                        DiscontentManager.MaxDiscontent -= 100;
                        house.Discontent = 0;
                    }
                }
                ResourceManager.SetFeedTextureState(p.x, p.y, EFeedState.None);
                DiscontentManager.SetDiscontentTextureState(p.x, p.y, 0, false);
            }
            building.ForceDestroyBuilding();
        }
    }

    public EBuilding GetBuildingTypeAt(Vector2Int pos)
    {
        if (IsBuildingAt(pos))
            return buildingsTypes[pos];
        if (Roads.ContainsKey(pos) && Roads[pos] != RoadType.None)
            return EBuilding.Road;

        return EBuilding.None;
    }

    public PlacedBuilding GetRandomHouse() {
        return houses[Random.Range(0, houses.Count)];
    }

    public List<B_House> GetAllHouses() {
        return houses;
    }

    public void OnDestroy()
    {
        Ticker.TryDeleteTickable(this);
    }


    public void AddTotalPeopleCount(int delta) => SetTotalPeopleCount(TotalPeopleCount + delta);
    public void SetTotalPeopleCount(int value) {
        TotalPeopleCount = value;
    }

    public void AddHealthyPeopleCount(int delta) => SetHealthyPeopleCount(HealthyPeopleCount + delta);
    public void SetHealthyPeopleCount(int value)
    {
        HealthyPeopleCount = value;
    }

    public void AddInfectedPeopleCount(int delta) => SetInfectedPeopleCount(InfectedPeopleCount + delta);
    public void SetInfectedPeopleCount(int value)
    {
        InfectedPeopleCount = value;
    }

    public void AddDeadPeopleCount(int delta) => SetDeadPeopleCount(DeadPeopleCount + delta);
    public void SetDeadPeopleCount(int value)
    {
        DeadPeopleCount = value;
    }


    IEnumerable<Vector2Int> GetOffsets()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.right;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
    }
}
