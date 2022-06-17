using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Doctor : UnitBase
{
    float timeTillHeal;
    float timeTillHealTargetNow;
    bool isOnThePosition;
    bool isHealingOutside;
    IInfectable infectableTarget;

    public float StartTimeTillHealOutside = 4f;
    public float StartTimeTillHealInHouse = 1f;

    public float Radius = 3;

    public Sprite HealSprite;

    public int Power = 1;

    public EffectZone HealEffectZone;

    public override void Inited()
    {
        base.Inited();
        HealEffectZone.Init(EffectZoneType.Heal, Radius, Power);
    }

    protected override void DoAction()
    {
        base.DoAction();
        if (TargetType == SelectedType.Building && Target.GetSelectedObject().TryGetComponent<IInfectable>(out infectableTarget))
        {
            timeTillHealTargetNow = StartTimeTillHealInHouse;
            HealEffectZone.SetIsVisible(false);
            isHealingOutside = false;
        }
        else
        {
            timeTillHealTargetNow = StartTimeTillHealOutside;
            HealEffectZone.SetIsVisible(true);
            isHealingOutside = true;
        }
        timeTillHeal = 0;
        isOnThePosition = true;
        Mark.SetActionEnabled(isOnThePosition);
        Mark.SetActionSprite(HealSprite);
    }

    protected override void DoActionUpdate()
    {
        base.DoActionUpdate();
        if (isOnThePosition)
        {
            Mark.SetActionFiller(1 - timeTillHeal / timeTillHealTargetNow);
            timeTillHeal += Time.deltaTime / Ticker.singleton.SecondsPerTick;
            if (timeTillHeal >= timeTillHealTargetNow)
            {
                Mark.SetActionEnabled(isOnThePosition);
                timeTillHeal -= timeTillHealTargetNow;
                Heal();
            }
        }
    }

    public void Heal() {
        if (isHealingOutside || infectableTarget == null)
        {
            //var targets = Physics.SphereCastAll(transform.position + Vector3.up, Radius, new Vector3(0, -2, 0));
            //foreach (var e in targets)
            //{
            //    if (e.collider.gameObject.TryGetComponent<IInfectable>(out var infectable))
            //    {
            //        infectable.Heal(Power);
            //    }
            //
            //    if (e.collider.gameObject.TryGetComponent<UnitBase>(out var unit))
            //    {
            //        if (unit.UnitTeam == UnitTeam) { 
            //            unit.Heal(1);
            //        }
            //    }
            //}
            HealEffectZone.CastEffect();
        }
        else {
            infectableTarget.Heal(Power);
        }
    }

    protected override void DoActionChangedTarget(ISelectable previousTarget, ISelectable newTarget)
    {
        base.DoActionChangedTarget(previousTarget, newTarget);
        if (previousTarget != newTarget)
        {
            isOnThePosition = false;
            timeTillHeal = 0;
            Mark.SetActionEnabled(isOnThePosition);
            HealEffectZone.SetIsVisible(false);
        }
    }

    public new static (UnitActionType, string)[] GetActionTypeOnSelectedType(UnitCard unitCard, ISelectable selectable)
    {
        if (UnitManager.singleton.Way == null || UnitManager.singleton.Way.Count == 0) {
            return new (UnitActionType, string)[0];
        }

        var moveTime = (UnitManager.singleton.Way.Count / (float)unitCard.Speed).ToString("0.0");
        switch (selectable.GetSelectedType()) {
            case SelectedType.Building:
                return new[] { 
                    (UnitActionType.Walk, moveTime), 
                    (UnitActionType.Heal, "") };
        }

        return new[] { (UnitActionType.Walk, moveTime) };
    }
}
