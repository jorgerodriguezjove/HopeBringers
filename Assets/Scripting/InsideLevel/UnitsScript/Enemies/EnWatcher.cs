using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnWatcher : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurrentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    //Lista que guarda los enmeigos y personajes que están dentro del rango de alerta del personaje (ya sea para comprobar personajes o alertar a enemigos)
    [HideInInspector]
    private List<UnitBase> unitsInRange = new List<UnitBase>();

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

        //if(unitsInRange.Count > 0)
        //{
        //    for (int i = 0; i < unitsInRange.Count; i++)
        //    {
        //        if (unitsInRange[i].GetComponent<PlayerUnit>())
        //        {
        //            unitsInRange[i].BuffbonusStateDamage -= attackDebuff;
                    
        //        }
        //    }
        //}
        
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

        if (!hasAttacked)
        {
            myCurrentEnemyState = enemyState.Moving;
        }

        else
        {
            myCurrentEnemyState = enemyState.Ended;

            //Espero 1 sec y cambio de estado a ended
            //StartCoroutine("AttackWait");
        }
    }


}
