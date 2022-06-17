using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_TownHall : PlacedBuilding
{
    public BuyCardInBuilding BuyCardInBuilding;
    public BuyCardInBuilding BuyCardInBuilding2;

    public PlacedBuilding TryGetPlacedBuilding()
    {
        return this;
    }

    public override bool IsHaveBuildingInfo()
    {
        return true;
    }

    public override string GetBuildingName()
    {
        return "Town Hall";
    }

    public override IEnumerable<IBuildingInfoBlock> GetBuildingInfo()
    {
        var block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.BuyCards);
        ((BI_BuyCards)block).Init(
            BuyCardInBuilding.GetText(), 
            BuyCardInBuilding.TicksTilAdd, 
            BuyCardInBuilding.ticks, 
            BuyCardInBuilding.GetBuyCardDatas(), 
            BuyCardInBuilding);
        yield return block;

        block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.BuyCards);
        ((BI_BuyCards)block).Init(
            BuyCardInBuilding2.GetText(),
            BuyCardInBuilding2.TicksTilAdd,
            BuyCardInBuilding2.ticks,
            BuyCardInBuilding2.GetBuyCardDatas(),
            BuyCardInBuilding2);
        yield return block;
    }

    public override void UpdateSelectedBuildingInfo()
    {
        var block = BuildingInfo.singleton.buildingInfoBlocks[0];
        ((BI_BuyCards)block).UpdateTimeNow(BuyCardInBuilding.GetText(), BuyCardInBuilding.ticks);

        block = BuildingInfo.singleton.buildingInfoBlocks[1];
        ((BI_BuyCards)block).UpdateTimeNow(BuyCardInBuilding2.GetText(), BuyCardInBuilding2.ticks);
    }

    public override EInfectedBuildingStatus GetEInfectedBuildingStatus()
    {
        return EInfectedBuildingStatus.None;
    }
}
