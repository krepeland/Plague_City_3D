using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class G_Form
{
    static int size = 5;
    static int partSize = 2;
    public List<G_Form_Patten> Pattens = new List<G_Form_Patten>();

    public List<G_Form_SubdividePatten> SubdividedPattens = new List<G_Form_SubdividePatten>();

    /// <summary>
    /// int - rotation count
    /// List<List<Vector2Int>> - list of cells
    /// List<Vector2Int> - cells
    /// </summary>
    public Dictionary<int, List<List<Vector2Int>>>[] RotatedPattensFilled = new Dictionary<int, List<List<Vector2Int>>>[4];
    public Dictionary<int, List<List<Vector2Int>>>[] RotatedPattensEmpty = new Dictionary<int, List<List<Vector2Int>>>[4];

    public G_Form() {
        Pattens = Resources.LoadAll<G_Form_Patten>("GeneratorPatterns/G_Form").ToList();
        SubdividedPattens = Resources.LoadAll<G_Form_SubdividePatten>("GeneratorPatterns/G_FormSubdivide").ToList();


        for (var i = 0; i < 4; i++) {
            RotatedPattensFilled[i] = new Dictionary<int, List<List<Vector2Int>>>();
            RotatedPattensEmpty[i] = new Dictionary<int, List<List<Vector2Int>>>();
        }

        foreach (var pattern in SubdividedPattens) {
            if (!pattern.IsActive)
                continue;
            for (var i = 0; i < 4; i++) {
                //Debug.Log("Rotation " + i);
                var cells = new List<Vector2Int>();
                for (var y = 0; y < pattern.Pattern.Length; y++) {
                    for (var x = 0; x < pattern.Pattern[y].row.Length; x++) {
                        if (pattern.Pattern[y].row[x])
                        {
                            cells.Add(RotateVector(new Vector2Int(x, y), i));
                            //Debug.Log(RotateVector(new Vector2Int(x, y), i));
                        }
                    }
                }

                if (pattern.IsFilled)
                {
                    if (!RotatedPattensFilled[i].ContainsKey(pattern.NeighbourCells))
                        RotatedPattensFilled[i][pattern.NeighbourCells] = new List<List<Vector2Int>>();
                    RotatedPattensFilled[i][pattern.NeighbourCells].Add(cells);
                }
                else
                {
                    if (!RotatedPattensEmpty[i].ContainsKey(pattern.NeighbourCells))
                        RotatedPattensEmpty[i][pattern.NeighbourCells] = new List<List<Vector2Int>>();
                    RotatedPattensEmpty[i][pattern.NeighbourCells].Add(cells);
                }
            }
        }
    }

    public bool[,] Generate(int seed, int count)
    {
        //Debug.Log("___USED PATTERN___");
        var result = new bool[size, size];
        //for (var y = 0; y < 5; y++)
        //    for (var x = 0; x < 5; x++)
        //        result[x, y] = true;

            var newPatterns = new List<G_Form_Patten>();
        foreach(var e in Pattens) {
            newPatterns.Add(e);
        }

        if (count > Pattens.Count) count = Pattens.Count;

        for (var i = 0; i < count; i++) {
            var nextPatternId = Random.Range(0, newPatterns.Count);
            var nextPattern = newPatterns[nextPatternId];
            //Debug.Log(nextPattern.name);
            newPatterns.RemoveAt(nextPatternId);
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    if(nextPattern.Pattern[y].row[x])
                        result[x, y] = true;
                }
            }
        }
        //Debug.Log("__________________");
        return result;
    }

    public bool[,] SubdivideBlocks(bool[,] map)
    {
        var baseWidth = map.GetLength(0);
        var baseHeight = map.GetLength(1);

        var result = new bool[baseWidth * 2 * partSize, baseHeight * 2 * partSize];

        List<(Vector2Int, int)> CheckOffsets = new List<(Vector2Int, int)>() {
            (new Vector2Int(-1, 0), 1),
            (new Vector2Int(-1, 1), 2),
            (new Vector2Int(0, 1), 4),
        };

        

        for (var x = 0; x < baseWidth; x++)
        {
            for (var y = 0; y < baseHeight; y++)
            {

                var posX = x * 2 * partSize;
                var posY = y * 2 * partSize;
                for (var dx = 0; dx < 2; dx++)
                {
                    for (var dy = 0; dy < 2; dy++)
                    {
                        var CheckList = new List<(Vector2Int, int)>() {
                            (new Vector2Int(-1, 0), 1),
                            (new Vector2Int(-1, -1), 2),
                            (new Vector2Int(0, -1), 4),
                        };
                        switch (dx)
                        {
                            case 0:
                                switch (dy)
                                {
                                    case 0:
                                        CheckList = new List<(Vector2Int, int)>() {
                                            (new Vector2Int(-1, 0), 1),
                                            (new Vector2Int(-1, -1), 2),
                                            (new Vector2Int(0, -1), 4),
                                        };
                                        break;
                                    case 1:
                                        CheckList = new List<(Vector2Int, int)>() {
                                            (new Vector2Int(-1, 0), 4),
                                            (new Vector2Int(-1, 1), 2),
                                            (new Vector2Int(0, 1), 1),
                                        };
                                        break;
                                }
                                break;
                            case 1:
                                switch (dy)
                                {
                                    case 0:
                                        CheckList = new List<(Vector2Int, int)>() {
                                            (new Vector2Int(0, -1), 1),
                                            (new Vector2Int(1, -1), 2),
                                            (new Vector2Int(1, 0), 4),
                                        };
                                        break;
                                    case 1:
                                        CheckList = new List<(Vector2Int, int)>() {
                                            (new Vector2Int(0, 1), 4),
                                            (new Vector2Int(1, 1), 2),
                                            (new Vector2Int(1, 0), 1),
                                        };
                                        break;
                                }
                                break;
                        }

                        var neighbourValue = 0;
                        foreach (var offset in CheckList) {
                            if (IsEarthAt(x + offset.Item1.x, y + offset.Item1.y, map)) {
                                neighbourValue += offset.Item2;
                            }
                        }

                        //Debug.Log(x + "; " + y + " dxdy = " + dx + "; " + dy + " nValue = " + neighbourValue);
                        var rotationId = 0;
                        switch (dx)
                        {
                            case 0:
                                switch (dy)
                                {
                                    case 0:
                                        rotationId = 0;
                                        break;
                                    case 1:
                                        rotationId = 1;
                                        break;
                                }
                                break;
                            case 1:
                                switch (dy)
                                {
                                    case 0:
                                        rotationId = 3;
                                        break;
                                    case 1:
                                        rotationId = 2;
                                        break;
                                }
                                break;
                        }
                        if (IsEarthAt(x, y, map))
                        {
                            if (RotatedPattensFilled[rotationId].ContainsKey(neighbourValue))
                            {
                                var cellsToAddList = RotatedPattensFilled[rotationId][neighbourValue]
                                    [Random.Range(0, RotatedPattensFilled[rotationId][neighbourValue].Count)];
                                foreach (var cell in cellsToAddList)
                                {
                                    result[
                                        x * 2 * partSize + cell.x + dx * partSize,
                                        y * 2 * partSize + cell.y + dy * partSize] = true;
                                }
                            }
                        }
                        else
                        {
                            if (RotatedPattensEmpty[rotationId].ContainsKey(neighbourValue))
                            {
                                var cellsToAddList = RotatedPattensEmpty[rotationId][neighbourValue]
                                    [Random.Range(0, RotatedPattensEmpty[rotationId][neighbourValue].Count)];
                                foreach (var cell in cellsToAddList)
                                {
                                    result[
                                        x * 2 * partSize + cell.x + dx * partSize,
                                        y * 2 * partSize + cell.y + dy * partSize] = true;
                                }
                            }
                        }

                        //Debug.Log(x + "; " + y + " - dxdy: " + dx + "; " + dy + " - " + "Rotation: " + rotationId);
                        //foreach (var e in CheckList)
                        //{
                        //    Debug.Log(x + "; " + y + " - dxdy: " + dx + "; " + dy + " - " + "value: " + e);
                        //}
                    }
                }
            }
        }

        return result;
    }

    bool IsEarthAt(int x, int y, bool[,] map) {
        return x >= 0 && y < map.GetLength(0) && x < map.GetLength(1) && y >= 0 && map[x, y];
        //return (x > 0 && y > 0 && y < map.GetLength(1) && x < map.GetLength(0) && map[x, y]);
    }

    public Vector2Int RotateVector(Vector2Int vector, int rotations)
    {
        switch (rotations)
        {
            case 1:
                return new Vector2Int(vector.y, partSize - 1 - vector.x);
            case 2:
                return new Vector2Int(partSize - 1 - vector.x, partSize - 1 - vector.y);
            case 3:
                return new Vector2Int(partSize - 1 - vector.y, vector.x);
        }
        return vector;
    }
}
