using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueMove : MonoBehaviour
{
    public int PointNow;

    public bool MovePerOneTick;

    [HideInInspector] public bool IsMoving;
    [HideInInspector] public Vector3 nextPoint = Vector3.zero;
    [HideInInspector] public Vector3 startPoint = Vector3.zero;
    private List<Vector3> way;

    private float Speed = 5;
    private float value;

    void Awake() {
        startPoint = transform.position;
        nextPoint = transform.position;
    }

    public Vector2Int GetPosNow()
    {
        return new Vector2Int(Mathf.RoundToInt(startPoint.x), Mathf.RoundToInt(startPoint.z));
    }


    void Update()
    {
        if (Ticker.IsPaused)
            return;

        Move();
    }

    void NextPoint()
    {
        if (way.Count == 0)
        {
            Finish();
        }

        if (way != null && way.Count > 0)
        {
            var pos = ConvertVector3ToVector2Int(nextPoint);
            if (TownManager.singleton.IsBuildingAt(pos))
            {
                var building = TownManager.singleton.GetBuildingAt(pos);
                if (building.GetBuildingName() == "Barricade")
                {
                    Finish();
                    return;
                }
            }

            startPoint = nextPoint;
            nextPoint = way[0];
            way.RemoveAt(0);
        }
        else
        {
            IsMoving = false;
            transform.position = nextPoint;
            nextPoint = transform.position;
            startPoint = transform.position;
        }
    }

    void Move()
    {
        value += Time.deltaTime / Ticker.singleton.SecondsPerTick * Speed;
        if (value > 1)
        {
            if (way.Count > 0)
            {
                value -= 1;
            }
            NextPoint();
        }

        transform.position = Vector3.Lerp(startPoint, nextPoint, value);
    }

    public void SetWay(List<Vector3> way, float speed)
    {
        value = 0;
        this.way = way;
        if (way.Count > 0)
        {
            IsMoving = true;
            NextPoint();
        }
        else
        {
            IsMoving = false;
        }
    }

    public void Finish()
    {
        var pos = ConvertVector3ToVector2Int(nextPoint);
        if (TownManager.singleton.GetBuildingAt(pos) != null)
        {
            if (TownManager.singleton.GetBuildingAt(pos).TryGetComponent<IInfectable>(out var infectable))
            {
                infectable.ChangeImmunty(-30);
                Instantiate(PrefabManager.GetPrefab("Effect_House_Infected"), transform.position, transform.rotation);
            }
        }
        InfectionManager.InfectionEnemyCount -= 1;
        TaskManager.UpdatedETask(ETask.InfectedPercent);
        //Debug.Log(InfectionManager.InfectionEnemyCount);
        Destroy(gameObject);
    }

    Vector2Int ConvertVector3ToVector2Int(Vector3 pos) {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }
}
