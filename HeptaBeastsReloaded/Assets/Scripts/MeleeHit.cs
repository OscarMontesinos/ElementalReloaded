using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeHit : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public HitData.Element element;
    [HideInInspector]
    public PjBase.AttackType type;
    [HideInInspector]
    public float dmg;
    public float area;
    public float range;

    public void NormalSetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.dmg = dmg;

        Trigger();
    }

    public virtual void Trigger()
    {
        Vector2 dir = user.transform.position + (transform.up.normalized * range);
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(dir, area, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
            }
        }
    }

    public virtual void UnparentOnEndAnim()
    {
        transform.parent = null;
    }


    private void OnDrawGizmos()
    {
        Vector2 dir = transform.position + (transform.up.normalized * range);
        Gizmos.DrawWireSphere(dir,area);
    }
}
