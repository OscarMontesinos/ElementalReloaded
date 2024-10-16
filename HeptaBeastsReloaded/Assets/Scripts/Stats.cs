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
    public float control;

    public float fResist;
    public float mResist;

    public float spd;

    public float atSpd;
    public float cdr;
}
