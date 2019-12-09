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
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    //Lista que guarda los enmeigos a los que el goblin puede alertar en tier 2
    [HideInInspector]
    private List<EnemyUnit> enemiesInRange = new List<EnemyUnit>();

    public override void SearchingObjectivesToAttack()
    {
        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(rangeOfAction, gameObject);

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
        if (myTierLevel == TierLevel.Level2)
        {
            if (!haveIBeenAlerted)
            {
                //Le pido al TileManager los enemigos dentro de mi rango
                enemiesInRange = LM.TM.GetAllEnemiesInRange(rangeOfAction, GetComponent<UnitBase>());

                //Alerto a los enemigos a mi alcance
                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    enemiesInRange[i].AlertEnemy();
                }

                //Me alerto a mi mismo
                AlertEnemy();
                return;
            }
        }


        //Si no he sido alertado, activo mi estado de alerta.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
        }
        

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
                hasAttacked = true;

                //Me pongo en waiting porque al salir del for va a entrar en la corrutina abajo.
                //myCurrentEnemyState = enemyState.Waiting;
                break;
            }
        }

        if (!hasMoved && !hasAttacked)
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

    //IEnumerator AttackWait()
    //{
    //    yield return new WaitForSeconds(timeWaitAfterAttack);
    //    myCurrentEnemyState = enemyState.Ended;
    //}

    int limitantNumberOfTilesToMove;

    public override void MoveUnit()
    {
        limitantNumberOfTilesToMove = 0;

        movementParticle.SetActive(true);

        //CAMBIAR ESTO 
        LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);
        pathToObjective = LM.TM.currentPath;

        //Como el path guarda el tile en el que esta el enemigo yel tile en el que esta el personaje del jugador resto 2.
        //Si esta resta se pasa del número de unidades que me puedo mover entonces solo voy a recorrer el número de tiles máximo que puedo llegar.
        if (pathToObjective.Count-2 > movementUds)
        {
            limitantNumberOfTilesToMove = movementUds;
        }

        //Si esta resta por el contrario es menor o igual a movementUds significa que me voy mover el máximo o menos tiles.
        else
        {
            limitantNumberOfTilesToMove = pathToObjective.Count-2;
        }

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(pathToObjective[0]);

        myCurrentEnemyState = enemyState.Waiting;
        
        //Actualizo info de los tiles
        UpdateInformationAfterMovement(pathToObjective[limitantNumberOfTilesToMove]);

        StartCoroutine("MovingUnitAnimation");
    }

    IEnumerator MovingUnitAnimation()
    {
        //Animación de movimiento
        //Es -1 ya que no me interesa que se mueva hasta el tile en el que está la otra unidad
        for (int j = 1; j <= limitantNumberOfTilesToMove; j++)
        {
            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = pathToObjective[j].transform.position;  //new Vector3(pathToObjective[j].transform.position.x, pathToObjective[j].transform.position.y, pathToObjective[j].transform.position.z);

            //Muevo y roto a la unidad
            transform.DOMove(currentTileVectorToMove, timeMovementAnimation);
            unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation, AxisConstraint.Y);

            //Espera entre casillas
            yield return new WaitForSeconds(timeMovementAnimation);
        }

        //Espero después de moverme para que no vaya demasiado rápido
        yield return new WaitForSeconds(timeWaitAfterMovement);
        hasMoved = true;
        myCurrentEnemyState = enemyState.Searching;

        movementParticle.SetActive(false);

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

    //Función que se encarga de hacer que el personaje este despierto/alerta
    public override void ShowActionPathFinding()
    {
        SearchingObjectivesToAttackShowActionPathFinding();

        if (currentUnitsAvailableToAttack.Count > 0)
        {
            //Cada enemigo realiza su propio path
            LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);

            //Mirar porque no va esto
            myLineRenderer.SetVertexCount(LM.TM.currentPath.Count);

            for (int i = 0; i < LM.TM.currentPath.Count; i++)
            {
                myLineRenderer.SetPosition(i, LM.TM.currentPath[i].transform.position);

            }
            //pathToObjective = LM.TM.currentPath;
        }
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
        
        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(rangeOfAction, gameObject);

        //Si no hay enemigos termina su turno
        if (currentUnitsAvailableToAttack.Count == 0)
        {
            
        }

        else if (currentUnitsAvailableToAttack.Count == 1)
        {
            myCurentObjective = currentUnitsAvailableToAttack[0];
            myCurrentObjectiveTile = myCurentObjective.myCurrentTile;
           
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

           
        }
    }

}
