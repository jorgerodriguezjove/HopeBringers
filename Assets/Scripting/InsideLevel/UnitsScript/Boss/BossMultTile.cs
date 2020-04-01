using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossMultTile : EnemyUnit
{
    int coneRange = 5;

    [Header("CRISTALES")]
    public List<Crystal> crystalList = new List<Crystal>();

    [Header("ATAQUES")]
    [SerializeField]
    private bool isPhase2;

    private int attackCountThisTurn;

    [SerializeField]
    private bool isBeamOrMeteoriteCharged;

    [SerializeField]
    bool sweepOrStompUsed;
    [SerializeField]
    bool coneUsed;


    [SerializeField]
    List<IndividualTiles> exteriorTiles = new List<IndividualTiles>();

    //Número de tiles que tengo que restar al path para compensar el hecho de que ocupa 3x3
    int offsetPathBecauseDragon = 1;

    [SerializeField]
    List<IndividualTiles> lastTileInPathSurroundingTiles = new List<IndividualTiles>();

    public override void InitializeUnitOnTile()
    {
        base.InitializeUnitOnTile();

        UpdateInformationAfterMovement(myCurrentTile);
    }

    //Override a la información que se actualiza al moverse
    public override void UpdateInformationAfterMovement(IndividualTiles newTile)
    {
        //Aviso a los tiles alrededor
        for (int i = 0; i < myCurrentTile.surroundingNeighbours.Count; i++)
        {
            myCurrentTile.surroundingNeighbours[i].unitOnTile = null;
            myCurrentTile.surroundingNeighbours[i].WarnInmediateNeighbours();
        }

        //Actualizo normal
        base.UpdateInformationAfterMovement(newTile);

        //Aviso a los tiles de alrededor
        for (int i = 0; i < myCurrentTile.surroundingNeighbours.Count; i++)
        {
            myCurrentTile.surroundingNeighbours[i].unitOnTile = GetComponent<UnitBase>();
            myCurrentTile.surroundingNeighbours[i].WarnInmediateNeighbours();
        }

        exteriorTiles.Clear();

        //Añado los tiles exteriores para el stomp.
        for (int i = 0; i < LM.TM.GetSurroundingTiles(myCurrentTile, 2, true, true).Count; i++)
        {
            exteriorTiles.Add(LM.TM.GetSurroundingTiles(myCurrentTile, 2, true, true)[i]);
        }
    }
    
    public void RemoveCrystal(Crystal _crystal)
    {
        crystalList.Remove(_crystal);

        Debug.Log(crystalList.Count);

        if (crystalList.Count == 0)
        {
            LM.InstaWin(true);
        }
    }

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

        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        else
        {
            if (isBeamOrMeteoriteCharged)
            {  
                //Usar Beam o Meteoritos
                Debug.Log("Ejectuar Global");

                DoBeamOrMeteorite();
                attackCountThisTurn++;

                //El check no se usa ya que se llama al final para comprobar si tiene que cargar el ataque
                //Importante no poner isBeamOrMeteoriteCharged = false; porque eso se llama al final para determinar si tiene que hacer la carga o no si ya ha disparado.

                //Salgo de la comprobación de acciones para volver a empezar
                CallWaitCoroutine();
                return;
            }

            else
            {
                if (CheckSweepOrStomp())
                {
                    Debug.Log("Fisico");

                    DoSweepOrStomp();
                    sweepOrStompUsed = true;
                    attackCountThisTurn++;

                    //Salgo de la comprobación de acciones para volver a empezar
                    CallWaitCoroutine();
                    return;
                }

                if (CheckFireCone())
                {
                    Debug.Log("Cono");

                    DoConoFuego();
                    coneUsed = true;
                    attackCountThisTurn++;

                    //Salgo de la comprobación de acciones para volver a empezar
                    CallWaitCoroutine();
                    return;
                }

                if (!hasMoved)
                {
                    Debug.Log("Mover");

                    myCurrentEnemyState = enemyState.Moving;
                }

                else
                {
                    if (!isBeamOrMeteoriteCharged)
                    {
                        //Hacer carga
                        CheckTilesForBeamOrMeteorite();
                    }

                    else
                    {
                        isBeamOrMeteoriteCharged = false;
                    }

                    myCurrentEnemyState = enemyState.Ended;
                    return;
                }
            }
        }
    }

    #region CHECK_ATTACKS
    //Cada check sirve para la versión normal y la mejorada. Además guardan los tiles para usar en las funciones de hacer ataque

    List<IndividualTiles> threeTilesInFront = new List<IndividualTiles>();

    private bool CheckSweepOrStomp()
    {
        threeTilesInFront.Clear();
        currentUnitsAvailableToAttack.Clear();
        exteriorTiles.Clear();

        if (!sweepOrStompUsed)
        {
            //Calculo pisotón
            if (isPhase2)
            {
                //Añado los tiles exteriores para el stomp.
                for (int i = 0; i < LM.TM.GetSurroundingTiles(myCurrentTile, 2, true, true).Count; i++)
                {
                    exteriorTiles.Add(LM.TM.GetSurroundingTiles(myCurrentTile, 2, true, true)[i]);
                }

                //Busco unidades en los tiles exteriores
                for (int i = 0; i < exteriorTiles.Count; i++)
                {
                    if (exteriorTiles[i].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack.Add(exteriorTiles[i].unitOnTile);
                    }
                }

                if (currentUnitsAvailableToAttack.Count >0)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }

            //Calculo Barrido
            else
            {
                threeTilesInFront = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, 2)[1].GetLateralTilesBasedOnDirection(currentFacingDirection, 1);
                threeTilesInFront.Add(myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, 2)[1]);

                for (int i = 0; i < threeTilesInFront.Count; i++)
                {
                    if (threeTilesInFront[i].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack.Add(threeTilesInFront[i].unitOnTile);
                    }
                }

                if (currentUnitsAvailableToAttack.Count > 0)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

        //No puedo hacer ninguno de los dos ataques
        else
        {
            return false;
        }
    }

    List<IndividualTiles> coneTiles = new List<IndividualTiles>();
    List<IndividualTiles> middleLineConeTiles = new List<IndividualTiles>();

    private bool CheckFireCone()
    {
        coneTiles.Clear();
        middleLineConeTiles.Clear();
        currentUnitsAvailableToAttack.Clear();

        if (!coneUsed)
        {
            //Guardo los tiles de la línea central del cono
            middleLineConeTiles = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, coneRange);
            //Quito el tile inicial para que el primer tile sea el central del frente. De esta forma el cono empezará con 3 tiles de ataque delante del dragón
            middleLineConeTiles.Remove(myCurrentTile);

            //Guardo todos los tiles del cono
            coneTiles = LM.TM.GetConeTiles(middleLineConeTiles, currentFacingDirection);
            //Quito el primer tile ya que es el central del frente del dragón
            coneTiles.RemoveAt(0);

            //Compruebo cada tile del área del cono en busca de personajes
            for (int i = 0; i < coneTiles.Count; i++)
            {
                if (coneTiles[i].unitOnTile != null &&
                    coneTiles[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    currentUnitsAvailableToAttack.Add(coneTiles[i].unitOnTile);
                }
            }
            return true;
        }

        else
        {
            return false;
        }      
    }

    List<IndividualTiles> surroundingPlayerTiles = new List<IndividualTiles>();
    List<IndividualTiles> middleLineTilesInFront = new List<IndividualTiles>();
    List<IndividualTiles> lateralMidLineTiles = new List<IndividualTiles>();
    List<IndividualTiles> beamOrMeteoriteTiles = new List<IndividualTiles>();

    //Este check es diferente al resto, no comprueba si se puede hacer el ataque, si no cuales son los tiles que cargar (ya que este ataque no necesita que haya
    //tiles o condiciones concretas para realizarse a parte de estar cargado)
    private void CheckTilesForBeamOrMeteorite()
    {
        surroundingPlayerTiles.Clear();
        middleLineTilesInFront.Clear();
        lateralMidLineTiles.Clear();
        beamOrMeteoriteTiles.Clear();
        currentUnitsAvailableToAttack.Clear();

        if (isPhase2)
        {
            for (int i = 0; i < LM.charactersOnTheBoard.Count; i++)
            {
                surroundingPlayerTiles = LM.TM.GetSurroundingTiles(LM.charactersOnTheBoard[i].myCurrentTile, 1, true, false);

                for (int j = 0; j < surroundingPlayerTiles.Count; j++)
                {
                    surroundingPlayerTiles[j].ColorAttack();
                    beamOrMeteoriteTiles.Add(surroundingPlayerTiles[i]);
                }
            }
        }

        //Beam
        else
        {
            middleLineTilesInFront = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, 60);

            //Quito el primer tile ya que es el central del frente del dragón
            middleLineTilesInFront.RemoveAt(0);

            for (int i = 0; i < middleLineTilesInFront.Count; i++)
            {
                beamOrMeteoriteTiles.Add(middleLineTilesInFront[i]);
                middleLineTilesInFront[i].ColorAttack();


                lateralMidLineTiles.Clear();

                lateralMidLineTiles = middleLineTilesInFront[i].GetLateralTilesBasedOnDirection(currentFacingDirection, 1);

                for (int j = 0; j < lateralMidLineTiles.Count; j++)
                {
                    beamOrMeteoriteTiles.Add(lateralMidLineTiles[j]);

                    lateralMidLineTiles[j].ColorAttack();
                }
            }

        }

        myCurrentEnemyState = enemyState.Waiting;

        isBeamOrMeteoriteCharged = true;
    }

    #endregion

    #region DO_ATTACKS
    //Al contrario de los checks hay un do por cada ataque (a excepción del cono de fuego que usa la misma)

    private void DoSweepOrStomp()
    {
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            //Particulas. Si queremos que sean distinta entre los ataques simplemente if (phase2)

            DoDamage(currentUnitsAvailableToAttack[i]);
        }
    }

    List<IndividualTiles> tilesListToPull = new List<IndividualTiles>();


    private void DoConoFuego()
    {
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            tilesListToPull.Clear();
            DoDamage(currentUnitsAvailableToAttack[i]);

            if (isPhase2)
            {
                #region CheckPullDirection
                //Por algún motivo, poniendolo a 1 atrae a los personajes 2 tiles asi que lo dejo a 0 que atrae unicamente 1 tile.

                if (currentFacingDirection == FacingDirection.North)
                {
                    tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile);

                    for (int j = 0; j < currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineDown.Count; j++)
                    {
                        tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineDown[j]);
                    }
                }

                if (currentFacingDirection == FacingDirection.South)
                {
                    tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile);

                    for (int j = 0; j < currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineUp.Count; j++)
                    {
                        tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineUp[j]);
                    }
                }

                if (currentFacingDirection == FacingDirection.East)
                {
                    tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile);

                    for (int j = 0; j < currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineLeft.Count; j++)
                    {
                        tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineLeft[j]);
                    }
                }

                if (currentFacingDirection == FacingDirection.West)
                {
                    tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile);

                    for (int j = 0; j < currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineRight.Count; j++)
                    {
                        tilesListToPull.Add(currentUnitsAvailableToAttack[i].myCurrentTile.tilesInLineRight[j]);
                    }
                }


                if (exteriorTiles.Contains(currentUnitsAvailableToAttack[i].myCurrentTile))
                {
                    DoDamage(currentUnitsAvailableToAttack[i]);

                    continue;
                }

                currentUnitsAvailableToAttack[i].ExecutePush(1, tilesListToPull, damageMadeByPush, damageMadeByFall);  

                #endregion
            }
        }

      
    }

    private void DoBeamOrMeteorite()
    {
        //Partículas en tiles

        for (int i = 0; i < beamOrMeteoriteTiles.Count; i++)
        {
            if (beamOrMeteoriteTiles[i].unitOnTile != null && beamOrMeteoriteTiles[i].unitOnTile.GetComponent<PlayerUnit>())
            {
                currentUnitsAvailableToAttack.Add(beamOrMeteoriteTiles[i].unitOnTile);
            }

            beamOrMeteoriteTiles[i].ColorDesAttack();
        }

        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            DoDamage(currentUnitsAvailableToAttack[i]);
        }
    }


    private void CallWaitCoroutine()
    {
        //Salgo de la comprobación de acciones para volver a empezar
        StartCoroutine("WaitBeforeNextAction");
        myCurrentEnemyState = enemyState.Waiting;
    }

    IEnumerator WaitBeforeNextAction()
    {
        yield return new WaitForSeconds(2f);

        myCurrentEnemyState = enemyState.Searching;
    }


    #endregion



    public override void Attack()
    {
        Debug.Log("ATTACK NO SE USA EN BOSSES");

        #region DeprecatedAttack

        //for (int i = 0; i < exteriorTiles.Count; i++)
        //{
        //    //Si mi objetivo es adyacente a mi le ataco
        //    if (exteriorTiles[i].unitOnTile != null && currentUnitsAvailableToAttack.Count > 0 && exteriorTiles[i].unitOnTile == currentUnitsAvailableToAttack[0] && Mathf.Abs(myCurrentTile.height - exteriorTiles[i].height) <= maxHeightDifferenceToAttack)
        //    {
        //        //Las comprobaciones para atacar arriba y abajo son iguales. Salvo por la dirección en la que tiene que girar el goblin
        //        if (myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
        //        {
        //            //Arriba
        //            if (myCurrentObjectiveTile.tileZ > myCurrentTile.tileZ)
        //            {
        //                RotateLogic(FacingDirection.North);
        //            }
        //            //Abajo
        //            else
        //            {
        //                RotateLogic(FacingDirection.South);
        //            }

        //            ColorAttackTile();

        //            //Atacar al enemigo
        //            DoDamage(currentUnitsAvailableToAttack[0]);
        //        }
        //        //Izquierda o derecha
        //        else
        //        {
        //            //Arriba
        //            if (myCurrentObjectiveTile.tileX > myCurrentTile.tileX)
        //            {
        //                RotateLogic(FacingDirection.East);
        //            }
        //            //Abajo
        //            else
        //            {
        //                RotateLogic(FacingDirection.West);
        //            }

        //            ColorAttackTile();

        //            //Atacar al enemigo
        //            DoDamage(currentUnitsAvailableToAttack[0]);
        //        }

        //        //Animación de ataque
        //        ExecuteAnimationAttack();
        //        hasAttacked = true;
        //        //Se tiene que poner en wait hasta que acabe la animación de ataque
        //        myCurrentEnemyState = enemyState.Waiting;

        //        break;
        //    }
        //}

        //if (!hasMoved && !hasAttacked)
        //{
        //    myCurrentEnemyState = enemyState.Moving;
        //}

        #endregion
    }

    int limitantNumberOfTilesToMove;

    public override void MoveUnit()
    {
        #region EQUIVALENTE_AL_SEARCH

        currentUnitsAvailableToAttack.Clear();

        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());

        //Si no hay enemigos termina su turno
        if (currentUnitsAvailableToAttack.Count == 0)
        {
            myCurrentEnemyState = enemyState.Searching;
            hasMoved = true;
            return;
        }

        else if (currentUnitsAvailableToAttack.Count > 0)
        {
            if (currentUnitsAvailableToAttack.Count == 1)
            {
                myCurrentObjective = currentUnitsAvailableToAttack[0];
                myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
            }

            //Si hay varios enemigos a la misma distancia
            else if (currentUnitsAvailableToAttack.Count > 1)
            {
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
            LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

            //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
            //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
            //ES -1 PORQUE EN EL CASO DEL DRAGÓN HAY QUE RESTAR UN TILE YA QUE ESTÁ OCUPADO POR EL PROPIO DRAGÓN!!!!!!!!!!!!!!!!!!!!!!!!
            for (int i = 0; i < LM.TM.currentPath.Count - offsetPathBecauseDragon; i++)
            {
                pathToObjective.Add(LM.TM.currentPath[i]);
            }

            lastTileInPathSurroundingTiles.Clear();

            //Despúes de haber restado uno al path compruebo que en este último tile sigue sin estar el jugador.
            //En caso contrario resto otro tile al path
            for (int i = 0; i < LM.TM.GetSurroundingTiles(pathToObjective[pathToObjective.Count - 2], 1, true, false).Count; i++)
            {
                lastTileInPathSurroundingTiles.Add(LM.TM.GetSurroundingTiles(pathToObjective[pathToObjective.Count - 2], 1, true, false)[i]);
            }

            for (int i = 0; i < lastTileInPathSurroundingTiles.Count; i++)
            {
                if (lastTileInPathSurroundingTiles[i].unitOnTile != null && lastTileInPathSurroundingTiles[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    pathToObjective.RemoveAt(pathToObjective.Count - 2);
                    break;
                }
            }
        }

        #endregion

        #region EQUIVALENTE_AL_MOVE

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

        #endregion
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

        //Espero después de moverme para que no vaya demasiado rápido
        yield return new WaitForSeconds(currentTimeForMovement);
        hasMoved = true;


        //Compruebo la dirección en la que se mueve para girar a la unidad
        CheckTileDirection(pathToObjective[pathToObjective.Count - 1]);

        //Vuelvo al search
        CallWaitCoroutine();

        movementParticle.SetActive(false);

        HideActionPathfinding();
        //ShowActionPathFinding(false);
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
                LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

                //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
                //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
                //ES -1 PORQUE EN EL CASO DEL DRAGÓN HAY QUE RESTAR UN TILE YA QUE ESTÁ OCUPADO POR EL PROPIO DRAGÓN!!!!!!!!!!!!!!!!!!!!!!!!
                for (int i = 0; i < LM.TM.currentPath.Count - offsetPathBecauseDragon; i++)
                {
                    pathToObjective.Add(LM.TM.currentPath[i]);
                }


                lastTileInPathSurroundingTiles.Clear();

                //Despúes de haber restado uno al path compruebo que en este último tile sigue sin estar el jugador.
                //En caso contrario resto otro tile al path
                for (int i = 0; i < LM.TM.GetSurroundingTiles(pathToObjective[pathToObjective.Count - 2], 1, true, false).Count; i++)
                {
                    lastTileInPathSurroundingTiles.Add(LM.TM.GetSurroundingTiles(pathToObjective[pathToObjective.Count - 2], 1, true, false)[i]);
                }

                for (int i = 0; i < lastTileInPathSurroundingTiles.Count; i++)
                {
                    if (lastTileInPathSurroundingTiles[i].unitOnTile != null && lastTileInPathSurroundingTiles[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        pathToObjective.RemoveAt(pathToObjective.Count - 2);
                        break;
                    }
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

    public override void FinishMyActions()
    {
        base.FinishMyActions();

        attackCountThisTurn = 0;

        sweepOrStompUsed = false;
        coneUsed = false;
    }


    #endregion

}
