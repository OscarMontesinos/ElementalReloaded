using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjinnEyeObject : MonoBehaviour
{
    PjBase user;
    HitData.Element element;
    PjBase.AttackType type;
    public GameObject eye;
    public FieldOfView fov;
    float area;
    float duration;
    float dmgPerSecond;
    float tickCounter = 1;
    public bool active;


    public void SetUp(PjBase user, HitData.Element element, PjBase.AttackType type,float area, float maxArea,float minArea,float duration, float dmgPerSecond)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        if(area > maxArea)
        {
            area = maxArea;
        }
        else if (area < minArea)
        {
            area = minArea;
        }
        this.area = area;
        eye.transform.localScale = new Vector3(area, area, area);
        this.duration = duration;
        this.dmgPerSecond = dmgPerSecond;
        fov.viewDistance = area;
        if(user.team != 0)
        {
            Destroy(fov);
        }
    }

    private void Update()
    {
        if (active)
        {
            if (tickCounter > 0)
            {
                tickCounter -= Time.deltaTime;
            }
            else
            {
                tickCounter = 1;
                Tick();
            }

            if (duration > 0)
            {
                duration -= Time.deltaTime;
            }
            else
            {
                Die();
            }

            if (duration > 0)
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
        }
    }

    void Tick()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.unitLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            if (enemy.team != user.team)
            {
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmgPerSecond, element, type);
            }
        }
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

        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
