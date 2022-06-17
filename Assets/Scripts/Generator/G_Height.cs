using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Height
{
    public G_Height() { 
    
    }

    public int[,] GenerateHeight(int size, int height, Vector2Int offset, float scale = 1)
    {
        var result = new int[size, size];

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                result[x, y] = Mathf.RoundToInt(Mathf.PerlinNoise((x + offset.x) * scale, (y + offset.y) * scale) * height) + 1;
            }
        }
        return result;
    }

    public int[,] ClearHeightForm(int size, int[,] heights, bool[,] form) {
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                if(!form[x, y])
                    heights[x, y] = 0;
            }
        }
        return heights;
    }

    public int[,] ClearHeightRiversToZero(int size, int[,] heights, HashSet<Vector2Int> riversPoints) {
        foreach (var p in riversPoints) {
            if (p.x < 0 || p.y < 0 || p.x >= size || p.y >= size)
                continue;
            heights[p.x, p.y] = 0;
        }
        return heights;
    }

    public int[,] ClearHeightRivers(int size, int[,] heights, List<List<Vector2Int>> riversPoints)
    {
        HashSet<Vector2Int> clearedPoints = new HashSet<Vector2Int>();
        bool fixFlag = false;
        foreach (var river in riversPoints) {
            int maxHeight = int.MaxValue;
            foreach (var p in river)
            {
                if (fixFlag) {
                    fixFlag = false;
                    foreach (var e in GetOffsets()) {
                        var newPoint = e + p;
                        if (clearedPoints.Contains(newPoint)) {
                            maxHeight = Mathf.Min(maxHeight, heights[newPoint.x, newPoint.y]);
                        }
                    }
                }

                if (maxHeight == int.MaxValue) {
                    maxHeight = heights[p.x, p.y];
                }
                var newHeight = heights[p.x, p.y];
                if (newHeight < maxHeight) {
                    maxHeight = newHeight;
                }

                heights[p.x, p.y] = maxHeight;
                clearedPoints.Add(p);
            }
            fixFlag = true;
        }

        HashSet<Vector2Int> allPoints = new HashSet<Vector2Int>();
        foreach (var river in riversPoints)
        {
            foreach (var p in river)
            {
                var maxHeight = heights[p.x, p.y];
                for (var dx = -1; dx < 1; dx++)
                {
                    for (var dy = -1; dy < 1; dy++)
                    {
                        var nX = p.x + dx;
                        var nY = p.y + dy;
                        if (nX < 0 || nY < 0 || nX >= size || nY >= size)
                            continue;

                        allPoints.Add(new Vector2Int(nX, nY));
                        if (clearedPoints.Contains(new Vector2Int(nX, nY)))
                            continue;
        
                        heights[nX, nY] = maxHeight;
                    }
                }
            }
        }

        foreach (var p in allPoints)
        {
            var height = heights[p.x, p.y];
            var fixValue = 0;
            foreach (var offset in GetOffsets()) {
                var newPoint = p + offset;
                var nX = newPoint.x;
                var nY = newPoint.y;
                if (nX < 0 || nY < 0 || nX >= size || nY >= size)
                    continue;

                if (heights[nX, nY] < height) {
                    fixValue++;
                }
            }
            if (fixValue == 3) {
                heights[p.x, p.y] = height - 1;
            }
        }

        foreach (var p in allPoints)
        {
            if (heights[p.x, p.y] == 0)
                continue;
            heights[p.x, p.y] -= 1;
        }

        return heights;
    }

    public Dictionary<Vector2Int, int> ClearRoadsHeights(int size, int[,] heights, bool[,] form, HashSet<Vector2Int> riversPoints, Dictionary<Vector2Int, RoadType> roads) {
        HashSet<Vector2Int> checkedRoads = new HashSet<Vector2Int>();
        var result = new Dictionary<Vector2Int, int>();

        foreach (var road in roads.Keys) {
            if (!riversPoints.Contains(road) && form[road.x, road.y])
                checkedRoads.Add(road);
        }
        
        foreach (var roadPair in roads) {
            var road = roadPair.Key;
            if (checkedRoads.Contains(road))
                continue;

            Dictionary<int, int> heightsDistribution = new Dictionary<int, int>();
            HashSet<Vector2Int> roadPoints = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(road);

            while (queue.Count > 0) {
                var nowItem = queue.Dequeue();
                if (roadPoints.Contains(nowItem))
                    continue;

                if (checkedRoads.Contains(nowItem))
                {
                    if (!heightsDistribution.ContainsKey(heights[nowItem.x, nowItem.y]))
                        heightsDistribution[heights[nowItem.x, nowItem.y]] = 0;
                    heightsDistribution[heights[nowItem.x, nowItem.y]] += 1;
                    continue;
                }
                else
                {
                    roadPoints.Add(nowItem);
                }
                foreach (var offset in GetOffsets()) {
                    var newPos = nowItem + offset;
                    if (roads.ContainsKey(newPos)) {
                        queue.Enqueue(newPos);
                    }
                }
            }

            var bestHeight = int.MaxValue;
            var bestValue = 0;
            foreach (var height in heightsDistribution) {
                //if (height.Value > bestValue || (height.Value == bestValue && height.Key < bestHeight)) 
                //{
                //    bestValue = height.Value;
                //    bestHeight = height.Key;
                //}

                if (height.Key < bestHeight)
                {
                    bestValue = height.Value;
                    bestHeight = height.Key;
                }
            }

            foreach (var p in roadPoints)
            {
                checkedRoads.Add(p);
                result[new Vector2Int(p.x, p.y)] = bestHeight;
            }
        }

        return result;
    }

    public IEnumerable<Vector2Int> GetOffsets()
    {
        yield return new Vector2Int(0, 1);
        yield return new Vector2Int(1, 0);
        yield return new Vector2Int(0, -1);
        yield return new Vector2Int(-1, 0);
    }
}
