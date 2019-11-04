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

        else if (currentUnitsAvailableToAttack.Count == 1)
        {
            myCurrentEnemyState = enemyState.Attacking;
        }

        //Si hay varios enemigos a la misma distancia, se queda con el que tenga más unidades adyacentes
        else if (currentUnitsAvailableToAttack.Count > 1)
        {
            //Ordeno la lista de posibles objetivos según el número de unidades dyacentes
            currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
            {
                return (b.myCurrentTile.neighboursOcuppied).CompareTo(a.myCurrentTile.neighboursOcuppied);
            });
            Debug.Log(currentUnitsAvailableToAttack[0]);

            //Elimino a todos los objetivos de la lista que no tengan el mayor número de enemigos adyacentes
            for (int i = currentUnitsAvailableToAttack.Count-1; i > 0; i--)
            {
                if (currentUnitsAvailableToAttack[0].myCurrentTile.neighboursOcuppied > currentUnitsAvailableToAttack[i].myCurrentTile.neighboursOcuppied)
                {
                    currentUnitsAvailableToAttack.RemoveAt(i);
                }
            }

            Debug.Log(currentUnitsAvailableToAttack[0]);

            //Si sigue habiendo varios enemigos los ordeno segun la vida
            if (currentUnitsAvailableToAttack.Count > 1)
            {
                
                //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                {
                    return (a.currentHealth).CompareTo(b.currentHealth);

                });   
            }
            Debug.Log(currentUnitsAvailableToAttack[0]);

            myCurrentEnemyState = enemyState.Attacking;
        }
    }

    public override void Attack()
    {
        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            //Si mi objetivo es adyacente a mi le ataco
            if (myCurrentTile.neighbours[i].unitOnTile != null && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0])
            {
                //Las comprobaciones para atacar arriba y abajo son iguales
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX == myCurrentTile.tileX)
                {
                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    }
                }
                //Izquierda o derecha
                else
                { 
                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                }

                myCurrentEnemyState = enemyState.Ended;
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
        myCurrentEnemyState = enemyState.Ended;

        //Comprueba la dirección en la que se encuentra el objetivo.
        //Si se encuentra justo en el mismo eje (movimiento torre), el gigante avanza en esa dirección.
        //Si se encuentra un bloqueo se queda en el sitio intentando avanzar contra el bloqueo.

        //Sin embargo si el objetivo se encuentra en diágonal (por ejemplo arriba a la derecha)
        //El gigante tiene que decidir una de las dos (DISEÑO REGLAS DE PATHFINDING)
        //Una vez decidida avanza en esta dirección hasta que no pueda más y si sigue estando en diagonal, avanza en la que había descartado antes.

        //Movimiento
        Debug.Log("move");
    }

}
