using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlastProjectile : Projectile
{
    public GameObject exlodeParticle;
    public float angle;
    public float area;
    float delay;
    public PjBase targetHit;
    public override void Update()
    {
        base.Update();
        if(delay < 0.75f)
        {
            delay += Time.deltaTime;
        }
        else
        {
            StopTrackingTarget();
        }
    }

    public override void NormalSetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg, float speed, float range)
    {
        base.NormalSetUp(user, element, type, dmg, speed, range);

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit") && collision.GetComponent<PjBase>().team != user.team && !targetsAffected.Contains(collision.GetComponent<PjBase>()))
        {
            collision.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
            targetHit = collision.GetComponent<PjBase>();
            Die();
        }
        else if (collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }

    public override void Die()
    {
        Instantiate(exlodeParticle, transform.position,transform.rotation);
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, range, GameManager.Instance.unitLayer);
        PjBase enemy;
        List<PjBase> targets = new List<PjBase>();
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team && enemy != targetHit)
            {
                Transform target = enemy.transform;
                Vector2 dir = target.position - transform.position;

                if (Vector3.Angle(transform.up, dir.normalized) < angle / 2)
                {
                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                }
            }
        }
        base.Die();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);

        Vector3 rightAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, -angle / 2);
        Vector3 leftAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, angle / 2);

        Gizmos.DrawLine(transform.position, transform.position + leftAngle * area);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle * area);
    }
}
