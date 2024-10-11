using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaShower : MonoBehaviour
{
    public float area;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
