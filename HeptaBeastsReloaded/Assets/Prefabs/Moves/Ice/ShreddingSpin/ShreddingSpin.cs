using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ShreddingSpin : Move
{
    public float dmg;
    public int ticks;
    public ParticleSystem fx;
    public Stats statsToChange;
    public float debuffDuration;


    public override void Trigger()
    {
        base.Trigger();

        StartCoroutine(Spin());
       
    }

    IEnumerator Spin()
    {
        yield return new WaitForSeconds(0.15f);
        moveObject.SetActive(true);
        float dmg;

        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        fx.Play();

        int times = ticks;
        while (times > 0)
        {
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, range, GameManager.Instance.unitLayer);
            PjBase enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<PjBase>();
                if (enemy.team != user.team)
                {
                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg/ticks, element, type);
                    Debuff(enemy);
                }
            }
            yield return new WaitForSeconds(0.5f / ticks);
            times--;
        }
        moveObject.SetActive(false);

    }

    void Debuff(PjBase target)
    {
        Stats statsToChange = new Stats();
        statsToChange.strength += user.CalculateControl(this.statsToChange.strength);
        statsToChange.sinergy += user.CalculateControl(this.statsToChange.sinergy);
        statsToChange.control += user.CalculateControl(this.statsToChange.control);
        statsToChange.atSpd += user.CalculateControl(this.statsToChange.atSpd);
        statsToChange.cdr += user.CalculateControl(this.statsToChange.cdr);
        statsToChange.fResist += user.CalculateControl(this.statsToChange.fResist);
        statsToChange.mResist += user.CalculateControl(this.statsToChange.mResist);
        if (this.statsToChange.spd != 0)
        {
            statsToChange.spd += user.stats.control / this.statsToChange.spd;
        }

        Buff currentBuff = target.gameObject.AddComponent<Buff>();
        currentBuff.NormalSetUp(user, target, statsToChange, debuffDuration,null, HitData.Element.ice);
    }
}
