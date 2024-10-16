using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;

public class IAStorminn : IABase
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
                case "Desert Breeze":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Air Pulse":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Cloud Burst":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Desert Tornado":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 6));
                    UseAttack(move);
                    break;
                case "Djinn Eye":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range));
                    UseAttack(move);
                    break;
                case "Wild Wind":
                    if ((user.currentHab1Cd > 0 && user.currentHab2Cd > 0) || (user.currentHab3Cd > 0 && user.currentHab2Cd > 0) || (user.currentHab1Cd > 0 && user.currentHab3Cd > 0))
                    {
                        UseAttack(move);
                    }
                    break;
                case "Sand Blast":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Sand Veil":
                    if (!GetAttack(move).GetComponent<SandVeil>().currentBuff)
                    {
                        PjBase target = null;
                        foreach (PjBase ally in allies)
                        {
                            if (ally != null)
                            {
                                if (target != null)
                                {
                                    if (target.stats.hp < ally.stats.hp)
                                    {
                                        target = ally;
                                    }
                                }
                                else
                                {
                                    target = ally;
                                }
                            }
                            else
                            {
                                target = user;
                            }
                        }
                        SetDestination(target.transform.position);
                        while (GetRemainingDistance(GetAttack(move).range - 1))
                        {
                            SetDestination(target.transform.position);
                            PointTo(target);
                            yield return null;
                        }
                        PointTo(target);
                        yield return null;
                        UseAttack(move);
                    }
                    break;
            }
        yield return base.ChooseAttack(move);
        }
    }

}
