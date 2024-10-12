using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int lvl;
    public float mHp;
    [HideInInspector]
    public float hp;
    public float healthRegen;
    [HideInInspector]
    public float shield;

    
    public float strength;
    public float sinergy;
    public float atSpd;
    public float control;
    [HideInInspector]
    public float cdr;

    public float fResist;
    public float mResist;

    public float spd;
}
