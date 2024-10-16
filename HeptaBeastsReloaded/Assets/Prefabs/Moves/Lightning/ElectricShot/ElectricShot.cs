using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShot : Move
{
    public int bursts;
    public float burstDelay;
    public float detour;
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
        StartCoroutine(Shoot(dmg));
    }

    IEnumerator Shoot(float dmg)
    {
        int bursts = this.bursts - 1;
        while (bursts > 0)
        {
            yield return new WaitForSeconds(burstDelay);
            Vector2 dir = user.cursor.transform.position - user.pointer.transform.position;
            user.pointer.transform.up = dir;
            yield return null;
            Projectile bullet = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<Projectile>();
            bullet.NormalSetUp(user, element, type, dmg, spd, range);
            bullet.transform.eulerAngles = new Vector3(0, 0, bullet.transform.localEulerAngles.z +  detour);
            bursts--;

            if (bursts > 0)
            {
                yield return new WaitForSeconds(burstDelay);
                dir = user.cursor.transform.position - user.pointer.transform.position;
                user.pointer.transform.up = dir;
                yield return null;
                bullet = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<Projectile>();
                bullet.NormalSetUp(user, element, type, dmg, spd, range);
                bullet.transform.eulerAngles = new Vector3(0, 0, bullet.transform.localEulerAngles.z - detour);
                bursts--;
            }
        }
    }
}
