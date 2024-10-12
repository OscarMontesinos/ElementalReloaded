using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IAStorminn : IABase
{
    
    private void Start()
    {
        StartCoroutine(IA());
    }
    IEnumerator IA()
    {
        CheckTargetsOnSight();
        GetClosestEnemy();

        if (targetLocked)
        {

        }

        yield return null;
        StartCoroutine(IA());
    }

    public override void SetUpRanges()
    {
        base.SetUpRanges();
    }

}
