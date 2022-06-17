using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ESelectorMode { 
    DirectHit,
    AutoAim,
    OnlyCoords
}

public class Selector : MonoBehaviour
{
    public static Selector singleton;

    public ESelectorMode SelectorMode;

    public float AutoAimRadius;
    
    private Camera mainCamera;

    public Transform Container;
    public Transform LightTransform;

    public Vector3 SelectedCoords;
    public Vector3Int SelectedIntCoords;
    public ISelectable SelectedObject;
    public SelectedType SelectedType;

    private Vector3 oldSelectedCoords;

    private ISelectable outlinedSelectable;

    private void Awake()
    {
        singleton = this;
        mainCamera = Camera.main;
    }

    public void SetSelectorMode(ESelectorMode mode) {
        SelectorMode = mode;
    }

    public void OutlineSelectable(ISelectable oldSelectable, ISelectable selectable) {
        if (oldSelectable != selectable)
        {
            if (oldSelectable != null)
                foreach (var e in oldSelectable.GetMeshObjects())
                {
                    if(e != null)
                        e.layer = 0;
                }

            if (selectable != null)
                foreach (var e in selectable.GetMeshObjects())
                {
                    if (e != null)
                        e.layer = 6;
                }
        }
    }


    public void UpdateSelector(Vector2 mousePosition)
    {
        var ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 1.5f));
        if (Physics.Raycast(ray, out var h1)) {
            LightTransform.position = h1.point - ray.direction * 2f;
        }

        if (CameraController.singleton.IsMouseOnGUI) {
        return;
        }

        if (UnitManager.singleton.IsUnitSelected) {
            if (UnitManager.singleton.SelectedUnit != null)
            {
                SelectorMode = UnitManager.singleton.SelectedUnit.UnitCardData.SelectorModeWhenTaked;
            }
        }

        if (Physics.Raycast(ray, out var hit))
        {
            SelectedCoords = hit.point;

            if (SelectorMode == ESelectorMode.OnlyCoords)
            {
                SelectedCoords = hit.point;
                oldSelectedCoords = hit.point;
                SelectedObject = null;

                var pos = new Vector2Int(Mathf.RoundToInt(SelectedCoords.x), Mathf.RoundToInt(SelectedCoords.z));
                SelectedIntCoords = new Vector3Int(pos.x, GeneratorSpawner.singleton.GetHeigthAt(pos), pos.y);
                Container.transform.position = SelectedIntCoords;

                CardsManager.singleton.UpdateBuildingPreviewIsCanBePlaced();
                return;
            }

            if (Vector3.Distance(SelectedCoords, oldSelectedCoords) < 0.2f)
                return;

            oldSelectedCoords = SelectedCoords;

            if (hit.collider.gameObject.TryGetComponent<ISelectable>(out var selectable))
            {
                if (selectable.IsCanBeSelected())
                {
                    SelectedObject = selectable;
                }
            }
            else {
                if (SelectorMode == ESelectorMode.AutoAim)
                {
                    var hitsResults = Physics.SphereCastAll(SelectedCoords, AutoAimRadius, Vector3.down);
                    var results = hitsResults.Where((x) => x.collider.gameObject.TryGetComponent<ISelectable>(out var selectable));
                    var bestValues = results.Where(
                        (e) => (Vector3.Distance(SelectedCoords, e.transform.position)) == results.Min(
                            (x) => Vector3.Distance(SelectedCoords, x.transform.position))
                        );
                    if (bestValues.Any())
                    {
                        var closest = bestValues.First();
                        SelectedObject = closest.collider.gameObject.GetComponent<ISelectable>();
                    }
                    else
                    {
                        SelectedObject = null;
                    }
                }
                else
                {
                    SelectedObject = null;
                }
            }
        }
        else {
            if (SelectorMode != ESelectorMode.AutoAim) {
                SelectedObject = null;
            }
        }

        if (SelectedObject != null)
        {
            Container.transform.position = SelectedObject.GetSelectedObject().transform.position;
            SelectedType = SelectedObject.GetSelectedType();
            UnitManager.singleton.SetTarget(SelectedObject);
        }
        else {
            Container.transform.position = Vector3.zero;
            SelectedType = SelectedType.None;
        }

        if (outlinedSelectable != (ISelectable)BuildingInfo.singleton.OutlinedBuilding)
        {
            OutlineSelectable(outlinedSelectable, SelectedObject);
        }
        else
        {
            OutlineSelectable(null, SelectedObject);
        }
        outlinedSelectable = SelectedObject;
    }
}
