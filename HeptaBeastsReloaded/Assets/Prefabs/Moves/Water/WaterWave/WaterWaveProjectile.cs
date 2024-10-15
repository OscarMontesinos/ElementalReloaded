using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaveProjectile : Projectile
{
    float knockBackSpd;
    float knockBackRange;

    public void SetUp(float knockBackSpd, float knockBackRange)
    {
        this.knockBackSpd = knockBackSpd;
        this.knockBackRange = knockBackRange;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit") && collision.GetComponent<PjBase>().team != user.team && !targetsAffected.Contains(collision.GetComponent<PjBase>()))
        {
            PjBase target = collision.GetComponent<PjBase>();
            target.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
            targetsAffected.Add(target);
            StartCoroutine(target.Dash(transform.up,knockBackSpd,knockBackRange));
            if (!pierce)
            {
                Die();
            }
        }
        else if (collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }
}
