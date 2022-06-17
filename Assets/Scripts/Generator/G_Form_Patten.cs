using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PatternRow
{
    public bool[] row;
}

[CreateAssetMenu(fileName = "Form_Pattern", menuName = "Generator/Form_Pattern", order = 0)]
public class G_Form_Patten : ScriptableObject 
{
    [SerializeField] public PatternRow[] Pattern = new PatternRow[] { 
        new PatternRow { row = new bool[5] }, 
        new PatternRow { row = new bool[5] },  
        new PatternRow { row = new bool[5] },  
        new PatternRow { row = new bool[5] },
        new PatternRow { row = new bool[5] }};
}
