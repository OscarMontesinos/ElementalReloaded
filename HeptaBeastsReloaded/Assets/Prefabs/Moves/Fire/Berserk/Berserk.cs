using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserk : Move
{
    public float duration;
    public Stats statsToChange;


    public override void Trigger()
    {
        base.Trigger();

        Stats statsToChange = new Stats();

        statsToChange.healthRegen += (this.statsToChange.healthRegen / user.stats.mHp)*100;
        statsToChange.strength += user.CalculateControl(this.statsToChange.strength);
        statsToChange.sinergy += user.CalculateControl(this.statsToChange.sinergy);
        statsToChange.control += user.CalculateControl(this.statsToChange.control);
        statsToChange.atSpd += user.stats.atSpd * this.statsToChange.atSpd;
        statsToChange.cdr += user.CalculateControl(this.statsToChange.cdr);
        statsToChange.fResist += user.CalculateControl(this.statsToChange.fResist);
        statsToChange.mResist += user.CalculateControl(this.statsToChange.mResist);
        if (this.statsToChange.spd != 0)
        {
            statsToChange.spd += user.stats.control / statsToChange.spd;
        }

        user.gameObject.AddComponent<Buff>().NormalSetUp(user,user,statsToChange,duration,moveObject,element);
    }   


}
