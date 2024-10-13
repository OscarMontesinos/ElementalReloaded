using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandVeilProjectile : Projectile
{
    SandVeil move;
    GameObject particleFx;
    Stats statsToChange;
    float duration;
    public override void NormalSetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg, float speed, float range)
    {
        base.NormalSetUp(user, element, type, dmg, speed, range);

    }

    public void SetUp(PjBase target, Stats statsToChange, float duration, GameObject particleFx, SandVeil move)
    {
        this.target = target;
        this.statsToChange = statsToChange;
        this.duration = duration;
        this.particleFx = particleFx;
        this.move = move;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit") && collision.GetComponent<PjBase>() == target && !targetsAffected.Contains(collision.GetComponent<PjBase>()))
        {
            move.currentBuff = collision.gameObject.AddComponent<Buff>();
            move.currentBuff.NormalSetUp(user, collision.GetComponent<PjBase>(), statsToChange, duration, particleFx);
            Die();
        }
        else if (collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }


}
