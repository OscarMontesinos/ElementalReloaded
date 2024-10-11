using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TakeDamage
{
    void TakeDamage(PjBase user,float value, HitData.Element element, PjBase.AttackType type);
    void Stunn(float stunnTime);
    void Die(PjBase killer);
}
