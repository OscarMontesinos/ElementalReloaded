using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundAmbush : Move
{
    public float spd;


    public override void Trigger()
    {
        base.Trigger();

        StartCoroutine(TriggerCorroutine());
    }
    IEnumerator TriggerCorroutine()
    {
        GameObject particle = Instantiate(moveObject, user.transform);
        particle.transform.position = user.transform.position;
        particle.transform.rotation = user.pointer.transform.rotation;

        Vector3 destination = user.cursor.transform.position;
        Vector3 dist = destination - user.transform.position;
        if (dist.magnitude > range)
        {

            yield return StartCoroutine(user.Dash(user.pointer.transform.up, spd, range, true, false));
        }
        else
        {

            yield return StartCoroutine(user.Dash(user.pointer.transform.up, spd, dist.magnitude, true, false));
        }


        user.AnimationCallStopAnim();
        particle.transform.parent = null;
    }
}
