using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float spd;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, transform.eulerAngles.z + (spd * Time.deltaTime));
    }
}
