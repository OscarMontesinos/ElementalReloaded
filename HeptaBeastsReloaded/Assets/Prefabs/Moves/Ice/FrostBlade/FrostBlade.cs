using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class FrostBlade : Move
{
    public float dmg;

    Animator animator;

    public List<PjBase> targetsAffected;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public override void Trigger()
    {
        base.Trigger();


        animator.Play("FrostBlade"); 
        transform.up = user.cursor.transform.position - user.transform.position;
        targetsAffected.Clear();

    }
    private void Update()
    {
    }

    public void Dmg()
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

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(moveObject.transform.position, range, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if(enemy.team != user.team && !targetsAffected.Contains(enemy))
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
                targetsAffected.Add(enemy);
            }

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(moveObject.transform.position, range);
    }
}
