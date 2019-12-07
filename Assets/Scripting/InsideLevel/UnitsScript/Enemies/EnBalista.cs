using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnBalista : EnemyUnit
{
    #region VARIBLES
    //Bool que indica si la balista ha preparado el ataque para atacar en el siguiente turno.
    [SerializeField]
    private bool isAttackPrepared = false;

    //Bool que indica si la balista se está moviendo hacia la izquierda o hacia la derecha.
    [SerializeField]
    public bool isMovingToHisRight;

    //Bools  que indican si los tiles laterales de la balista están disponibles para moverse o no.
    private bool isRightTileOccupied;
    private bool isLeftTileOccupied;

    #endregion 

    public override void SearchingObjectivesToAttack()
    {
        if (!isDead)
        {
            if (isAttackPrepared)
            {
                //Disparas
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

    public override void MoveUnit()
    {
        if (!isDead)
        {
            movementParticle.SetActive(true);
            //En función de a donde este mirando su derecha o su izquierda cambia
            if (currentFacingDirection == FacingDirection.North)
            {
                //Compruebo si los tiles de la derecha están ocupados
                if (myCurrentTile.tilesInLineRight.Count <= 0               ||
                    myCurrentTile.tilesInLineRight[0].isObstacle            ||
                    myCurrentTile.tilesInLineRight[0].isEmpty               ||
                    myCurrentTile.tilesInLineRight[0].unitOnTile != null    ||
                    Mathf.Abs(myCurrentTile.tilesInLineRight[0].height - myCurrentTile.height) > maxHeightDifferenceToMove)
                {
                    isRightTileOccupied = true;
                }
                else
                {
                    isRightTileOccupied = false;
                }

                //Compruebo si los tiles de la izquierda están ocupados
                if (myCurrentTile.tilesInLineLeft.Count <= 0            ||
                    myCurrentTile.tilesInLineLeft[0].isObstacle         ||
                    myCurrentTile.tilesInLineLeft[0].isEmpty            ||
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
                        MovementLogic(myCurrentTile.tilesInLineLeft);
                    }

                    else if (!isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineRight);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
                    }
                }

                else if (!isMovingToHisRight)
                {
                    if (isLeftTileOccupied && !isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; // new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                        isMovingToHisRight = true;
                        MovementLogic(myCurrentTile.tilesInLineRight);

                    }

                    else if (!isLeftTileOccupied)
                    {
                        //Muevo a la izquierda
                        currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineLeft);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
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
                        MovementLogic(myCurrentTile.tilesInLineRight);
                    }

                    else if (!isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineLeft);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
                    }
                }

                else if (!isMovingToHisRight)
                {
                    if (isLeftTileOccupied && !isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position; // new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height, myCurrentTile.tilesInLineLeft[0].tileZ);
                        isMovingToHisRight = true;
                        MovementLogic(myCurrentTile.tilesInLineLeft);

                    }

                    else if (!isLeftTileOccupied)
                    {
                        //Muevo a la izquierda
                        currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position; // new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height, myCurrentTile.tilesInLineRight[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineRight);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
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
                        MovementLogic(myCurrentTile.tilesInLineUp);
                    }

                    else if (!isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; // new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineDown);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
                    }
                }

                else if (!isMovingToHisRight)
                {
                    if (isLeftTileOccupied && !isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; //  new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                        isMovingToHisRight = true;
                        MovementLogic(myCurrentTile.tilesInLineDown);

                    }

                    else if (!isLeftTileOccupied)
                    {
                        //Muevo a la izquierda
                        currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; // new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineUp);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
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
                        MovementLogic(myCurrentTile.tilesInLineDown);
                    }

                    else if (!isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; //new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineUp);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
                    }
                }

                else if (!isMovingToHisRight)
                {
                    if (isLeftTileOccupied && !isRightTileOccupied)
                    {
                        //Muevo a la derecha
                        currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position; //new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height, myCurrentTile.tilesInLineUp[0].tileZ);
                        isMovingToHisRight = true;
                        MovementLogic(myCurrentTile.tilesInLineUp);

                    }

                    else if (!isLeftTileOccupied)
                    {
                        //Muevo a la izquierda
                        currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height, myCurrentTile.tilesInLineDown[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineDown);
                    }

                    //Si ambos lados están bloqueados no se mueve, pero a nivel de lógica cuenta cómo si lo hubiese hecho.
                    else
                    {
                        hasMoved = true;
                    }
                }
            }

            movementParticle.SetActive(false);
            myCurrentEnemyState = enemyState.Searching;

            //Espero después de moverme para que no vaya demasiado rápido
            //myCurrentEnemyState = enemyState.Waiting;
            //StartCoroutine("MovementWait");
        }
        else
        {
            myCurrentEnemyState = enemyState.Waiting;
        }
    }

    //Lógica actual del movimiento. Básicamente es el encargado de mover al modelo y setear las cosas
    private void MovementLogic(List<IndividualTiles> ListWithNewTile)
    {
        //Muevo a la balista
        transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

        //Actualizo las variables de los tiles
        UpdateInformationAfterMovement(ListWithNewTile[0]);

        //Aviso de que se ha movido
        hasMoved = true;
    }

    //IEnumerator MovementWait()
    //{
    //    yield return new WaitForSeconds(timeWaitAfterMovement);
    //    myCurrentEnemyState = enemyState.Searching;
    //}

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
                //Dispara
                if (currentFacingDirection == FacingDirection.North)
                {
                    for (int i = 0; i < myCurrentTile.tilesInLineUp.Count; i++)
                    {
                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                        {
                            DoDamage(myCurrentTile.tilesInLineUp[i].unitOnTile);
                        }

                        //Quito el feedback de ataque de los tiles
                        myCurrentTile.tilesInLineUp[i].ColorDesAttack();
                    }
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    for (int i = 0; i < myCurrentTile.tilesInLineDown.Count; i++)
                    {
                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                        {
                            DoDamage(myCurrentTile.tilesInLineDown[i].unitOnTile);
                        }

                        myCurrentTile.tilesInLineDown[i].ColorDesAttack();
                    }
                    
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    for (int i = 0; i < myCurrentTile.tilesInLineRight.Count; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                        {
                            DoDamage(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        }

                        myCurrentTile.tilesInLineRight[i].ColorDesAttack();
                    }
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    for (int i = 0; i < myCurrentTile.tilesInLineLeft.Count; i++)
                    {
                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                        {
                            DoDamage(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        }

                        myCurrentTile.tilesInLineLeft[i].ColorDesAttack();
                    }
                }

                isAttackPrepared = false;
                myCurrentEnemyState = enemyState.Ended;


                //Espero 1 sec
                //myCurrentEnemyState = enemyState.Waiting;
                //StartCoroutine("AttackWait");
            }

            //Si el ataque no esta preparado pinto los tiles a los que voy a atacar
            else
            {
                FeedbackTilesToAttack(true);

                 //Prepara ataque
                isAttackPrepared = true;
                myCurrentEnemyState = enemyState.Ended;
                
                //Espero 1 sec
                //StartCoroutine("AttackWait");
                //Colorear los tiles visualmente
            }
        }

        else
        {
            myCurrentEnemyState = enemyState.Waiting;
        }
    }

    //IEnumerator AttackWait()
    //{
    //    yield return new WaitForSeconds(timeWaitAfterAttack);
    //    myCurrentEnemyState = enemyState.Ended;
    //}

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
        if (currentFacingDirection == FacingDirection.North)
        {
            for (int i = 0; i < myCurrentTile.tilesInLineUp.Count; i++)
            {
                if (shouldColorTiles)
                {
                    myCurrentTile.tilesInLineUp[i].ColorAttack();
                }
                else
                {
                    myCurrentTile.tilesInLineUp[i].ColorDesAttack();
                }  
            }
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            for (int i = 0; i < myCurrentTile.tilesInLineDown.Count; i++)
            {
                if (shouldColorTiles)
                {
                    myCurrentTile.tilesInLineDown[i].ColorAttack();
                }
                else
                {
                    myCurrentTile.tilesInLineDown[i].ColorDesAttack();
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            for (int i = 0; i < myCurrentTile.tilesInLineRight.Count; i++)
            {
                if (shouldColorTiles)
                {
                    myCurrentTile.tilesInLineRight[i].ColorAttack();
                }
                else
                {
                    myCurrentTile.tilesInLineRight[i].ColorDesAttack();
                }
            }
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            for (int i = 0; i < myCurrentTile.tilesInLineLeft.Count; i++)
            {
                if (shouldColorTiles)
                {
                    myCurrentTile.tilesInLineLeft[i].ColorAttack();
                }
                else
                {
                    myCurrentTile.tilesInLineLeft[i].ColorDesAttack();
                }
            }
        }
    }

    protected override void CheckCharactersInLine()
    {
        if (!isDead)
        {
            currentUnitsAvailableToAttack.Clear();

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
                    if (myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        break;
                    }

                    //Independientemente de que sea charger o balista este código sirve para ambos
                    else if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades.
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
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
                    if (myCurrentTile.tilesInLineRight[i].isObstacle)
                    {
                        break;
                    }

                    //Independientemente de que sea charger o balista este código sirve para ambos
                    else if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades.
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
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
                    
                    if (myCurrentTile.tilesInLineDown[i].isObstacle)
                    {
                        break;
                    }

                    //Independientemente de que sea charger o balista este código sirve para ambos
                    else if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades.
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
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
                    if (myCurrentTile.tilesInLineLeft[i].isObstacle)
                    {
                        break;
                    }

                    //Independientemente de que sea charger o balista este código sirve para ambos
                    else if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades.
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    }
                }
            }
        }
    }

    protected override void MoveToTilePushed(IndividualTiles newTile)
    {
        FeedbackTilesToAttack(false);
        base.MoveToTilePushed(newTile);

        if (isAttackPrepared)
        {
            FeedbackTilesToAttack(true);
        }
    }

    public override void Die()
    {
        FeedbackTilesToAttack(false);
        base.Die();
    }
}
