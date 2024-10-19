using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcyGlide : Move
{
    public float spd;
    public float dmg;
    public float area;

    public GameObject areaShower;

    bool firstUsage = true;
    float recastTime;
    public override void Trigger()
    {
        base.Trigger();

        StartCoroutine(TriggerCorroutine());

        if (firstUsage)
        {
            firstUsage = false;
            recastTime = 5;

            if (user.currentMove1 == this)
            {
                user.currentHab1Cd = 0;
            }
            else if (user.currentMove2 == this)
            {
                user.currentHab2Cd = 0;
            }
            else
            {
                user.currentHab3Cd = 0;
            }
        }
        else
        {
            firstUsage=true;
        }
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
        particle.transform.rotation = user.pointer.transform.rotation;

        StartCoroutine(user.Dash(user.pointer.transform.up, spd, range));

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
                    targetsAffected.Add(enemy);
                }
            }
            yield return null;
        }

        areaShower.SetActive(false);

        user.AnimationCallStopAnim();
        particle.transform.parent = null;
    }

    private void Update()
    {
        if(recastTime > 0)
        {
            recastTime -= Time.deltaTime;
        }
        else if(!firstUsage)
        {
            firstUsage = true;

            if (user.currentMove1 == this)
            {
                user.currentHab1Cd = user.CDR(cd);
            }
            else if (user.currentMove2 == this)
            {
                user.currentHab2Cd = user.CDR(cd);
            }
            else
            {
                user.currentHab3Cd = user.CDR(cd);
            }
        }
    }
}
