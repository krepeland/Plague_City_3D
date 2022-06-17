using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMarkButton : CustomButton
{
    [SerializeField] UnitMark owner;
    [SerializeField] Image image;
    public HashSet<UnitMarkButton> unitMarks = new HashSet<UnitMarkButton>();
    public int Index;
    public bool IsInteractable;

    private void Awake()
    {
        unitMarks.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<UnitMarkButton>(out var res)) {
            unitMarks.Add(res);
            Recalculate(null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<UnitMarkButton>(out var res))
        {
            if (unitMarks.Contains(res))
            {
                unitMarks.Remove(res);
                Recalculate(null);
            }
        }
    }

    public void SetIsInteractable(bool isInteractable)
    {
        IsInteractable = isInteractable;
        image.raycastTarget = IsInteractable;
    }

    void Recalculate(UnitMarkButton destroyedMark)
    {
        var count = 0;
        foreach (var e in unitMarks)
        {
            if (e == null || e == destroyedMark)
                continue;
            if (e.Index < Index)
            {
                count += 1;
            }
        }
        owner.TargetAngle = -60 * count;
    }

    public void RecalculateOther(UnitMarkButton destroyedMark)
    {
        foreach (var e in unitMarks) {
            if (e == null)
                continue;
            e.Recalculate(destroyedMark);
        }
    }

    public override void OnHover()
    {
        if (!IsInteractable)
            return;
        owner.transform.localScale = Vector3.one * 1.1f;
    }

    public override void OutHover()
    {
        if (!IsInteractable)
            return;
        if (owner != null)
            owner.transform.localScale = Vector3.one * 1f;
    }

    public override float GetPressedTime()
    {
        return 0;
    }

    public override void OnHold(int mouseButton)
    {
        if (!IsInteractable)
            return;
        owner.Holded(mouseButton);
        CameraController.IsUpdatingByUnit = true;
    }
}
