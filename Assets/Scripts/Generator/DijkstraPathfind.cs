using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfind
{
    int[,] Map;
    Vector2Int[,] Offsets;
    int[,] Weights;

    Vector2Int mapSize;

    public List<Vector2Int> FindPath(Vector2Int startPoint, Vector2Int finishPoint, int[,] map)
    {
        Map = map;
        mapSize = new Vector2Int(Map.GetLength(0), Map.GetLength(1));

        Weights = new int[mapSize.x, mapSize.y];

        for (var x = 0; x < mapSize.x; x++)
            for (var y = 0; y < mapSize.y; y++)
                Weights[x, y] = int.MaxValue;

        SetWeightAt(startPoint, 0);

        Offsets = new Vector2Int[mapSize.x, mapSize.y];
        Offsets[startPoint.x, startPoint.y] = startPoint;

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(startPoint);

        while (queue.Count > 0)
        {
            var pointNow = queue.Dequeue();
            if (!IsInMap(pointNow))
                continue;
            var weightNow = GetWeightAt(pointNow);

            foreach (var offset in GetNeighboursOffsets())
            {
                var newPoint = pointNow + offset;
                if (!IsInMap(newPoint))
                    continue;
                var newWeight = weightNow + GetMapValueAt(newPoint);
                if (newWeight < GetWeightAt(newPoint))
                {
                    SetWeightAt(newPoint, newWeight);
                    Offsets[newPoint.x, newPoint.y] = pointNow;

                    queue.Enqueue(newPoint);
                }
            }
        }

        var point = finishPoint;
        var path = new List<Vector2Int>();

        while (point != startPoint) {
            path.Add(point);
            point = Offsets[point.x, point.y];
        }

        path.Add(point);
        path.Reverse();

        return path;
    }

    public int[,] GetMapWeights(Vector2Int startPoint, int[,] map) {
        Map = map;
        mapSize = new Vector2Int(Map.GetLength(0), Map.GetLength(1));

        Weights = new int[mapSize.x, mapSize.y];

        for (var x = 0; x < mapSize.x; x++)
            for (var y = 0; y < mapSize.y; y++)
                Weights[x, y] = int.MaxValue;

        SetWeightAt(startPoint, 0);

        Offsets = new Vector2Int[mapSize.x, mapSize.y];
        Offsets[startPoint.x, startPoint.y] = startPoint;

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(startPoint);

        while (queue.Count > 0)
        {
            var pointNow = queue.Dequeue();
            if (!IsInMap(pointNow))
                continue;
            var weightNow = GetWeightAt(pointNow);

            foreach (var offset in GetNeighboursOffsets())
            {
                var newPoint = pointNow + offset;
                if (!IsInMap(newPoint))
                    continue;
                var newWeight = weightNow + GetMapValueAt(newPoint);
                if (newWeight < GetWeightAt(newPoint))
                {
                    SetWeightAt(newPoint, newWeight);
                    Offsets[newPoint.x, newPoint.y] = pointNow;

                    queue.Enqueue(newPoint);
                }
            }
        }

        return Weights;
    }

    bool IsInMap(Vector2Int pos)
    {
        return IsInMap(pos.x, pos.y);
    }

    bool IsInMap(int x, int y)
    {
        return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
    }

    public int GetMapValueAt(Vector2Int pos)
    {
        return Map[pos.x, pos.y];
    }

    public int GetWeightAt(Vector2Int pos)
    {
        return Weights[pos.x, pos.y];
    }

    public void SetWeightAt(Vector2Int pos, int newWeight)
    {
        Weights[pos.x, pos.y] = newWeight;
    }

    IEnumerable<Vector2Int> GetNeighboursOffsets()
    {
        yield return Vector2Int.down;
        yield return Vector2Int.right;
        yield return Vector2Int.left;
        yield return Vector2Int.up;
    }
}
