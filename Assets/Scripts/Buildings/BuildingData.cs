using UnityEngine;

public class BuildingData
{
    public Vector2Int[] poses;
    public int floors = 1;
    public BuildingData(Vector2Int[] Poses, int Floors)
    {
        poses = Poses;
        if (Floors > 2)
            Floors = 2;
        floors = Floors;
    }
}
