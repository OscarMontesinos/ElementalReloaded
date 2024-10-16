using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stampede : Move
{
    public float spd;
    public float dmg;
    public float stunTime;
    public float area;

    public GameObject areaShower;
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

        StartCoroutine(user.Dash(user.pointer.transform.up, spd, range,false,false));

        while (!user.dashing)
        {
            yield return null;
        }

        areaShower.SetActive(true);
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
                    user.Stunn(enemy,stunTime);
                    targetsAffected.Add(enemy);
                }
            }
            yield return null;
        }

        areaShower.SetActive(false);

        user.AnimationCallStopAnim();
        particle.transform.parent = null;
    }
}
