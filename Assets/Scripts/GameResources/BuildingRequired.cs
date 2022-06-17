using System;

public enum CountComparison { 
    MoreOrEqualTo,
    EqualTo,
    NoMoreThan,
}

[Serializable]
public struct BuildingRequired
{
    public string CardName;
    public CountComparison CountComparison;
    public int Count;
}
