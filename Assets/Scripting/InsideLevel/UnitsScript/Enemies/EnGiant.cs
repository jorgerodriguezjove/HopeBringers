using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGiant : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurrentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    // Copia de la lista del goblin que en este caso uso para que la acción del gigante solo aparezca cuando hay players a su alrededor
    [HideInInspector]
    private List<UnitBase> unitsInRange = new List<UnitBase>();

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

            else if (currentUnitsAvailableToAttack.Count > 0)
            {
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

                //CAMBIAR ESTO (lm.tm)
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);

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

            if (myCurrentTile.neighbours[i].unitOnTile != null && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0] && Mathf.Abs(myCurrentTile.height - myCurrentTile.neighbours[i].height) <= maxHeightDifferenceToAttack)
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
        //ShowActionPathFinding(true);
        movementParticle.SetActive(true);

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

   
    //Muestra la sombra y el line renderer
    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //Si se tiene que mostrar la acción por el hover calculamos el enemigo
        if ( _shouldRecalculate)
        {
            pathToObjective.Clear();

            //Si no es el turno enemigo (es decir hay que pintar la acción por hacer hover) calculo la unidad más cercana
            SearchingObjectivesToAttackShowActionPathFinding();
            if (myCurrentObjectiveTile != null)
            {
                //Cada enemigo realiza su propio path
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ);

                //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                for (int i = 0; i < LM.TM.currentPath.Count; i++)
                {
                    pathToObjective.Add(LM.TM.currentPath[i]);
                }
            }
        }

        if (myCurrentObjectiveTile != null)
        {
            myLineRenderer.enabled = true;

            //El 2 es porque son los tiles que tiene cuando es adyacente a un enemigo.
            if (pathToObjective.Count > 2)
            {
                if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                {
                    shaderHover.SetActive(true);
                }

                Vector3 iniPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

                myLineRenderer.SetPosition(0, iniPosition);

                Vector3 pointPosition = new Vector3(pathToObjective[1].transform.position.x, pathToObjective[1].transform.position.y + 0.5f, pathToObjective[1].transform.position.z);
                myLineRenderer.SetPosition(1, pointPosition);

                Vector3 spawnPoint = new Vector3(pathToObjective[1].transform.position.x, pathToObjective[1].transform.position.y + 0.25f, pathToObjective[1].transform.position.z);
                shaderHover.transform.position = spawnPoint;

                Vector3 unitDirection = new Vector3(pathToObjective[2].transform.position.x, pathToObjective[1].transform.position.y + 0.25f, pathToObjective[2].transform.position.z);

                shaderHover.transform.DOLookAt(unitDirection, 0f, AxisConstraint.Y);
            }
            else
            {
                myLineRenderer.enabled = false;
            }

            //IMPORTANTE: EN EL CASO DEL GIGANTE, EL COLOR SE TIENE QUE PINTAR DESPUÉS DE HABER MOVIDO A LA SOMBRA.
            //La sombra se usa de referencia para calcular los tiles que hay que pintar de rojo
            if (!_shouldRecalculate)
            {
                ColorAttackTile();
            }
        }
    }

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
            wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.isUnderAttack);

            tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile);

            myCurrentObjectiveTile.ColorAttack();

            //Esto significa que el enemigo está adyacente del player (son el tile del gigante y el del player vamos)
            if (pathToObjective.Count > 2)
            {
                Debug.DrawRay(new Vector3(shaderHover.transform.position.x, shaderHover.transform.position.y + 0.5f, shaderHover.transform.position.z), transform.TransformDirection(Vector3.down), Color.yellow, 20f);

                if (Physics.Raycast(new Vector3(shaderHover.transform.position.x, shaderHover.transform.position.y + 0.5f, shaderHover.transform.position.z), transform.TransformDirection(Vector3.down), out hit))
                {
                    shadowHoverTile = hit.collider.gameObject.GetComponent<IndividualTiles>();
                }

                tileToUseBetweenShadowOrMine = shadowHoverTile;
            }

            else if (pathToObjective.Count == 2)
            {
                tileToUseBetweenShadowOrMine = myCurrentTile;
            }

            if (pathToObjective.Count > 2 && myCurrentObjectiveTile.tileX == shadowHoverTile.tileX
                || pathToObjective.Count == 2 && myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
            {
                //Hago lo mismo con los tiles laterales, compruebo si hay que pintarlos en rojo y de ser así lo guarto en la lista de bools

                if (myCurrentObjectiveTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null && Mathf.Abs(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].height - tileToUseBetweenShadowOrMine.height) <= maxHeightDifferenceToAttack)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineRight[0]);

                    myCurrentObjectiveTile.tilesInLineRight[0].ColorAttack();
                }

                if (myCurrentObjectiveTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null && Mathf.Abs(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].height - tileToUseBetweenShadowOrMine.height) <= maxHeightDifferenceToAttack)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineLeft[0]);

                    myCurrentObjectiveTile.tilesInLineLeft[0].ColorAttack();
                }
            }

            else if (pathToObjective.Count > 2 && myCurrentObjectiveTile.tileZ == shadowHoverTile.tileZ || pathToObjective.Count == 2 && myCurrentObjectiveTile.tileZ == myCurrentTile.tileZ)
            {
                //Comprobar si a sus lados hay unidades
                if (myCurrentObjectiveTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null && Mathf.Abs(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].height - tileToUseBetweenShadowOrMine.height) <= maxHeightDifferenceToAttack)
                {
                    wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0].isUnderAttack);

                    tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.tilesInLineUp[0]);

                    myCurrentObjectiveTile.tilesInLineUp[0].ColorAttack();
                }

                if (myCurrentObjectiveTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null && Mathf.Abs(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].height - tileToUseBetweenShadowOrMine.height) <= maxHeightDifferenceToAttack)
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
