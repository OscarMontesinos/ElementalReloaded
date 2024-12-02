using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
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

    float waitingTime = 0.5f;
    float movePatience = 2;
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

        user.rb.mass = 1000;
    }

    public virtual void Start()
    {
        user.UIManager.gameObject.SetActive(false);
        StartCoroutine(PostStart());
        StartCoroutine(IA());
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
            agent.speed = user.stats.spd - 1;
        }
        else
        {
            agent.speed = 0;
        }


        if (enemiesOnSight.Contains(targetLocked))
        {
            if (targetLocked != null)
            {
                targetLastPosition = targetLocked.transform.position;
            }
        }

        if (user.walk && agent.isOnNavMesh && agent.remainingDistance > 0)
        {
            user.animator.SetFloat("FrontVelocity", 2);
        }
        else if (user.walk)
        {
            user.animator.SetFloat("FrontVelocity", 0);
        }

    }


    public virtual IEnumerator IA()
    {
        CheckTargetsOnSight();
        if (!targetLocked)
        {
            GetClosestEnemy();
        }

        if (targetLocked)
        {

            if (GetDistanceToTarget(targetLocked) > averageRange)
            {
                Vector2 dest = targetLocked.transform.position - ((targetLocked.transform.position - user.transform.position).normalized * (agentAcceptanceRadius - 1));
                SetDestination(dest);
                while (GetRemainingDistance(averageRange) && targetLocked != null)
                {
                    dest = targetLocked.transform.position - ((targetLocked.transform.position - user.transform.position).normalized * (agentAcceptanceRadius - 1));
                    SetDestination(dest);
                    PointTo(targetLocked);
                    yield return null;
                }
            }
            else
            {
                Vector2 dir = PivotPos();
                SetDestination(dir);
                while (GetRemainingDistance(averageRange) && targetLocked != null)
                {
                    SetDestination(dir);
                    yield return null;
                }
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
            }
            else if (enemiesOnSight.Count == 0)
            {
                SetDestination(targetLastPosition);
                yield return null;
                while (targetLocked != null && !enemiesOnSight.Contains(targetLocked) && GetRemainingDistance(agentAcceptanceRadius))
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
            while (GetRemainingDistance(agentAcceptanceRadius) && enemiesOnSight.Count == 0)
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

    public void CheckTargetsOnSight()
    {
        enemiesOnSight.Clear();
        foreach (PjBase unit in GameManager.Instance.pjList)
        {
            if (unit != null && !unit.invisible)
            {
                if (unit.team != user.team)
                {
                    if (unit.revealedByList.Count == 0)
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
                                else if (!enemiesOnSight.Contains(unit))
                                {
                                    enemiesOnSight.Add(unit);
                                }
                            }
                            else if (!enemiesOnSight.Contains(unit))
                            {

                                enemiesOnSight.Add(unit);
                            }
                        }
                        else if (enemiesOnSight.Contains(unit))
                        {


                        }
                    }
                    else
                    {
                        enemiesOnSight.Add(unit);
                    }
                }
            }
        }
    }

    public void GetClosestEnemy()
    {
        targetLastPosition = new Vector2(1000, 1000);
        foreach (PjBase unit in enemiesOnSight)
        {
            if (unit != null)
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

        if (user.currentBasicCd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMoveBasic.mName;
            info.moveSlot = 0;
            moves.Add(info);
        }
        if (user.currentHab1Cd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMove1.mName;
            info.moveSlot = 1;
            moves.Add(info);
        }
        if (user.currentHab2Cd <= 0)
        {
            MoveInfo info = new MoveInfo();
            info.moveName = user.currentMove2.mName;
            info.moveSlot = 2;
            moves.Add(info);
        }
        if (user.currentHab3Cd <= 0)
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
        if (target != null)
        {
            PointTo(target.gameObject);
        }
    }

    public void PointTo(GameObject target)
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

    public bool GetRemainingDistance(float range)
    {
        bool isInDistance = true;
        if (user != null && agent.isOnNavMesh)
        {
            if (agent.remainingDistance < range)
            {
                isInDistance = false;
            }
        }
        return isInDistance;
    }

    public void SetDestination(Vector2 pos)
    {
        if (user != null && agent.isOnNavMesh)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(pos, out hit, 100, 1);
            agent.SetDestination(hit.position);
            Debug.DrawLine(user.transform.position, hit.position);
        }
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

    public void UseAttack(MoveInfo move)
    {
        UseAttack(move.moveSlot);
    }
    public void UseAttack(int move)
    {
        switch (move)
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

    public GameObject GetRandomWaypoint()
    {
        return GameManager.Instance.waypoints[Random.Range(0, GameManager.Instance.waypoints.Count)];
    }


    public IEnumerator GetOnRange(float range)
    {
        if (targetLocked != null)
        {
            SetDestination(targetLocked.transform.position);
            movePatience = 2;
            while (targetLocked != null && GetRemainingDistance(range) && movePatience > 0)
            {
                movePatience -= Time.deltaTime;
                SetDestination(targetLocked.transform.position);
                PointTo(targetLocked);
                yield return null;
            }
            PointTo(targetLocked);
            yield return null;
        }
    }

    public virtual IEnumerator ChooseAttack(MoveInfo move)
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
                        PjBase target2 = null;
                        foreach (PjBase ally in allies)
                        {
                            if (ally != null)
                            {
                                if (target2 != null)
                                {
                                    if (target2.stats.hp < ally.stats.hp)
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
                        while (GetRemainingDistance(GetAttack(move).range - 1))
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
                case "Ice Claw":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
                    break;
                case "Frost Blade":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range + 1));
                    UseAttack(move);
                    break;
                case "Icy Glide":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range));
                    UseAttack(move);
                    break;
                case "Berserk":
                    UseAttack(move);
                    break;
                case "Ice Breaker":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 0.5f));
                    UseAttack(move);
                    break;
                case "Shredding Spin":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 0.5f));
                    UseAttack(move);
                    SetDestination(targetLocked.transform.position);
                    break;
                case "Ice Shard Shield":
                    UseAttack(move);
                    break;
                case "Water Shot":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 0.5f));
                    UseAttack(move);
                    break;
                case "Freezing Shot":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 0.5f));
                    UseAttack(move);
                    break;


            }
        }
    }
}

