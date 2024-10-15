using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanPulseProjectile : Projectile
{
    public float sizeSpeed;
    public override void Update()
    {
        base.Update();

        transform.localScale = new Vector3(transform.localScale.x + (sizeSpeed*Time.deltaTime), transform.localScale.y + (sizeSpeed * Time.deltaTime), transform.localScale.z + (sizeSpeed * Time.deltaTime));
    }
}
