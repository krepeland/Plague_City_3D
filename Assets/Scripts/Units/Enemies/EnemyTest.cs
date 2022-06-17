using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : UnitBase
{
    [SerializeField] float TicksToRandomMove;
    [SerializeField] int MaxRandomRunDistance = 5;

    public void Awake()
    {
        TicksToRandomMove = Random.Range(0, 5);
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

    void MakeRandomMove() {
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

    public override bool DoActionUnitMoved(Vector2Int pos)
    {
        if (IsDied)
            return false;
        
        var units = UnitManager.singleton.UnitsPoses[pos];
        foreach (var unit in units) {
            if (unit == this)
                continue;
            if (!unit.IsDied && unit.UnitCardData.CardName == "EnemyTest") {
        
                var newUnit = UnitManager.singleton.SpawnAndInitEnemy("EnemyBig", pos, Target, true);
                if (newUnit == null) {
                    return false;
                }
                unit.Die(false);
                Die(false);
                newUnit.ReinitValues(
                    Health + unit.Health, 
                    MaxHealth + unit.MaxHealth,
                    TimePerAttack / 2f,
                    DodgePercent,
                    Damage,
                    0);
        
                return true;
            }
        
            if (!unit.IsDied && unit.UnitCardData.CardName == "EnemyBig")
            {
                if (((EnemyBig)unit).TryAddOneMoreUnit(this))
                {
                    Die(false);
                    return true;
                }
            }
        }
        return false;
    }
}
