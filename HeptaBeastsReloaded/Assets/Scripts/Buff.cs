using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public PjBase target;
    public float time;
    [HideInInspector]
    public bool untimed;
    public Stats statsToChange;

    public void NormalSetUp(PjBase user, Stats statsToChange,float duration)
    {
        this.user = user;
        this.time = duration;
        this.statsToChange = statsToChange;

        user.stats.strength += this.statsToChange.strength;
        user.stats.sinergy += this.statsToChange.sinergy;
        user.stats.control += this.statsToChange.control;
        user.stats.atSpd += this.statsToChange.atSpd;
        user.stats.cdr += this.statsToChange.cdr;
        user.stats.fResist += this.statsToChange.fResist;
        user.stats.mResist += this.statsToChange.mResist;
        if (this.statsToChange.spd <= user.stats.control / 10)
        {
            user.stats.spd += this.statsToChange.spd;
        }
        else
        {
            this.statsToChange.spd = user.stats.control / 10;
            user.stats.spd += this.statsToChange.spd;
        }

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!untimed)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        user.stats.strength -= statsToChange.strength;
        user.stats.sinergy -= statsToChange.sinergy;
        user.stats.control -= statsToChange.control;
        user.stats.atSpd -= statsToChange.atSpd;
        user.stats.cdr -= statsToChange.cdr;
        user.stats.fResist -= statsToChange.fResist;
        user.stats.mResist -= statsToChange.mResist;
        user.stats.spd -= statsToChange.spd;
        Destroy(this);
    }
}
