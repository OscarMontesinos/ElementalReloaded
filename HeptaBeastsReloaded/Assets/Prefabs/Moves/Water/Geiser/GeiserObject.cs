using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeiserObject : MonoBehaviour
{
    PjBase user;
    HitData.Element element;
    PjBase.AttackType type;
    float dmg;
    float area;
    float delay;
    public GameObject particleObject;

    public void SetUp(PjBase user, HitData.Element element, PjBase.AttackType type,float dmg,float area, float delay)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.dmg = dmg;
        this.area = area;
        this.delay = delay;
    }

    private void Update()
    {

            if (delay > 0)
            {
                delay -= Time.deltaTime;
            }
            else
            {
                Tick();
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
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg , element, type);
            }
        }

        particleObject.SetActive(true);
        Die();
    }

    void Die()
    {
        particleObject.transform.parent = null;
        Destroy(gameObject);
    }
}
