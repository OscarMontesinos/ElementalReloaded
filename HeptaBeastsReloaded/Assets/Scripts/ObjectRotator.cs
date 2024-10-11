using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float spd;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + (spd * Time.deltaTime));
    }
}
