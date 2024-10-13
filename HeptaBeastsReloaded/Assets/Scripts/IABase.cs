using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;

public class IABase : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public float agentAcceptanceRadius = 1.5f;
    public List<GameObject> availableBasicMoves;
    public List<GameObject> availableMoves;

    [HideInInspector]
    public List<PjBase> enemiesOnSight;
    [HideInInspector]
    public List<PjBase> allies;
    [HideInInspector]
    public PjBase targetLocked;
    [HideInInspector]
    public Vector2 targetLastPosition;
    [HideInInspector]
    public float averageRange;
    private void Awake()
    {
        user = GetComponent<PjBase>();
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(transform.position);
        agent.speed = user.stats.spd;
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        user.moveBasic = availableBasicMoves[Random.Range(0, availableBasicMoves.Count)];
        user.move1 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move1);
        user.move2 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move2);
        user.move3 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move3);

        user.MoveSetUp();

        averageRange = (user.currentMoveBasic.range + user.currentMove1.range + user.currentMove2.range + user.currentMove3.range) / 4;
    }

    public virtual void Start()
    {
        StartCoroutine(PostStart());
    }

    public virtual IEnumerator PostStart()
    {
        yield return null;
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit != null && unit != this && unit.team == user.team)
            {
                allies.Add(unit);
            }
        }
    }
    private void Update()
    {
        if (user.stunTime <= 0)
        {
            agent.speed = user.stats.spd;
        }
        else
        {
            agent.speed = 0;
        }
        if(enemiesOnSight.Contains(targetLocked))
        {
            if (targetLocked != null)
            {
                targetLastPosition = targetLocked.transform.position;
            }
        }
    }

    public void CheckTargetsOnSight()
    {
        enemiesOnSight.Clear();
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit != null && !unit.invisible)
            {
                if (unit.team != user.team)
                {
                    var dir = unit.transform.position - transform.position;
                    if (!Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
                    {
                        if (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.playerWallLayer))
                        {
                            Barrier barrier = Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.playerWallLayer).rigidbody.gameObject.GetComponent<Barrier>();
                            if (barrier.user.team != user.team && barrier.deniesVision && enemiesOnSight.Contains(unit))
                            {
                               
                            }
                            else if(!enemiesOnSight.Contains(unit))
                            {
                                enemiesOnSight.Add(unit);
                            }
                        }
                        else if(!enemiesOnSight.Contains(unit))
                        {

                            enemiesOnSight.Add(unit);
                        }
                    }
                    else if(enemiesOnSight.Contains(unit))
                    {

                       
                    }
                }
            }
        }
    }

    public void GetClosestEnemy()
    {
        targetLastPosition = new Vector2(1000, 1000);
        foreach(PjBase unit in enemiesOnSight)
        {
            if(unit != null)
            {
                Vector2 dir = unit.transform.position - transform.position;
                if (dir.magnitude < targetLastPosition.magnitude)
                {
                    targetLastPosition = dir;
                    targetLocked = unit;
                }
            }
        }
    }
    public struct MoveInfo
    {
        public string moveName;
        public int moveSlot;
    }

    public List<MoveInfo> GetAvailableMoves()
    {
        
        List<MoveInfo> moves = new List<MoveInfo>();

        if(user.currentBasicCd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMoveBasic.mName;
            info.moveSlot = 0;
            moves.Add(info);
        }
        if(user.currentHab1Cd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMove1.mName;
            info.moveSlot = 1;
            moves.Add(info);
        }
        if(user.currentHab2Cd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMove2.mName;
            info.moveSlot = 2;
            moves.Add(info);
        }
        if(user.currentHab3Cd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMove3.mName;
            info.moveSlot = 3;
            moves.Add(info);
        }

        return moves;
    }

    public float GetDistanceToTarget(PjBase target)
    {
        Vector2 targetDist = target.transform.position - transform.position;
        return targetDist.magnitude;
    }

    public void PointTo(PjBase target)
    {
        if (!user.lockPointer)
        {
            if (target != null)
            {
                Vector2 dir = target.transform.position - transform.position;
                user.pointer.transform.up = dir;
                user.cursor.transform.position = target.transform.position;
            }
        }
    }

    public void SetDestination(Vector2 pos)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(pos, out hit, 100, 1);
        agent.SetDestination(hit.position);
    }

    public Vector2 PivotPos()
    {
        int times = 15;
        Vector2 dest = transform.position + (targetLocked.transform.position - user.transform.position).normalized * -(agentAcceptanceRadius - 1);
        dest = new Vector2(dest.x + Random.Range(5, -5), dest.y + Random.Range(5, -5));

        Vector2 dir = targetLocked.transform.position - transform.position;
        while (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer) && times > 0)
        {
            dest = new Vector2(dest.x + Random.Range(5, -5), dest.y + Random.Range(5, -5));
            times--;
        }

        return dest;
    }

    public virtual IEnumerator ChooseAttack(MoveInfo randomMove)
    {
        yield return null;
    }

    public void UseAttack(MoveInfo move)
    {
        switch (move.moveSlot)
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

    public Move GetAttack(MoveInfo move)
    {
        switch (move.moveSlot)
        {
            case 0:
                return user.currentMoveBasic;
            case 1:
                return user.currentMove1;
            case 2:
                return user.currentMove2;
            case 3:
                return user.currentMove3;
            default:
                return null;
        }

    }
}

