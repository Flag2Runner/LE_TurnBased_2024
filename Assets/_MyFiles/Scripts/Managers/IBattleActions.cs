using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IBattleActions
{
    void Attack(Character TargetToAttack);
    void Heal();

    void Gaurd();

    void Pass();
}