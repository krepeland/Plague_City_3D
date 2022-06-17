using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable 
{
    public bool IsCanBeSelected();
    public void Selected();
    public GameObject GetSelectedObject();
    public List<GameObject> GetMeshObjects();
    public SelectedType GetSelectedType();
    public bool IsCanBeVisitedByUnits();
}
