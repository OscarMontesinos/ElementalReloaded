using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    public bool cast = true;
    public bool lockPointer = true;
    public string bName;
    public string description;
    public Sprite sprite;
    public float cd;
    public HitData.Element element;
    public PjBase.AttackType type;
    public string anim;
    public GameObject moveObject;

    public virtual void Trigger()
    {

    }

}
