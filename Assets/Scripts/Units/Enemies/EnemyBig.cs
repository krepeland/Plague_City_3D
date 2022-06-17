using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBig : UnitBase
{
    [SerializeField] float TicksToRandomMove;
    [SerializeField] int MaxRandomRunDistance = 5;

    int count = 1;

    [SerializeField] List<GameObject> UnitObjects;

    public void Awake()
    {
        TicksToRandomMove = Random.Range(0, 5);

        for (var i = 0; i < count; i++)
        {
            UnitObjects[i].SetActive(true);
        }
    }

    protected override void DoActionTick()
    {
        base.DoActionTick();
        TicksToRandomMove -= 1;
        if (TicksToRandomMove <= 0)
        {
            TicksToRandomMove = Random.Range(0, 3);
            MakeRandomMove();
        }
    }

    void MakeRandomMove()
    {
        var way = UnitManager.singleton.FindRandomWay(
            new Vector2Int(Mathf.RoundToInt(startPoint.x), Mathf.RoundToInt(startPoint.z)),
            MaxRandomRunDistance,
            true,
            false);

        if (way == null)
        {
            return;
        }

        var resultWay = new List<Vector3>();
        foreach (var e in way)
        {
            resultWay.Add(new Vector3(e.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(e.x, e.y)), e.y));
        }

        SetWay(resultWay, null, SelectedType.None);
    }

    public bool TryAddOneMoreUnit(UnitBase unit)
    {
        if (count >= 5)
            return false;
        count++;
        ReinitValues(
            Health + unit.Health,
            MaxHealth + unit.MaxHealth,
            1 / (UnitCard.AttacksPerHour * count),
            DodgePercent,
            Damage + unit.Damage,
            8);

        for (var i = 0; i < count; i++)
        {
            UnitObjects[i].SetActive(true);
        }
        return true;
    }

    public override bool DoActionUnitMoved(Vector2Int pos)
    {
        if (IsDied)
            return false;

        var units = UnitManager.singleton.UnitsPoses[pos];
        foreach (var unit in units)
        {
            if (unit == this)
                continue;
            if (!unit.IsDied && unit.UnitCardData.CardName == "EnemyTest")
            {
                if (TryAddOneMoreUnit(unit))
                {
                    unit.Die(false);
                    break;
                }
            }
        }
        return false;
    }
}
