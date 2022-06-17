using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Guard : UnitBase
{
    [SerializeField] float timeToBurn;
    bool isOnThePosition;
    public Sprite BurnActionSprite;

    public static int BurnDownPower = 2;
    private static float StartTimeToBurnDown = 1;

    bool isBurningHouse;
    public EffectZone DiscontentZoneBig;
    public EffectZone DiscontentZoneMedium;
    public EffectZone DiscontentZoneSmall;

    public override void Inited()
    {
        DiscontentZoneBig.Init(EffectZoneType.ChangeDiscontent, 8, 1);
        DiscontentZoneMedium.Init(EffectZoneType.ChangeDiscontent, 5, 2);
        DiscontentZoneSmall.Init(EffectZoneType.ChangeDiscontent, 2, 10);
        base.Inited();
    }

    protected override void DoAction()
    {
        base.DoAction();
        if (TargetType == SelectedType.Building)
        {
            var b = (PlacedBuilding)Target;
            if (b.IsCanBeBurnedDown())
            {
                isOnThePosition = true;
                timeToBurn = 0;
                Mark.SetActionEnabled(isOnThePosition);
                Mark.SetActionSprite(BurnActionSprite);
                if (b.GetBuildingName() == "House")
                {
                    SetZonesVisible(true);
                    isBurningHouse = true;
                }
                else
                {
                    SetZonesVisible(false);
                    isBurningHouse = false;
                }
            }
        }
    }

    void SetZonesVisible(bool isVisible) {
        DiscontentZoneBig.SetIsVisible(isVisible);
        DiscontentZoneMedium.SetIsVisible(isVisible);
        DiscontentZoneSmall.SetIsVisible(isVisible);
    }

    void CastZones()
    {
        DiscontentZoneBig.CastEffect();
        DiscontentZoneMedium.CastEffect();
        DiscontentZoneSmall.CastEffect();
    }

    protected override void DoActionChangedTarget(ISelectable previousTarget, ISelectable newTarget)
    {
        base.DoActionChangedTarget(previousTarget, newTarget);
        if (previousTarget != newTarget)
        {
            isOnThePosition = false;
            timeToBurn = 0;
            Mark.SetActionEnabled(isOnThePosition);
            SetZonesVisible(false);
        }
    }

    protected override void DoActionUpdate()
    {
        base.DoActionUpdate();
        if (isOnThePosition)
        {
            Mark.SetActionFiller(timeToBurn / StartTimeToBurnDown);
            timeToBurn += Time.deltaTime / Ticker.singleton.SecondsPerTick;
            if (timeToBurn >= StartTimeToBurnDown)
            {
                ((PlacedBuilding)Target).BurnDown(BurnDownPower);
                isOnThePosition = ((PlacedBuilding)Target).IsCanBeBurnedDown();
                Mark.SetActionEnabled(isOnThePosition);
                timeToBurn = 0;

                if (isBurningHouse) {
                    CastZones();
                }
            }
        }
    }

    public new static (UnitActionType, string)[] GetActionTypeOnSelectedType(UnitCard unitCard, ISelectable selectable)
    {
        if (UnitManager.singleton.Way == null)
        {
            return new (UnitActionType, string)[0];
        }

        var moveTime = (UnitManager.singleton.Way.Count / (float)unitCard.Speed).ToString("0.0");
        switch (selectable.GetSelectedType())
        {
            case SelectedType.Building:
                return new[] {
                    (UnitActionType.Walk, moveTime),
                    (UnitActionType.Fire, "5.0") };
        }

        return new[] { (UnitActionType.Walk, moveTime) };
    }
}
