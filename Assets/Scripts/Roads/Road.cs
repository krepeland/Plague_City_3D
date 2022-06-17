using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour, ISelectable
{
    public List<GameObject> MeshesObjects;

    public bool IsCanBeVisitedByUnits()
    {
        return true;
    }

    public GameObject GetSelectedObject()
    {
        return gameObject;
    }

    public SelectedType GetSelectedType()
    {
        return SelectedType.Road;
    }

    public bool IsCanBeSelected()
    {
        return true;
    }

    public bool IsHaveBuildingInfo()
    {
        return false;
    }

    public void Selected()
    {

    }

    public List<GameObject> GetMeshObjects()
    {
        return MeshesObjects;
    }
}
