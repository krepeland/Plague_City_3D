using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoadType 
{ 
    None,
    Main,
    Side,
    BuildingRoad,
    Out
}

public class G_Roads
{
    List<CityRegion> CityRegions;
    Dictionary<Vector2Int, RoadType> Roads;
    DijkstraPathfind dijkstraPathfind;
    Vector2Int size;

    public G_Roads()
    {
    }


    public Dictionary<Vector2Int, RoadType> GenerateRoads(List<CityRegion> cityRegions, bool[,] form, List<List<Vector2Int>> rivers)
    {
        dijkstraPathfind = new DijkstraPathfind();
        Roads = new Dictionary<Vector2Int, RoadType>();
        CityRegions = cityRegions;

        size = new Vector2Int(form.GetLength(0), form.GetLength(1));
        var weights = new int[size.x, size.y];

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                if (!form[x, y])
                    weights[x, y] = 10000;
                else
                    weights[x, y] = 100;
            }
        }

        HashSet<Vector2Int> riverPoints = new HashSet<Vector2Int>();
        foreach (var river in rivers)
        {
            foreach (var p in river) {
                weights[p.x, p.y] = 1000;
                riverPoints.Add(p);
            }
        }

        foreach (var e in cityRegions) {
            if (e.Depth >= 2) {
                foreach (var s in e.subRegions) {
                    foreach (var p in s.Value.points) {
                        weights[p.x, p.y] += 100;
                    }
                }
            }
        }

        Vector2Int roadsCenter = GetRoadTargetInRegion(GetMainRegion());

        foreach (var region in GetCityRegionsWithDepth(1)) {
            var regionTarget = GetRoadTargetInRegion(region);
            var path = dijkstraPathfind.FindPath(roadsCenter, regionTarget, weights);
            AddPath(path, RoadType.Main, true, weights);
        }

        foreach (var region in GetCityRegionsWithDepth(2))
        {
            var regionTarget = GetRoadTargetInRegion(region);
            var path = dijkstraPathfind.FindPath(roadsCenter, regionTarget, weights);
            AddPath(path, RoadType.Main, true, weights);
        }

        foreach (var region in GetCityRegionsWithDepth(3))
        {
            var regionTarget = GetRoadTargetInRegion(region);
            var path = dijkstraPathfind.FindPath(roadsCenter, regionTarget, weights);
            AddPath(path, RoadType.Main, true, weights);
        }

        var subRegionsDict = GetSubRegionsDict();

        foreach (var subRegion in subRegionsDict)
        {
            //var regionTarget = subRegion.Key + new Vector2Int(2, 2);
            var regionTarget = subRegion.Key + new Vector2Int(Random.Range(1, 4), Random.Range(1, 4));
            if (riverPoints.Contains(regionTarget))
                continue;
            var path = dijkstraPathfind.FindPath(roadsCenter, regionTarget, weights);
            AddPath(path, RoadType.Main, true, weights);
        }


        var newWeights = new int[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                if (!form[x, y])
                    newWeights[x, y] = 10000;
                else
                    newWeights[x, y] = 2;
            }
        }
        foreach (var p in Roads)
        {
            newWeights[p.Key.x, p.Key.y] = 1;
        }
        foreach (var region in GetCityRegionsWithDepth(2))
        {
            foreach (var region2 in GetCityRegionsWithDepth(2))
            {
                var regionStart = GetRoadTargetInRegion(region);
                var regionTarget = GetRoadTargetInRegion(region2);
                var path = dijkstraPathfind.FindPath(regionStart, regionTarget, newWeights);
                AddPath(path, RoadType.Main, true, weights);
            }
            foreach (var region2 in GetCityRegionsWithDepth(3))
            {
                var regionStart = GetRoadTargetInRegion(region);
                var regionTarget = GetRoadTargetInRegion(region2);
                var path = dijkstraPathfind.FindPath(regionStart, regionTarget, newWeights);
                AddPath(path, RoadType.Main, true, weights);
            }
        }
        foreach (var region in GetCityRegionsWithDepth(3))
        {
            foreach (var region2 in GetCityRegionsWithDepth(4))
            {
                var regionStart = GetRoadTargetInRegion(region);
                var regionTarget = GetRoadTargetInRegion(region2);
                var path = dijkstraPathfind.FindPath(regionStart, regionTarget, newWeights);
                AddPath(path, RoadType.Main, true, weights);
            }
        }

        //var newPath = dijkstraPathfind.FindPath(roadsCenter, new Vector2Int(0, size.y / 2 + Random.Range(0, 10)), weights);
        //AddPath(newPath, RoadType.Side, false, weights);

        ClearRoads();

        return Roads;
    }

    void ClearRoads() {
        for (var i = 0; i <= 10; i++)
        {
            List<Vector2Int> RoadsToDelete = new List<Vector2Int>();
            for (var x = 0; x < size.x - 1; x++)
            {
                for (var y = 0; y < size.x - 1; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var value = 0;
                    foreach (var p in GetPointsToClear(pos))
                    {
                        if (Roads.ContainsKey(p))
                            value++;
                    }

                    if (value >= 4)
                    {
                        foreach (var p in GetPointsToClear(pos))
                        {
                            var newValue = 0;
                            foreach (var offset in GetOffsets())
                            {
                                var newP = p + offset;
                                if (Roads.ContainsKey(newP))
                                    newValue++;

                            }
                            if (newValue == 2)
                            {
                                RoadsToDelete.Add(p);
                            }
                        }

                    }
                }
            }
            foreach (var p in RoadsToDelete)
            {
                Roads.Remove(p);
            }
        }

        for (var x = 0; x < size.x - 1; x++)
        {
            for (var y = 0; y < size.x - 1; y++)
            {
                var pos = new Vector2Int(x, y);
                if (!Roads.ContainsKey(pos) &&
                    Roads.ContainsKey(pos + new Vector2Int(1, 0)) &&
                    Roads.ContainsKey(pos + new Vector2Int(0, 1)) &&
                    !Roads.ContainsKey(pos + new Vector2Int(1, 1)))
                {
                    Roads[pos] = RoadType.Main;
                }
                if (Roads.ContainsKey(pos) &&
                    !Roads.ContainsKey(pos + new Vector2Int(1, 0)) &&
                    !Roads.ContainsKey(pos + new Vector2Int(0, 1)) &&
                    Roads.ContainsKey(pos + new Vector2Int(1, 1))
                    )
                {
                    Roads[pos + new Vector2Int(1, 0)] = RoadType.Main;
                }
            }
        }
    }

    void AddPath(List<Vector2Int> path, RoadType roadType, bool IsAddToWeights, int[,] weights) {
        foreach (var e in path)
        {
            var pos = new Vector2Int(e.x, e.y);
            if (!Roads.ContainsKey(pos))
            {
                Roads[pos] = roadType;
            }
            if(IsAddToWeights)
                weights[e.x, e.y] = 1;
        }
    }

    Vector2Int GetRoadTargetInRegion(CityRegion cityRegion)
    {
        Vector2Int roadsCenter;
        if (cityRegion.subRegions.ContainsKey(Vector2Int.one))
            roadsCenter = cityRegion.subRegions[Vector2Int.one].pivotPos;
        else
            roadsCenter = cityRegion.subRegions.ToList()[Random.Range(0, cityRegion.subRegions.Count)].Value.pivotPos;

        roadsCenter += new Vector2Int(2, 2);
        return roadsCenter;
    }

    public CityRegion GetMainRegion()
    {
        foreach (var region in CityRegions)
            if (region.Depth == 0)
                return region;
        return null;
    }

    public List<CityRegion> GetCityRegionsWithDepth(int depth) {
        var result = new List<CityRegion>();
        foreach (var region in CityRegions) {
            if (region.Depth == depth) {
                result.Add(region);
            }
        }
        return result;
    }

    public Dictionary<Vector2Int, CitySubRegion> GetSubRegionsDict() {
        var result = new Dictionary<Vector2Int, CitySubRegion>();

        foreach (var region in CityRegions)
        {
            foreach (var subRegion in region.subRegions)
            {
                var pos = subRegion.Key * 5 + region.pivotPos;
                result[pos] = subRegion.Value;
            }
        }

        return result;
    }

    public IEnumerable<Vector2Int> GetPointsToClear(Vector2Int pos) {
        yield return pos;
        yield return pos + Vector2Int.up;
        yield return pos + Vector2Int.right;
        yield return pos + new Vector2Int(1, 1);
    }

    public IEnumerable<Vector2Int> GetOffsets() {
        yield return new Vector2Int(0, 1);
        //yield return new Vector2Int(1, 1);
        yield return new Vector2Int(1, 0);
        //yield return new Vector2Int(1, -1);
        yield return new Vector2Int(0, -1);
        //yield return new Vector2Int(-1, -1);
        yield return new Vector2Int(-1, 0);
        //yield return new Vector2Int(-1, 1);
    }
}
