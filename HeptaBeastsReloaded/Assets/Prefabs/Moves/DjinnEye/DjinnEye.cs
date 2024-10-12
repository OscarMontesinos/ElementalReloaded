using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjinnEye : Move
{
    public float dmgPerSecond;
    public float duration;
    public float areaPerControlPoint;
    public float minArea;
    public float maxArea;
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
        DjinnEyeObject eye = Instantiate(moveObject, destination, new Quaternion(0, 0, 0, 0)).GetComponent<DjinnEyeObject>();
        eye.SetUp(user,element,type, areaPerControlPoint * user.stats.control, maxArea ,minArea, duration, dmg);
    }
}
