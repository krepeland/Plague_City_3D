using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour, ITickable
{
    public bool IsOpened;

    public Text BuildingName;
    public Image BuildingImage;

    public GameObject IsInfectedTextBlock;
    public Text IsInfectedText;

    public Transform BlocksContainer;

    public List<IBuildingInfoBlock> buildingInfoBlocks;

    public PlacedBuilding Building;

    public static BuildingInfo singleton;

    Dictionary<BuildingInfoBlockType, Transform> infoBlocksPrefabs;

    public PlacedBuilding OutlinedBuilding;

    public void Close() {
        IsOpened = false;
        gameObject.SetActive(false);
        Building = null;
        Selector.singleton.OutlineSelectable(OutlinedBuilding, null);
        OutlinedBuilding = null;

        SellCardBlock.singleton.gameObject.SetActive(true);
    }

    private void Awake()
    {
        singleton = this;
        buildingInfoBlocks = new List<IBuildingInfoBlock>();
    }

    private void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Main);

        infoBlocksPrefabs = new Dictionary<BuildingInfoBlockType, Transform>();

        infoBlocksPrefabs[BuildingInfoBlockType.Text] = PrefabManager.GetPrefab("BuildingInfoBlocks_TextBlock");
        infoBlocksPrefabs[BuildingInfoBlockType.HouseData] = PrefabManager.GetPrefab("BuildingInfoBlocks_HouseDataBlock");
        infoBlocksPrefabs[BuildingInfoBlockType.TimedAction] = PrefabManager.GetPrefab("BuildingInfoBlocks_TimedActionBlock");
        infoBlocksPrefabs[BuildingInfoBlockType.BuyCards] = PrefabManager.GetPrefab("BuildingInfoBlocks_BuyCards");

        gameObject.SetActive(false);
    }

    public void LoadBuilding(PlacedBuilding building)
    {
        if (Building == building || building == null)
        {
            return;
        }
        
        Selector.singleton.OutlineSelectable(OutlinedBuilding, building);
        OutlinedBuilding = building;

        Building = building;
        gameObject.SetActive(true);
        IsOpened = true;

        BuildingName.text = building.GetBuildingName();
        BuildingImage.sprite = building.GetBuildingImage();

        SetInfectedStatus(building.GetEInfectedBuildingStatus());

        for (var i = 0; i < buildingInfoBlocks.Count; i++) {
            Destroy(buildingInfoBlocks[i].gameObject);
        }
        buildingInfoBlocks = new List<IBuildingInfoBlock>();

        foreach (var infoBlock in building.GetBuildingInfo()) {
            if (infoBlock == null)
                continue;
            buildingInfoBlocks.Add(infoBlock);
            infoBlock.LoadBlockAfterCanvasUpdate();
        }
        Canvas.ForceUpdateCanvases();

        var posNow = -35;
        if (!IsInfectedTextBlock.gameObject.activeSelf)
        {
            posNow += 15;
        }

        foreach (var infoBlock in buildingInfoBlocks)
        {
            infoBlock.LoadBlockAfterCanvasUpdate();
            infoBlock.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posNow);
            posNow -= infoBlock.GetHeightSize();
        }
        UpdateSize();

        SellCardBlock.singleton.gameObject.SetActive(false);
    }

    public void UpdateSize() {
        var height = 350f;
        if (!IsInfectedTextBlock.gameObject.activeSelf)
        {
            height -= 35;
        }

        foreach (var e in buildingInfoBlocks) {
            height += e.GetHeightSize();
        }
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public IBuildingInfoBlock SpawnBuildingInfoBlock(BuildingInfoBlockType buildingInfoBlockType) {
        if (infoBlocksPrefabs.ContainsKey(buildingInfoBlockType)) {
            var block = Instantiate(infoBlocksPrefabs[buildingInfoBlockType], BlocksContainer);
            return block.GetComponent<IBuildingInfoBlock>();
        }
        
        //switch (buildingInfoBlockType) {
        //    case BuildingInfoBlockType.Text:
        //        var block = Instantiate(infoBlocksPrefabs[buildingInfoBlockType], BlocksContainer);
        //        return block.GetComponent<IBuildingInfoBlock>();
        //}

        Debug.LogError($"No building info block {buildingInfoBlockType}");
        return null;
    }

    public void SetInfectedStatus(EInfectedBuildingStatus status)
    {
        IsInfectedTextBlock.SetActive(status != EInfectedBuildingStatus.None);
        switch (status) {
            case EInfectedBuildingStatus.None:

                return;
            case EInfectedBuildingStatus.Infected:
                IsInfectedText.text = "INFECTED";
                return;
            case EInfectedBuildingStatus.NotInfected:
                IsInfectedText.text = "NOT INFECTED";
                return;
            case EInfectedBuildingStatus.Empty:
                IsInfectedText.text = "EMPTY";
                return;
        }
    }

    public void TryUpdateBuilding(PlacedBuilding building) {
        if (Building == building && Building != null)
        {
            Building.UpdateSelectedBuildingInfo();
            CheckAllIsUnlocked();
        }
    }

    public void Tick(int luck)
    {
        if (Building != null)
        {
            Building.UpdateSelectedBuildingInfo();
            CheckAllIsUnlocked();
        }
        else
        {
            if (IsOpened)
            {
                gameObject.SetActive(false);
                IsOpened = false;
            }
        }
    }

    public void CheckAllIsUnlocked()
    {
        foreach (var e in buildingInfoBlocks)
        {
            if (e.GetBuildingInfoBlockType() == BuildingInfoBlockType.BuyCards) {
                ((BI_BuyCards)e).UpdateIsLocked();
            }
        }
    }
}
