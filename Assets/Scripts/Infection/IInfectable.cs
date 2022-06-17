using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInfectable
{
    public void ChangeImmunty(int delta);
    public void Heal(int power);
    public bool CheckIsCanBeInfected();
    public void CalculateInfection();
    public PlacedBuilding TryGetPlacedBuilding();
}
