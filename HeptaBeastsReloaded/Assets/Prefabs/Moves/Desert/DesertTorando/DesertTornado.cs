using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertTornado : Move
{
    public float dmgPerSecond;
    public float duration;
    public float area;
    public float spd;
    public float slow;
    public override void Trigger()
    {
        base.Trigger();
        float dmg;
        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(dmgPerSecond);
        }
        else
        {
            dmg = user.CalculateSinergy(dmgPerSecond);
        }

        Vector3 destination = user.cursor.transform.position;
        Vector3 dist = destination - user.transform.position;
        if (dist.magnitude > range)
        {
            destination = user.transform.position + (dist.normalized * range);
        }
        DesertTornadoObject tornado = Instantiate(moveObject, destination, user.pointer.transform.rotation).GetComponent<DesertTornadoObject>();
        tornado.SetUp(user, element, type,spd,area, duration, dmg,slow);
    }
}
