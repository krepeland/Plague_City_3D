using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BI_HouseData : IBuildingInfoBlock
{
    public RectTransform background;
    public RectTransform Container;

    public static GameObject HouseDataBlockPart;

    List<Image> images;

    public static Color HealthyColor = new Color(0.41f, 0.84f, 0.37f);
    public static Color InfectedColor = new Color(0.75f, 0.25f, 0.25f);
    public static Color DeadColor = new Color(0.13f, 0.13f, 0.13f);

    public void Init(int healthy, int infected, int dead)
    {
        var peopleCount = healthy + infected + dead;
        if (HouseDataBlockPart == null)
            HouseDataBlockPart = PrefabManager.GetPrefab("BuildingInfoBlocks_HouseDataBlockPart").gameObject;

        if (images != null) {
            for (var i = 0; i < images.Count; i++) {
                Destroy(images[i]);
            }
        }

        images = new List<Image>();
        var half = peopleCount / 2f;
        for (var i = 0; i < peopleCount; i++) {
            var part = Instantiate(HouseDataBlockPart, Container).GetComponent<RectTransform>();
            part.anchoredPosition = new Vector2(15f * (i - half + 0.5f), 0);
            images.Add(part.GetComponent<Image>());
        }
        if (peopleCount == 0)
            peopleCount = 1;
        Container.transform.localScale = new Vector3(24f / (peopleCount), 1, 1);

        UpdateData(healthy, infected, dead);
    }

    public void UpdateData(int healthy, int infected, int dead) {
        var e = 0;
        for (var i = 0; i < dead; i++) {
            images[e].color = DeadColor;
            e++;
        }
        for (var i = 0; i < infected; i++)
        {
            images[e].color = InfectedColor;
            e++;
        }
        for (var i = 0; i < healthy; i++)
        {
            images[e].color = HealthyColor;
            e++;
        }
    }

    public override int GetHeightSize()
    {
        return 55;
    }

    public override void LoadBlockBeforeCanvasUpdate()
    {

    }

    public override void LoadBlockAfterCanvasUpdate()
    {

    }

    public override BuildingInfoBlockType GetBuildingInfoBlockType()
    {
        return BuildingInfoBlockType.HouseData;
    }
}
