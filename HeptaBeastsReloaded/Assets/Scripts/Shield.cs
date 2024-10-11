using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    public float shieldAmount;

    public virtual float ChangeShieldAmount(float value)
    {
        if (value >= -shieldAmount)
        {
            if (value < 0)
            {
                user.RegisterDamage(-value);
            }
            shieldAmount += value;
            target.stats.shield += value;
            value = 0;
        }
        else
        {
            user.RegisterDamage(-value);
            value += shieldAmount;
            shieldAmount = 0;
            target.stats.shield += value;
        }

        if(target.stats.shield < 0)
        {
            target.stats.shield = 0;
        }

        return -value;
    }

    public override void Die()
    {
        user.RegisterDamage(-shieldAmount);
        ChangeShieldAmount(-shieldAmount);
        base.Die();
    }
}
