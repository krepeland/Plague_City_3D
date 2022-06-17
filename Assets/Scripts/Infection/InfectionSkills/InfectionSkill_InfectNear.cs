using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectionSkill_InfectNear : InfectionSkillBase
{
    public int MaxDistance = 15;
    public int ImmunityDamage = 30;
    public float Speed = 1f;

    public override bool TryCastSkill(InfectionManager infectionManager)
    {
        var randomInfectedHouse = infectionManager.GetRandomInfectedObject();
        if (randomInfectedHouse == null || !randomInfectedHouse.CheckIsCanBeInfected())
            return false;

        var startPoints = new List<Vector2Int>();
        var exceptPoint = new HashSet<Vector2Int>();
        foreach (var p in randomInfectedHouse.TryGetPlacedBuilding().buildingData.poses) {
            exceptPoint.Add(p);
            startPoints.Add(p);
        }
        var wayToNearestHouse = infectionManager.FindPathToInfectClosestHouse(startPoints, MaxDistance, exceptPoint);
        if (wayToNearestHouse.Count == 0)
            return false;

        var e = wayToNearestHouse.Last();
        var plague_move = Instantiate(
            PrefabManager.GetPrefab("Plague_Move"),
            new Vector3(e.x, GeneratorSpawner.singleton.Heights[e.x, e.y] + 1, e.y), 
            transform.rotation).GetComponent<PlagueMove>();
        plague_move.PointNow = wayToNearestHouse.Count - 1;

        var newWay = new List<Vector3>();
        foreach (var pos in wayToNearestHouse)
            newWay.Add(new Vector3(pos.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(pos.x, pos.y)), pos.y));

        newWay.Reverse();
        plague_move.SetWay(newWay, Speed);
        InfectionManager.InfectionEnemyCount += 1;

        return true;
    }
}
