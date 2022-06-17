using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectZoneType { 
    ChangeInfection,
    Heal,
    HealUnits,
    ChangeDiscontent,
}

public class EffectZone : MonoBehaviour
{
    public EffectZoneType ZoneType;
    public float Radius;
    public float Power;
    public bool IsVisible { get; private set; }

    [SerializeField] MeshRenderer zoneRenderer;

    public void Init(EffectZoneType zoneType, float radius, float power) {
        ZoneType = zoneType;
        Radius = radius;
        Power = power;
        transform.localScale = new Vector3(2 * radius, transform.localScale.y, 2 * radius);
    }

    public void SetIsVisible(bool isVisible) {
        IsVisible = isVisible;
    }

    public void CastEffect(int specialValue = 0) {
        var results = Physics.SphereCastAll(transform.position - new Vector3(0, 5, 0), Radius, new Vector3(0, 10, 0));
        var effected = new HashSet<GameObject>();
        foreach (var e in results) {
            var target = e.collider.gameObject;
            if (effected.Contains(target))
                continue;
            effected.Add(target);

            switch (ZoneType)
            {
                case EffectZoneType.ChangeInfection:
                    {
                        if (target.TryGetComponent<IInfectable>(out var infectable))
                        {
                            infectable.ChangeImmunty((int)Power);
                        }
                    }
                    break;
                case EffectZoneType.Heal:
                    {
                        if (target.TryGetComponent<B_House>(out var house))
                        {
                            house.Heal((int)Power);
                        }
                    }
                    break;
                case EffectZoneType.HealUnits:
                    {
                        if (e.collider.gameObject.TryGetComponent<UnitBase>(out var unit))
                        {
                            if (unit.UnitTeam == (UnitTeam)specialValue)
                            {
                                unit.Heal(1);
                            }
                        }
                    }
                    break;
                case EffectZoneType.ChangeDiscontent:
                    {
                        if (target.TryGetComponent<B_House>(out var house))
                        {
                            house.AddDiscontent((int)Power);
                        }
                    }
                    break;
            }
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        switch (ZoneType) {
            case EffectZoneType.Heal:
                if (GameCameraMode.CameraMode == EGameCameraMode.Infection)
                    zoneRenderer.enabled = IsVisible;
                else
                    zoneRenderer.enabled = false;
                return;
            case EffectZoneType.ChangeDiscontent:
                if (GameCameraMode.CameraMode == EGameCameraMode.Discontent)
                    zoneRenderer.enabled = IsVisible;
                else
                    zoneRenderer.enabled = false;
                return;
        }
    }
}
