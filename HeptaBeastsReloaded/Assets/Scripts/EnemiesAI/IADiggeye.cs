using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;

public class IADiggeye : IABase
{
    
    public override void Start()
    {
        base.Start();
        StartCoroutine(IA());
    }
   

    public override IEnumerator ChooseAttack(MoveInfo move)
    {
        yield return null;
        if (user.stunTime <= 0)
        {
            PointTo(targetLocked);
            switch (move.moveName)
            {
                case "Slash":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 1.5f));
                    UseAttack(move);
                    break;
                case "Blunt Blow":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 1.5f));
                    UseAttack(move);
                    break;
                case "Piercing Strike":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 2));
                    UseAttack(move);
                    break;
                case "Ground Mine":
                    GameObject target = GetRandomWaypoint();
                    SetDestination(target.transform.position);
                    while (GetRemainingDistance(GetAttack(move).range))
                    {
                        SetDestination(target.transform.position);
                        PointTo(target);
                        yield return null;
                    }
                    PointTo(target);
                    yield return null;
                    UseAttack(move);
                    break;
                case "Underground Ambush":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    while (user.dashing)
                    {
                        yield return null;
                    }
                    PointTo(targetLocked);
                    yield return null;
                    UseAttack(0);
                    break;
                case "Wind Slash":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range -1));
                    UseAttack(move);
                    while (user.dashing)
                    {
                        yield return null;
                    }
                    PointTo(targetLocked);
                    yield return null;
                    UseAttack(0);
                    break;
                
            }
        yield return base.ChooseAttack(move);
        }
    }

}
