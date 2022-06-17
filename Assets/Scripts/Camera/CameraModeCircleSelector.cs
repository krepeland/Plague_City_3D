using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModeCircleSelector : MonoBehaviour
{
    public static CameraModeCircleSelector singleton;
    public Transform CursorContainer;

    public int IndexNow { get; private set; }

    public bool IsVisible { get; private set; } = true;
    public List<CameraModeSelectorSector> Sectors = new List<CameraModeSelectorSector>();

    private void Awake()
    {
        singleton = this;
        SetIsVisible(false);
    }

    public void SetIndex(int index) {
        Sectors[IndexNow].SetIsSelected(false);
        IndexNow = index;
        Sectors[IndexNow].SetIsSelected(true);
    }

    public void SetIsVisible(bool isVisible) {
        if (IsVisible == isVisible)
            return;

        if (!IsVisible && isVisible)
        {
            SetIndex(0);
            transform.position = CursorContainer.position;
        }
        IsVisible = isVisible;
        gameObject.SetActive(IsVisible);
    }
}
