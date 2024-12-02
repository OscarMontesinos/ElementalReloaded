using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotatorX : MonoBehaviour
{
    public float spd;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + (spd * Time.deltaTime), transform.rotation.y, transform.rotation.z);
    }
}
