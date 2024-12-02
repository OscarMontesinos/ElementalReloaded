using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreaker : Move
{
    public float spd;
    public float dmg1;
    public float dmg2;
    public float area;
    public float area2;
    public float range2;

    public float stunTime;

    bool firstUsage = true;
    float recastTime;
    public override void Trigger()
    {
        base.Trigger();
        StartCoroutine(TriggerCorroutine());
    }
    IEnumerator TriggerCorroutine()
    {
        float dmg;
        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg1);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg1);
        }

        GameObject particle = null;

        StartCoroutine(user.Dash(user.pointer.transform.up, spd, range));

        while (!user.dashing)
        {
            yield return null;
        }

        List<PjBase> targetsAffected = new List<PjBase>();
        while (user.dashing)
        {
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, area, GameManager.Instance.unitLayer);
            PjBase enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<PjBase>();
                if (enemy.team != user.team && !targetsAffected.Contains(enemy))
                {
                    user.dashing = false;

                    yield return null;

                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                    StartCoroutine(user.Dash(user.pointer.transform.up, spd, range2));
                    StartCoroutine(enemy.Dash(user.pointer.transform.up, spd, range2));

                    while (!user.dashing)
                    {
                        yield return null;
                    }

                    while (user.dashing && enemy)
                    {
                        Collider2D wallHit = Physics2D.OverlapCircle(enemy.transform.position, area2, GameManager.Instance.wallLayer);
                        if (wallHit)
                        {
                            if (type == PjBase.AttackType.Physical)
                            {
                                dmg = user.CalculateStrength(this.dmg2);
                            }
                            else
                            {
                                dmg = user.CalculateSinergy(this.dmg2);
                            }

                            enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                            user.Stunn(enemy, stunTime);

                            particle = Instantiate(moveObject, enemy.transform.position, user.pointer.transform.rotation);

                            user.dashing = false;
                        }
                        yield return null;
                    }

                    break;
                }
            }
            yield return null;
        }


        user.AnimationCallStopAnim();
        if (particle)
        {
            particle.transform.parent = null;
        }
    }

}
