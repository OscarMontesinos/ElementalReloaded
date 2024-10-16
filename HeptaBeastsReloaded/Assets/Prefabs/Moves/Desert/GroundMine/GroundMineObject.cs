using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundMineObject : MonoBehaviour
{
    PjBase user;
    Move move;
    HitData.Element element;
    PjBase.AttackType type;
    float dmg;
    float stunTime;
    float area;
    float delay;
    public bool isActive;
    bool isActivated;
    public GameObject mineSprite;
    public GameObject activateSprite;
    public GameObject particleObject;
    public FieldOfView fov;

    private void Update()
    {
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit != null && unit.team != user.team)
            {
                float dist = (unit.transform.position - transform.position).magnitude;
                if (dist <= area)
                {
                    if (!unit.revealedByList.Contains(gameObject))
                    {
                        unit.revealedByList.Add(gameObject);
                    }
                }
                else if (unit.revealedByList.Contains(gameObject))
                {
                    unit.revealedByList.Remove(gameObject);
                }
            }
        }
    }

    public void SetUp(PjBase user, Move move, HitData.Element element, PjBase.AttackType type,float dmg,float area, float delay, float stunTime)
    {
        this.user = user;
        this.move = move;
        this.element = element;
        this.type = type;
        this.dmg = dmg;
        this.area = area;
        this.delay = delay;
        this.stunTime = stunTime;
        fov.viewDistance = area;
        if(user.team != 0)
{
            fov.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        if (user.team != 0)
        {
            mineSprite.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit") && collision.GetComponent<PjBase>().team != user.team && isActive && !isActivated)
        {
            StartCoroutine(Tick());
        }
    }

    IEnumerator Tick()
    {
        if(move == null)
        {
            Die();
        }

        if(user.currentMove1 == move)
        {
            user.currentHab1Cd = user.CDR(move.cd);
        }
        else if (user.currentMove2 == move)
        {
            user.currentHab2Cd = user.CDR(move.cd);
        }
        else
        {
            user.currentHab3Cd = user.CDR(move.cd);
        }
        

            mineSprite.SetActive(false);
        activateSprite.SetActive(true);
        yield return new WaitForSeconds(delay);

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg , element, type);
                user.Stunn(enemy,stunTime);
            }
        }

        particleObject.SetActive(true);
        Die();
    }

    void Die()
    {
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit != null && unit.revealedByList.Contains(gameObject))
            {
                unit.revealedByList.Remove(gameObject);

            }
        }


        particleObject.transform.parent = null;
        Destroy(gameObject);
    }
}
