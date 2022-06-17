using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WFC_Pattern", menuName = "Generator/WFC_Pattern", order = 0)]
public class WFC_Pattern : ScriptableObject
{
    public int Weight = 1;
    public bool IsActive = true;
    [SerializeField]
    public PatternRow[] Pattern = new PatternRow[] {
        new PatternRow { row = new bool[3] },
        new PatternRow { row = new bool[3] },
        new PatternRow { row = new bool[3] } 
    };


    public List<bool> GetXline(int y) {
        var result = new List<bool>();
        for (var x = 0; x < Pattern[y].row.Length; x++)
        {
            result.Add(Pattern[y].row[x]);
        }
        return result;
    }

    public List<bool> GetYline(int x)
    {
        var result = new List<bool>();
        for (var y = 0; y < Pattern[x].row.Length; y++)
        {
            result.Add(Pattern[y].row[x]);
        }
        return result;
    }

    public bool GetPoint(int x, int y) {
        return Pattern[y].row[x];
    }
}
