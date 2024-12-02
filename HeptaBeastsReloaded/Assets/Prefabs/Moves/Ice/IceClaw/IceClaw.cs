using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceClaw : Move
{
    public float dmg;

    public override void Trigger()
    {
        base.Trigger();
        float dmg;
        if(type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        MeleeHit hit = Instantiate(moveObject,user.transform.position, user.pointer.transform.rotation).GetComponent<MeleeHit>();
        hit.NormalSetUp(user, element, type, dmg);
    }

  
}
