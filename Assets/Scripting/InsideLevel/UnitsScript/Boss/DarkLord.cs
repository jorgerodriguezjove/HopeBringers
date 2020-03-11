using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DarkLord : EnemyUnit
{
    [Header("TRASPASO DE ALMA")]
    [SerializeField]
    private int maxCooldownSoulsSkill;
    private int currentCooldownSoulSkill = 3;

    //Bool que indica cuál es el dark lord original (ya que los enemigos controlados usan este script también)
    [SerializeField]
    private bool amITheOriginalDarkLord;

    bool coneUsed;
    bool areaUsed;
    bool attackUsed;

    private int attackCountThisTurn;

    [Header("ÁREA")]
    bool areaCharged;

    [Header("CONO")]
    [SerializeField]
    int coneRange = 5;

    [Header("ATAQUE NORMAL")]
    [SerializeField]
    int normalAttackRange = 2;

    //Lista que va guardando las listas de tiles que saco de los calculos del TileManager
    List<IndividualTiles> tilesToCheck = new List<IndividualTiles>();
    //El cono es especial porque en tilesToCheck guardo la línea central del cono y en cone tile guardo el cono entero
    List<IndividualTiles> coneTiles = new List<IndividualTiles>();


    #region COPIA_GOBLIN

    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurrentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    //Path de tiles a seguir hasta el objetivo
    [HideInInspector]
    private List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    //Lista que guarda los enmeigos y personajes que están dentro del rango de alerta del personaje (ya sea para comprobar personajes o alertar a enemigos)
    [HideInInspector]
    private List<UnitBase> unitsInRange = new List<UnitBase>();

    public override void SearchingObjectivesToAttack()
    {
        myCurrentObjective = null;
        myCurrentObjectiveTile = null;
        pathToObjective.Clear();
        coneTiles.Clear();
        tilesToCheck.Clear();
        currentUnitsAvailableToAttack.Clear();

        //CAMBIAR ESTE HASATTACKED
        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        else
        { 
            if (areaCharged)
            {
                //Explotar área

                Debug.Log("0.Area Explota");
                areaCharged = false;
                attackCountThisTurn++;
            }

            ///Comprueba si puede hacer el traspaso de alma
            if (currentCooldownSoulSkill <= 0)
            {
                ///Haz traspaso de alma
                Debug.Log("0.5 Traspaso de alma");
                myCurrentEnemyState = enemyState.Ended;
            }

            //Como no puedo hacer traspaso, compruebo que ataques puedo hacer
            else
            {
                if (CheckCono())
                {
                    //Do área
                    Debug.Log("1.Cono");

                    attackCountThisTurn++;

                    if (attackCountThisTurn >= 2)
                    {
                        Debug.Log("Cono era el 2º ataque");
                        myCurrentEnemyState = enemyState.Ended;
                        return;

                    }

                    else if (CheckArea())
                    {
                        Debug.Log("2. Área");
                        attackCountThisTurn++;
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }
                }

                //NO PUEDE SER ELSE. Tanto si falla el área como si ni siquiera entra en el cono tiene que pasar por aquí.
                if (CheckNormal() && attackCountThisTurn < 2)
                {
                    //Do físico
                    attackCountThisTurn++;

                    Debug.Log("3. Físico");

                    if (attackCountThisTurn >= 2)
                    {
                        Debug.Log("Físico era el 2º ataque");
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }

                    else if (CheckArea())
                    {
                        //Do Area
                        Debug.Log("4. Area post Fisico");
                        attackCountThisTurn++;
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }

                    else
                    {
                        //Do Stun
                        Debug.Log("5. Stun");
                        attackCountThisTurn++;
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }
                }

                else if (hasMoved)
                {
                    if (CheckArea())
                    {
                        Debug.Log("5. Área");
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }

                    else
                    {
                        Debug.Log("6. Solo movimiento");
                        myCurrentEnemyState = enemyState.Ended;
                        return;
                    }
                }

                else if (attackCountThisTurn < 2)
                {
                    currentUnitsAvailableToAttack.Clear();
                    tilesToCheck.Clear();
                    coneTiles.Clear();

                    Debug.Log("7. Movimiento");
                    ///Comprueba si se ha movido (si no, se mueve y repite todas las comprobaciones menos el traspaso)

                    //Determinamos el enemigo más cercano.
                    currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());

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
                        LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false, false);

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
            }
        }
    }

    #region CHECK_ATTACK_TO_CHOOSE

    bool CheckArea()
    {
        currentUnitsAvailableToAttack.Clear();
        tilesToCheck.Clear();

        if (areaUsed)
        {
            return false;
        }

        else
        {
            //Guardo los tiles que rodean al señor oscuro
            tilesToCheck = LM.TM.GetSurroundingTiles(myCurrentTile, 1, true, false);

            for (int i = 0; i < tilesToCheck.Count; i++)
            {
                if (tilesToCheck[i].unitOnTile != null && tilesToCheck[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    currentUnitsAvailableToAttack.Add(tilesToCheck[i].unitOnTile);
                }
            }

            ///Comprueba si tiene + de 1 objetivo para hacer área
            if (currentUnitsAvailableToAttack.Count > 1)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
    }

    bool CheckNormal()
    {
        currentUnitsAvailableToAttack.Clear();
        tilesToCheck.Clear();

        if (attackUsed)
        {
            return false;
        }

        else
        {
            //Guardo los dos tiles en frente del personaje
            tilesToCheck = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, normalAttackRange);

            //Compruebo si en los 2 tiles de delante hay al menos un enemigo
            for (int i = 0; i < tilesToCheck.Count; i++)
            {
                if (tilesToCheck[i].unitOnTile != null &&
                    tilesToCheck[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    currentUnitsAvailableToAttack.Add(tilesToCheck[i].unitOnTile);
                }
            }


            if (currentUnitsAvailableToAttack.Count >= 1)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
    }

    bool CheckCono()
    {
        currentUnitsAvailableToAttack.Clear();
        tilesToCheck.Clear();
        coneTiles.Clear();

        if (coneUsed)
        {
            return false;
        }

        else
        {
            ///Comprueba si tiene 2 objetivos a rango de cono para hacer cono
            tilesToCheck.Clear();
            coneTiles.Clear();
            currentUnitsAvailableToAttack.Clear();

            //Guardo los tiles de la línea central del cono
            tilesToCheck = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, coneRange);

            //Guardo todos los tiles del cono
            coneTiles = LM.TM.GetConeTiles(tilesToCheck, currentFacingDirection);

            //Compruebo cada tile del área del cono en busca de personajes
            for (int i = 0; i < coneTiles.Count; i++)
            {
                if (coneTiles[i].unitOnTile != null &&
                    coneTiles[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    currentUnitsAvailableToAttack.Add(coneTiles[i].unitOnTile);
                }
            }

            //Si hay al menos 2 unidades en rango de cono
            if (currentUnitsAvailableToAttack.Count > 1)
            {
                return true;
            }

            //Si hay sólo 1 unidad pero no está en el rango del ataque normal hago el cono
            else if (currentUnitsAvailableToAttack.Count == 1 && !CheckNormal())
            {
                return true;
            }

            //Si no hay nadie o sólo hay 1 en rango de normal NO HAGO CONO
            else
            {
                return false;
            }
        }
    }

    #endregion


    public override void Attack()
    {
        //CAMBIAR ESTO (PROBABLEMENTE)
        base.Attack();

        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            //Si mi objetivo es adyacente a mi le ataco
            if (myCurrentTile.neighbours[i].unitOnTile != null && currentUnitsAvailableToAttack.Count > 0 && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0] && Mathf.Abs(myCurrentTile.height - myCurrentTile.neighbours[i].height) <= maxHeightDifferenceToAttack)
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

                    ColorAttackTile();

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

                    ColorAttackTile();

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);
                }

                //Animación de ataque
                hasAttacked = true;
                ExecuteAnimationAttack();
                //Se tiene que poner en wait hasta que acabe la animación de ataque
                myCurrentEnemyState = enemyState.Waiting;

                //Me pongo en waiting porque al salir del for va a entrar en la corrutina abajo.
                //myCurrentEnemyState = enemyState.Waiting;
                break;
            }
        }

        if (!hasMoved && !hasAttacked)
        {
            myCurrentEnemyState = enemyState.Moving;
        }
    }

    int limitantNumberOfTilesToMove;

    public override void MoveUnit()
    {
        limitantNumberOfTilesToMove = 0;

        movementParticle.SetActive(true);

        ShowActionPathFinding(false);

        //Como el path guarda el tile en el que esta el enemigo yel tile en el que esta el personaje del jugador resto 2.
        //Si esta resta se pasa del número de unidades que me puedo mover entonces solo voy a recorrer el número de tiles máximo que puedo llegar.
        if (pathToObjective.Count - 2 > movementUds)
        {
            limitantNumberOfTilesToMove = movementUds;
        }

        //Si esta resta por el contrario es menor o igual a movementUds significa que me voy mover el máximo o menos tiles.
        else
        {
            limitantNumberOfTilesToMove = pathToObjective.Count - 2;
        }

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(pathToObjective[pathToObjective.Count - 1]);

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
            transform.DOMove(currentTileVectorToMove, currentTimeForMovement);
            unitModel.transform.DOLookAt(currentTileVectorToMove, timeDurationRotation, AxisConstraint.Y);

            //Espera entre casillas
            yield return new WaitForSeconds(currentTimeForMovement);
        }

        hasMoved = true;

        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(pathToObjective[pathToObjective.Count - 1]);
        myCurrentEnemyState = enemyState.Searching;

        movementParticle.SetActive(false);

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

    //Esta función muestra la acción del enemigo.
    //Para esconderla hay otra función (esta en el EnemyUnit)
    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //Si se tiene que mostrar la acción por el hover calculamos el enemigo
        if (_shouldRecalculate)
        {
            pathToObjective.Clear();

            SearchingObjectivesToAttackShowActionPathFinding();
            if (myCurrentObjectiveTile != null)
            {
                //Cada enemigo realiza su propio path
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false, false);

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
                shaderHover.SetActive(true);
            }

            //Coge
            myLineRenderer.positionCount += (limitantNumberOfTilesToMove + 1);

            //myLineRenderer.SetVertexCount(LM.TM.currentPath.Count);

            for (int i = 0; i < limitantNumberOfTilesToMove + 1; i++)
            {
                Vector3 pointPosition = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);

                if (i < pathToObjective.Count - 1)
                {
                    myLineRenderer.SetPosition(i, pointPosition);

                    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                    {
                        shaderHover.transform.position = pointPosition;
                        if ((pathToObjective[i]) == currentUnitsAvailableToAttack[0].myCurrentTile)
                        {

                            CalculateDamagePreviousAttack(currentUnitsAvailableToAttack[0], this, pathToObjective[1]);
                        }
                        else
                        {

                            damageWithMultipliersApplied = -999;
                        }

                        Vector3 positionToLook = new Vector3(myCurrentObjective.transform.position.x, myCurrentObjective.transform.position.y + 0.5f, myCurrentObjective.transform.position.z);
                        shaderHover.transform.DOLookAt(positionToLook, 0, AxisConstraint.Y);
                    }
                }
            }

            ///En el gigante es importante que esta función vaya después de colocar la sombra. Por si acaso asegurarse de que este if nunca se pone antes que el reposicionamiento de la sombra

            //A pesar de que ya se llama a esta función desde el levelManager en caso de hover, si se tiene que mostrar porque el goblin está atacando se tiene que llamar desde aqui (ya que no pasa por el level manager)
            //Tiene que ser en falso porque si no pongo la condicion la función se cree que el tileya estaba pintado de antes
            if (!_shouldRecalculate)
            {
                ColorAttackTile();
            }
        }
    }

    //Se llama desde el LevelManager. Al final del showAction se encarga de mostrar el tile al que va a atacar
    public override void ColorAttackTile()
    {
        //El +2 es porque pathToObjective tiene en cuenta tanto el tile inicial (ocupado por goblin) como el final (ocupado por player)
        if (pathToObjective.Count > 0 && pathToObjective.Count <= movementUds + 2 && myCurrentObjective != null)
        {
            wereTilesAlreadyUnderAttack.Add(myCurrentObjectiveTile.isUnderAttack);

            tilesAlreadyUnderAttack.Add(myCurrentObjectiveTile);

            myCurrentObjectiveTile.ColorAttack();
        }
    }

    //Bool que indica si almenos una de las unidades encontradas en rango de acción es un player
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

    #endregion

    public override void FinishMyActions()
    {
        base.FinishMyActions();

        attackCountThisTurn = 0;
    }

}
