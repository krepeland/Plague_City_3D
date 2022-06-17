using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Buildings
{
    public G_Buildings() { 
    
    }

    public HashSet<Vector2Int> GenerateBuildingsCoords(Dictionary<Vector2Int, RoadType> roads, bool[,] form, HashSet<Vector2Int> rivers, int[,] heights) {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();
        var size = new Vector2Int(form.GetLength(0), form.GetLength(1));

        foreach (var road in roads)
        {
            var heightNow = heights[road.Key.x, road.Key.y];
            if (heightNow <= 0)
                continue;
            if (road.Value != RoadType.Main)
                continue;
            foreach (var offset in GetOffsets()) {
                var pos = road.Key + offset;
                if (pos.x == 0 || pos.x == size.x - 1 || pos.y == 0 || pos.y == size.y - 1)
                    continue;
                if (roads.ContainsKey(pos) || rivers.Contains(pos) || pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y || !form[pos.x, pos.y])
                    continue;
                if (heightNow != heights[pos.x, pos.y])
                    continue;
                result.Add(pos);
            }
        }

        return result;
    }

    public Dictionary<Vector2Int, int> ExpandBuildingsCoords(HashSet<Vector2Int> oldBuildings, Dictionary<Vector2Int, RoadType> roads, bool[,] form, HashSet<Vector2Int> rivers, int[,] heights) {
        Dictionary<Vector2Int, int> result = new Dictionary<Vector2Int, int>();
        var size = new Vector2Int(form.GetLength(0), form.GetLength(1));

        foreach (var b in oldBuildings) {
            result[b] = 0;
            var heightNow = heights[b.x, b.y];
            foreach (var offset in GetOffsets())
            {
                var pos = b + offset;
                if (result.ContainsKey(pos))
                    continue;

                if (roads.ContainsKey(pos) || rivers.Contains(pos) || pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y || !form[pos.x, pos.y])
                    continue;

                if (heightNow != heights[pos.x, pos.y])
                    continue;

                if (pos.x == 0 || pos.x == size.x - 1 || pos.y == 0 || pos.y == size.y - 1)
                    continue;

                result[pos] = 1;
            }
        }

        return result;
    }

    public List<BuildingData> GenerateBuildings(Dictionary<Vector2Int, int> buildingsCoords, int[,] heights) {
        var result = new List<BuildingData>();

        HashSet<Vector2Int> occupiedCoords = new HashSet<Vector2Int>();
        foreach (var coord in buildingsCoords) {
            if (coord.Value != 0)
                continue;
            if (occupiedCoords.Contains(coord.Key))
                continue;

            var building = GenerateBuildingAt(coord.Key, buildingsCoords, heights, occupiedCoords);
            result.Add(building);
        }

        return result;
    }

    public BuildingData GenerateBuildingAt(Vector2Int pos, Dictionary<Vector2Int, int> buildingsCoords, int[,] heights, HashSet<Vector2Int> occupiedCoords) {
        var poses = new List<Vector2Int>() { pos };
        occupiedCoords.Add(pos);

        var buildSize = Random.Range(1, 6);
        if (buildSize > 1)
        {
            var possibleMainPoses = new List<Vector2Int>();
            var possibleOtherPoses = new List<Vector2Int>();

            var heightNow = heights[pos.x, pos.y];

            foreach (var offset in GetOffsets()) {
                var posToCheck = pos + offset;
                if (!buildingsCoords.ContainsKey(posToCheck) || heights[posToCheck.x, posToCheck.y] != heightNow || occupiedCoords.Contains(posToCheck))
                    continue;

                if (buildingsCoords[posToCheck] == 0)
                    possibleMainPoses.Add(posToCheck);
                else
                    possibleOtherPoses.Add(posToCheck);
            }

            if (possibleMainPoses.Count != 0)
            {
                var newPos = possibleMainPoses[Random.Range(0, possibleMainPoses.Count)];
                poses.Add(newPos);
                occupiedCoords.Add(newPos);

                if (buildSize > 3)
                {
                    foreach (var offset in GetOffsets())
                    {
                        var posToCheck = newPos + offset;
                        if (!buildingsCoords.ContainsKey(posToCheck) || heights[posToCheck.x, posToCheck.y] != heightNow || occupiedCoords.Contains(posToCheck))
                            continue;

                        possibleOtherPoses.Add(posToCheck);
                    }

                    if (possibleOtherPoses.Count != 0)
                    {
                        newPos = possibleOtherPoses[Random.Range(0, possibleOtherPoses.Count)];
                        poses.Add(newPos);
                        occupiedCoords.Add(newPos);
                    }
                }
            }
        }

        return new BuildingData(poses.ToArray(), Random.Range(1, 3));
    }


    public IEnumerable<Vector2Int> GetOffsets()
    {
        yield return new Vector2Int(0, 1);
        yield return new Vector2Int(1, 0);
        yield return new Vector2Int(0, -1);
        yield return new Vector2Int(-1, 0);
    }
}
