using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour, IPauseable
{
    public Vector2Int FightPosition;
    public Dictionary<UnitTeam, HashSet<UnitBase>> UnitsInFight;
    public List<UnitBase> AllUnits;
    public Dictionary<UnitBase, UnitBase> UnitsTargets;
    bool isPaused;

    public void Init(Vector2Int pos) {
        Debug.Log("Fight started at " + pos);
        FightPosition = pos;
        UnitsInFight = new Dictionary<UnitTeam, HashSet<UnitBase>>();
        AllUnits = new List<UnitBase>();
        UnitsTargets = new Dictionary<UnitBase, UnitBase>();

        Ticker.singleton.AddPausable(this);
    }

    private void Update()
    {
        if (isPaused)
            return;
        UpdateAllUnitTimesTillFight();
    }

    public void AddUnitToFight(UnitBase unit) {
        if (!UnitsInFight.ContainsKey(unit.UnitTeam))
            UnitsInFight[unit.UnitTeam] = new HashSet<UnitBase>();
        UnitsInFight[unit.UnitTeam].Add(unit);
        if(!AllUnits.Contains(unit))
            AllUnits.Add(unit);
        unit.ResetUnitFightTime();
        unit.SetIsUnitInFight(true);
    }

    public bool FindTargetForUnit(UnitBase unit) {
        List<UnitBase> possibleTargets = new List<UnitBase>();
        foreach (var team in UnitsInFight) {
            if (team.Key == UnitTeam.EnemyToAll || team.Key != unit.UnitTeam)
            {
                foreach (var otherUnit in team.Value)
                {
                    if (otherUnit == null || otherUnit.IsDied)
                        continue;
                    possibleTargets.Add(otherUnit);
                }
            }
        }
        if (possibleTargets.Count <= 0)
            return false;

        UnitsTargets[unit] = possibleTargets[Random.Range(0, possibleTargets.Count)];
        return true;
    }

    void UpdateAllUnitTimesTillFight() {
        var delta = Time.deltaTime / Ticker.singleton.SecondsPerTick;

        for (var i = 0; i < AllUnits.Count; i++) {
            var unit = AllUnits[i];
            if (unit == null || unit.IsDied)
                continue;

            if (!UnitsTargets.ContainsKey(unit) || UnitsTargets[unit] == null)
            {
                if (!FindTargetForUnit(unit)) {
                    FightEnded();
                    return;
                }
            }

            if (unit.UpdateUnitFightTime(delta))
            {
                if (UnitsTargets.ContainsKey(unit) && UnitsTargets[unit] != null)
                {
                    UnitsTargets[unit].TryGetDamage(unit.Damage);
                }
            }
        }
    }

    void FightEnded()
    {
        Debug.Log("Fight started at " + FightPosition);
        for (var i = 0; i < AllUnits.Count; i++)
        {
            var unit = AllUnits[i];

            unit.ResetUnitFightTime();
            unit.SetIsUnitInFight(false);
        }
        UnitManager.singleton.FightEndedAt(FightPosition);
        Ticker.singleton.DeletePauseable(this);
        Destroy(gameObject);
    }

    public void SetPause(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
