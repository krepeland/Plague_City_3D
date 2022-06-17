using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionSkill_InfectRandom : InfectionSkillBase
{
    public int ImmunityDamage = 30;
    public override bool TryCastSkill(InfectionManager infectionManager)
    {
        var houseToInfect = TownManager.singleton.GetRandomHouse();
        if (houseToInfect == null)
            return false;

        ((B_House)houseToInfect).ChangeImmunty(-ImmunityDamage);
        Instantiate(PrefabManager.GetPrefab("Effect_House_Infected"), houseToInfect.transform.position, transform.rotation);
        return true;
    }
}
