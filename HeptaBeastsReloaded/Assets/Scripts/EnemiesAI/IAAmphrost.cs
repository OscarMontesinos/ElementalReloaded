using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;

public class IAAmphrost : IABase
{
    
    public override void Start()
    {
        base.Start();
        StartCoroutine(IA());
    }

    public override IEnumerator ChooseAttack(MoveInfo move)
    {
        yield return null;
        if (user.stunTime <= 0)
        {
            PointTo(targetLocked);
            switch (move.moveName)
            {
                case "Slash":
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range - 1));
                    UseAttack(move);
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
                    yield return StartCoroutine(GetOnRange(GetAttack(move).range- 0.5f));
                    UseAttack(move);
                    break;
            }
        yield return base.ChooseAttack(move);
        }
    }

}
