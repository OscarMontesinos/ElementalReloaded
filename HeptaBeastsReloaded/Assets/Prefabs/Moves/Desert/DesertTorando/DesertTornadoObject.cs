using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertTornadoObject : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    PjBase user;
    HitData.Element element;
    PjBase.AttackType type;
    float spd;
    float area;
    float duration;
    float dmgPerSecond;
    float tickCounter = 0.4f;
    float slow;
    public bool active;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetUp(PjBase user, HitData.Element element, PjBase.AttackType type,float spd,float area, float duration, float dmgPerSecond, float slow)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.spd = spd;
        this.area = area;
        this.duration = duration;
        this.dmgPerSecond = dmgPerSecond;
        this.slow = slow;
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
                tickCounter = 0.4f;
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

        }
            _rigidbody.velocity = transform.up.normalized * spd;
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
                enemy.GetComponent<TakeDamage>().TakeDamage(user, dmgPerSecond * 0.4f, element, type);

                Stats stats = new Stats();
                stats.spd = user.stats.control / -slow;
                enemy.gameObject.AddComponent<Buff>().NormalSetUp(user, enemy, stats, 0.4f, null);
            }
        }
    }

    void Die()
    {

        Destroy(gameObject);
    }
}
