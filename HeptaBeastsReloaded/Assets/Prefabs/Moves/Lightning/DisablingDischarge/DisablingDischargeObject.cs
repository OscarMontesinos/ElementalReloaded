using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisablingDischargeObject : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    PjBase user;
    HitData.Element element;
    PjBase.AttackType type;
    float dmg;
    float area;
    float debufDuration;
    float delay;
    float slow;
    public GameObject particleObject;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetUp(PjBase user, HitData.Element element, PjBase.AttackType type,float dmg,float area, float delay, float slow, float debufDuration)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.dmg = dmg;
        this.area = area;
        this.delay = delay;
        this.slow = slow;
        this.debufDuration = debufDuration;
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

                Stats stats = new Stats();
                stats.spd = user.stats.control / -slow;
                enemy.gameObject.AddComponent<Buff>().NormalSetUp(user, enemy, stats, debufDuration, null);
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
