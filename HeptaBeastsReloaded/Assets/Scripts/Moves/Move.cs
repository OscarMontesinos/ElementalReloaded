using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    public bool cursorAffecter;
    public bool cast = true;
    public bool lockPointer = true;
    public bool resetAutoattack;
    public string mName;
    public string description;
    public Sprite sprite;
    public float cd;
    public HitData.Element element;
    public PjBase.AttackType type;
    public string anim;
    public GameObject moveObject;
    public float range;

    public virtual void Trigger()
    {
        if (resetAutoattack)
        {
            user.currentBasicCd = 0;
        }
    }
    public virtual void EndAnim()
    {
        
    }

    public virtual void OnStun()
    {

    }

}
