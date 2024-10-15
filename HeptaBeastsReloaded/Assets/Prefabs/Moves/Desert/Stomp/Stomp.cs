using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomp : Move
{
    public float dmg;


    public override void Trigger()
    {
        base.Trigger();
        float dmg;

        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        StompObject stomp = Instantiate(moveObject,user.transform.position, new Quaternion(0,0,0,0)).GetComponent<StompObject>();
        stomp.SetUp(user,element,type,dmg);
    }
}
