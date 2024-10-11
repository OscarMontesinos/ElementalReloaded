using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    public float time;
    [HideInInspector]
    public bool untimed;
    public List<PjBase> targets = new List<PjBase>();

    // Update is called once per frame
    public virtual void Update()
    {
        if (!untimed)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Die();
            }
        }
    }
    public virtual void SpellEnter(PjBase target)
    {

    }
    public virtual void SpellExit(PjBase target)
    {

    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
