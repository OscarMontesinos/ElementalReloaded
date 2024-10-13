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
    IEnumerator IA()
    {
        CheckTargetsOnSight();
        if (!targetLocked)
        {
            GetClosestEnemy();
        }

        if (targetLocked)
        {

            if(GetDistanceToTarget(targetLocked) > averageRange)
            {
                Vector2 dest = targetLocked.transform.position - ((targetLocked.transform.position - user.transform.position).normalized * (agentAcceptanceRadius - 1));
                SetDestination(dest);
                while (agent.remainingDistance > averageRange)
                {
                    dest = targetLocked.transform.position - ((targetLocked.transform.position - user.transform.position).normalized * (agentAcceptanceRadius - 1));
                    SetDestination(dest);
                    PointTo(targetLocked);
                    yield return null;
                }
                yield return new WaitForSeconds(1);
            }
            else
            {
                SetDestination(PivotPos());
                while (agent.remainingDistance > agentAcceptanceRadius)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(1);
            }

            CheckTargetsOnSight();
            if (enemiesOnSight.Contains(targetLocked))
            {
                List<MoveInfo> moveList = GetAvailableMoves();

                if (moveList.Count > 1)
                {
                    MoveInfo randomMove = moveList[Random.Range(0, moveList.Count)];

                    yield return StartCoroutine(ChooseAttack(randomMove));

                   
                }
                yield return new WaitForSeconds(0.5f);
            }
            else if(enemiesOnSight.Count == 0)
            {
                SetDestination(targetLastPosition);
                yield return null;
                while (targetLocked != null && !enemiesOnSight.Contains(targetLocked) && agent.remainingDistance > agentAcceptanceRadius)
                {
                    CheckTargetsOnSight();
                    yield return null;
                }
                if (!enemiesOnSight.Contains(targetLocked))
                {
                    targetLocked = null;
                }
            }

        }
        else
        {
            SetDestination(GameManager.Instance.waypoints[Random.Range(0, GameManager.Instance.waypoints.Count)].transform.position);
            while (agent.remainingDistance > agentAcceptanceRadius && enemiesOnSight.Count == 0)
            {
                CheckTargetsOnSight();
                yield return null;
            }
            if (enemiesOnSight.Count > 0)
            {
                SetDestination(transform.position);
            }
        }

        yield return null;
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
                    UseAttack(move);
                    break;
                case "Air Pulse":
                    UseAttack(move);
                    break;
                case "Cloud Burst":
                    UseAttack(move);
                    break;
                case "Desert Tornado":
                    UseAttack(move);
                    break;
                case "Djinn Eye":
                    UseAttack(move);
                    break;
                case "Wild Wind":
                    if ((user.currentHab1Cd > 0 && user.currentHab2Cd > 0) || (user.currentHab3Cd > 0 && user.currentHab2Cd > 0) || (user.currentHab1Cd > 0 && user.currentHab3Cd > 0))
                    {
                        UseAttack(move);
                    }
                    break;
                case "Wind Barrage":
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
                        while (agent.remainingDistance > GetAttack(move).range - 1)
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
