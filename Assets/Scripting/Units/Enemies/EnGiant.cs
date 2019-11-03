using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnGiant : EnemyUnit
{
    public override void SearchingObjectivesToAttack()
    {
        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(range, this);

        //Si no hay enemigos termina su turno
        if (currentUnitsAvailableToAttack.Count == 0)
        {
            myCurrentEnemyState = enemyState.Ended;
        }

        //Si hay varios enemigos a la misma distancia, se queda con el que tenga más unidades adyacentes
        else if (currentUnitsAvailableToAttack.Count > 1)
        {
            //Ordeno la lista de posibles objetivos según el número de unidades dyacentes
            currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
            {
                return (b.myCurrentTile.neighboursOcuppied).CompareTo(a.myCurrentTile.neighboursOcuppied);
            });

            //Elimino a todos los objetivos de la lista que no tengan el mayor número de enemigos adyacentes
            for (int i = currentUnitsAvailableToAttack.Count-1; i > 0; i--)
            {
                if (currentUnitsAvailableToAttack[0].myCurrentTile.neighboursOcuppied > currentUnitsAvailableToAttack[i].myCurrentTile.neighboursOcuppied)
                {
                    currentUnitsAvailableToAttack.RemoveAt(i);
                }
            }

            //Si sigue habiendo varios enemigos los ordeno segun la vida
            if (currentUnitsAvailableToAttack.Count > 1)
            {
                //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                {
                    return (a.currentHealth).CompareTo(b.currentHealth);

                });
            }
        }

        //Compruebo si puedo atacar al objetivo
        //Si no me muevo hacia él
        else if (currentUnitsAvailableToAttack.Count == 1)
        {
            myCurrentEnemyState = enemyState.Attacking;
        }

        //Si no hay objetivo disponible termina su turno
        else
        {
            myCurrentEnemyState = enemyState.Ended;
        }
    }

    public override void Attack()
    {
        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            if (myCurrentTile.neighbours[i].unitOnTile != null && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0])
            {
                //Ataque
                break;
            }

            else
            {
                if (!hasMoved)
                {
                    myCurrentEnemyState = enemyState.Moving;
                }

                else
                {
                    myCurrentEnemyState = enemyState.Ended;
                }
            }
        }
    }

    public override void MoveUnit()
    {
        //Movimiento
    }

}
