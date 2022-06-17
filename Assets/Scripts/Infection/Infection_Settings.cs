using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InfectionSkill {
    public InfectionSkillBase InfectionSkillBase;
    public int TicksTillUnlockingSKill;
}

[CreateAssetMenu(fileName = "Infection_Settings", menuName = "Settings/Infection_Settings", order = 0)]
public class Infection_Settings : ScriptableObject
{
    public int TimeToDeathSample = 12;
    public int TimeToDeathRandom = 6;

    public int PowerPerTick = 10;
    public float PowerPerInfectedHouse = 2f;

    public List<InfectionSkill> InfectionSkillBase = new List<InfectionSkill>();
}
