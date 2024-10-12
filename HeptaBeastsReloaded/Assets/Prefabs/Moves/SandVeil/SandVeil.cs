using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class SandVeil : Move
{
    public float duration;
    public Stats statsToChange;
    public float range;
    public float spd;
    public GameObject particleFx;

    [HideInInspector]
    public Buff currentBuff;


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
        if (this.statsToChange.spd != 0)
        {
            statsToChange.spd += user.stats.control / this.statsToChange.spd;
        }


        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(UtilsClass.GetMouseWorldPosition(), 6, GameManager.Instance.allyLayer);
        PjBase ally;
        PjBase target = null;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            ally = enemyColl.GetComponent<PjBase>();
            Vector2 dir = ally.transform.position - user.transform.position;

            if (dir.magnitude <= range && !Physics2D.Raycast(user.transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
            {
                if (target != null)
                {
                    Vector2 targetDir = target.transform.position - user.transform.position;
                    if (targetDir.magnitude > dir.magnitude)
                    {
                        target = ally;
                    }
                }
                else
                {
                    target = ally;
                }
            }
        }
        if (target)
        {
            if (currentBuff)
            {
                currentBuff.Die();
            }
            SandVeilProjectile projectile = Instantiate(moveObject, user.transform.position, user.transform.rotation).GetComponent<SandVeilProjectile>();
            projectile.NormalSetUp(user,element,type,0,spd,range);
            projectile.SetUp(target,statsToChange,duration, particleFx,this);
        }
        else
        {
            if(user.currentMove1 == this)
            {
                user.currentHab1Cd = 1.5f;
            }
            if(user.currentMove2 == this)
            {
                user.currentHab2Cd = 1.5f;
            }
            if(user.currentMove3 == this)
            {
                user.currentHab3Cd = 1.5f;
            }
        }
    }

}
