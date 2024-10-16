using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingStrike : Move
{
    public float dmg;
    public float delay;

    public override void Trigger()
    {
        base.Trigger();
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        float dmg;
        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        MeleeHit hit = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<MeleeHit>();
        hit.NormalSetUp(user, element, type, dmg);

        yield return new WaitForSeconds(delay);

        hit = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<MeleeHit>();
        hit.transform.localScale = new Vector3(-hit.transform.localScale.x, hit.transform.localScale.y, hit.transform.localScale.z);
        hit.NormalSetUp(user, element, type, dmg);
    }
}
