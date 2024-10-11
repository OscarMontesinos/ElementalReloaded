using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitData
{
    public enum Element
    {
        none,ice,fire,water,desert,wind,nature,lightning
    }

    public enum AttackType
    {
        range, melee, aoe, dot, pot, heal, passive
    }

    public enum HabType
    {
        physical, magical, status
    }
}
