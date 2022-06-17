using System;
using UnityEngine;

[Serializable]
public struct CellOffset
{
    public bool IsFilled;
    [Range(-1, 1)]
    public int OffsetX;
    [Range(-1, 1)]
    public int OffsetY;
}


[CreateAssetMenu(fileName = "Form_SubdividePattern", menuName = "Generator/Form_SubdividePattern", order = 0)]
public class G_Form_SubdividePatten : ScriptableObject
{
    public bool IsActive = true;

    public bool IsFilled = true;

    public int NeighbourCells;

    public PatternRow[] Pattern = new PatternRow[] {
        new PatternRow { row = new bool[3] },
        new PatternRow { row = new bool[3] },
        new PatternRow { row = new bool[3] },
    };
}
