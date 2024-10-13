using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PebbleBullet : Move
{
    public float dmg;
    public float spd;


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
        Projectile bullet = Instantiate(moveObject,user.transform.position, user.pointer.transform.rotation).GetComponent<Projectile>();
        bullet.NormalSetUp(user, element, type, dmg, spd, range);
    }
}
