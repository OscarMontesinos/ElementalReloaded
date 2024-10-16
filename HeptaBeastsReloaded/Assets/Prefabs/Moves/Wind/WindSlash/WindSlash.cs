using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class WindSlash : Move
{
    public float spd;
    public float dmg;
    public float area;


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
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        GameObject particle = Instantiate(moveObject, user.transform);
        particle.transform.position = user.transform.position;

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
                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                    user.dashing = false;
                }
            }
            yield return null;
        }

        user.AnimationCallStopAnim();
        particle.transform.parent = null;
    }
}
