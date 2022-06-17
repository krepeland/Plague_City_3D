using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsShop : MonoBehaviour
{
    public RectTransform Mask;
    public RectTransform Content;
    public RectTransform MaskBorder;

    public RectTransform BuildingShopTarget;
    public RectTransform BuildingShopTarget2;
    public RectTransform BuildingShopTargetOuter;
    public RectTransform BuildingShopTargetOuter2;

    public bool IsOpened;
    public float OpenValue;

    public float OpenSpeed = 2;

    private Transform BuildingShopBlockPrefab;
    private List<BuildingsShopBlock> buildingsShopBlocks;

    public static BuildingsShop singleton;

    private void Awake()
    {
        singleton = this;
    }

    public void Init()
    {
        BuildingShopBlockPrefab = PrefabManager.GetPrefab("Cards_BuildingShopBlock");
        SpawnBuyCards();
        BuildingShopTarget2.transform.position = BuildingShopTargetOuter2.transform.position;
    }

    void SpawnBuyCards() {
        buildingsShopBlocks = new List<BuildingsShopBlock>();
        var allBuildingsCard = CardsManager.singleton.GetAllBuyCardsOfType(CardClassType.Building);
        allBuildingsCard.Sort((x, y) => (x.priority.CompareTo(y.priority)));

        var pos = 70;
        foreach (var e in allBuildingsCard)
        {
            if (!e.IsShowInShop)
                continue;
            var buyBlock = Instantiate(BuildingShopBlockPrefab, Content);
            buildingsShopBlocks.Add(buyBlock.GetComponent<BuildingsShopBlock>());
            buyBlock.GetComponent<BuildingsShopBlock>().SetBuildingShopData(e);
            var rect = buyBlock.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(pos, 0);
            pos += 130;
        }
    }

    public void CheckAllIsUnlocked() {
        foreach (var e in buildingsShopBlocks) {
            e.CheckIsUnlocked();
        }
    }

    public void SwitchIsOpened() {
        IsOpened = !IsOpened;
        BuildingShopTarget.transform.position = BuildingShopTargetOuter.transform.position;
        BuildingShopTarget2.transform.position = BuildingShopTargetOuter2.transform.position;
        if (!IsOpened)
        {
            var p = CardsManager.singleton.CardsPacks[CardClassType.Building];
            p.CheckIsHidden();

            foreach (var cardPack in CardsManager.singleton.CardsPacks) {
                cardPack.Value.CheckIsHidden();
            }
            CardsManager.singleton.RecalculateTotalCardsCount(true);
        }
        CardsManager.singleton.CardsPacksWidthUpdated(IsOpened ? 1 : 4);
    }

    void Update()
    {
        OpenValue = Mathf.Clamp01(OpenValue + Time.deltaTime * (IsOpened ? OpenSpeed : -OpenSpeed));
        Mask.localScale = new Vector3(OpenValue, 1, 1);
        if (OpenValue > 0.05f)
        {
            Content.localScale = new Vector3(1 / OpenValue, 1, 1);
            MaskBorder.localScale = new Vector3(1 / OpenValue, 1, 1);
        }
    }

}
