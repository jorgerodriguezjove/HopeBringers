using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGrabber : EnemyUnit
{
    public int turn2StunEnemy;

    [SerializeField]
    GameObject particleAttack5, particleAttack4, particleAttack3, particleAttack2;

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

    public override void Attack()
    {

        List<IndividualTiles> tilesInFront = new List<IndividualTiles>();

        tilesInFront = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection, attackRange);

        //Particulas
        for (int i = 0; i < tilesInFront.Count; i++)
        {
            if (tilesInFront[i].unitOnTile != null && tilesInFront[i].unitOnTile == currentUnitsAvailableToAttack[0])
            {
                if (i == 2)
                {
                    particleAttack2.SetActive(true);
                }

                else if (i == 3)
                {
                    particleAttack3.SetActive(true);
                }

                else if (i == 4)
                {
                    particleAttack4.SetActive(true);
                }

                else if (i == 5)
                {
                    particleAttack5.SetActive(true);
                }
            }
        }

        //Si no he sido alertado, activo mi estado de alerta.
        //Al alertarme salo del void de ataque para hacer la busqueda normal de jugadores.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
            myCurrentEnemyState = enemyState.Searching;
            return;
        }

        CheckUnitToAttack(myCurrentTile);

        if (currentUnitsAvailableToAttack.Count == 0)
        {
            if (!hasMoved && !hasAttacked)
            {
                myCurrentEnemyState = enemyState.Moving;
            }

            else
            {
                myCurrentEnemyState = enemyState.Ended;
            }
        }
        else
        {

            if (!hasAttacked)
            {
                ColorAttackTile();

                if (currentFacingDirection == FacingDirection.North)
                {
                    //Calcula el vector al que se tiene que mover.
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position;
                    currentUnitsAvailableToAttack[0].transform.DOMove(currentTileVectorToMove, 0.1f);
                    currentUnitsAvailableToAttack[0].UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[0]);
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    //Calcula el vector al que se tiene que mover.
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position;
                    currentUnitsAvailableToAttack[0].transform.DOMove(currentTileVectorToMove, 0.1f);
                    currentUnitsAvailableToAttack[0].UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[0]);

                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    //Calcula el vector al que se tiene que mover.
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position;
                    currentUnitsAvailableToAttack[0].transform.DOMove(currentTileVectorToMove, 0.1f);
                    currentUnitsAvailableToAttack[0].UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[0]);


                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    //Calcula el vector al que se tiene que mover.
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position;
                    currentUnitsAvailableToAttack[0].transform.DOMove(currentTileVectorToMove, 0.1f);
                    currentUnitsAvailableToAttack[0].UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[0]);
                }

                //Atacar al enemigo
                DoDamage(currentUnitsAvailableToAttack[0]);

                if(myTierLevel== TierLevel.Level2
                    && !currentUnitsAvailableToAttack[0].isDead)
                {
                    currentUnitsAvailableToAttack[0].isStunned = true;
                    if (currentUnitsAvailableToAttack[0].turnStunned < 0)
                    {
                        currentUnitsAvailableToAttack[0].turnStunned = 0;
                    }
                    currentUnitsAvailableToAttack[0].turnStunned += turn2StunEnemy;

                }

                base.Attack();

                //Animación de ataque
                ExecuteAnimationAttack();
                hasAttacked = true;
            }
            else
            {
                myCurrentEnemyState = enemyState.Ended;
            }
        }
    }
  
    public void CheckUnitToAttack(IndividualTiles _referenceTile)
    {
        currentUnitsAvailableToAttack.Clear();
        previousTileHeight = 0;

        if (currentFacingDirection == FacingDirection.North)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (_referenceTile.tilesInLineUp[i].height > previousTileHeight)
                {
                    previousTileHeight = _referenceTile.tilesInLineUp[i].height;
                }

                //Si hay una unidad
                if (_referenceTile.tilesInLineUp[i].unitOnTile != null && _referenceTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(_referenceTile.tilesInLineUp[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(_referenceTile.tilesInLineUp[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(_referenceTile.tilesInLineUp[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (_referenceTile.tilesInLineUp[i].unitOnTile != null)
                {
                    break;
                }


                if (_referenceTile.tilesInLineUp[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (_referenceTile.tilesInLineDown[i].height > previousTileHeight)
                {
                    previousTileHeight = _referenceTile.tilesInLineDown[i].height;
                }

                //Si hay una unidad
                if (_referenceTile.tilesInLineDown[i].unitOnTile != null && _referenceTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(_referenceTile.tilesInLineDown[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(_referenceTile.tilesInLineDown[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(_referenceTile.tilesInLineDown[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (_referenceTile.tilesInLineDown[i].unitOnTile != null)
                {
                    break;
                }

                if (_referenceTile.tilesInLineDown[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (_referenceTile.tilesInLineRight[i].height > previousTileHeight)
                {
                    previousTileHeight = _referenceTile.tilesInLineRight[i].height;
                }

                //Si hay una unidad
                if (_referenceTile.tilesInLineRight[i].unitOnTile != null && _referenceTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(_referenceTile.tilesInLineRight[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(_referenceTile.tilesInLineRight[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(_referenceTile.tilesInLineRight[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (_referenceTile.tilesInLineRight[i].unitOnTile != null)
                {
                    break;
                }

                if (_referenceTile.tilesInLineRight[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (_referenceTile.tilesInLineLeft[i].height > previousTileHeight)
                {
                    previousTileHeight = _referenceTile.tilesInLineLeft[i].height;
                }

                //Si hay una unidad
                if (_referenceTile.tilesInLineLeft[i].unitOnTile != null && _referenceTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - _referenceTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(_referenceTile.tilesInLineLeft[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(_referenceTile.tilesInLineLeft[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (_referenceTile.tilesInLineLeft[i].unitOnTile != null) {
                    break;
                }

                if (_referenceTile.tilesInLineLeft[i].isEmpty)
                {
                    break;
                }
            }

        }

        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            CalculateDamage(currentUnitsAvailableToAttack[i]);
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);

        }
    }

    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //Cada enemigo realiza su propio path

        //AL IGUAL QUE CON EL MOVIMIENTO ESTO ES LA LÓGICA DEL GOBLIN QUE SE USA DE BASE
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
                shadowTile = pathToObjective[i];

                Vector3 pointPosition = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);

                if (i < pathToObjective.Count - 1)
                {
                    if (!dontShowShadow)
                    {
                        myLineRenderer.SetPosition(i, pointPosition);
                    }

                    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
                    {
                        sombraHoverUnit.transform.position = pointPosition;

                        if ((pathToObjective[limitantNumberOfTilesToMove + 1]) == currentUnitsAvailableToAttack[0].myCurrentTile)
                        {
                            Debug.Log("unit to grab" +name + " " + currentUnitsAvailableToAttack[0].name);
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

            ///En el gigante es importante que esta función vaya después de colocar la sombra. Por si acaso asegurarse de que este if nunca se pone antes que el reposicionamiento de la sombra

            //A pesar de que ya se llama a esta función desde el levelManager en caso de hover, si se tiene que mostrar porque el goblin está atacando se tiene que llamar desde aqui (ya que no pasa por el level manager)
            //Tiene que ser en falso porque si no pongo la condicion la función se cree que el tileya estaba pintado de antes
            if (!_shouldRecalculate)
            {
                ColorAttackTile();
            }

            dontShowShadow = false;
        }
    }

    //Función que muetra la sombra del objetivo donde va a ser atraida
    public void ShowGrabShadow(IndividualTiles _referenceTile,FacingDirection _referenceDirection)
    {
        currentUnitsAvailableToAttack[0].sombraHoverUnit.SetActive(true);

        List<IndividualTiles> tileUnitGrabbed = new List<IndividualTiles>();
        tileUnitGrabbed = _referenceTile.GetTilesInFrontOfTheCharacter(_referenceDirection, 1);

        Vector3 pointPosition = new Vector3(tileUnitGrabbed[0].transform.position.x, tileUnitGrabbed[0].transform.position.y + 0.5f, tileUnitGrabbed[0].transform.position.z);

        currentUnitsAvailableToAttack[0].sombraHoverUnit.transform.position = pointPosition;

    }

    private bool dontShowShadow;

    public void CalculateDamageForEnemiesGrabbedAfterMovement()
    {
        dontShowShadow = true;
    }


    public void HideGrabShadow()
    {
        currentUnitsAvailableToAttack[0].sombraHoverUnit.SetActive(false);
    }
}