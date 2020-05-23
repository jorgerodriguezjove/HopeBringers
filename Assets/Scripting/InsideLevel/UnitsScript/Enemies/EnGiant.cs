 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGiant : EnemyUnit
{
    public override void SearchingObjectivesToAttack()
    {
        if (!amIBeingPossesed)
        {
            myCurrentObjective = null;
            myCurrentObjectiveTile = null;
            pathToObjective.Clear();

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

                //Si esta oculto lo quito de la lista de objetivos
                for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
                {
                    if (currentUnitsAvailableToAttack[i].isHidden)
                    {
                        currentUnitsAvailableToAttack.RemoveAt(i);
                        i--;
                    }
                }

                //Si no hay enemigos termina su turno
                if (currentUnitsAvailableToAttack.Count == 0)
                {
                    myCurrentEnemyState = enemyState.Ended;
                    return;
                }

                else if (currentUnitsAvailableToAttack.Count > 0)
                {
                    if (currentUnitsAvailableToAttack.Count == 1)
                    {
                        //Añado esto para eliminar a los personajes ocultos
                        base.SearchingObjectivesToAttack();
                        if (currentUnitsAvailableToAttack.Count == 1)
                        {
                            myCurrentObjective = currentUnitsAvailableToAttack[0];
                            myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
                        }
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
                            //Añado esto para eliminar a los personajes ocultos
                            base.SearchingObjectivesToAttack();

                            //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                            currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                            {
                                return (a.currentHealth).CompareTo(b.currentHealth);
                            });
                        }

                        myCurrentObjective = currentUnitsAvailableToAttack[0];
                        myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
                    }

                    //CAMBIAR ESTO (lm.tm)
                    LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

                    //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                    //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                    for (int i = 0; i < LM.TM.currentPath.Count; i++)
                    {
                        pathToObjective.Add(LM.TM.currentPath[i]);
                    }

                    myCurrentEnemyState = enemyState.Attacking;
                }
            }
        }

        else
        {
            myCurrentObjective = null;
            myCurrentObjectiveTile = null;
            pathToObjective.Clear();
            coneTiles.Clear();
            tilesToCheck.Clear();
            currentUnitsAvailableToAttack.Clear();

            if (isDead || attackCountThisTurn >= 2)
            {
                myCurrentEnemyState = enemyState.Ended;
                return;
            }

            else
            {
                if (attackCountThisTurn >= 2)
                {
                    if (!hasMoved)
                    {
                        myCurrentEnemyState = enemyState.Moving;
                        return;
                    }
                    else
                    {
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }
                }

                if (areaCharged)
                {
                    //Explotar área
                    Debug.Log("0.Area Explota");
                    DoAreaAttack();

                    areaCharged = false;
                    attackCountThisTurn++;

                    CallWaitCoroutine();
                    return;
                }

                //Como no puedo hacer traspaso, compruebo que ataques puedo hacer
                else
                {
                    if (CheckCono())
                    {
                        Debug.Log("1.Cono");
                        DoConeAttack();

                        coneUsed = true;
                        attackCountThisTurn++;

                        CallWaitCoroutine();
                        return;
                    }

                    //Si he usado el cono lo primero que compruebo es si puedo hacer el área
                    if (coneUsed)
                    {
                        if (CheckArea())
                        {
                            //Do area
                            Debug.Log("1.5. Área");
                            DoAreaAttack();

                            attackCountThisTurn++;

                            CallWaitCoroutine();
                            return;
                        }
                    }

                    //No se puede poner else porque puede no usar el cono y el área o no 
                    if (CheckNormal())
                    {
                        //Do físico
                        Debug.Log("2. Físico");
                        DoNormalAttack();

                        normalAttackUsed = true;
                        attackCountThisTurn++;

                        CallWaitCoroutine();
                        return;
                    }

                    if (CheckArea())
                    {
                        //Do Área
                        Debug.Log("2.5 Area");
                        DoAreaAttack();

                        attackCountThisTurn++;

                        CallWaitCoroutine();
                        return;
                    }

                    else if (normalAttackUsed)
                    {
                        //Do Stun
                        Debug.Log("3. Stun");
                        DoStunAttack();

                        attackCountThisTurn++;

                        CallWaitCoroutine();
                        return;
                    }

                    else if (!hasMoved)
                    {
                        Debug.Log("6. Movimiento");
                        currentUnitsAvailableToAttack.Clear();
                        //tilesToCheck.Clear();
                        //coneTiles.Clear();

                        ///Comprueba si se ha movido (si no, se mueve y repite todas las comprobaciones menos el traspaso)

                        //Determinamos el enemigo más cercano.
                        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());

                        //Si esta oculto lo quito de la lista de objetivos
                        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
                        {
                            if (currentUnitsAvailableToAttack[i].isHidden)
                            {
                                currentUnitsAvailableToAttack.RemoveAt(i);
                                i--;
                            }
                        }

                        //Si no hay enemigos termina su turno
                        if (currentUnitsAvailableToAttack.Count == 0)
                        {
                            myCurrentEnemyState = enemyState.Ended;
                            return;
                        }

                        else if (currentUnitsAvailableToAttack.Count > 0)
                        {
                            if (currentUnitsAvailableToAttack.Count == 1)
                            {
                                base.SearchingObjectivesToAttack();

                                if (currentUnitsAvailableToAttack.Count == 1)
                                {
                                    myCurrentObjective = currentUnitsAvailableToAttack[0];
                                    myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
                                }
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
                                    //Añado esto para eliminar a los personajes ocultos
                                    base.SearchingObjectivesToAttack();

                                    //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                                    currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                                    {
                                        return (a.currentHealth).CompareTo(b.currentHealth);

                                    });
                                }

                                myCurrentObjective = currentUnitsAvailableToAttack[0];
                                myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
                            }

                            //CAMBIAR ESTO (lm.tm)
                            LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

                            //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                            //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                            for (int i = 0; i < LM.TM.currentPath.Count; i++)
                            {
                                pathToObjective.Add(LM.TM.currentPath[i]);
                            }


                            myCurrentEnemyState = enemyState.Moving;
                            //myCurrentEnemyState = enemyState.Attacking;
                        }
                    }

                    else
                    {
                        Debug.Log("Ended with: " + attackCountThisTurn + " attackCountThisTurn");

                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }
                }
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

        //base.Attack();
        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            //Si mi objetivo es adyacente a mi le ataco

            if (myCurrentTile.neighbours[i].unitOnTile != null && 
                currentUnitsAvailableToAttack.Count > 0 &&
                myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0] && 
                Mathf.Abs(myCurrentTile.height - myCurrentTile.neighbours[i].height) <= maxHeightDifferenceToAttack)
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

                    ColorAttackTile();

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null && Mathf.Abs(myCurrentTile.height - myCurrentTile.tilesInLineRight[0].height) <= maxHeightDifferenceToAttack)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null && Mathf.Abs(myCurrentTile.height - myCurrentTile.tilesInLineLeft[0].height) <= maxHeightDifferenceToAttack)
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

                    ColorAttackTile();

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null && Mathf.Abs(myCurrentTile.height - myCurrentTile.tilesInLineUp[0].height) <= maxHeightDifferenceToAttack)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null && Mathf.Abs(myCurrentTile.height - myCurrentTile.tilesInLineDown[0].height) <= maxHeightDifferenceToAttack)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                }

                base.Attack();

                hasAttacked = true;
                ExecuteAnimationAttack();
                //Se tiene que poner en wait hasta que acabe la animación de ataque
                myCurrentEnemyState = enemyState.Waiting;

                break;
            }
        }

        if (!hasMoved && !hasAttacked)
        {
            myCurrentEnemyState = enemyState.Moving;
        }

        else if (hasMoved)
        {
            myCurrentEnemyState = enemyState.Ended;
        }

        Debug.Log("hasmoved " + hasMoved);
        Debug.Log("hasattacked " + hasAttacked);
    }

    //PARA MOVEUNIT SE USA LA BASE DEL ENEMIGO (Que es la lógica del goblin).
    //PASA LO MISMO CON ShowActionPathFinding(bool _shouldRecalculate) QUE MUESTRA LA ACCIÓN DEL ENEMIGO;

        //Quizás HAY QUE CAMBIAR ShowActionPathFinding PARA VER QUE HACER CON PINTAR ENEMIGOS ADYACENTES
        //O QUIZÁS ES CAMBIAR EL COLOR ATTACK TILE NO LO SE

    //Hit y tile en el que esta la sombra del show action
    RaycastHit hit;
    IndividualTiles shadowHoverTile;

    //Esta variable guarda el tile que se va a usar para tomar como referencia si el gigante puede atacar a varios enemigos y pintar sus tiles de rojo
    IndividualTiles tileToUseBetweenShadowOrMine;

    //Se llama desde el LevelManager. Al final del showAction se encarga de mostrar el tile al que va a atacar
    public override void ColorAttackTile()
    {
        shadowHoverTile = null;
        tileToUseBetweenShadowOrMine = null;

        //El +2 es porque pathToObjective tiene en cuenta tanto el tile inicial (ocupado por goblin) como el final (ocupado por player)
        if (pathToObjective.Count > 0 &&  pathToObjective.Count <= movementUds + 2 && myCurrentObjectiveTile != null)
        {
            //Compruebo si tile al que voy a atacar estaba bajo ataque
            base.ColorAttackTile();

            //Esto significa que el enemigo está adyacente del player (son el tile del gigante y el del player vamos)
            if (pathToObjective.Count > 2)
            {
                Debug.DrawRay(new Vector3(sombraHoverUnit.transform.position.x, sombraHoverUnit.transform.position.y + 0.5f, sombraHoverUnit.transform.position.z), transform.TransformDirection(Vector3.down), Color.yellow, 20f);

                if (Physics.Raycast(new Vector3(sombraHoverUnit.transform.position.x, sombraHoverUnit.transform.position.y + 0.5f, sombraHoverUnit.transform.position.z), transform.TransformDirection(Vector3.down), out hit))
                {
                    shadowHoverTile = hit.collider.gameObject.GetComponent<IndividualTiles>();
                }

                tileToUseBetweenShadowOrMine = shadowHoverTile;
            }

            else if (pathToObjective.Count == 2)
            {
                tileToUseBetweenShadowOrMine = myCurrentTile;
            }

            //Pinto tile izq y derecha
            if ((pathToObjective.Count > 2 && myCurrentObjectiveTile != null && shadowHoverTile != null && myCurrentObjectiveTile.tileX == shadowHoverTile.tileX)
                || pathToObjective.Count == 2 && myCurrentObjectiveTile != null && myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
            {
                //Hago lo mismo con los tiles laterales, los pinto de rojo y los guardo en la lista de bools

                //Tile lateral der
                wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0].isUnderAttack);
                tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0]);
                myCurrentObjectiveTile.tilesInLineRight[0].ColorAttack();

                //Tile lateral izq
                wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0].isUnderAttack);
                tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0]);
                myCurrentObjectiveTile.tilesInLineLeft[0].ColorAttack();
            }

            //O pinto tile arriba y debajo, depende de dirección
            else if (pathToObjective.Count > 2 && myCurrentObjectiveTile.tileZ == shadowHoverTile.tileZ || pathToObjective.Count == 2 && myCurrentObjectiveTile.tileZ == myCurrentTile.tileZ)
            {
                //Tile lateral arriba
                wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0].isUnderAttack);
                tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0]);
                myCurrentObjectiveTile.tilesInLineUp[0].ColorAttack();

                //Tile lateral abajo
                wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineDown[0].isUnderAttack);
                tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineDown[0]);
                myCurrentObjectiveTile.tilesInLineDown[0].ColorAttack();
            }
        }
    }

    [HideInInspector]
    public List<IndividualTiles> tempLateralTilesToFutureObjective = new List<IndividualTiles>();

    public void SaveLateralUnitsForNumberAttackInLevelManager()
    {
        tempLateralTilesToFutureObjective.Clear();

        if (pathToObjective.Count > 0)
        {
            tempLateralTilesToFutureObjective = pathToObjective[limitantNumberOfTilesToMove + 1].GetLateralTilesBasedOnDirection(CheckTileDirection(pathToObjective[limitantNumberOfTilesToMove], pathToObjective[limitantNumberOfTilesToMove + 1], false), 1);
        }
    }

    //Esta función se llama únicamente en el hover
    public void CalculateDamagePreviousAttackLateralEnemies(UnitBase unitAttacked)
    {
        CalculateDamagePreviousAttack(unitAttacked, this, pathToObjective[limitantNumberOfTilesToMove], CheckTileDirection(pathToObjective[limitantNumberOfTilesToMove], pathToObjective[limitantNumberOfTilesToMove + 1], false));
    }

    //Override a función encargada de pintar el line renderer + sombra y el número de daño.
    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        if (!amIBeingPossesed)
        {
            //Recalcular pathToObjective solo si es necesario
            if (_shouldRecalculate)
            {
                pathToObjective.Clear();

                SearchingObjectivesToAttackShowActionPathFinding();
                if (myCurrentObjectiveTile != null)
                {
                    //Cada enemigo realiza su propio path
                    LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

                    //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                    //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                    for (int i = 0; i < LM.TM.currentPath.Count; i++)
                    {
                        pathToObjective.Add(LM.TM.currentPath[i]);
                    }
                }
            }

            //Si se va a mostrar la acción en el turno enemigo entonces no recalculo y directamente enseño la acción.
            //Esta parte es común para cuando se hace desde el hover como cuando se hace en turno enemigo.
            if (myCurrentObjectiveTile != null)
            {
                myLineRenderer.positionCount = 0;

                if (pathToObjective.Count - 2 > movementUds)
                {
                    limitantNumberOfTilesToMove = movementUds;
                }
                else
                {
                    limitantNumberOfTilesToMove = pathToObjective.Count - 2;
                }

                myLineRenderer.enabled = true;

                if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && pathToObjective.Count > 2)
                {
                    sombraHoverUnit.SetActive(true);
                }

                myLineRenderer.positionCount += (limitantNumberOfTilesToMove + 1);


                for (int i = 0; i < limitantNumberOfTilesToMove + 1; i++)
                {
                    Vector3 pointPosition = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);
                    shadowTile = pathToObjective[i];
                    Debug.Log(shadowTile.name);

                    if (i < pathToObjective.Count - 1)
                    {
                        myLineRenderer.SetPosition(i, pointPosition);

                        if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                        {
                            sombraHoverUnit.transform.position = pointPosition;


                            if ((pathToObjective[limitantNumberOfTilesToMove + 1]) == currentUnitsAvailableToAttack[0].myCurrentTile)
                            {
                                Debug.Log(name + " " + currentUnitsAvailableToAttack[0].name);
                                CalculateDamagePreviousAttack(currentUnitsAvailableToAttack[0], this, pathToObjective[limitantNumberOfTilesToMove], CheckTileDirection(pathToObjective[limitantNumberOfTilesToMove], pathToObjective[limitantNumberOfTilesToMove + 1], false));
                            }
                            else
                            {
                                damageWithMultipliersApplied = -999;
                            }

                            Vector3 positionToLook = new Vector3(myCurrentObjective.transform.position.x, myCurrentObjective.transform.position.y + 0.5f, myCurrentObjective.transform.position.z);
                            sombraHoverUnit.transform.DOLookAt(positionToLook, 0, AxisConstraint.Y);
                        }
                    }
                }

                CheckTilesInRange(shadowTile, SpecialCheckRotation(shadowTile, false));
                Debug.Log(shadowTile.name);
                ///En el gigante es importante que esta función vaya después de colocar la sombra. Por si acaso asegurarse de que este if nunca se pone antes que el reposicionamiento de la sombra

                //A pesar de que ya se llama a esta función desde el levelManager en caso de hover, si se tiene que mostrar porque el goblin está atacando se tiene que llamar desde aqui (ya que no pasa por el level manager)
                //Tiene que ser en falso porque si no pongo la condicion la función se cree que el tileya estaba pintado de antes
                if (!_shouldRecalculate)
                {
                    ColorAttackTile();
                }
            }
        }
    }

    public override void CheckTilesInRange(IndividualTiles _referenceTile, FacingDirection _referenceDirection)
    {
        currentTilesInRangeForAttack.Clear();

        Debug.Log(_referenceTile);
        Debug.Log(_referenceDirection);

        if (_referenceDirection == FacingDirection.North)
        {
            if (attackRange <= _referenceTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineUp[i].isEmpty && !_referenceTile.tilesInLineUp[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineUp[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineUp[i]);

                    for (int j = 0; j < _referenceTile.tilesInLineUp[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1).Count; j++)
                    {
                        currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineUp[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1)[j]);
                    }
                }

                if (_referenceTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineUp[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        if (_referenceDirection == FacingDirection.South)
        {
            if (attackRange <= _referenceTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineDown[i].isEmpty && !_referenceTile.tilesInLineDown[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineDown[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineDown[i]);


                    for (int j = 0; j < _referenceTile.tilesInLineDown[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1).Count; j++)
                    {
                        currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineDown[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1)[j]);
                    }
                }

                if (_referenceTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineDown[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        if (_referenceDirection == FacingDirection.East)
        {
            if (attackRange <= _referenceTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineRight[i].isEmpty && !_referenceTile.tilesInLineRight[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineRight[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineRight[i]);

                    for (int j = 0; j < _referenceTile.tilesInLineRight[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1).Count; j++)
                    {
                        currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineRight[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1)[j]);
                        Debug.Log("Tile lateral " + _referenceTile.tilesInLineRight[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1)[j].name);
                    }
                }

                if (_referenceTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineRight[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        if (_referenceDirection == FacingDirection.West)
        {
            if (attackRange <= _referenceTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = _referenceTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (!_referenceTile.tilesInLineLeft[i].isEmpty && !_referenceTile.tilesInLineLeft[i].isObstacle && Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineLeft[i]);

                    for (int j = 0; j < _referenceTile.tilesInLineLeft[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1).Count; j++)
                    {
                        currentTilesInRangeForAttack.Add(_referenceTile.tilesInLineLeft[i].GetLateralTilesBasedOnDirection(_referenceDirection, 1)[j]);
                    }
                }

                if (_referenceTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack)
                {
                    break;
                }
            }
        }

        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
        if (!amIBeingPossesed)
        {

            myCurrentObjective = null;
            myCurrentObjectiveTile = null;

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

                //Si esta oculto lo quito de la lista de objetivos
                for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
                {
                    if (currentUnitsAvailableToAttack[i].isHidden)
                    {
                        currentUnitsAvailableToAttack.RemoveAt(i);
                        i--;
                    }
                }

                if (currentUnitsAvailableToAttack.Count == 1)
                {
                    myCurrentObjective = currentUnitsAvailableToAttack[0];
                    myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
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

                    myCurrentObjective = currentUnitsAvailableToAttack[0];
                    myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
                }
            }

            keepSearching = false;
        }
    }


    public override void FinishMyActions()
    {
        base.FinishMyActions();

        if (amIBeingPossesed)
        {
            attackCountThisTurn = 0;
            coneUsed = false;
            normalAttackUsed = false;
            areaUsed = false;
        }
    }
}
