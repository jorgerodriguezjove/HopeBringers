using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGiant : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    // Copia de la lista del goblin que en este caso uso para que la acción del gigante solo aparezca cuando hay players a su alrededor
    [HideInInspector]
    private List<UnitBase> unitsInRange = new List<UnitBase>();

    public override void SearchingObjectivesToAttack()
    {
        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        if (!haveIBeenAlerted)
        {
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

        else
        {

            //Determinamos el enemigo más cercano.
            currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());

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
    }

  

    public override void Attack()
    {
        //Si no he sido alertado, activo mi estado de alerta.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
            myCurrentEnemyState = enemyState.Searching;
            return;
        }

        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            //Si mi objetivo es adyacente a mi le ataco

            if (myCurrentTile.neighbours[i].unitOnTile != null && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0])
            {
                //Las comprobaciones para atacar arriba y abajo son iguales. Salvo por la dirección en la que tiene que girar el gigante
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

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    }
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

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                }

                hasAttacked = true;
                myAnimator.SetTrigger("Attack");
                //Me pongo en waiting porque al salir del for va a entrar en la corrutina abajo
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

    public override void MoveUnit()
    {
        pathToObjective.Clear();

        //ShowActionPathFinding(true);
        movementParticle.SetActive(true);

        //CAMBIAR ESTO (lm.tm)
        LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);
        pathToObjective = LM.TM.currentPath;

        ShowActionPathFinding(false);

        //Como solo se mueve un tile no hay que hacer ninguna comprobacion
        currentTileVectorToMove = pathToObjective[1].transform.position;
        MovementLogic(pathToObjective[1]);
    }

    //Lógica actual del movimiento. Básicamente es el encargado de mover al modelo y setear las cosas
    private void MovementLogic(IndividualTiles tileToMove)
    {
        //Muevo al gigante
        transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

        StartCoroutine("MovementWait");

        movementParticle.SetActive(false);
        CheckTileDirection(tileToMove);
        myCurrentEnemyState = enemyState.Searching;

        //Actualizo las variables de los tiles
        UpdateInformationAfterMovement(tileToMove);

        //Aviso de que se ha movido
        hasMoved = true;
    }

    IEnumerator MovementWait()
    {
        yield return new WaitForSeconds(timeWaitAfterMovement);
        HideActionPathfinding();
        //ShowActionPathFinding(false);
    }

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

    //HACER LO MISMO QUE EN GOBLIN Y QUITAR QUE RECALCULE CUANDO ES EL TURNO ENEMIGO. MIRAR BIEN QUE HACE EXACTAMENTE CUANDO HAY QUE DESPINTAR PARA PONER EN FUNCION HIDEACTIONHOVER.
    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //Si se tiene que mostrar la acción por el hover calculamos el enemigo
        if ( _shouldRecalculate)
        {
            SearchingObjectivesToAttackShowActionPathFinding();
            Debug.Log(myCurrentObjectiveTile);
            if (myCurrentObjectiveTile != null)
            {
                //Cada enemigo realiza su propio path
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);
                pathToObjective = LM.TM.currentPath;
            }
        }

        if (myCurrentObjectiveTile != null)
        {
            myLineRenderer.enabled = true;

            if (!_shouldRecalculate)
            {
                ColorAttackTile();
            }

            if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
            {
                shaderHover.SetActive(true);
            }
        }

            //else
            //{
            //    myLineRenderer.enabled = false;
            //    shaderHover.SetActive(false);


            //   myCurrentObjectiveTile.ColorDesAttack();

            //    if ( myCurrentObjectiveTile != null)
            //    {
            //        if (myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
            //        {
            //            if (myCurrentObjectiveTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
            //            {
            //                myCurrentObjectiveTile.tilesInLineRight[0].ColorDesAttack();
            //            }


            //            if (myCurrentObjectiveTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
            //            {
            //                myCurrentObjectiveTile.tilesInLineLeft[0].ColorDesAttack();
            //            }
                        
                        
            //        }
            //        else
            //        {
            //            if (myCurrentObjectiveTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
            //            {
            //                myCurrentObjectiveTile.tilesInLineUp[0].ColorDesAttack();
            //            }

            //            if (myCurrentObjectiveTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
            //            {
            //                myCurrentObjectiveTile.tilesInLineDown[0].ColorDesAttack();
            //            }

            //        }

            //    }
            //}
            
            myLineRenderer.positionCount = 2;


            if (LM.TM.currentPath.Count > 2)
            {
                Vector3 iniPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

                myLineRenderer.SetPosition(0, iniPosition);

                Vector3 pointPosition = new Vector3(LM.TM.currentPath[1].transform.position.x, LM.TM.currentPath[1].transform.position.y + 0.5f, LM.TM.currentPath[1].transform.position.z);
                myLineRenderer.SetPosition(1, pointPosition);

                Vector3 spawnPoint = new Vector3(LM.TM.currentPath[1].transform.position.x, LM.TM.currentPath[1].transform.position.y + 0.25f, LM.TM.currentPath[1].transform.position.z);
                shaderHover.transform.position = spawnPoint;
                Vector3 unitDirection = new Vector3(LM.TM.currentPath[2].transform.position.x, LM.TM.currentPath[1].transform.position.y + 0.25f, LM.TM.currentPath[2].transform.position.z);

                shaderHover.transform.DORotate(unitDirection, 0f);
            }

            //else
            //{
            //    myLineRenderer.enabled = false;
            //    shaderHover.SetActive(false);
            //}
        
       
    }

    //Se llama desde el LevelManager. Al final del showAction se encarga de mostrar el tile al que va a atacar
    public override void ColorAttackTile()
    {
        if (pathToObjective.Count <= movementUds && myCurrentObjectiveTile != null)
        {
            //Compruebo si tile al que voy a atacar estaba bajo ataque
            wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.isUnderAttack);

            tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile);

            myCurrentObjectiveTile.ColorAttack();

            if (myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
            {
                //Hago lo mismo con los tiles laterales, compruebo si hay que pintarlos en rojo y de ser así lo guarto en la lista de bools

                if (myCurrentObjectiveTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0]);

                    myCurrentObjectiveTile.tilesInLineRight[0].ColorAttack();
                }

                if (myCurrentObjectiveTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0]);

                    myCurrentObjectiveTile.tilesInLineLeft[0].ColorAttack();
                }
            }
            //Izquierda o derecha
            else
            {
                //Comprobar si a sus lados hay unidades
                if (myCurrentObjectiveTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0]);

                    myCurrentObjectiveTile.tilesInLineUp[0].ColorAttack();
                }

                if (myCurrentObjectiveTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineDown[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineDown[0]);

                    myCurrentObjectiveTile.tilesInLineDown[0].ColorAttack();
                }
            }
        }
    }


    bool keepSearching;

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
        //Si no ha sido alertado compruebo si hay players al alcance que van a hacer que se despierte y se mueva
        if (!haveIBeenAlerted)
        {
            //Comprobar las unidades que hay en mi rango de acción
            unitsInRange = LM.TM.GetAllUnitsInRangeWithoutPathfinding(rangeOfAction, GetComponent<UnitBase>());

            for (int i = 0; i < unitsInRange.Count; i++)
            {
                if (unitsInRange[i].GetComponent<PlayerUnit>())
                {
                    keepSearching = true;
                    Debug.Log(unitsInRange[i]);
                    currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
                    break;
                }
            }
        }

        //Si ha sido alertado compruebo simplemente hacia donde se va a mover
        else
        {
            //Determinamos el enemigo más cercano.
            //currentUnitsAvailableToAttack = LM.TM.OnlyCheckClosestPathToPlayer();
            currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
            //Debug.Log("Line 435 " + currentUnitsAvailableToAttack.Count);

            keepSearching = true;
        }


        if (keepSearching)
        {

            //Determinamos el enemigo más cercano.
            currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());

            if (currentUnitsAvailableToAttack.Count == 1)
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

        keepSearching = false;
    }
}
