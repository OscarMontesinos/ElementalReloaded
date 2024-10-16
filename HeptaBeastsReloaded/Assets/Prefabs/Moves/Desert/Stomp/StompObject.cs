using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompObject : MonoBehaviour
{
    PjBase user;
    HitData.Element element;
    PjBase.AttackType type;
    float dmg;
    public float area;
    List<PjBase> targetsAffected = new List<PjBase>();
    public bool active = true;
    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.unitLayer);
            PjBase enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<PjBase>();
                if (enemy.team != user.team && !targetsAffected.Contains(enemy))
                {
                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                    targetsAffected.Add(enemy);
                }
            }
        }

    }

    public void SetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.dmg = dmg;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }

}
