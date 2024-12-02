using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistBombProjectile : Projectile
{
    float area;
    public GameObject explosionFx;

    public void MistBombSetUp(float area)
    {
        this.area = area;
    }
    public override void Die()
    {
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
        Instantiate(explosionFx, transform.position, transform.rotation);
        

        base.Die();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }

}
