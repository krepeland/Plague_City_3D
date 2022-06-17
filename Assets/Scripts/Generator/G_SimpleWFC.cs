using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction { 
    Up,
    Right,
    Down,
    Left
}

public class G_SimpleWFC : MonoBehaviour
{
    public List<WFC_Pattern> Patterns;

    public Dictionary<WFC_Pattern, Dictionary<Direction, HashSet<WFC_Pattern>>> PossibleConnections;

    public Dictionary<Vector2Int, WFC_Pattern> placedTiles;

    public Vector2Int MapSize = new Vector2Int(20, 20);
    private WFC_Pattern[,] spawnedTiles;

    private Queue<Vector2Int> recalcPossibleTilesQueue = new Queue<Vector2Int>();
    private List<WFC_Pattern>[,] possibleTiles;


    public void InitGenerator(string resourcePatternFolder)
    {
        Patterns = Resources.LoadAll<WFC_Pattern>(resourcePatternFolder).Where(x => x.IsActive).ToList();
        PossibleConnections = new Dictionary<WFC_Pattern, Dictionary<Direction, HashSet<WFC_Pattern>>>();
        spawnedTiles = new WFC_Pattern[MapSize.x, MapSize.y];

        foreach (var mainPattern in Patterns)
        {
            foreach (var direction in GetNeighbourDirections())
            {
                List<bool> side = new List<bool>();
                switch (direction) {
                    case Direction.Up:
                        side = mainPattern.GetXline(mainPattern.Pattern.Length - 1);
                        break;
                    case Direction.Right:
                        side = mainPattern.GetYline(mainPattern.Pattern.Length - 1);
                        break;
                    case Direction.Down:
                        side = mainPattern.GetXline(0);
                        break;
                    case Direction.Left:
                        side = mainPattern.GetYline(0);
                        break;
                }

                foreach (var otherPattern in Patterns)
                {
                    List<bool> otherSide = new List<bool>();
                    switch (direction)
                    {
                        case Direction.Down:
                            otherSide = otherPattern.GetXline(mainPattern.Pattern.Length - 1);
                            break;
                        case Direction.Left:
                            otherSide = otherPattern.GetYline(mainPattern.Pattern.Length - 1);
                            break;
                        case Direction.Up:
                            otherSide = otherPattern.GetXline(0);
                            break;
                        case Direction.Right:
                            otherSide = otherPattern.GetYline(0);
                            break;
                    }

                    var flag = false;
                    for (var i = 0; i < side.Count(); i++)
                    {
                        if (side[i] != otherSide[i]) {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag) {
                        if (!PossibleConnections.ContainsKey(mainPattern)) {
                            PossibleConnections[mainPattern] = new Dictionary<Direction, HashSet<WFC_Pattern>>();
                        }
                        if (!PossibleConnections[mainPattern].ContainsKey(direction))
                        {
                            PossibleConnections[mainPattern][direction] = new HashSet<WFC_Pattern>();
                        }

                        PossibleConnections[mainPattern][direction].Add(otherPattern);
                        Debug.Log("Direction " + direction + "; " + mainPattern.name + " - " + otherPattern.name);
                    }

                }
            }
        }
    }

    public WFC_Pattern[,] Generate()
    {
        possibleTiles = new List<WFC_Pattern>[MapSize.x, MapSize.y];

        int maxAttempts = 10;
        int attempts = 0;
        while (attempts++ < maxAttempts)
        {
            for (int x = 0; x < MapSize.x; x++)
                for (int y = 0; y < MapSize.y; y++)
                {
                    possibleTiles[x, y] = new List<WFC_Pattern>(Patterns);
                }


            recalcPossibleTilesQueue.Clear();

            if (placedTiles == null || placedTiles.Count == 0)
            {
                WFC_Pattern tileInCenter = GetRandomTile(Patterns);
                possibleTiles[MapSize.x / 2, MapSize.y / 2] = new List<WFC_Pattern> { tileInCenter };

                recalcPossibleTilesQueue.Clear();
                EnqueueNeighboursToRecalc(new Vector2Int(MapSize.x / 2, MapSize.y / 2));
            }
            else
            {
                foreach (var tile in placedTiles)
                {
                    if (possibleTiles[tile.Key.x, tile.Key.y].Count <= 1)
                        continue;
                    possibleTiles[tile.Key.x, tile.Key.y] = new List<WFC_Pattern> { tile.Value };
                    EnqueueNeighboursToRecalc(tile.Key);
                }
            }


            bool success = GenerateAllPossibleTiles();

            if (success) break;
        }

        PlaceAllTiles();
        return spawnedTiles;
    }

    private void PlaceTile(int x, int y)
    {
        if (possibleTiles[x, y].Count == 0) return;

        WFC_Pattern selectedTile = GetRandomTile(possibleTiles[x, y]);
        spawnedTiles[x, y] = selectedTile;
    }

    private void PlaceAllTiles()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                PlaceTile(x, y);
            }
    }


    private void EnqueueNeighboursToRecalc(Vector2Int position)
    {
        //var list = new List<Vector2Int>() {
        //    new Vector2Int(position.x + 1, position.y),
        //    new Vector2Int(position.x - 1, position.y),
        //    new Vector2Int(position.x, position.y + 1),
        //    new Vector2Int(position.x, position.y - 1)
        //};
        //
        //for (var i = 0; i < 4; i++) {
        //    var value = Random.Range(0, list.Count);
        //    recalcPossibleTilesQueue.Enqueue(list[value]);
        //    list.RemoveAt(value);
        //}

        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x + 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x - 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y + 1));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y - 1));
    }


    private bool GenerateAllPossibleTiles()
    {
        int maxIterations = MapSize.x * MapSize.y;
        int iterations = 0;
        int backtracks = 0;

        while (iterations++ < maxIterations)
        {
            int maxInnerIterations = 500;
            int innerIterations = 0;

            while (recalcPossibleTilesQueue.Count > 0 && innerIterations++ < maxInnerIterations)
            {
                Vector2Int position = recalcPossibleTilesQueue.Dequeue();
                if (position.x <= 0 || position.y <= 0 ||
                    position.x >= MapSize.x - 1 || position.y >= MapSize.y - 1)
                {
                    continue;
                }

                List<WFC_Pattern> possibleTilesHere = possibleTiles[position.x, position.y];

                int countRemoved = possibleTilesHere.RemoveAll(t => !IsTilePossible(t, position));

                if (countRemoved > 0) EnqueueNeighboursToRecalc(position);

                if (possibleTilesHere.Count == 0)
                {
                    // Зашли в тупик, в этих координатах невозможен ни один тайл. Попробуем ещё раз, разрешим все тайлы
                    // в этих и соседних координатах, и посмотрим устаканится ли всё
                    //if(!placedTiles.ContainsKey(position))
                        possibleTilesHere.AddRange(Patterns);

                    //if(!placedTiles.ContainsKey(new Vector2Int(position.x + 1, position.y)))
                        possibleTiles[position.x + 1, position.y] = new List<WFC_Pattern>(Patterns);

                    //if (!placedTiles.ContainsKey(new Vector2Int(position.x - 1, position.y)))
                        possibleTiles[position.x - 1, position.y] = new List<WFC_Pattern>(Patterns);

                    //if (!placedTiles.ContainsKey(new Vector2Int(position.x, position.y + 1)))
                        possibleTiles[position.x, position.y + 1] = new List<WFC_Pattern>(Patterns);

                    //if (!placedTiles.ContainsKey(new Vector2Int(position.x, position.y - 1)))
                        possibleTiles[position.x, position.y - 1] = new List<WFC_Pattern>(Patterns);

                    EnqueueNeighboursToRecalc(position);

                    backtracks++;
                }
            }
            if (innerIterations == maxInnerIterations) break;

            List<WFC_Pattern> maxCountTile = possibleTiles[1, 1];
            Vector2Int maxCountTilePosition = new Vector2Int(1, 1);

            for (int x = 1; x < MapSize.x - 1; x++)
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (possibleTiles[x, y].Count > maxCountTile.Count)
                    {
                        maxCountTile = possibleTiles[x, y];
                        maxCountTilePosition = new Vector2Int(x, y);
                    }
                }

            if (maxCountTile.Count == 1)
            {
                Debug.Log($"Generated for {iterations} iterations, with {backtracks} backtracks");
                return true;
            }

            WFC_Pattern tileToCollapse = GetRandomTile(maxCountTile);
            possibleTiles[maxCountTilePosition.x, maxCountTilePosition.y] = new List<WFC_Pattern> { tileToCollapse };
            EnqueueNeighboursToRecalc(maxCountTilePosition);
        }

        Debug.Log($"Failed, run out of iterations with {backtracks} backtracks");
        return false;
    }

    private bool IsTilePossible(WFC_Pattern tile, Vector2Int position)
    {
        bool isAllRightImpossible = possibleTiles[position.x - 1, position.y]
            .All(rightTile => !CanAppendTile(tile, rightTile, Direction.Left));
        if (isAllRightImpossible) return false;

        bool isAllLeftImpossible = possibleTiles[position.x + 1, position.y]
            .All(leftTile => !CanAppendTile(tile, leftTile, Direction.Right));
        if (isAllLeftImpossible) return false;

        bool isAllForwardImpossible = possibleTiles[position.x, position.y - 1]
            .All(fwdTile => !CanAppendTile(tile, fwdTile, Direction.Down));
        if (isAllForwardImpossible) return false;

        bool isAllBackImpossible = possibleTiles[position.x, position.y + 1]
            .All(backTile => !CanAppendTile(tile, backTile, Direction.Up));
        if (isAllBackImpossible) return false;

        return true;
    }

    private bool CanAppendTile(WFC_Pattern existingTile, WFC_Pattern tileToAppend, Direction direction)
    {
        if (existingTile == null) return true;

        return PossibleConnections[existingTile][direction].Contains(tileToAppend);
    }

    private WFC_Pattern GetRandomTile(List<WFC_Pattern> availableTiles)
    {
        List<float> chances = new List<float>();
        for (int i = 0; i < availableTiles.Count; i++)
        {
            chances.Add(availableTiles[i].Weight);
        }

        float value = Random.Range(0, chances.Sum());
        float sum = 0;

        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return availableTiles[i];
            }
        }

        return availableTiles[availableTiles.Count - 1];
    }

    public IEnumerable<Direction> GetNeighbourDirections()
    {
        yield return Direction.Up;
        yield return Direction.Right;
        yield return Direction.Down;
        yield return Direction.Left;
    }
}
