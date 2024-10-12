using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildWind : Move
{
    public float reducedCdTime;
    public float duration;
    public Stats statsToChange;


    public override void Trigger()
    {
        base.Trigger();

        Stats statsToChange = new Stats();
        statsToChange.strength += user.CalculateControl(this.statsToChange.strength);
        statsToChange.sinergy += user.CalculateControl(this.statsToChange.sinergy);
        statsToChange.control += user.CalculateControl(this.statsToChange.control);
        statsToChange.atSpd += user.CalculateControl(this.statsToChange.atSpd);
        statsToChange.cdr += user.CalculateControl(this.statsToChange.cdr);
        statsToChange.fResist += user.CalculateControl(this.statsToChange.fResist);
        statsToChange.mResist += user.CalculateControl(this.statsToChange.mResist);
        statsToChange.spd += user.stats.control/ this.statsToChange.spd;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, range, GameManager.Instance.unitLayer);
        PjBase ally;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            ally = enemyColl.GetComponent<PjBase>();
            if (ally.team == user.team)
            {
                ally.gameObject.AddComponent<Buff>().NormalSetUp(user, ally, statsToChange, duration, null);
                ally.currentHab1Cd -= reducedCdTime;
                ally.currentHab2Cd -= reducedCdTime;
                ally.currentHab3Cd -= reducedCdTime;
            }
        }

        Instantiate(moveObject, user.transform.position, new Quaternion(0, 0, 0, 0));

    }
}
