using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Barricade : PlacedBuilding, IInfectable
{
    public int MaxHealth = 100;
    public int Health;

    public void CalculateInfection()
    {

    }

    public override void BuildingDataSetted()
    {
        base.BuildingDataSetted();
        Health = MaxHealth;
        var pos = buildingData.poses[0];
        InfectionManager.SetInfectionTextureState(pos.x, pos.y, CellInfectionState.Defensed);
    }

    public void ChangeImmunty(int delta)
    {
        Health = Mathf.Clamp(Health + delta, 0, MaxHealth);
        if (Health <= 0) {
            TownManager.singleton.DestroyBuildingAt(buildingData.poses[0]);
        }
        BuildingInfo.singleton.TryUpdateBuilding(this);
    }

    public bool CheckIsCanBeInfected()
    {
        return true;
    }

    public PlacedBuilding TryGetPlacedBuilding()
    {
        return this;
    }

    public override bool IsHaveBuildingInfo()
    {
        return true;
    }

    public override IEnumerable<IBuildingInfoBlock> GetBuildingInfo()
    {
        var block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.Text);
        ((BI_Text)block).Init($"Durability: {Health}");
        yield return block;
    }

    public override void UpdateSelectedBuildingInfo()
    {
        base.UpdateSelectedBuildingInfo();

        var block = BuildingInfo.singleton.buildingInfoBlocks[0];
        ((BI_Text)block).Init($"Durability: {Health}");
    }

    public override string GetBuildingName()
    {
        return "Barricade";
    }

    public void Heal(int power)
    {

    }
}
