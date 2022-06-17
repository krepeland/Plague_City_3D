using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public G_RiverSettings Settings_River;
    public G_RiverSettings Settings_SecondRiver;

    private int subdivides = 2;

    public ERiverType RiverType;

    private G_Form Generator_Form;
    private G_River Generator_River;
    private G_CityRegions Generator_CityRegions;
    private G_Roads Generator_Roads;
    private G_Buildings Generator_Buildings;
    private G_Height Generator_Heights;
    public GeneratorSpawner GeneratorSpawner;

    public bool DebugDraw;
    public SpriteRenderer DebugRenderer;

    public int seed = -1;
    public float Scale = 0.1f;

    public int height = 8;

    public int CityCellsMin = 1000;
    public int CityCellsMax = 1200;

    public void InitGenerators(int cityCellsMin, int cityCellsMax, ERiverType riverType) {
        Generator_Form = new G_Form();
        Generator_River = new G_River();
        Generator_CityRegions = new G_CityRegions();
        Generator_Roads = new G_Roads();
        Generator_Buildings = new G_Buildings();
        Generator_Heights = new G_Height();

        RiverType = riverType;
        CityCellsMin = cityCellsMin;
        CityCellsMax = cityCellsMax;
    }

    public void Generate() {
        if(seed != -1)
            Random.InitState(seed);

        #region form
        var form = Generator_Form.Generate(Random.Range(0, 100000), Random.Range(2, 5));

        for (var i = 0; i < subdivides; i++)
            form = Generator_Form.SubdivideBlocks(form);

        var size = (int)(5 * Mathf.Pow(4, subdivides));

        //DebugDrawMap(form, 1);
        #endregion


        #region river
        var rivers = new List<List<Vector2Int>>();
        var riversHashset = new HashSet<Vector2Int>();
        if (RiverType != ERiverType.NoRiver)
        {
            Generator_River.SetSettings(Settings_River);
            rivers.Add(Generator_River.GenerateRiver(form, new Vector2Int(Random.Range(0, 40), form.GetLength(1) - 1), null));

            if (RiverType == ERiverType.DoubledRiver)
            {
                rivers.Add(GetSecondRiver(form, rivers[0]));
            }
            riversHashset = Generator_River.GetRiversHashset(rivers, true);

            //DebugDrawAddPath(rivers[0], size, Color.cyan, true);
            //if (DoubleRiver)
            //    DebugDrawAddPath(rivers[1], size, Color.cyan, true);
        }
        #endregion


        #region CityRegions
        var cityCells = Generator_CityRegions.GenerateCityCells(Random.Range(CityCellsMin, CityCellsMax), form);
        CameraController.singleton.ForceSetNewPosition(new Vector3(cityCells.Item2.x - 5, 3, cityCells.Item2.y + 5));
        var citySubRegions = Generator_CityRegions.GenerateCitySubRegion(cityCells.Item1, size);
        var cityRegionsBlanks = Generator_CityRegions.GenerateCityRegionsCoordinates(citySubRegions);
        var cityRegions = Generator_CityRegions.CompileCityRegions(cityRegionsBlanks);

        //DebugDrawAddRegions(cityRegions, size, Color.white, new Color(0.1f, 0.1f, 0.1f, 0f));
        #endregion


        #region Roads
        var roads = Generator_Roads.GenerateRoads(cityRegions, form, rivers);
        //DebugDrawAddRoads(roads, size, Color.black, Color.gray);
        #endregion

        #region Heights
        var heights = Generator_Heights.GenerateHeight(size, height, new Vector2Int(Random.Range(-10000, 10000), Random.Range(-10000, 10000)), Scale);
        Generator_Heights.ClearHeightForm(size, heights, form);
        //Generator_Heights.ClearHeightRivers(size, heights, rivers);
        Generator_Heights.ClearHeightRiversToZero(size, heights, riversHashset);
        var bridgesHeights = Generator_Heights.ClearRoadsHeights(size, heights, form, riversHashset, roads);
        //DebugAddDrawHeight(heights, size, new Color(1, 1, 1, 0.15f));
        #endregion


        #region Buildings
        var buildingsCoordinates = Generator_Buildings.GenerateBuildingsCoords(roads, form, riversHashset, heights);
        var expandedBuildingsCoordinates = Generator_Buildings.ExpandBuildingsCoords(buildingsCoordinates, roads, form, riversHashset, heights);
        var buildings = Generator_Buildings.GenerateBuildings(expandedBuildingsCoordinates, heights);
        //DebugDrawAddBuildings(buildings, size, Color.gray);
        #endregion

        GeneratorSpawner.InitGenerator(
            form, 
            rivers, 
            riversHashset, 
            roads, 
            heights, 
            buildings, 
            size, 
            bridgesHeights, 
            cityRegions);
        GeneratorSpawner.Generate();

        DebugRenderer.gameObject.SetActive(DebugDraw);
        if (DebugDraw)
        {
            DebugDrawMap(form, 1);
            if (RiverType != ERiverType.NoRiver)
                DebugDrawAddPath(rivers[0], size, Color.cyan, true);
            if (RiverType == ERiverType.DoubledRiver)
                DebugDrawAddPath(rivers[1], size, Color.cyan, true);
            //DebugDrawAddRegions(cityRegions, size, Color.white, new Color(0.1f, 0.1f, 0.1f, 0f));
            DebugDrawAddRoads(roads, size, Color.black, Color.gray);
            //DebugAddDrawHeight(heights, size, new Color(1, 1, 1, 0.15f));
            DebugDrawAddBuildings(buildings, size, Color.gray);
        }
    }

    List<Vector2Int> GetSecondRiver(bool[,] map, List<Vector2Int> river) {
        var result = new List<Vector2Int>();

        var size = new Vector2Int(map.GetLength(0), map.GetLength(1));
        var riveredMap = new bool[size.x, size.y];

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                riveredMap[x, y] = map[x, y];
            }
        }
        foreach (var e in river) {
            riveredMap[e.x, e.y] = false;
        }

        var separator = new RegionSeparator();
        separator.InitSeparator(riveredMap);

        var regions = separator.GetRegions();

        var riverStartPoint = river[(int)(river.Count * Settings_SecondRiver.RiverStart)];
        foreach (var region in regions) {
            if (region.Item1.Contains(riverStartPoint + new Vector2Int(-2, -2)))
            {
                Generator_River.SetSettings(Settings_SecondRiver);
                result = Generator_River.GenerateRiver(riveredMap, riverStartPoint + new Vector2Int(0, -1), river);
                //DebugDrawAddRegion(region.Item1, 80, Color.red);
                break;
            }
        }

        return result;
    }

    void DebugDrawMap(bool[,] array, int scaleFactor = 1)
    {
        var resultImage = new Texture2D(array.GetLength(0) * scaleFactor, array.GetLength(1) * scaleFactor);
        for (var y = array.GetLength(1) * scaleFactor - 1; y >= 0; y--)
        {
            for (var x = 0; x < array.GetLength(1) * scaleFactor; x++)
            {
                resultImage.SetPixel(x, y, (array[x / scaleFactor, y / scaleFactor] ? Color.green : Color.cyan));
            }
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage, 
            new Rect(Vector2.zero, new Vector2(array.GetLength(0) * scaleFactor, array.GetLength(1) * scaleFactor)), 
            Vector2.zero);
    }

    void DebugAddDrawHeight(int[,] heights, int size, Color color)
    {
        var resultImage = DebugRenderer.sprite.texture;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var newColor = new Color(color.r, color.g, color.b, color.a * heights[x, y]);
                resultImage.SetPixel(x, y, resultImage.GetPixel(x, y) + (newColor * newColor.a));
            }
        }

        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }


    void DebugDrawAddPath(List<Vector2Int> path, int size, Color color, bool scale) {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in path) {
            if (scale)
            {
                for (var dx = -1; dx < 1; dx++)
                {
                    for (var dy = -1; dy < 1; dy++)
                    {
                        var nX = e.x + dx;
                        var nY = e.y + dy;
                        resultImage.SetPixel(nX, nY, color);
                    }
                }
            }
            else {
                resultImage.SetPixel(e.x, e.y, color);
            }
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddRoads(Dictionary<Vector2Int, RoadType> roads, int size, Color colorMain, Color colorSide)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in roads)
        {
            Color color;
            if (e.Value == RoadType.Main)
                color = colorMain;
            else
                color = colorSide;
            resultImage.SetPixel(e.Key.x, e.Key.y, color);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }
    
    void DebugDrawAddBuildings(List<BuildingData> buildings, int size, Color color)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var building in buildings)
        {
            var newColor = new Color(color.r + Random.Range(-0.3f, 0.3f), color.g + Random.Range(-0.3f, 0.3f), color.b + Random.Range(-0.3f, 0.3f), 1);
            foreach(var e in building.poses)
                resultImage.SetPixel(e.x, e.y, newColor);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddBuildingsCoords(Dictionary<Vector2Int, int> buildingsCoords, int size, Color colorMain, Color colorSide)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in buildingsCoords)
        {
            Color color;
            if (e.Value == 0)
                color = colorMain;
            else
                color = colorSide;
            resultImage.SetPixel(e.Key.x, e.Key.y, color);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddRegion(HashSet<Vector2Int> region, int size, Color color)
    {
        //Debug.Log("region " + region.Count + " started");
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in region)
        {
            resultImage.SetPixel(e.x, e.y, color);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddHashset(HashSet<Vector2Int> hashset, int size, Color color)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in hashset)
        {
            resultImage.SetPixel(e.x, e.y, color);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddSubRegions(HashSet<Vector2Int> regionsCoords, int size, Color color1, Color color2)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach (var e in regionsCoords)
        {
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    resultImage.SetPixel(e.x + x, e.y + y, (e.x + e.y) % 2 == 0 ? color1 : color2);
                }
            }
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawAddRegions(List<CityRegion> regions, int size, Color color1, Color deltaColor)
    {
        var resultImage = DebugRenderer.sprite.texture;
        foreach(var region in regions)
        {
            foreach (var subRegion in region.subRegions)
            {
                foreach (var e in subRegion.Value.points)
                {
                    for (var x = 0; x < 5; x++)
                    {
                        for (var y = 0; y < 5; y++)
                        {
                            var c = (e.x + e.y) % 2 == 0 ? color1 : color1;
                            if (region.Depth >= 2)
                            {
                                c -= (deltaColor * region.Depth);
                            }
                            resultImage.SetPixel(e.x + x, e.y + y, c);
                        }
                    }
                }
                resultImage.SetPixel(subRegion.Value.pivotPos.x, subRegion.Value.pivotPos.y, Color.red);
            }
            resultImage.SetPixel(region.pivotPos.x, region.pivotPos.y, Color.yellow);
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size, size)),
            Vector2.zero);
    }

    void DebugDrawWFC(WFC_Pattern[,] array)
    {
        var resultImage = new Texture2D(array.GetLength(0) * 3, array.GetLength(1) * 3);
        for (var y = 0; y < array.GetLength(1); y++)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            {
                if (array[x, y] == null)
                    continue;
                for (var dy = 0; dy < 3; dy++)
                {
                    for (var dx = 0; dx < 3; dx++)
                    {
                        resultImage.SetPixel(x * 3 + dx, y * 3 + dy, array[x, y].GetPoint(dx, dy) ? Color.green : Color.cyan);
                    }
                }

            }
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(array.GetLength(0) * 3, array.GetLength(1) * 3)),
            Vector2.zero);
    }

    void DebugDrawWFC(Dictionary<Vector2Int, WFC_Pattern> dict, int size, Color color)
    {
        var oldImage = DebugRenderer.sprite.texture;
        var resultImage = new Texture2D(size * 3, size * 3);

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                for (var dy = 0; dy < 3; dy++)
                {
                    for (var dx = 0; dx < 3; dx++)
                    {
                        resultImage.SetPixel(x * 3 + dx, y * 3 + dy, oldImage.GetPixel(x, y));
                    }
                }

            }
        }

        foreach (var e in dict) {
            var pos = new Vector2Int(e.Key.x, e.Key.y);

            for (var dy = 0; dy < 3; dy++)
            {
                for (var dx = 0; dx < 3; dx++)
                {
                    if(dict[pos].GetPoint(dx, dy))
                        resultImage.SetPixel(pos.x * 3 + dx, pos.y * 3 + dy, Color.yellow);
                }
            }
        }

        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(size * 3, size * 3)),
            Vector2.zero);
    }

    void DebugDrawAddWFC(WFC_Pattern[,] array)
    {
        var resultImage = DebugRenderer.sprite.texture;
        for (var y = 0; y < array.GetLength(1); y++)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            {
                if (array[x, y] == null)
                    continue;

                if (array[x, y].name == "WFC_Pattern")
                    continue;

                for (var dy = 0; dy < 10; dy++)
                {
                    for (var dx = 0; dx < 10; dx++)
                    {
                        if (x + dx > 80 || y + dy > 80) continue;
                        resultImage.SetPixel(x * 10 + dx, y * 10 + dy, Color.cyan);
                    }
                }

            }
        }
        resultImage.Apply();
        resultImage.filterMode = FilterMode.Point;

        DebugRenderer.sprite = Sprite.Create(
            resultImage,
            new Rect(Vector2.zero, new Vector2(80, 80)),
            Vector2.zero);
    }

}
