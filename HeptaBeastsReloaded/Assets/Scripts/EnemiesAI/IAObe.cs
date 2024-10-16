using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;

public class IAObe : IABase
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
                case "Blunt Blow":

                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 1.5f));
                    UseAttack(move);
                    break;
                case "Arching Spark":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Ocean Pulse":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Stomp":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Stampede":
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
                case "Electric Shot":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Disabling Discharge":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range));
                    UseAttack(move);
                    break;
                case "Water Wave":
                    SetDestination(targetLocked.transform.position);
                    while (GetRemainingDistance(GetAttack(move).range - 3))
                    {
                        SetDestination(targetLocked.transform.position);
                        PointTo(targetLocked);
                        yield return null;
                    }
                    PointTo(targetLocked);
                    yield return null;
                    UseAttack(move);
                    break;
                case "Geiser":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range));
                    UseAttack(move);
                    break;
                case "Amp Up":
                    if (!GetAttack(move).GetComponent<AmpUp>().currentBuff)
                    {
                        PjBase target2 = null;
                        foreach (PjBase ally in allies)
                        {
                            if (ally != null)
                            {
                                if (target2 != null)
                                {
                                    if (target2.stats.sinergy < ally.stats.sinergy)
                                    {
                                        target2 = ally;
                                    }
                                }
                                else
                                {
                                    target2 = ally;
                                }
                            }
                            else
                            {
                                target2 = user;
                            }
                        }
                        SetDestination(target2.transform.position);
                        while (GetRemainingDistance(GetAttack(move).range - 1) && target2 != null)
                        {
                            SetDestination(target2.transform.position);
                            PointTo(target2);
                            yield return null;
                        }
                        PointTo(target2);
                        yield return null;
                        UseAttack(move);
                    }
                    break;
                case "Sea Bless":
                    if (!GetAttack(move).GetComponent<SeaBless>().currentBuff)
                    {
                        PjBase target2 = null;
                        float target2Highest = 0;
                        foreach (PjBase ally in allies)
                        {
                            if (ally != null)
                            {
                                if (target2 != null)
                                {
                                    if (ally.stats.strength < ally.stats.sinergy)
                                    {
                                        if (target2Highest < ally.stats.sinergy)
                                        {
                                            target2 = ally;
                                            target2Highest = ally.stats.sinergy;
                                        }
                                    }
                                    else
                                    {
                                        if (target2Highest < ally.stats.strength)
                                        {
                                            target2 = ally;
                                            target2Highest = ally.stats.strength;
                                        }
                                    }
                                }
                                else
                                {
                                    if (ally.stats.strength < ally.stats.sinergy)
                                    {
                                        target2 = ally;
                                        target2Highest = ally.stats.sinergy;
                                    }
                                    else
                                    {
                                        target2 = ally;
                                        target2Highest = ally.stats.strength;
                                    }
                                }
                            }
                            else
                            {
                                target2 = user;
                            }
                        }
                        SetDestination(target2.transform.position);
                        while (GetRemainingDistance(GetAttack(move).range - 1) && target2 != null)
                        {
                            SetDestination(target2.transform.position);
                            PointTo(target2);
                            yield return null;
                        }
                        PointTo(target2);
                        yield return null;
                        UseAttack(move);
                    }
                    break;

            }
        yield return base.ChooseAttack(move);
        }
    }

}
