using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Field : PlacedBuilding, ITickable
{
    public int TicksTilFood = 24;
    public int FoodPerDay = 40;

    void Start() {
        Ticker.AddTickable(this, TickablePriority.High);
    }

    public void Tick(int luck)
    {
        TicksTilFood -= 1;
        if (TicksTilFood < 1)
        {
            TicksTilFood = 24;
            ResourceManager.singleton.AddResourceCount(EResource.Food, FoodPerDay, false);
        }
    }

    public override bool IsHaveBuildingInfo()
    {
        return true;
    }

    public override string GetBuildingName()
    {
        return "Field";
    }

    public override bool IsCanBeVisitedByUnits()
    {
        return false;
    }

    public override IEnumerable<IBuildingInfoBlock> GetBuildingInfo()
    {
        var block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.TimedAction);
        ((BI_TimedAction)block).Init($"Produces {FoodPerDay} Food every 24 hours.", 24, TicksTilFood);
        yield return block;
    }

    public override void UpdateSelectedBuildingInfo()
    {
        base.UpdateSelectedBuildingInfo();

        var block = BuildingInfo.singleton.buildingInfoBlocks[0];
        ((BI_TimedAction)block).UpdateTimeNow(TicksTilFood);
    }

    public override Sprite GetBuildingImage()
    {
        return base.GetBuildingImage();
    }
}
