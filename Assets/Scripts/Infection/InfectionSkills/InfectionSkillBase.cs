using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionSkillBase : MonoBehaviour, ITickable
{
    public int PowerCost;
    public int TicksReload;
    public int TicksReloadRandom;
    public int Priority;

    [HideInInspector] public int TicksToUse;

    public void Awake()
    {
        Ticker.AddTickable(this, TickablePriority.Low);
    }

    public virtual bool TryCastSkill(InfectionManager infectionManager) {
        return false;
    }

    void ITickable.Tick(int luck)
    {
        if(TicksToUse > 0)
            TicksToUse -= 1;
        Ticked();
    }

    public virtual void Ticked() { 
    
    }
}

public class InfectionSkillBaseComparer : IComparer<InfectionSkillBase>
{
    public int Compare(InfectionSkillBase x, InfectionSkillBase y)
    {
        if(x.Priority > y.Priority)
            return -1;
        if (x.Priority < y.Priority)
            return 1;
        return 0;
    }
}
