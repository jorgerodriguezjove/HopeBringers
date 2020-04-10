using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGrabber : EnemyUnit
{
    public int turn2StunEnemy;

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
        //Si no he sido alertado, activo mi estado de alerta.
        //Al alertarme salo del void de ataque para hacer la busqueda normal de jugadores.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
            myCurrentEnemyState = enemyState.Searching;
            return;
        }

        CheckUnitToAttack();

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
  
    public void CheckUnitToAttack()
    {
        currentUnitsAvailableToAttack.Clear();
        previousTileHeight = 0;

        if (currentFacingDirection == FacingDirection.North)
        {
            if (attackRange <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineUp[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineUp[i].height;
                }

                //Si hay una unidad
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                {
                    break;
                }


                if (myCurrentTile.tilesInLineUp[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South)
        {
            if (attackRange <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineDown[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineDown[i].height;
                }

                //Si hay una unidad
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                {
                    break;
                }

                if (myCurrentTile.tilesInLineDown[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East)
        {
            if (attackRange <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineRight[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineRight[i].height;
                }

                //Si hay una unidad
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                {
                    break;
                }

                if (myCurrentTile.tilesInLineRight[i].isEmpty)
                {
                    break;
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West)
        {
            if (attackRange <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineLeft[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineLeft[i].height;
                }

                //Si hay una unidad
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                    if (Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                        || Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }
                else if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null) {
                    break;
                }

                if (myCurrentTile.tilesInLineLeft[i].isEmpty)
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
}