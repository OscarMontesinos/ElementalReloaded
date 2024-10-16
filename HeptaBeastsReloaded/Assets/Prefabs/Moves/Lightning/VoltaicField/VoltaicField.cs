using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoltaicField : Move
{
    public float area;
    public float dmg;
    public float duration;
    public float shield;
    public float shieldPerHit;

    float actualDuration;
    float tick = 0.5f;

    Shield currentShield;

    public override void Trigger()
    {
        base.Trigger();

        currentShield = user.gameObject.AddComponent<Shield>();
        currentShield.ShieldSetUp(user, user, user.CalculateControl(shield), duration, null);
        moveObject.SetActive(true);
        moveObject.transform.position = user.transform.position;
        actualDuration = duration;
        tick = 0.5f;
    }

    void Tick()
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

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, area, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                currentShield.ChangeShieldAmount(user.CalculateControl(shieldPerHit));
            }
        }


    }

    private void Update()
    {
        if (actualDuration > 0)
        {
            if(tick <= 0)
            {
                tick = 0.5f;
                Tick();
            }
            tick -= Time.deltaTime;
            actualDuration -= Time.deltaTime;
        }
        else
        {
            moveObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
