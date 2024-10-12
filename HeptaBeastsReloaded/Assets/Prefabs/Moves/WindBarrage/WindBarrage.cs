using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class WindBarrage : Move
{
    public int bursts;
    public float burstsDelay;
    public float dmg;
    public float spd;
    public float spdOverTime;
    public float area;
    public float angle;


    public override void Trigger()
    {
        base.Trigger();

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, range, GameManager.Instance.unitLayer);
        PjBase enemy;
        List<PjBase> targets = new List<PjBase>();
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                Transform target = enemy.transform;
                Vector2 dir = target.position - user.transform.position;

                if (Vector3.Angle(user.pointer.transform.up, dir.normalized) < angle / 2 && !Physics2D.Raycast(user.transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
                {
                    targets.Add(enemy);

                }
            }
        }
        if (targets.Count > 0)
        {
            StartCoroutine(Shoot(targets));
        }
        else
        {
            if (user.currentMove1 == this)
            {
                user.currentHab1Cd = 1.5f;
            }
            if(user.currentMove2 == this)
            {
                user.currentHab2Cd = 1.5f;
            }
            if(user.currentMove3 == this)
            {
                user.currentHab3Cd = 1.5f;
            }
        }
    }

    IEnumerator Shoot(List<PjBase> targets)
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


        int lastTarget=100;
        int bursts = this.bursts;
        while (bursts > 0)
        {
            int targetInt = Random.Range(0, targets.Count);
            while(lastTarget == targetInt)
            {
                targetInt = Random.Range(0, targets.Count);
            }
            WindBarrageProjectile bullet = Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation).GetComponent<WindBarrageProjectile>();
            bullet.NormalSetUp(user, element, type, dmg, spd, range);
            bullet.SetUp(targets[targetInt], spdOverTime, area);
            bursts--;
            if (targets.Count > 1)
            {
                lastTarget = targetInt;
            }
            yield return new WaitForSeconds(burstsDelay);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        Vector3 rightAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, -angle / 2);
        Vector3 leftAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, angle   / 2);

        Gizmos.DrawLine(transform.position, transform.position + leftAngle * range);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle * range);
    }
}
