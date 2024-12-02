using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class IceShardShield : Move
{
    public float dmg;
    public float amount;
    public float duration;
    Shield shield;
    bool triggered;

    public override void Trigger()
    {
        base.Trigger();

        triggered = false;

        shield = user.AddComponent<Shield>();

        shield.ShieldSetUp(user, user, user.CalculateControl(amount), duration, null);

        StartCoroutine(BurstCorroutine());
    }
    public override void OnStun()
    {
        base.OnStun();
        if (!triggered && shield.shieldAmount > 0)
        {
            Burst();
        }
    }

    IEnumerator BurstCorroutine()
    {
        while(shield != null)
        {
            if(shield.shieldAmount > 0)
            {
                StopCoroutine(BurstCorroutine());
            }
            yield return null;
        }

        if (!triggered)
        {
            Burst();
        }
        
    }

    void Burst()
    {
        triggered = true;
        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        Instantiate(moveObject, user.transform.position, transform.rotation);

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, range, GameManager.Instance.unitLayer);
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

   
}
