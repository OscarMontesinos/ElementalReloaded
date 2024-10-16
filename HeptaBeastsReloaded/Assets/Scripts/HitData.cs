using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitData
{
    public enum Element
    {
        water, desert, crystal, lightning, wind, ice, nature, fire, none
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
