using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBarrageProjectile : Projectile
{
    public GameObject exlodeParticle;
    float area;
    float delay;
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

    public void SetUp(PjBase target, float spdOverTime, float area)
    {
        this.area = area;
        this.target = target;
        this.spdOverTime = spdOverTime;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit") && collision.GetComponent<PjBase>().team != user.team && !targetsAffected.Contains(collision.GetComponent<PjBase>()))
        {
            
            Die();
        }
        else if (collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }

    public override void Die()
    {
        Instantiate(exlodeParticle, transform.position, new Quaternion(0, 0, 0, 0));
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
            }
        }
        base.Die();
    }

}
