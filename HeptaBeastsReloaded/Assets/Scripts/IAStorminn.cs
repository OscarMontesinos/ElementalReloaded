using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class IAStorminn : IABase
{
    
    private void Start()
    {
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
                while (agent.remainingDistance > agentAcceptanceRadius)
                {
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


                    switch (randomMove.moveSlot)
                    {
                        case 0:
                            StartCoroutine(user.MainAttack());
                            break;
                        case 1:
                            StartCoroutine(user.Hab1());
                            break;
                        case 2:
                            StartCoroutine(user.Hab2());
                            break;
                        case 3:
                            StartCoroutine(user.Hab3());
                            break;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
            else if(enemiesOnSight.Count == 0)
            {
                SetDestination(targetLastPosition);
                while (!enemiesOnSight.Contains(targetLocked) && agent.remainingDistance > agentAcceptanceRadius)
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


}
