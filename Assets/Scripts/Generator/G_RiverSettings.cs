using UnityEngine;

[CreateAssetMenu(fileName = "G_RiverSettings", menuName = "Settings/G_RiverSettings", order = 0)]
public class G_RiverSettings : ScriptableObject {
    public bool RandomRiverTargets;
    [Range(-2, 2)]
    public float RiverXmultiplier = 1;
    [Range(0, 2)]
    public float RiverYmultiplier = 1;
    [Header("Only if second")]
    [Range(0, 1)]
    public float RiverStart;

    public bool IsCanBeDeleted;
    public int DeleteValue = 5;
}
