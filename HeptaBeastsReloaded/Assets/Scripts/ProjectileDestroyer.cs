using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestroyer : MonoBehaviour
{
    Projectile projectile;
    // Start is called before the first frame update
    void Start()
    {
        projectile = transform.parent.GetComponent<Projectile>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (projectile != null)
        {
            if (collision.CompareTag("Wall"))
            {
                projectile.Die();

            }
        }
    }
}
