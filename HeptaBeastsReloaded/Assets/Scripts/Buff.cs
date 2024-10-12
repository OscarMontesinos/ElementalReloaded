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
    float spdThreshold = 15;
    public GameObject particleFx;

    public void NormalSetUp(PjBase user, PjBase target, Stats statsToChange,float duration, GameObject particleFx)
    {
        this.user = user;
        this.target = target;
        this.time = duration;
        this.statsToChange = statsToChange;

        target.stats.strength += this.statsToChange.strength;
        target.stats.sinergy += this.statsToChange.sinergy;
        target.stats.control += this.statsToChange.control;
        target.stats.atSpd += this.statsToChange.atSpd;
        target.stats.cdr += this.statsToChange.cdr;
        target.stats.fResist += this.statsToChange.fResist;
        target.stats.mResist += this.statsToChange.mResist;

        if (this.statsToChange.spd > 0)
        {
            if (this.statsToChange.spd <= user.stats.control / spdThreshold)
            {
                target.stats.spd += this.statsToChange.spd;
            }
            else
            {
                this.statsToChange.spd = user.stats.control / spdThreshold;
                target.stats.spd += this.statsToChange.spd;
            }
        }
        else if (this.statsToChange.spd < 0)
        {
            if (this.statsToChange.spd <= user.stats.control / -spdThreshold)
            {
                target.stats.spd += this.statsToChange.spd;
            }
            else
            {
                this.statsToChange.spd = user.stats.control / -spdThreshold;
                target.stats.spd += this.statsToChange.spd;
            }
        }

        if (particleFx)
        {
            this.particleFx = Instantiate(particleFx, target.visuals.transform);
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
        if (particleFx)
        {
            Destroy(particleFx);
        }

        target.stats.strength -= statsToChange.strength;
        target.stats.sinergy -= statsToChange.sinergy;
        target.stats.control -= statsToChange.control;
        target.stats.atSpd -= statsToChange.atSpd;
        target.stats.cdr -= statsToChange.cdr;
        target.stats.fResist -= statsToChange.fResist;
        target.stats.mResist -= statsToChange.mResist;
        target.stats.spd -= statsToChange.spd;
        Destroy(this);
    }
}
