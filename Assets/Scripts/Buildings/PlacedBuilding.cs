using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacedBuilding : MonoBehaviour, ISelectable
{
    public BuildingData buildingData;

    public (GameObject, int)[] buildingPrefabs;
    private bool canBeSelected = true;

    public List<GameObject> MeshObjects = new List<GameObject>();

    public Sprite BuildingInfoSprite;
    public bool IsUnitCanWalkThrough;

    protected bool isDestroyed;

    public void SetBuildingData(BuildingData data) {
        buildingData = data;
        UpdateInfectionDisplay(CellInfectionState.None);
        BuildingDataSetted();
    }

    public virtual void BuildingDataSetted() { 
    
    }

    public void SetBuildingPrefabs((GameObject, int)[] prefabs) {
        buildingPrefabs = prefabs;
        foreach (var buildingPart in buildingPrefabs) {
            for (var i = 0; i < buildingPart.Item1.transform.childCount; i++)
            {
                MeshObjects.Add(buildingPart.Item1.transform.GetChild(i).gameObject);
            }
        }
    }

    public virtual bool IsCanBeBurnedDown() {
        return false;
    }

    public virtual void BurnDown(int count) { 
        
    }

    public virtual void ForceDestroyBuilding()
    {
        if (isDestroyed)
            return;
        Destroy(gameObject);
    }

    public void Selected()
    {

    }

    public void SetIsCanBeSelected(bool isCanBeSelected) {
        canBeSelected = isCanBeSelected;
    }

    public bool IsCanBeSelected()
    {
        return canBeSelected;
    }

    public GameObject GetSelectedObject()
    {
        if (isDestroyed)
            return null;
        return gameObject;
    }

    public virtual bool IsHaveBuildingInfo() {
        return false;
    }

    public virtual string GetBuildingName() {
        return "";
    }
    public virtual void SetBuildingImage(Sprite sprite)
    {
        BuildingInfoSprite = sprite;
    }

    public virtual Sprite GetBuildingImage()
    {
        return BuildingInfoSprite;
    }

    public virtual IEnumerable<IBuildingInfoBlock> GetBuildingInfo()
    {
        yield return null;
    }

    public virtual void UpdateSelectedBuildingInfo()
    {

    }

    public virtual EInfectedBuildingStatus GetEInfectedBuildingStatus() {
        return EInfectedBuildingStatus.None;  
    }

    public SelectedType GetSelectedType()
    {
        return SelectedType.Building;
    }

    public void TryUpdateBuildingInfo() {
        BuildingInfo.singleton.TryUpdateBuilding(this);
    }

    public virtual bool IsCanBeVisitedByUnits()
    {
        return true;
    }

    public virtual void UpdateInfectionDisplay(CellInfectionState state) {
        if (isDestroyed)
            return;

        foreach (var e in buildingData.poses) {
            InfectionManager.SetInfectionTextureState(e.x, e.y, state);
        }
    }

    public List<GameObject> GetMeshObjects()
    {
        if (isDestroyed)
            return new List<GameObject>();
        return MeshObjects;
    }

    void OnDestroy() {
        isDestroyed = true;
    }
}
