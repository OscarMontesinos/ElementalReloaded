using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWave : Move
{
    public float dmg;
    public float spd;
    public float knockBackSpd;
    public float knockBackRange;


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

        WaterWaveProjectile wave = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<WaterWaveProjectile>();
        wave.NormalSetUp(user, element, type, dmg, spd, range);
        wave.SetUp(knockBackSpd, knockBackRange);
    }

}
