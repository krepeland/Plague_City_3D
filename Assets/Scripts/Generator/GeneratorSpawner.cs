using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorSpawner : MonoBehaviour
{
    private GameObject Prefab_Earth_Corner_Top;
    private GameObject Prefab_Earth_Corner_Middle;
    private GameObject Prefab_Earth_Corner_Bottom;
    private GameObject Prefab_Earth_Side_Top;
    private GameObject Prefab_Earth_Side_Middle;
    private GameObject Prefab_Earth_Side_Bottom;
    private GameObject Prefab_Earth_Inner_Top;
    private GameObject Prefab_Earth_Inner_Middle;
    private GameObject Prefab_Earth_Plane_Top;
    private GameObject Prefab_Earth_Plane_Top_Big;

    private GameObject Prefab_B_House;
    private GameObject Prefab_House1;
    private GameObject Prefab_House2_1;
    private GameObject Prefab_House3_1;
    private GameObject Prefab_House3_2;

    private GameObject Prefab_House1_2;
    private GameObject Prefab_House2_1_2;
    private GameObject Prefab_House3_1_2;
    private GameObject Prefab_House3_2_2;

    private GameObject Prefab_Road1;
    private GameObject Prefab_Road2;
    private GameObject Prefab_Road2_2;
    private GameObject Prefab_Road3;
    private GameObject Prefab_Road4;

    private GameObject Prefab_Bridge1;
    private GameObject Prefab_Bridge2;
    private GameObject Prefab_Bridge2_2;
    private GameObject Prefab_Bridge3;
    private GameObject Prefab_Bridge4;

    private GameObject Prefab_Ladder1;

    private GameObject Prefab_Wall;
    private GameObject Prefab_Wall_Road;

    private List<GameObject> SpawnedObjects;

    public bool[,] Form;
    public List<List<Vector2Int>> Rivers;
    public HashSet<Vector2Int> RiversHashset;
    public Dictionary<Vector2Int, RoadType> Roads;
    public int[,] Heights;
    public Dictionary<Vector2Int, int> BridgesHeights;
    public List<BuildingData> Buildings;
    public List<CityRegion> CityRegions;

    private int Size;

    private float HeightStep = 1;

    public static GeneratorSpawner singleton;

    private void Awake()
    {
        singleton = this;
    }

    public void InitGenerator(
        bool[,] form, 
        List<List<Vector2Int>> rivers,
        HashSet<Vector2Int> riversHashset,
        Dictionary<Vector2Int, RoadType> roads, 
        int[,] heights, 
        List<BuildingData> buildings,
        int size,
        Dictionary<Vector2Int, int> bridgesHeights,
        List<CityRegion> cityRegions) 
    {
        Form = form;
        Rivers = rivers;
        RiversHashset = riversHashset;
        Roads = roads;
        Heights = heights;
        Buildings = buildings;
        Size = size;
        BridgesHeights = bridgesHeights;
        CityRegions = cityRegions;
    }

    void InitPrefabs() {
        if (Prefab_B_House != null)
            return;
        //Prefab_Block = PrefabManager.GetPrefab("Earth").gameObject;

        Prefab_Earth_Corner_Top = PrefabManager.GetPrefab("Earth_Corner_Top").gameObject;
        Prefab_Earth_Corner_Middle = PrefabManager.GetPrefab("Earth_Corner_Middle").gameObject;
        Prefab_Earth_Corner_Bottom = PrefabManager.GetPrefab("Earth_Corner_Bottom").gameObject;
        Prefab_Earth_Side_Top = PrefabManager.GetPrefab("Earth_Side_Top").gameObject;
        Prefab_Earth_Side_Middle = PrefabManager.GetPrefab("Earth_Side_Middle").gameObject;
        Prefab_Earth_Side_Bottom = PrefabManager.GetPrefab("Earth_Side_Bottom").gameObject;
        Prefab_Earth_Inner_Top = PrefabManager.GetPrefab("Earth_Inner_Top").gameObject;
        Prefab_Earth_Inner_Middle = PrefabManager.GetPrefab("Earth_Inner_Middle").gameObject;
        Prefab_Earth_Plane_Top = PrefabManager.GetPrefab("Earth_Plane_Top").gameObject;
        Prefab_Earth_Plane_Top_Big = PrefabManager.GetPrefab("Earth_Plane_Top_Big").gameObject;

        Prefab_B_House = PrefabManager.GetPrefab("Houses_B_House").gameObject;

        Prefab_House1 = PrefabManager.GetPrefab("Houses_House1").gameObject;
        Prefab_House2_1 = PrefabManager.GetPrefab("Houses_House2-1").gameObject;
        Prefab_House3_1 = PrefabManager.GetPrefab("Houses_House3-1").gameObject;
        Prefab_House3_2 = PrefabManager.GetPrefab("Houses_House3-2").gameObject;

        Prefab_House1_2 = PrefabManager.GetPrefab("Houses_House1-2").gameObject;
        Prefab_House2_1_2 = PrefabManager.GetPrefab("Houses_House2-1-2").gameObject;
        Prefab_House3_1_2 = PrefabManager.GetPrefab("Houses_House3-1-2").gameObject;
        Prefab_House3_2_2 = PrefabManager.GetPrefab("Houses_House3-2-2").gameObject;

        Prefab_Road1 = PrefabManager.GetPrefab("Roads_Road1").gameObject;
        Prefab_Road2 = PrefabManager.GetPrefab("Roads_Road2").gameObject;
        Prefab_Road2_2 = PrefabManager.GetPrefab("Roads_Road2_2").gameObject;
        Prefab_Road3 = PrefabManager.GetPrefab("Roads_Road3").gameObject;
        Prefab_Road4 = PrefabManager.GetPrefab("Roads_Road4").gameObject;

        Prefab_Bridge1 = PrefabManager.GetPrefab("Bridges_Bridge1").gameObject;
        Prefab_Bridge2 = PrefabManager.GetPrefab("Bridges_Bridge2").gameObject;
        Prefab_Bridge2_2 = PrefabManager.GetPrefab("Bridges_Bridge2_2").gameObject;
        Prefab_Bridge3 = PrefabManager.GetPrefab("Bridges_Bridge3").gameObject;
        Prefab_Bridge4 = PrefabManager.GetPrefab("Bridges_Bridge4").gameObject;

        Prefab_Ladder1 = PrefabManager.GetPrefab("Ladders_Ladder_1").gameObject;

        Prefab_Wall = PrefabManager.GetPrefab("Walls_Wall").gameObject;
        Prefab_Wall_Road = PrefabManager.GetPrefab("Walls_Wall_Road").gameObject;
    }

    public void Generate() {
        InitPrefabs();

        if (SpawnedObjects != null) {
            Debug.Log(SpawnedObjects.Count);
            for (var i = 0; i < SpawnedObjects.Count;) {
                if(SpawnedObjects[i] != null)
                    Destroy(SpawnedObjects[i]);
                SpawnedObjects.RemoveAt(i);
            }
        }
        SpawnedObjects = new List<GameObject>();

        #region Containers
        var container_Town = SpawnObject(
            PrefabManager.GetPrefab("Managers_Town").gameObject, 
            transform.position, 
            transform.rotation, 
            null, 
            true);

        var container_Ground = SpawnObject(
            PrefabManager.GetPrefab("Managers_Ground_Container").gameObject, 
            transform.position, transform.rotation, 
            null, 
            true).transform;

        var container_Roads = SpawnObject(
            PrefabManager.GetPrefab("Managers_Roads_Container").gameObject,
            transform.position, transform.rotation,
            null,
            true).transform;

        var container_Bridges = SpawnObject(
            PrefabManager.GetPrefab("Managers_Bridges_Container").gameObject,
            transform.position, transform.rotation,
            null,
            true).transform;

        var container_Walls = SpawnObject(
            PrefabManager.GetPrefab("Managers_Walls_Container").gameObject,
            transform.position, transform.rotation,
            null,
            true).transform;
        #endregion

        #region Earth
        var chunkSize = 10;
        for (var chunkX = 0; chunkX < Size / chunkSize; chunkX++) {
            for (var chunkY = 0; chunkY < Size / chunkSize; chunkY++)
            {
               var chunk = SpawnObject(
                PrefabManager.GetPrefab("Chunk").gameObject,
                transform.position, transform.rotation,
                container_Ground,
                true).transform;
                chunk.name = $"EarthChunk-{chunkX}-{chunkY}";

                for (var dx = 0; dx < chunkSize; dx++)
                {
                    for (var dy = 0; dy < chunkSize; dy++)
                    {
                        var x = chunkX * chunkSize + dx;
                        var y = chunkY * chunkSize + dy;
                        if(Form[x, y] && !RiversHashset.Contains(new Vector2Int(x, y)))
                            SpawnEarthBlockAt(new Vector2Int(x, y), chunk);
                    }
                }

                var mesh = Combine_Mesh(chunk);
                chunk.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
        #endregion

        container_Town.GetComponent<TownManager>().RegisterRoads(Roads);
        container_Town.GetComponent<TownManager>().RegisterRegions(CityRegions);

        //#region Walls
        //SpawnWallsAroundRegions(1, container_Walls);
        //#endregion

        #region buildings
        List<B_House> houses = new List<B_House>();
        foreach (var building in Buildings)
        {
            if (SpawnHouse(building, container_Town.GetComponent<TownManager>().HousesContainer, out var newHouse))
            {
                houses.Add(newHouse);
            }
        }
        container_Town.GetComponent<TownManager>().RegisterHouses(houses);
        #endregion

        #region roads
        MergeBridgeHeightToHeights();

        var roadsCheckList = new List<(Vector2Int, int)>() {
            (Vector2Int.up, 1),
            (Vector2Int.right, 2),
            (Vector2Int.down, 4),
            (Vector2Int.left, 8),
        };
        foreach (var road in Roads) {
            SpawnRoadAt(road.Key, container_Roads, container_Bridges, roadsCheckList);
        }
        #endregion
    }

    public int GetHeigthAt(Vector2Int pos) {
        if (!IsInBounds(pos))
            return 0;
        return Heights[pos.x, pos.y];
    }

    public bool GetFormAt(Vector2Int pos)
    {
        if (!IsInBounds(pos))
            return false;
        return Form[pos.x, pos.y] && !RiversHashset.Contains(pos);
    }

    public bool IsInBounds(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Size && pos.y < Size;
    }

    public void SpawnEarthBlockAt(Vector2Int pos, Transform container_Ground)
    {
        List<(Vector2Int, int)> pointToCheck = new List<(Vector2Int, int)>();
        var heightNow = Heights[pos.x, pos.y];

        var neighboursCount = 0;
        for (var dx = -1; dx < 2; dx++)
        {
            for (var dy = -1; dy < 2; dy++)
            {
                var newP = new Vector2Int(pos.x + dx, pos.y + dy);
                if (newP.x < 0 || newP.x >= Size || newP.y < 0 || newP.y >= Size)
                    continue;

                if (heightNow == Heights[newP.x, newP.y])
                    neighboursCount++;
            }
        }
        if (neighboursCount == 9)
        {
            SpawnObject(
                Prefab_Earth_Plane_Top_Big,
                new Vector3(pos.x, Heights[pos.x, pos.y] * HeightStep - 2, pos.y),
                Quaternion.Euler(-90, 0, 0), 
                container_Ground, 
                false);
            return;
        }


        for (var dx = 0; dx < 2; dx++)
        {
            for (var dy = 0; dy < 2; dy++)
            {
                var rotationValue = 0;
                switch (dx)
                {
                    case 0:
                        switch (dy)
                        {
                            case 0:
                                rotationValue = 3;
                                pointToCheck = new List<(Vector2Int, int)>() {
                                    (new Vector2Int(-1, 0), 1),
                                    (new Vector2Int(-1, -1), 2),
                                    (new Vector2Int(0, -1), 4),
                                };
                                break;
                            case 1:
                                rotationValue = 0;
                                pointToCheck = new List<(Vector2Int, int)>() {
                                    (new Vector2Int(0, 1), 1),
                                    (new Vector2Int(-1, 1), 2),
                                    (new Vector2Int(-1, 0), 4),
                                };
                                break;
                        }
                        break;
                    case 1:
                        switch (dy)
                        {
                            case 0:
                                rotationValue = 2;
                                pointToCheck = new List<(Vector2Int, int)>() {
                                (new Vector2Int(0, -1), 1),
                                (new Vector2Int(1, -1), 2),
                                (new Vector2Int(1, 0), 4),
                            };
                                break;
                            case 1:
                                rotationValue = 1;
                                pointToCheck = new List<(Vector2Int, int)>() {
                                (new Vector2Int(1, 0), 1),
                                (new Vector2Int(1, 1), 2),
                                (new Vector2Int(0, 1), 4),
                            };
                                break;
                        }
                        break;
                }
                var value = 0;
                foreach (var p in pointToCheck)
                {
                    var newP = new Vector2Int(pos.x + p.Item1.x, pos.y + p.Item1.y);
                    if (newP.x < 0 || newP.x >= Size || newP.y < 0 || newP.y >= Size)
                        continue;
                    if (heightNow == Heights[newP.x, newP.y])
                    {
                        value += p.Item2;
                    }
                }

                GameObject prefab = null;
                GameObject prefab2 = null;
                GameObject prefab3 = null;
                switch (value)
                {
                    case 0:
                        prefab = Prefab_Earth_Corner_Top;
                        prefab2 = Prefab_Earth_Corner_Middle;
                        prefab3 = Prefab_Earth_Corner_Bottom;
                        break;
                    case 1:
                        rotationValue = (rotationValue + 3) % 4;
                        prefab = Prefab_Earth_Side_Top;
                        prefab2 = Prefab_Earth_Side_Middle;
                        prefab3 = Prefab_Earth_Side_Bottom;
                        break;
                    case 2:
                        prefab = Prefab_Earth_Corner_Top;
                        prefab2 = Prefab_Earth_Corner_Middle;
                        prefab3 = Prefab_Earth_Corner_Bottom;
                        break;
                    case 3:
                        rotationValue = (rotationValue + 3) % 4;
                        prefab = Prefab_Earth_Side_Top;
                        prefab2 = Prefab_Earth_Side_Middle;
                        prefab3 = Prefab_Earth_Side_Bottom;
                        break;
                    case 4:
                        prefab = Prefab_Earth_Side_Top;
                        prefab2 = Prefab_Earth_Side_Middle;
                        prefab3 = Prefab_Earth_Side_Bottom;
                        break;
                    case 5:
                        prefab = Prefab_Earth_Inner_Top;
                        prefab2 = Prefab_Earth_Inner_Middle;
                        break;
                    case 6:
                        prefab = Prefab_Earth_Side_Top;
                        prefab2 = Prefab_Earth_Side_Middle;
                        prefab3 = Prefab_Earth_Side_Bottom;
                        break;
                    case 7:
                        prefab = Prefab_Earth_Plane_Top;
                        break;
                }

                if (prefab == null)
                    continue;

                SpawnObject(
                    prefab, 
                    new Vector3(pos.x + dx * 0.5f - 0.25f, Heights[pos.x, pos.y] * HeightStep - 2, pos.y + dy * 0.5f - 0.25f), 
                    Quaternion.Euler(-90, 90 * rotationValue, 0), 
                    container_Ground, 
                    false);

                if (prefab2 == null)
                    continue;

                SpawnObject(
                    prefab2,
                    new Vector3(pos.x + dx * 0.5f - 0.25f, Heights[pos.x, pos.y] * HeightStep - 2, pos.y + dy * 0.5f - 0.25f),
                    Quaternion.Euler(-90, 90 * rotationValue, 0),
                    container_Ground,
                    false);

                if (prefab3 == null)
                    continue;

                SpawnObject(
                    prefab3,
                    new Vector3(pos.x + dx * 0.5f - 0.25f, Heights[pos.x, pos.y] * HeightStep - 2, pos.y + dy * 0.5f - 0.25f),
                    Quaternion.Euler(-90, 90 * rotationValue, 0),
                    container_Ground,
                    false);
            }
        }
    }

    public GameObject SpawnObject(GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent, bool registerInList = true) {
        var spawnedObject = Instantiate(prefab, pos, rotation);
        if (parent != null)
            spawnedObject.transform.SetParent(parent);

        if (registerInList)
            SpawnedObjects.Add(spawnedObject);
        return spawnedObject;
    }

    public bool SpawnHouse(BuildingData buildingData, Transform housesContainer, out B_House result) {
        result = null;
        if (buildingData.poses.Length == 0)
            return false;
        var basePosCoords = buildingData.poses[0];
        var basePos = new Vector3(basePosCoords.x, Heights[basePosCoords.x, basePosCoords.y], basePosCoords.y);
        var h_base = SpawnObject(
            Prefab_B_House, 
            basePos,
            Quaternion.Euler(0, 0, 0),
            housesContainer, 
            true);

        List<(GameObject, int)> houseBlocks = new List<(GameObject, int)>();

        var floors = buildingData.floors;

        if (buildingData.poses.Length == 1) {
            var posNow = buildingData.poses[0];
            houseBlocks.Add(
                (SpawnObject(
                floors == 1 ? Prefab_House1 : Prefab_House1_2,
                new Vector3(posNow.x, Heights[posNow.x, posNow.y] * HeightStep, posNow.y),
                Quaternion.Euler(0, 90 * Random.Range(0, 4), 0),
                h_base.transform,
                false), 0)
                );
        }

        foreach (var posNow in buildingData.poses)
        {
            var collider = h_base.AddComponent<BoxCollider>();
            collider.size = new Vector3(1, buildingData.floors <= 1 ? 0.8f : 1.2f, 1);
            collider.center = new Vector3(posNow.x - h_base.transform.position.x, collider.size.y / 2, posNow.y - h_base.transform.position.z);

            var neighboursOffset = new List<Vector2Int>();
            foreach (var offset in GetOffsets())
                if (buildingData.poses.Contains(posNow + offset))
                    neighboursOffset.Add(offset);
        
            if (neighboursOffset.Count == 1)
            {
                var neighbourOffset = neighboursOffset[0];
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                if (neighbourOffset == Vector2Int.down)
                    rotation = Quaternion.Euler(0, 90, 0);
                if (neighbourOffset == Vector2Int.right)
                    rotation = Quaternion.Euler(0, 0, 0);
                if (neighbourOffset == Vector2Int.up)
                    rotation = Quaternion.Euler(0, -90, 0);
                if (neighbourOffset == Vector2Int.left)
                    rotation = Quaternion.Euler(0, 180, 0);

                houseBlocks.Add(
                    (SpawnObject(
                    floors == 1 ? Prefab_House2_1 : Prefab_House2_1_2,
                    new Vector3(posNow.x, Heights[posNow.x, posNow.y] * HeightStep, posNow.y),
                    rotation,
                    h_base.transform,
                    false), 1)
                    );
            }

            if (neighboursOffset.Count == 2)
            {
                if ((neighboursOffset[0].x == neighboursOffset[1].x && neighboursOffset[0].y == -neighboursOffset[1].y) ||
                    (neighboursOffset[0].y == neighboursOffset[1].y && neighboursOffset[0].x == -neighboursOffset[1].x))
                {

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    if (neighboursOffset[0].x == neighboursOffset[1].x)
                        rotation = Quaternion.Euler(0, 90, 0);
                    if (neighboursOffset[0].y == neighboursOffset[1].y)
                        rotation = Quaternion.Euler(0, 0, 0);


                    houseBlocks.Add(
                        (SpawnObject(
                        floors == 1 ? Prefab_House3_1 : Prefab_House3_1_2,
                        new Vector3(posNow.x, Heights[posNow.x, posNow.y] * HeightStep, posNow.y),
                        rotation,
                        h_base.transform,
                        false), 2)
                        );
                }
                else
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    var value = neighboursOffset[0] + neighboursOffset[1];
                    if (value == new Vector2Int(-1, 1)) {
                        rotation = Quaternion.Euler(0, 0, 0);
                    }
                    if (value == new Vector2Int(1, 1))
                    {
                        rotation = Quaternion.Euler(0, 90, 0);
                    }
                    if (value == new Vector2Int(1, -1))
                    {
                        rotation = Quaternion.Euler(0, 180, 0);
                    }
                    if (value == new Vector2Int(-1, -1))
                    {
                        rotation = Quaternion.Euler(0, 270, 0);
                    }


                    houseBlocks.Add(
                        (SpawnObject(
                        floors == 1 ? Prefab_House3_2 : Prefab_House3_2_2,
                        new Vector3(posNow.x, Heights[posNow.x, posNow.y] * HeightStep, posNow.y),
                        rotation,
                        h_base.transform,
                        false), 3)
                        );
                }
            }
        }
        var b_house = h_base.GetComponent<B_House>();
        b_house.SetBuildingData(buildingData);
        b_house.SetBuildingPrefabs(houseBlocks.ToArray());
        b_house.InitData(houseBlocks.Count());
        result = b_house;
        return true;
    }

    void MergeBridgeHeightToHeights()
    {
        foreach (var e in BridgesHeights)
        {
            Heights[e.Key.x, e.Key.y] = e.Value;
        }
    }

    void SpawnRoadAt(Vector2Int pos, Transform container_Road, Transform container_Bridges, List<(Vector2Int, int)> checkList) {
        var value = 0;
        bool isBridge = RiversHashset.Contains(pos) || !Form[pos.x, pos.y];
        var heightNow = isBridge ? BridgesHeights[new Vector2Int(pos.x, pos.y)] : Heights[pos.x, pos.y];
        if (heightNow <= 0)
            isBridge = true;
        bool isHasLadder = false;
        int ladderDelta = 0;
        foreach (var offset in checkList) {
            var newPos = pos + offset.Item1;
            if (newPos.x < 0 || newPos.y < 0 || newPos.x >= Size || newPos.y >= Size)
                continue;

            if (Roads.ContainsKey(newPos))
            {
                if (Heights[newPos.x, newPos.y] <= heightNow)
                {
                    value += offset.Item2;
                }

                var heightDelta = Heights[newPos.x, newPos.y] - heightNow;
                if (heightDelta > 0)
                {
                    if (ladderDelta < heightDelta)
                    {
                        ladderDelta = heightDelta;
                    }
                    isHasLadder = true;
                }
            }
            //if (TownManager.singleton.IsBuildingAt(newPos) || Heights[newPos.x, newPos.y] == heightNow) {
            //}

        }
        var pair = GetRoadObject(value, isBridge);
        var prefab = pair.Item1;
        var rotation = pair.Item2;

        var roadPos = new Vector3(pos.x, heightNow + 0.01f, pos.y);
        if (prefab != null)
        {
            SpawnObject(
                prefab,
                roadPos,
                Quaternion.Euler(0, 90 * rotation, 0),
                isBridge ? container_Bridges : container_Road,
                false);
        }

        if (isHasLadder) {
            for (var i = 0; i < ladderDelta; i++)
            {
                var ladderPos = new Vector3(pos.x, heightNow + 0.5f + i, pos.y);
                SpawnObject(
                    Prefab_Ladder1,
                    ladderPos,
                    Quaternion.Euler(0, 90 * (rotation + 1), 0),
                    isBridge ? container_Bridges : container_Road,
                    false);
            }


            heightNow += ladderDelta;
            value = 0;
            foreach (var offset in checkList)
            {
                var newPos = pos + offset.Item1;
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= Size || newPos.y >= Size)
                    continue;

                if (Roads.ContainsKey(newPos) && Heights[newPos.x, newPos.y] == heightNow)
                {
                    value += offset.Item2;
                }
            }

            pair = GetRoadObject(value, true);
            prefab = pair.Item1;
            rotation = pair.Item2;

            if (prefab != null)
            {
                roadPos = new Vector3(pos.x, heightNow + 0.01f, pos.y);
                SpawnObject(
                    prefab,
                    roadPos,
                    Quaternion.Euler(0, 90 * rotation, 0),
                    isBridge ? container_Bridges : container_Road,
                    false);
            }
        }
    }

    (GameObject, int) GetRoadObject(int value, bool isBridge) {
        GameObject prefab = null;
        var rotation = 0;
        switch (value)
        {
            case 0:
                prefab = null;
                break;
            case 1:
                prefab = isBridge ? Prefab_Bridge1 : Prefab_Road1;
                rotation = 0;
                break;
            case 2:
                prefab = isBridge ? Prefab_Bridge1 : Prefab_Road1;
                rotation = 1;
                break;
            case 3:
                prefab = isBridge ? Prefab_Bridge2 : Prefab_Road2;
                rotation = 0;
                break;
            case 4:
                prefab = isBridge ? Prefab_Bridge1 : Prefab_Road1;
                rotation = 2;
                break;
            case 5:
                prefab = isBridge ? Prefab_Bridge2_2 : Prefab_Road2_2;
                rotation = 0;
                break;
            case 6:
                prefab = isBridge ? Prefab_Bridge2 : Prefab_Road2;
                rotation = 1;
                break;
            case 7:
                prefab = isBridge ? Prefab_Bridge3 : Prefab_Road3;
                rotation = 1;
                break;
            case 8:
                prefab = isBridge ? Prefab_Bridge1 : Prefab_Road1;
                rotation = 3;
                break;
            case 9:
                prefab = isBridge ? Prefab_Bridge2 : Prefab_Road2;
                rotation = 3;
                break;
            case 10:
                prefab = isBridge ? Prefab_Bridge2_2 : Prefab_Road2_2;
                rotation = 1;
                break;
            case 11:
                prefab = isBridge ? Prefab_Bridge3 : Prefab_Road3;
                rotation = 0;
                break;
            case 12:
                prefab = isBridge ? Prefab_Bridge2 : Prefab_Road2;
                rotation = 2;
                break;
            case 13:
                prefab = isBridge ? Prefab_Bridge3 : Prefab_Road3;
                rotation = 3;
                break;
            case 14:
                prefab = isBridge ? Prefab_Bridge3 : Prefab_Road3;
                rotation = 2;
                break;
            case 15:
                prefab = isBridge ? Prefab_Bridge4 : Prefab_Road4;
                rotation = 0;
                break;
        }
        return (prefab, rotation);
    }

    //public void SpawnWallsAroundRegions(int aroundDepth, Transform container_Walls) {
    //    var townManager = TownManager.singleton;
    //    foreach (var subregion in townManager.Subregions)
    //    {
    //        if (subregion.Parent.Depth == aroundDepth)
    //        {
    //            for (var x = 0; x < 5; x++)
    //            {
    //                for (var y = 0; y < 5; y++)
    //                {
    //                    var pos = new Vector2Int(subregion.pivotPos.x + x, subregion.pivotPos.y + y);
    //
    //                    foreach (var offset in GetOffsets())
    //                    {
    //                        var neighbour = townManager.GetSubRegionAt(pos + offset);
    //                        if (neighbour != subregion && neighbour != null && neighbour.Parent.Depth > aroundDepth)
    //                        {
    //                            var objectPos = new Vector3(pos.x, Heights[pos.x, pos.y], pos.y);
    //                            SpawnObject(
    //                                Prefab_Wall,
    //                                objectPos,
    //                                Quaternion.identity,
    //                                container_Walls,
    //                                false);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public Mesh Combine_Mesh(Transform target) {
        MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        target.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        target.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        target.transform.gameObject.SetActive(true);
        return target.transform.GetComponent<MeshFilter>().mesh;
    }

    public IEnumerable<Vector2Int> GetOffsets()
    {
        yield return new Vector2Int(0, 1);
        yield return new Vector2Int(1, 0);
        yield return new Vector2Int(0, -1);
        yield return new Vector2Int(-1, 0);
    }
}
