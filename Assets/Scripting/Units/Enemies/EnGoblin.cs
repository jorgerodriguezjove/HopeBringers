using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGoblin : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    public override void SearchingObjectivesToAttack()
    {
        currentUnitsAvailableToAttack.Clear();

        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(range, gameObject);

        //Si no hay enemigos termina su turno
        if (currentUnitsAvailableToAttack.Count == 0)
        {
            myCurrentEnemyState = enemyState.Ended;
        }

        else if (currentUnitsAvailableToAttack.Count == 1)
        {
            myCurentObjective = currentUnitsAvailableToAttack[0];
            myCurrentObjectiveTile = myCurentObjective.myCurrentTile;
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

            //Elimino a todos los objetivos de la lista que no tengan el mayor número de enemigos adyacentes
            for (int i = currentUnitsAvailableToAttack.Count - 1; i > 0; i--)
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

            myCurentObjective = currentUnitsAvailableToAttack[0];
            myCurrentObjectiveTile = myCurentObjective.myCurrentTile;

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
                //Las comprobaciones para atacar arriba y abajo son iguales. Salvo por la dirección en la que tiene que girar el goblin
                if (myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
                {
                    //Arriba
                    if (myCurrentObjectiveTile.tileZ > myCurrentTile.tileZ)
                    {
                        RotateLogic(FacingDirection.North);
                    }
                    //Abajo
                    else
                    {
                        RotateLogic(FacingDirection.South);
                    }

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);
                }
                //Izquierda o derecha
                else
                {
                    //Arriba
                    if (myCurrentObjectiveTile.tileX > myCurrentTile.tileX)
                    {
                        RotateLogic(FacingDirection.East);
                    }
                    //Abajo
                    else
                    {
                        RotateLogic(FacingDirection.West);
                    }

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);
                }

                //Animación de ataque
                myAnimator.SetTrigger("Attack");

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
        movementParticle.SetActive(true);

        //CAMBIAR ESTO 
        LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);
        pathToObjective = LM.TM.currentPath;

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(pathToObjective[0]);

        myCurrentEnemyState = enemyState.Waiting;
        movementParticle.SetActive(false);

        myCurrentTile.unitOnTile = null;
        myCurrentTile = pathToObjective[pathToObjective.Count-2];
        myCurrentTile.unitOnTile = this;

        StartCoroutine("MovingUnitAnimation");
    }

    IEnumerator MovingUnitAnimation()
    {
        //Animación de movimiento
        //Es -1 ya que no me interesa que se mueva hasta el tile en el que está la otra unidad
        for (int j = 1; j < pathToObjective.Count-1; j++)
        {
            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = new Vector3(pathToObjective[j].transform.position.x, pathToObjective[j].transform.position.y, pathToObjective[j].transform.position.z);

            //Muevo y roto a la unidad
            transform.DOMove(currentTileVectorToMove, timeMovementAnimation);
            unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation);

            //Espera entre casillas
            yield return new WaitForSeconds(timeMovementAnimation);
        }

        hasMoved = true;

        myCurrentEnemyState = enemyState.Searching;

    }

    //MEJORAR ESTO. PROBABLEMENTE NO NECESITO DOS FUNCIONES  PARA ESTO Y ADEMÁS SE REPITE EN EL PLAYER UNIT

    //Decidir rotación al moverse por los tiles.
    public void CheckTileDirection(IndividualTiles tileToCheck)
    {
        //Arriba o abajo
        if (tileToCheck.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (tileToCheck.tileZ > myCurrentTile.tileZ)
            {
                unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
            //Abajo
            else
            {
                unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileToCheck.tileX > myCurrentTile.tileX)
            {
                unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
            //Izquierda
            else
            {
                unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
        }
    }

    //Decidir rotación al terminar de moverse para atacar
    private void RotateLogic(FacingDirection newDirection)
    {
        //Roto al gigante
        if (newDirection == FacingDirection.North)
        {
            unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.North;
        }

        else if (newDirection == FacingDirection.South)
        {
            unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.South;
        }

        else if (newDirection == FacingDirection.East)
        {
            unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.East;
        }

        else if (newDirection == FacingDirection.West)
        {
            unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
            currentFacingDirection = FacingDirection.West;
        }
    }

}
