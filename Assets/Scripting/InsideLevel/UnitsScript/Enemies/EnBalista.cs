using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnBalista : EnemyUnit
{
    #region VARIBLES
    //Bool que indica si la balista ha preparado el ataque para atacar en el siguiente turno.
    //Lo he puesto público porque el LevelManager tiene que acceder para hacer el hover.
    [SerializeField]
    public bool isAttackPrepared = false;

    //Bool que indica si la balista se está moviendo hacia la izquierda o hacia la derecha.
    [SerializeField]
    public bool isMovingToHisRight;

    //Bools  que indican si los tiles laterales de la balista están disponibles para moverse o no.
    private bool isRightTileOccupied;
    private bool isLeftTileOccupied;

    //Guarda los tiles que tengo que pintar. No puedo decirle que pinte toda la línea por si en medio de la línea hay obstáculos pero luego hay tiles accesibles
    [SerializeField]
    private List<IndividualTiles> tilesToShoot = new List<IndividualTiles>();

    #endregion 

    public override void SearchingObjectivesToAttack()
    {
        if (!isDead)
        {
            if (isAttackPrepared)
            {
                //Si pongo esta linea al disparar recarga y actualiza con los movimientos de los personajes (pero no el feedback)
                //CheckCharactersInLine();

                //Ataque
                myCurrentEnemyState = enemyState.Attacking;
            }

            else
            {
                //Buscas los enemigos en la línea de visión
                CheckCharactersInLine();

                //Si encuentra enemigos ataca
                if (currentUnitsAvailableToAttack.Count > 0)
                {
                    myCurrentEnemyState = enemyState.Attacking;
                }

                else
                {
                    if (hasMoved)
                    {
                        myCurrentEnemyState = enemyState.Ended;
                    }
                    else
                    {
                        myCurrentEnemyState = enemyState.Moving;
                    }
                }
            }
        }
        else
        {
            myCurrentEnemyState = enemyState.Waiting;
        }
    }

    //Esta función se encarga de mover a la unidad
    public override void MoveUnit()
    {
        if (!isDead)
        {
            movementParticle.SetActive(true);

            IndividualTiles tileToMove = GetTileToMove();

            //Si recibe null es porque ambos tiles laterales están ocupados
            if (tileToMove == null)
            {
                hasMoved = true;
            }

            else
            {
                MovementLogic(tileToMove);
            }
            
            movementParticle.SetActive(false);

            tilesToShoot.Clear();

            myCurrentEnemyState = enemyState.Searching;
        }

        else
        {
            myCurrentEnemyState = enemyState.Waiting;
        }
    }

    //Lógica actual del movimiento. Básicamente es el encargado de mover al modelo y setear las cosas
    private void MovementLogic(IndividualTiles tileToMove)
    {
        //Muevo a la balista
        transform.DOMove(currentTileVectorToMove, currentTimeForMovement);

        //Actualizo las variables de los tiles
        UpdateInformationAfterMovement(tileToMove);

        //Aviso de que se ha movido
        hasMoved = true;
    }

    public override void Attack()
    {
        //Si no he sido alertado, activo mi estado de alerta.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
        }

        if (!isDead)
        {
            if (isAttackPrepared)
            {
                base.Attack();
                StartCoroutine("AttackCorroutine");
                myCurrentEnemyState = enemyState.Waiting;
            }

            //Si el ataque no esta preparado pinto los tiles a los que voy a atacar
            else
            {
                FeedbackTilesToAttack(true);

                 //Prepara ataque
                isAttackPrepared = true;
                myCurrentEnemyState = enemyState.Ended;
            }
        }

        else
        {
            myCurrentEnemyState = enemyState.Waiting;
        }
    }

    //Necesito una corrutina para ir instanciando las partículas y que el daño vaya acorde con esto
    IEnumerator AttackCorroutine()
    {
        tilesToShoot.Clear();
        CheckCharactersInLine();

        Debug.Log(tilesToShoot.Count);
        for (int i = 0; i < tilesToShoot.Count; i++)
        {
            //En principio se destruye la particula por si sola al terminar
            Instantiate(attackParticle, new Vector3(tilesToShoot[i].transform.position.x, tilesToShoot[i].transform.position.y + 0.5f, tilesToShoot[i].transform.position.z), attackParticle.transform.rotation);
            yield return new WaitForSeconds(0.1f);

            if (tilesToShoot[i].unitOnTile != null)
            {
                DoDamage(tilesToShoot[i].unitOnTile);
            }

            //Quito el feedback de ataque de los tiles
            tilesToShoot[i].ColorDesAttack();
        }

        isAttackPrepared = false;

        tilesToShoot.Clear();
        myCurrentEnemyState = enemyState.Ended;
    }

    public override void FinishMyActions()
    {
        hasMoved = false;

        base.FinishMyActions();
    }

    
    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        CalculateDamage(unitToDealDamage);
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
    }

    //Función que pinta o despinta los tiles a los que está atcando la ballesta
    public void FeedbackTilesToAttack(bool shouldColorTiles)
    {
        for (int i = 0; i < tilesToShoot.Count; i++)
        {
            if (shouldColorTiles)
            {
                tilesToShoot[i].ColorAttack();
            }
            else
            {
                tilesToShoot[i].ColorDesAttack();
            }
        }
    }

    //Función que pinta o despinta los tiles a los que está atcando la ballesta
    public void FeedbackTilesToCharge(bool shouldColorTiles)
    {
        for (int i = 0; i < tilesToShoot.Count; i++)
        {
            if (shouldColorTiles)
            {
                tilesToShoot[i].ColorBorderRed();
            }
            else
            {
                tilesToShoot[i].ColorDesAttack();
            }
        }
    }

    //Pongo público para acceder a la hora de hacer hover
    //ESTA FUNCIÓN NO PUEDE CAMBIAR EL CURRENTSTATE DEL ENEMIGO, se llama fuera del turno enemigo.
    public override void CheckCharactersInLine()
    {
        if (!isDead)
        {
            currentUnitsAvailableToAttack.Clear();
            tilesToShoot.Clear();

            if (currentFacingDirection == FacingDirection.North)
            {
                if (rangeOfAction <= myCurrentTile.tilesInLineUp.Count)
                {
                    rangeVSTilesInLineLimitant = rangeOfAction;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
                }

                for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                {
                    //Si hay un obstáculo paro de comprobar
                    if (myCurrentTile.tilesInLineUp[i].isObstacle || myCurrentTile.tilesInLineUp[i] == null)
                    {
                        break;
                    }

                    //Si el tile no es un obstáculo lo añado a la lista de tiles a los que disparar y compruebo si tiene una unidad.
                    else if (!myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        //Si la diferencia de altura supera la que puede tener el personaje paro de buscar
                        if (i> 0 && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.tilesInLineUp[i - 1].height) > maxHeightDifferenceToAttack)
                        {
                            break;
                        }

                        tilesToShoot.Add(myCurrentTile.tilesInLineUp[i]);

                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>())
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades.
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);

                            //Si el personaje es un caballero mi disparo acaba aqui
                            if (myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<Knight>())
                            {
                                if (myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<Knight>().currentFacingDirection == FacingDirection.South)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.East)
            {
                if (rangeOfAction <= myCurrentTile.tilesInLineRight.Count)
                {
                    rangeVSTilesInLineLimitant = rangeOfAction;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
                }

                for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                {
                    //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                    if (myCurrentTile.tilesInLineRight[i].isObstacle || myCurrentTile.tilesInLineRight[i] == null)
                    {
                        break;
                    }

                    else if (!myCurrentTile.tilesInLineRight[i].isObstacle)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.tilesInLineRight[i - 1].height) > maxHeightDifferenceToAttack)
                        {
                            break;
                        }

                        tilesToShoot.Add(myCurrentTile.tilesInLineRight[i]);

                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>())
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades.
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);

                            //Si el personaje es un caballero mi disparo acaba aqui
                            if (myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<Knight>())
                            {
                                if (myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<Knight>().currentFacingDirection == FacingDirection.West)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.South)
            {
                if (rangeOfAction <= myCurrentTile.tilesInLineDown.Count)
                {
                    rangeVSTilesInLineLimitant = rangeOfAction;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
                }

                for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                {
                    
                    if (myCurrentTile.tilesInLineDown[i].isObstacle || myCurrentTile.tilesInLineDown[i] == null)
                    {
                        break;
                    }

                    else if (!myCurrentTile.tilesInLineDown[i].isObstacle)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.tilesInLineDown[i - 1].height) > maxHeightDifferenceToAttack)
                        {
                            break;
                        }

                        tilesToShoot.Add(myCurrentTile.tilesInLineDown[i]);

                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>())
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades.
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);

                            //Si el personaje es un caballero mi disparo acaba aqui
                            if (myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<Knight>())
                            {
                                if (myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<Knight>().currentFacingDirection == FacingDirection.North)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (currentFacingDirection == FacingDirection.West)
            {
                if (rangeOfAction <= myCurrentTile.tilesInLineLeft.Count)
                {
                    rangeVSTilesInLineLimitant = rangeOfAction;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
                }

                for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                {
                    //Tanto la balista cómo el charger detiene su comprobación si hay un obstáculo
                    if (myCurrentTile.tilesInLineLeft[i].isObstacle || myCurrentTile.tilesInLineLeft[i] == null)
                    {
                        break;
                    }

                    else if (!myCurrentTile.tilesInLineLeft[i].isObstacle)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.tilesInLineLeft[i - 1].height) > maxHeightDifferenceToAttack)
                        {
                            break;
                        }

                        tilesToShoot.Add(myCurrentTile.tilesInLineLeft[i]);

                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>())
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades.
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);

                            //Si el personaje es un caballero mi disparo acaba aqui
                            if (myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<Knight>())
                            {
                                if (myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<Knight>().currentFacingDirection == FacingDirection.East)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Aviso a los tiles de que tienen que buscar al caballero.
            WarnOrResetTilesToShoot(true);
        }
    }

    public void WarnOrResetTilesToShoot(bool _isWarning)
    {
        Debug.Log("Warn Tiles");
        for (int i = 0; i < tilesToShoot.Count; i++)
        {
            if (_isWarning)
            {
                tilesToShoot[i].lookingForKnightToWarnBalista = true;

                if (!isDead)
                {
                    tilesToShoot[i].allBalistasToWarn.Add(GetComponent<EnBalista>());
                }
            }

            else
            {
                tilesToShoot[i].lookingForKnightToWarnBalista = false;
                if (!isDead)
                {
                    tilesToShoot[i].allBalistasToWarn.Remove(GetComponent<EnBalista>());
                }
            }   
        }
    }

    public void BeWarnedByTile()
    {
        //Despinto los tiles actuales
        if (isAttackPrepared)
        {
            FeedbackTilesToAttack(false);
        }
       
        //Aviso a los tiles actuales de que ya no tienen que buscar
        WarnOrResetTilesToShoot(false);

        //Vuelvo a buscar los tiles a disparar
        CheckCharactersInLine();

        //Pinto los nuevos tiles
        if (isAttackPrepared)
        {
            FeedbackTilesToAttack(true);
        }
    }


    //Esta función calcula el tile al que se tiene que mover. Sirve tanto para moverse como para mostrar la acción de la balista
    public IndividualTiles GetTileToMove()
    {
        //En función de a donde este mirando su derecha o su izquierda cambia
        if (currentFacingDirection == FacingDirection.North)
        {
            //Compruebo si los tiles de la derecha están ocupados
            if (myCurrentTile.tilesInLineRight.Count <= 0 ||
                myCurrentTile.tilesInLineRight[0].isObstacle ||
                myCurrentTile.tilesInLineRight[0].isEmpty ||
                myCurrentTile.tilesInLineRight[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineRight[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isRightTileOccupied = true;
            }
            else
            {
                isRightTileOccupied = false;
            }

            //Compruebo si los tiles de la izquierda están ocupados
            if (myCurrentTile.tilesInLineLeft.Count <= 0 ||
                myCurrentTile.tilesInLineLeft[0].isObstacle ||
                myCurrentTile.tilesInLineLeft[0].isEmpty ||
                myCurrentTile.tilesInLineLeft[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineLeft[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isLeftTileOccupied = true;
            }
            else
            {
                isLeftTileOccupied = false;
            }

            //Me muevo en función de mi dirección de movimiento actual y los tiles ocupados
            if (isMovingToHisRight)
            {
                if (isRightTileOccupied && !isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; // new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                    isMovingToHisRight = false;

                    return (myCurrentTile.tilesInLineLeft[0]);
                }

                else if (!isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                    return (myCurrentTile.tilesInLineRight[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }

            else if (!isMovingToHisRight)
            {
                if (isLeftTileOccupied && !isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; // new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                    isMovingToHisRight = true;

                    return (myCurrentTile.tilesInLineRight[0]);
                }

                else if (!isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                    return (myCurrentTile.tilesInLineLeft[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            //Compruebo si los tiles de los lados están ocupados
            if (myCurrentTile.tilesInLineLeft.Count <= 0 || myCurrentTile.tilesInLineLeft[0].isObstacle || myCurrentTile.tilesInLineLeft[0].isEmpty || myCurrentTile.tilesInLineLeft[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineLeft[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isRightTileOccupied = true;
            }
            else
            {
                isRightTileOccupied = false;
            }

            if (myCurrentTile.tilesInLineRight.Count <= 0 || myCurrentTile.tilesInLineRight[0].isObstacle || myCurrentTile.tilesInLineRight[0].isEmpty || myCurrentTile.tilesInLineRight[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineRight[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isLeftTileOccupied = true;
            }
            else
            {
                isLeftTileOccupied = false;
            }

            if (isMovingToHisRight)
            {
                if (isRightTileOccupied && !isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; // new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                    isMovingToHisRight = false;
                    return (myCurrentTile.tilesInLineRight[0]);
                }

                else if (!isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                    return (myCurrentTile.tilesInLineLeft[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }

            else if (!isMovingToHisRight)
            {
                if (isLeftTileOccupied && !isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; // new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                    isMovingToHisRight = true;
                    return (myCurrentTile.tilesInLineLeft[0]);

                }

                else if (!isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; // new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                    return (myCurrentTile.tilesInLineRight[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            //Compruebo si los tiles de los lados están ocupados
            if (myCurrentTile.tilesInLineDown.Count <= 0 || myCurrentTile.tilesInLineDown[0].isObstacle || myCurrentTile.tilesInLineDown[0].isEmpty || myCurrentTile.tilesInLineDown[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineDown[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isRightTileOccupied = true;
            }
            else
            {
                isRightTileOccupied = false;
            }

            if (myCurrentTile.tilesInLineUp.Count <= 0 || myCurrentTile.tilesInLineUp[0].isObstacle || myCurrentTile.tilesInLineUp[0].isEmpty || myCurrentTile.tilesInLineUp[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineUp[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isLeftTileOccupied = true;
            }
            else
            {
                isLeftTileOccupied = false;
            }

            if (isMovingToHisRight)
            {
                if (isRightTileOccupied && !isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; // new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                    isMovingToHisRight = false;

                    return (myCurrentTile.tilesInLineUp[0]);
                }

                else if (!isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; // new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);

                    return (myCurrentTile.tilesInLineDown[0]);

                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }

            else if (!isMovingToHisRight)
            {
                if (isLeftTileOccupied && !isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; //  new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                    isMovingToHisRight = true;
                    return (myCurrentTile.tilesInLineDown[0]);

                }

                else if (!isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; // new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                    return (myCurrentTile.tilesInLineUp[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            //Compruebo si los tiles de los lados están ocupados
            if (myCurrentTile.tilesInLineUp.Count <= 0 || myCurrentTile.tilesInLineUp[0].isObstacle || myCurrentTile.tilesInLineUp[0].isEmpty || myCurrentTile.tilesInLineUp[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineUp[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isRightTileOccupied = true;
            }
            else
            {
                isRightTileOccupied = false;
            }

            if (myCurrentTile.tilesInLineDown.Count <= 0 || myCurrentTile.tilesInLineDown[0].isObstacle || myCurrentTile.tilesInLineDown[0].isEmpty || myCurrentTile.tilesInLineDown[0].unitOnTile != null ||
                Mathf.Abs(myCurrentTile.tilesInLineDown[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
            {
                isLeftTileOccupied = true;
            }
            else
            {
                isLeftTileOccupied = false;
            }

            if (isMovingToHisRight)
            {
                if (isRightTileOccupied && !isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                    isMovingToHisRight = false;
                    return (myCurrentTile.tilesInLineDown[0]);
                }

                else if (!isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; //new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                    return (myCurrentTile.tilesInLineUp[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }

            else if (!isMovingToHisRight)
            {
                if (isLeftTileOccupied && !isRightTileOccupied)
                {
                    //Muevo a la derecha
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; //new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                    isMovingToHisRight = true;
                    return (myCurrentTile.tilesInLineUp[0]);

                }

                else if (!isLeftTileOccupied)
                {
                    //Muevo a la izquierda
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                    return (myCurrentTile.tilesInLineDown[0]);
                }

                //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                else
                {
                    return (null);
                }
            }
        }

        //En principio no va a llegar nunca hasta aqui
        return null;
    }

    public override void MoveToTilePushed(IndividualTiles newTile)
    {
        FeedbackTilesToAttack(false);
        base.MoveToTilePushed(newTile);

        if (isAttackPrepared)
        {
            FeedbackTilesToAttack(false);
            tilesToShoot.Clear();
            CheckCharactersInLine();
            FeedbackTilesToAttack(true);
        }
    }

    public override void Die()
    {
        FeedbackTilesToAttack(false);
        base.Die();
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    //De momento no hace falta usarla.
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
                             
    }

}
