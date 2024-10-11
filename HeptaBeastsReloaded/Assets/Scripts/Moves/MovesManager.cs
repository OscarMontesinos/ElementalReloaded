using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public static MovesManager instance;

    public PjBase testingUnit;

    public List<Move> desertPhysicalAttacks;
    public List<Move> desertMagicalAttacks;
    public List<Move> desertStatusAttacks;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
