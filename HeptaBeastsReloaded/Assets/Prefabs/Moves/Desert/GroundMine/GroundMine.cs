using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMine : Move
{
    public int setUpCd;
    public GameObject currentGroundMine;
    public float dmg;
    public float area;
    public float delay;
    public float stunTime;
    public override void Trigger()
    {
        base.Trigger();

        if (user.currentMove1 == this)
        {
            user.currentHab1Cd = user.CDR(setUpCd);
        }
        if (user.currentMove2 == this)
        {
            user.currentHab2Cd = user.CDR(setUpCd);
        }
        if (user.currentMove3 == this)
        {
            user.currentHab3Cd = user.CDR(setUpCd);
        }


            if (currentGroundMine != null)
        {
            Destroy(currentGroundMine);
        }

        float dmg;
        if (type == PjBase.AttackType.Physical)
        {
            dmg = user.CalculateStrength(this.dmg);
        }
        else
        {
            dmg = user.CalculateSinergy(this.dmg);
        }

        Vector3 destination = user.cursor.transform.position;
        Vector3 dist = destination - user.transform.position;
        if (dist.magnitude > range)
        {
            destination = user.transform.position + (dist.normalized * range);
        }

        GroundMineObject mine = Instantiate(moveObject, destination, transform.rotation).GetComponent<GroundMineObject>();
        mine.SetUp(user, this, element, type, dmg, area, delay, stunTime);
        currentGroundMine = mine.gameObject;
    }
}
