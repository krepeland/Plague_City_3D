using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSeparator
{
    public bool[,] Map;
    public Vector2Int Size;
    public void InitSeparator(bool[,] map) {
        Map = map;
        Size = new Vector2Int(map.GetLength(0), map.GetLength(1));
    }

    public (HashSet<Vector2Int>, int) GetRegionAt(Vector2Int pos, int stopValue = int.MaxValue)
    {
        return GetRegionAt(pos, new HashSet<Vector2Int>(), stopValue);
    }

    public List<(HashSet<Vector2Int>, int)> GetRegions() {
        var result = new List<(HashSet<Vector2Int>, int)>();
        var checkedBlocks = new HashSet<Vector2Int>();
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = Size.y - 1; y >= 0; y--)
            {
                var newPoint = new Vector2Int(x, y);
                if (checkedBlocks.Contains(newPoint) || !Map[newPoint.x, newPoint.y])
                    continue;
                result.Add(GetRegionAt(newPoint, checkedBlocks));
            }
        }

        return result;
    }

    (HashSet<Vector2Int>, int) GetRegionAt(Vector2Int startPos, HashSet<Vector2Int> checkedBlocks, int stopValue = 10000) {
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);
        var result = new HashSet<Vector2Int>();

        while (queue.Count > 0) {
            var value = queue.Dequeue();
            result.Add(value);
            if (checkedBlocks.Contains(value))
                continue;
            checkedBlocks.Add(value);
            var x = value.x;
            var y = value.y;

            foreach (var offset in GetNeighboursOffsets()) {
                var newX = x + offset.x;
                var newY = y + offset.y;
                if(!IsInMap(newX, newY) || !Map[newX, newY])
                {
                    checkedBlocks.Add(new Vector2Int(newX, newY));
                    continue;
                }
                var newPos = new Vector2Int(newX, newY);
                queue.Enqueue(newPos);

            }

            if (result.Count >= stopValue)
            {
                return (result, result.Count);
            }
        }

        return (result, result.Count);
    }

    public HashSet<Vector2Int> GetOutterBorderLine((HashSet<Vector2Int>, int) region, Vector2Int Size)
    {
        var result = new HashSet<Vector2Int>();

        foreach (var point in region.Item1)
        {
            foreach (var offset in GetNeighboursOffsets())
            {
                var newPos = point + offset;
                if (region.Item1.Contains(newPos))
                    continue;
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= Size.x || newPos.y >= Size.y)
                    continue;
                result.Add(newPos);
            }
        }

        return result;
    }

    public HashSet<Vector2Int> GetOutterBorderLine(HashSet<Vector2Int> region, Vector2Int Size) {
        var result = new HashSet<Vector2Int>();
        foreach (var point in region) {
            foreach (var offset in GetNeighboursOffsets()) {
                var newPos = point + offset;
                if (region.Contains(newPos))
                    continue;
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= Size.x || newPos.y >= Size.y)
                    continue;
                result.Add(newPos);
            }
        }

        return result;
    }

    bool IsInMap(Vector2Int pos)
    {
        return IsInMap(pos.x, pos.y);
    }

    bool IsInMap(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Size.x && y < Size.y;
    }

    IEnumerable<Vector2Int> GetNeighboursOffsets()
    {
        yield return Vector2Int.down;
        yield return Vector2Int.right;
        yield return Vector2Int.left;
        yield return Vector2Int.up;
    }
}
