using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CityRegionBlank
{
    public int Depth;
    public Vector2Int pivotPos;
    public HashSet<Vector2Int> subRegionsPoints;
}

public class CityRegion
{
    public int Depth;
    public Vector2Int pivotPos;
    /// <summary>
    /// Vector2Int - coords from 0 to 3 in region
    /// </summary>
    public Dictionary<Vector2Int, CitySubRegion> subRegions;
}


public class CitySubRegion
{
    public Vector2Int pivotPos;
    public HashSet<Vector2Int> points;
    public CityRegion Parent;
}

public class G_CityRegions
{
    DijkstraPathfind dijkstraPathfind;

    public G_CityRegions()
    {
        dijkstraPathfind = new DijkstraPathfind();
    }

    public (HashSet<Vector2Int>, Vector2Int) GenerateCityCells(int maxCount, bool[,] form)
    {
        var size = new Vector2Int(form.GetLength(0), form.GetLength(1));

        var RegionSeparator = new RegionSeparator();
        RegionSeparator.InitSeparator(form);
        var regions = RegionSeparator.GetRegions();
        var outterLine = RegionSeparator.GetOutterBorderLine(regions[0].Item1, size);

        float bestValue = 0;
        Vector2Int bestPoint = Vector2Int.zero;
        var height = size.y;
        var heightOffset = height * Random.Range(0.3f, 0.7f);
        foreach (var point in outterLine)
        {
            float value = point.x + (height - point.y);
            if ((bestValue < value) || (bestValue == value && point.x > bestPoint.x && point.y < heightOffset))
            {
                bestValue = value;
                bestPoint = point;
            }
        }

        var weights = new int[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                weights[x, y] = Random.Range(1, 10);

                if (!form[x, y])
                {
                    weights[x, y] += 10000;
                }

                for (var dx = -2; dx < 3; dx++)
                {
                    for (var dy = -2; dy < 3; dy++)
                    {
                        var nX = x + dx;
                        var nY = y + dy;

                        if (nX < 0 || nX > size.x - 1 || nY < 0 || nY > size.y - 1)
                        {
                            weights[x, y] += 10;
                            continue;
                        }
                        if (!form[nX, nY])
                        {
                            weights[x, y] += 10;
                        }
                    }
                }

            }
        }

        var newWeights = dijkstraPathfind.GetMapWeights(bestPoint, weights);
        var sortedWeights = new List<(Vector2Int, int)>();

        for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
                sortedWeights.Add((new Vector2Int(x, y), newWeights[x, y]));

        var sorter = new SectionSorter();
        sortedWeights.Sort((a, b) => sorter.Compare(a, b));

        var possibleTiles = new HashSet<Vector2Int>();

        for (var i = 0; i < maxCount; i++)
        {
            if (sortedWeights.Count <= i)
                break;
            possibleTiles.Add(sortedWeights[i].Item1);
        }

        return (possibleTiles, bestPoint);
    }

    public HashSet<Vector2Int> GenerateCitySubRegion(HashSet<Vector2Int> cityCells, int size) {
        var result = new HashSet<Vector2Int>();

        var subSize = size / 5;
        for (var x = 0; x < subSize; x++)
        {
            for (var y = 0; y < subSize; y++)
            {
                var flag = false;
                for (var dx = 0; dx < 5; dx++)
                {
                    for (var dy = 0; dy < 5; dy++)
                    {
                        if (cityCells.Contains(new Vector2Int(x * 5 + dx, y * 5 + dy))) {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        break;
                }
                if (flag)
                    result.Add(new Vector2Int(x * 5, y * 5));

            }
        }

        return result;
    }

    public List<CityRegionBlank> GenerateCityRegionsCoordinates(HashSet<Vector2Int> subRegions) {
        var bestValue = int.MinValue;
        var bestPos = Vector2Int.zero;

        foreach (var e in subRegions) {
            var value = e.x - e.y;
            if (value > bestValue || (value == bestValue && e.x < bestPos.x)) {
                bestValue = value;
                bestPos = e;
            }
        }
        bestPos = bestPos + Vector2Int.left * 10;

        var result = new List<CityRegionBlank>();

        var queue = new Queue<(Vector2Int, int)>();
        queue.Enqueue((bestPos, 0));
        var checkedPoses = new HashSet<Vector2Int>();

        while (queue.Count > 0)
        {
            var e = queue.Dequeue();
            var pointNow = e.Item1;
            if (checkedPoses.Contains(pointNow))
                continue;
            checkedPoses.Add(pointNow);
            var newRegion = new HashSet<Vector2Int>();
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    var offset = new Vector2Int(x * 5, y * 5);
                    var newPos = pointNow + offset;
                    if (subRegions.Contains(newPos))
                    {
                        newRegion.Add(newPos);
                    }
                }
            }
            if (newRegion.Count > 0)
            {
                result.Add(new CityRegionBlank() { subRegionsPoints = newRegion, Depth = e.Item2, pivotPos = pointNow });
            
                queue.Enqueue((pointNow + Vector2Int.down * 15, e.Item2 + 1));
                queue.Enqueue((pointNow + Vector2Int.up * 15, e.Item2 + 1));
                queue.Enqueue((pointNow + Vector2Int.left * 15, e.Item2 + 1));
                queue.Enqueue((pointNow + Vector2Int.right * 15, e.Item2 + 1));
            }
        }

        return result;
    }

    public List<CityRegion> CompileCityRegions(List<CityRegionBlank> cityRegionsBlanks) {
        var result = new List<CityRegion>();
        foreach (var regionBlank in cityRegionsBlanks) {
            var newSubRegions = new Dictionary<Vector2Int, CitySubRegion>();
            var newRegion = new CityRegion()
            {
                Depth = regionBlank.Depth,
                pivotPos = regionBlank.pivotPos,
                subRegions = newSubRegions
            };

            var allRegionPoints = regionBlank.subRegionsPoints;
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    var subRegionPoints = new HashSet<Vector2Int>();
                    for (var dx = 0; dx < 5; dx++)
                    {
                        for (var dy = 0; dy < 5; dy++)
                        {
                            var newPos = new Vector2Int(x * 5 + dx, y * 5 + dy) + regionBlank.pivotPos;
                            if (allRegionPoints.Contains(newPos)) {
                                subRegionPoints.Add(newPos);
                            }
                        }
                    }

                    if (subRegionPoints.Count == 0)
                        continue;
                    newRegion.subRegions[new Vector2Int(x, y)] = new CitySubRegion()
                    {
                        pivotPos = regionBlank.pivotPos + new Vector2Int(x * 5, y * 5),
                        points = subRegionPoints,
                        Parent = newRegion
                    };
                }
            }
            result.Add(newRegion);
        }
        return result;
    }

    private class SectionSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var a = ((Vector2Int, int))(x);
            var b = ((Vector2Int, int))(y);
            if (a.Item2 == b.Item2)
                return 0;
            return a.Item2 > b.Item2 ? 1 : -1;
        }
    }
}
