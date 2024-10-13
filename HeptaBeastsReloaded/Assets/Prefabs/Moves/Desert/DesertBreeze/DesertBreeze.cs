using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBreeze : Move
{
    public int bursts;
    public float burstsDelay;
    public float dmg;
    public float spd;


    public override void Trigger()
    {
        base.Trigger();

        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
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

        int bursts = this.bursts;
        while (bursts > 0)
        {
            Projectile bullet = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<Projectile>();
            bullet.NormalSetUp(user, element, type, dmg, spd, range);
            bursts--;
            yield return new WaitForSeconds(burstsDelay);
        }
    }
}
