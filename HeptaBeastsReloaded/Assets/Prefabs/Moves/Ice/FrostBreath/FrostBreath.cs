using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class FrostBreath : Move
{
    public float dmg;
    public float stunTime;
    public float angle;


    public override void Trigger()
    {
        base.Trigger();

        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        Instantiate(moveObject, user.transform.position, user.pointer.transform.rotation);

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(user.transform.position, range, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            Transform target = enemy.transform;
            Vector2 dir = target.position - user.transform.position;

            if (Vector3.Angle(user.pointer.transform.up, dir.normalized) < angle / 2 && !Physics2D.Raycast(user.transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
            {
                if (enemy.team != user.team)
                {
                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                    user.Stunn(enemy, stunTime);
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        Vector3 rightAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, -angle / 2);
        Vector3 leftAngle = GameManager.DirectionFromAngle(transform.eulerAngles.z, angle   / 2);

        Gizmos.DrawLine(transform.position, transform.position + leftAngle * range);
        Gizmos.DrawLine(transform.position, transform.position + rightAngle * range);
    }
}
