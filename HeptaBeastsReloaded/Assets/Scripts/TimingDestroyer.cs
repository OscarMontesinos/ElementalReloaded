using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingDestroyer : MonoBehaviour
{
    public float time;

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
