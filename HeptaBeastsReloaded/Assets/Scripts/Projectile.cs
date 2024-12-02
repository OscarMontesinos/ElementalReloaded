using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public PjBase target;
    bool lookAtTarget = true;
    [HideInInspector]
    public HitData.Element element;
    [HideInInspector]
    public PjBase.AttackType type;
    [HideInInspector]
    public float dmg;
    [HideInInspector]
    public float stunTime;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float spdOverTime;
    [HideInInspector]
    public float range;
    [HideInInspector]
    public  Rigidbody2D _rigidbody;
    [HideInInspector]
    public Vector2 startPos;
    public bool pierce;
    public bool collideWalls;
    public GameObject particle;
    public bool withoutRange;
    [HideInInspector]
    public float dist = 0;

    public List<PjBase> targetsAffected = new List<PjBase>();
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }
    public virtual void NormalSetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg, float speed, float range, float stunTime)
    {
        this.stunTime = stunTime;
        NormalSetUp(user, element, type, dmg, speed, range);
    }
    public virtual void NormalSetUp(PjBase user, HitData.Element element, PjBase.AttackType type, float dmg, float speed, float range)
    {
        this.user = user;
        this.element = element;
        this.type = type;
        this.dmg = dmg; 
        this.speed = speed;
        this.range = range;
    }

    public virtual void Update()
    {
        speed += spdOverTime * Time.deltaTime;

        if (target && lookAtTarget)
        {
            Vector2 dir = target.transform.position - transform.position;
            transform.up = dir;
        }
    }
    public virtual void FixedUpdate()
    {
        Vector2 dir = transform.up;
        _rigidbody.velocity = dir.normalized * speed;
        if (!withoutRange)
        {
            dist += speed * Time.deltaTime;
            if (dist > range)
            {
                Die();
            }
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Unit") && collision.GetComponent<PjBase>().team != user.team && !targetsAffected.Contains( collision.GetComponent<PjBase>()))
        {
            PjBase target = collision.GetComponent<PjBase>();
            target.GetComponent<TakeDamage>().TakeDamage(user, dmg, element, type);
            targetsAffected.Add(target);
            if (stunTime > 0)
            {
                user.Stunn(target, stunTime);
            }
            if (!pierce)
            {
                Die();
            }
        }
        else if(collideWalls && collision.CompareTag("Wall"))
        {
            Die();
        }
    }

    public void StopTrackingTarget()
    {
        lookAtTarget = false;
    }

    public virtual void Die()
    {
        if(particle != null)
        {
            particle.transform.parent = null;
        }
        Destroy(gameObject);
    }
}
