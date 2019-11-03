using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnGiant : EnemyUnit
{
    public override void SearchingObjectivesToAttack()
    {
        Debug.Log("search");
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(range, this);

        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            Debug.Log(currentUnitsAvailableToAttack[i].name);
        }
    }

    public override void MoveUnit()
    {
        base.MoveUnit();
    }

    public override void Attack()
    {
        base.Attack();
    }




}
