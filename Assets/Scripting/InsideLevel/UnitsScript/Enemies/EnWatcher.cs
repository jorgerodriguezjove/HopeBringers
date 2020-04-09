using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnWatcher : EnemyUnit
{
    //Debuff de ataque que mete a las unidades que están dentro de su alcance
    public int attackDebuff;

    //Debuff de movimient que mete a las unidades que están dentro de su alcance en el Tier 2
    public int movementDebuff;

    public override void SearchingObjectivesToAttack()
    {
        myCurrentObjective = null;
        myCurrentObjectiveTile = null;
        pathToObjective.Clear();

        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }
        
        //Comprobar las unidades que hay en mi rango de acción
        unitsInRange = LM.TM.GetAllUnitsInRangeWithoutPathfinding(rangeOfAction, GetComponent<UnitBase>());

        //Si hay personajes del jugador en mi rango de acción paso a attacking donde me alerto y hago mi accion
        for (int i = 0; i < unitsInRange.Count; i++)
        {
            if (unitsInRange[i].GetComponent<PlayerUnit>())
            {
                myCurrentEnemyState = enemyState.Attacking;
                return;
            }
        }

        //Si llega hasta aqui significa que no había personajes en rango y termina
        myCurrentEnemyState = enemyState.Ended;
    }

    public override void Attack()
    {
        hasAttacked = true;
        //Realizar acción de debuff de daño

        for (int i = 0; i < unitsInRange.Count; i++)
        {
            if (unitsInRange[i].GetComponent<PlayerUnit>())
            {
                ApplyBuffOrDebuffdamage(unitsInRange[i], attackDebuff, 3);
               
                if (myTierLevel == TierLevel.Level2)
                {
                    unitsInRange[i].GetComponent<PlayerUnit>().movementUds = movementUds - movementDebuff;
                }
            }
        }
        myCurrentEnemyState = enemyState.Ended;
    }
}
