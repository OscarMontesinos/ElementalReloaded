using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistBomb : Move
{
    public float dmg;
    public float area;
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

        float range = (UtilsClass.GetMouseWorldPosition() - user.transform.position).magnitude;
        if(range > this.range)
        {
            range = this.range;
        }


        MistBombProjectile bullet = Instantiate(moveObject,user.transform.position, user.pointer.transform.rotation).GetComponent<MistBombProjectile>();
        bullet.NormalSetUp(user, element, type, dmg, spd, range);
        bullet.MistBombSetUp(area);

    }
}
