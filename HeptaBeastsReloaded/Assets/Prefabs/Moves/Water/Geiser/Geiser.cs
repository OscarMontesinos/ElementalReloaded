using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geiser : Move
{
    public float dmg;
    public float area;
    public float delay;
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

        Vector3 destination = user.cursor.transform.position;
        Vector3 dist = destination - user.transform.position;
        if (dist.magnitude > range)
        {
            destination = user.transform.position + (dist.normalized * range);
        }

        GeiserObject discharge = Instantiate(moveObject, destination, transform.rotation).GetComponent<GeiserObject>();
        discharge.SetUp(user, element, type, dmg, area, delay);
    }
}
