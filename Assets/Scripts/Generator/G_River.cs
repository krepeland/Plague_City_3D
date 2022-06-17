using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class G_River
{
    DijkstraPathfind dijkstraPathfind;
    G_RiverSettings settings;

    public G_River()
    {
        dijkstraPathfind = new DijkstraPathfind();
    }

    public void SetSettings(G_RiverSettings settings) {
        this.settings = settings;
    }

    public List<Vector2Int> GenerateRiver(bool[,] map, Vector2Int startPos, List<Vector2Int> excludedPoints) 
    {
        //Initializing map
        var size = new Vector2Int(map.GetLength(0), map.GetLength(1));
        var weights = new int[size.x, size.y];

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                weights[x, y] = Random.Range(1, 50);

                if (!map[x, y])
                {
                    weights[x, y] += 100000;
                }

                for (var dx = -2; dx < 3; dx++)
                {
                    for (var dy = -2; dy < 3; dy++)
                    {
                        var nX = x + dx;
                        var nY = y + dy;

                        if (nX < 0 || nX > size.x - 1 || nY < 0 || nY > size.y - 1)
                        {
                            weights[x, y] += 1000;
                            continue;
                        }
                        if (!map[nX, nY])
                        {
                            weights[x, y] += 1000;
                        }
                    }
                }

            }
        }

        //Separeting map on regions
        var RegionSeparator = new RegionSeparator();
        RegionSeparator.InitSeparator(map);
        var regions = RegionSeparator.GetRegions();

        var outterLine = RegionSeparator.GetOutterBorderLine(regions[0].Item1, size);

        var excludedPointsCopy = new HashSet<Vector2Int>();
        if (excludedPoints != null)
        {
            var newPoints = new HashSet<Vector2Int>();
            foreach (var e in excludedPoints)
            {
                for (var dx = -2; dx < 3; dx++)
                {
                    for (var dy = -2; dy < 3; dy++)
                    {
                        var nX = e.x + dx;
                        var nY = e.y + dy;
                        var newPos = new Vector2Int(nX, nY);
                        newPoints.Add(newPos);
                        if (outterLine.Contains(newPos))
                        {
                            outterLine.Remove(newPos);
                        }
                        excludedPointsCopy.Add(newPos);
                    }
                }
            }
        }

        //if (excludedPoints != null) {
        //    foreach (var e in excludedPoints)
        //    {
        //        if (outterLine.Contains(e)) {
        //            outterLine.Remove(e);
        //        }
        //    }
        //}

        //Find river end
        var bestPoint = Vector2Int.zero;
        if (!settings.RandomRiverTargets)
        {
            float bestValue = 0;
            var height = size.y;
            foreach (var point in outterLine)
            {
                float value = point.x * settings.RiverXmultiplier + (height - point.y) * settings.RiverYmultiplier;
                if ((bestValue < value) || (bestValue == value && point.x > bestPoint.x))
                {
                    bestValue = value;
                    bestPoint = point;
                }
            }
        }
        else {
            var list = outterLine.ToList();
            bestPoint = list[Random.Range(0, list.Count)];
        }

        var path = dijkstraPathfind.FindPath(startPos, bestPoint, weights);

        if (settings.IsCanBeDeleted)
        {
            var removeValue = 0;
            foreach (var e in path)
            {
                if (excludedPointsCopy.Contains(e))
                {
                    removeValue++;
                }
            }
            if (removeValue > settings.DeleteValue)
                return new List<Vector2Int>();
        }

        return path;
    }

    public HashSet<Vector2Int> GetRiversHashset(List<List<Vector2Int>> rivers, bool scale) {
        var result = new HashSet<Vector2Int>();

        foreach (var river in rivers)
        {
            foreach (var pos in river)
            {
                if (scale)
                {
                    for (var dx = -1; dx < 1; dx++)
                    {
                        for (var dy = -1; dy < 1; dy++)
                        {
                            var nX = pos.x + dx;
                            var nY = pos.y + dy;
                            result.Add(new Vector2Int(nX, nY));
                        }
                    }
                }
                else
                {
                    result.Add(pos);
                }
            }
        }

        return result;
    }
}
