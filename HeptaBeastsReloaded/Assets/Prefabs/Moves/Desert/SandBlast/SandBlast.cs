using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class SandBlast : Move
{
    
    public float dmg;
    public float spd;


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


        SandBlastProjectile bullet = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<SandBlastProjectile>();
        bullet.NormalSetUp(user, element, type, dmg, spd, range);
    }


}
