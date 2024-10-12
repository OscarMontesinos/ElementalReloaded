using System;
using System.Collections;
using System.Collections.Generic;
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
    PjBase user;
    public List<GameObject> availableBasicMoves;
    public List<GameObject> availableMoves;

    [HideInInspector]
    public List<PjBase> enemiesOnSight;
    [HideInInspector]
    public PjBase targetLocked;
    [HideInInspector]
    public Vector2 targetLastPosition;

    public float basicRange;
    public float move1Range;
    public float move2Range;
    public float move3Range;
    private void Awake()
    {
        user = GetComponent<PjBase>();

        user.moveBasic = availableBasicMoves[Random.Range(0, availableBasicMoves.Count)];
        user.move1 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move1);
        user.move2 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move2);
        user.move3 = availableMoves[Random.Range(0, availableMoves.Count)];
        availableMoves.Remove(user.move3);

        user.MoveSetUp();
        SetUpRanges();
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

    public List<string> GetAvailableMoves()
    {
        List<string> moves = new List<string>();

        if(user.currentBasicCd <= 0)
        {
            moves.Add(user.currentMoveBasic.mName);
        }
        if(user.currentHab1Cd <= 0)
        {
            moves.Add(user.currentMove1.mName);
        }
        if(user.currentHab2Cd <= 0)
        {
            moves.Add(user.currentMove2.mName);
        }
        if(user.currentHab3Cd <= 0)
        {
            moves.Add(user.currentMove3.mName);
        }

        return moves;
    }

    public virtual void SetUpRanges()
    {

    }

}

