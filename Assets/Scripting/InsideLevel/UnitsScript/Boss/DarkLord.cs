using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DarkLord : EnemyUnit
{
    [SerializeField]
    private int damageReceivedAfterEndingPossesion;

    [SerializeField]
    private GameObject obstacleWhilePossesing;

    [SerializeField]
    private GameObject particleDisappear;

    [SerializeField]
    private GameObject particleEnemyPossesed;

    #region COPIA_GOBLIN

    protected override void Awake()
    {
        amITheOriginalDarkLord = true;
        amIBeingPossesed = false;

        //Le digo al enemigo cual es el LevelManager del nivel actual
        LevelManagerRef = FindObjectOfType<LevelManager>().gameObject;
        UIM = FindObjectOfType<UIManager>();

        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();

        if (amITheOriginalDarkLord)
        {
            LM.enemiesOnTheBoard.Add(this);
            currentHealth = maxHealth;
        }

        else
        {
            Instantiate(particleEnemyPossesed, transform);
        }

        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        //Inicializo componente animator
        myAnimator = GetComponent<Animator>();

        myCurrentEnemyState = enemyState.Waiting;
        
        initMaterial = unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material;

        currentTimeForMovement = timeMovementAnimation;


        Debug.Log(1);
        currentTimeWaitingBeforeStarting = timeWaitingBeforeStarting;
        currentTimeWaitinBeforeMovement  = timeWaitingBeforeMovement;
        currentTimeWaitinBeforeAttacking = timeWaitingBeforeAttacking;
        currentTimeWaitingBeforeEnding   = timeWaitingBeforeEnding;

        fMovementUds = movementUds;
    }

    //public void InitializeAfterPosesion(int _currentEnemyHealth)
    //{
    //    currentHealth = _currentEnemyHealth;
    //    LM.enemiesOnTheBoard.Insert(1, this);
    //    FindAndSetFirstTile();
    //    //myCurrentEnemyState = enemyState.Searching;

    //    bossPortrait = FindObjectOfType<PortraitBoss>();
    //}

    private void Start()
    {
        currentCooldownSoulSkill = maxCooldownSoulsSkill;
    }

    //Pasada a gob
    public override void SearchingObjectivesToAttack()
    {
        myCurrentObjective = null;
        myCurrentObjectiveTile = null;
        pathToObjective.Clear();
        coneTiles.Clear();
        tilesToCheck.Clear();
        currentUnitsAvailableToAttack.Clear();

        if (isDead || attackCountThisTurn >=2)
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

            if (currentlyPossesing)
            {
                //Resto al contador para explotar al enemigo
                Debug.Log("Aqui tengo que restar para explotar al enemigo");
                myCurrentEnemyState = enemyState.Ended;
                return;
            }

            ///Comprueba si puede hacer el traspaso de alma
            if (amITheOriginalDarkLord && currentCooldownSoulSkill <= 0 && CheckSoul())
            {
                ///Haz traspaso de alma
                Debug.Log("0.5 Traspaso de alma");
                DoSoulAttack();
                myCurrentEnemyState = enemyState.Ended;
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

    //LOS ATAQUES Y LOS CHECKS QUE FALTAN ESTAN AL FINAL DE ENEMYUNIT PARA QUE LOS USEN EL GOBLIN,CHARGER Y GIGANTE

    #region CHECK_ATTACK_TO_CHOOSE

    bool CheckSoul()
    {
        //Elegir enemigo
        if (LM.enemiesOnTheBoard.Count > 0)
        {
            if (LM.enemiesOnTheBoard[0] != null && !LM.enemiesOnTheBoard[0].isDead && !LM.enemiesOnTheBoard[0].GetComponent<DarkLord>())
            {
                chosenEnemy = LM.enemiesOnTheBoard[0];
                return true;
            }

            else if (LM.enemiesOnTheBoard.Count > 1 && LM.enemiesOnTheBoard[1] != null && !LM.enemiesOnTheBoard[1].isDead)
            {
                chosenEnemy = LM.enemiesOnTheBoard[1];
                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    #endregion

    #region ATTACKS

    EnemyUnit chosenEnemy;
    DarkLord newEnemyDarkLordRef;

    private void DoSoulAttack()
    {
        //Desactivar personaje
        unitModel.SetActive(false);
        GetComponent<Collider>().enabled = false;
        currentlyPossesing = true;

        Instantiate(particleDisappear, transform.position, particleDisappear.transform.rotation);

        //Aparece bloque en su lugar
        obstacleWhilePossesing.SetActive(true);

        //Nuevo enemigo cambia comportamiento y da feedback de que está poseido
        chosenEnemy.particleWhileBeingPossesedRefernce = Instantiate(particleEnemyPossesed, new Vector3(chosenEnemy.transform.position.x, chosenEnemy.transform.position.y +0.5f, chosenEnemy.transform.position.z) , particleEnemyPossesed.transform.rotation);
        chosenEnemy.StartPosesion();
    }

    public void EndPosesion()
    {
        Debug.Log("EndPosesion");

        //Hacer daño al personaje 
        ReceiveDamage(damageReceivedAfterEndingPossesion, null);

        //Reseteo el cooldown
        currentCooldownSoulSkill = maxCooldownSoulsSkill;

        //Desactivar personaje
        unitModel.SetActive(true);
        GetComponent<Collider>().enabled = true;
        currentlyPossesing = false;

        //Aparece bloque en su lugar
        obstacleWhilePossesing.SetActive(false);
    }

    #endregion

    public override void Attack()
    {
        Debug.Log("ATTACK NO SE USA EN BOSSES");

        #region DeprecatedAttack
        ////CAMBIAR ESTO (PROBABLEMENTE)
        //base.Attack();

        //for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        //{
        //    //Si mi objetivo es adyacente a mi le ataco
        //    if (myCurrentTile.neighbours[i].unitOnTile != null && currentUnitsAvailableToAttack.Count > 0 && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0] && Mathf.Abs(myCurrentTile.height - myCurrentTile.neighbours[i].height) <= maxHeightDifferenceToAttack)
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
        //        hasAttacked = true;
        //        ExecuteAnimationAttack();
        //        //Se tiene que poner en wait hasta que acabe la animación de ataque
        //        myCurrentEnemyState = enemyState.Waiting;

        //        //Me pongo en waiting porque al salir del for va a entrar en la corrutina abajo.
        //        //myCurrentEnemyState = enemyState.Waiting;
        //        break;
        //    }
        //}

        //if (!hasMoved && !hasAttacked)
        //{
        //    myCurrentEnemyState = enemyState.Moving;
        //}

        #endregion
    }

    //PARA MOVEUNIT SE USA LA BASE DEL ENEMIGO (Que es la lógica del goblin).
    public override void MoveUnit()
    {
        bossPortrait.FlipMovementToken();
        base.MoveUnit();
    }

    //Esta función muestra la acción del enemigo.
    //Para esconderla hay otra función (esta en el EnemyUnit)
    public override void ShowActionPathFinding(bool _shouldRecalculate)
    {
        //A pesar de que esta función está vacía, no se puede quitar por que hay que hacer override y que no haga nada.

        #region DEPRECATED

        ////Si se tiene que mostrar la acción por el hover calculamos el enemigo
        //if (_shouldRecalculate)
        //{
        //    pathToObjective.Clear();

        //    SearchingObjectivesToAttackShowActionPathFinding();
        //    if (myCurrentObjectiveTile != null)
        //    {
        //        //Cada enemigo realiza su propio path
        //        LM.TM.CalculatePathForMovementCost(myCurrentObjectiveTile.tileX, myCurrentObjectiveTile.tileZ, false);

        //        //No vale con igualar pathToObjective= LM.TM.currentPath porque entonces toma una referencia de la variable no de los valores.
        //        //Esto significa que si LM.TM.currentPath cambia de valor también lo hace pathToObjective
        //        for (int i = 0; i < LM.TM.currentPath.Count; i++)
        //        {
        //            pathToObjective.Add(LM.TM.currentPath[i]);
        //        }
        //    }
        //}

        ////Si se va a mostrar la acción en el turno enemigo entonces no recalculo y directamente enseño la acción.
        ////Esta parte es común para cuando se hace desde el hover como cuando se hace en turno enemigo.
        //if (myCurrentObjectiveTile != null)
        //{
        //    myLineRenderer.positionCount = 0;

        //    if (pathToObjective.Count - 2 > movementUds)
        //    {
        //        limitantNumberOfTilesToMove = movementUds;
        //    }
        //    else
        //    {
        //        limitantNumberOfTilesToMove = pathToObjective.Count - 2;
        //    }

        //    myLineRenderer.enabled = true;

        //    if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions && pathToObjective.Count > 2)
        //    {
        //        sombraHoverUnit.SetActive(true);
        //    }

        //    //Coge
        //    myLineRenderer.positionCount += (limitantNumberOfTilesToMove + 1);

        //    //myLineRenderer.SetVertexCount(LM.TM.currentPath.Count);

        //    for (int i = 0; i < limitantNumberOfTilesToMove + 1; i++)
        //    {
        //        Vector3 pointPosition = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);

        //        if (i < pathToObjective.Count - 1)
        //        {
        //            myLineRenderer.SetPosition(i, pointPosition);

        //            if (LM.currentLevelState == LevelManager.LevelState.ProcessingPlayerActions)
        //            {
        //                sombraHoverUnit.transform.position = pointPosition;
        //                if ((pathToObjective[i]) == currentUnitsAvailableToAttack[0].myCurrentTile)
        //                {

        //                    //CalculateDamagePreviousAttack(currentUnitsAvailableToAttack[0], this, pathToObjective[1]);
        //                }
        //                else
        //                {

        //                    damageWithMultipliersApplied = -999;
        //                }

        //                Vector3 positionToLook = new Vector3(myCurrentObjective.transform.position.x, myCurrentObjective.transform.position.y + 0.5f, myCurrentObjective.transform.position.z);
        //                sombraHoverUnit.transform.DOLookAt(positionToLook, 0, AxisConstraint.Y);
        //            }
        //        }
        //    }

        //    ///En el gigante es importante que esta función vaya después de colocar la sombra. Por si acaso asegurarse de que este if nunca se pone antes que el reposicionamiento de la sombra

        //    //A pesar de que ya se llama a esta función desde el levelManager en caso de hover, si se tiene que mostrar porque el goblin está atacando se tiene que llamar desde aqui (ya que no pasa por el level manager)
        //    //Tiene que ser en falso porque si no pongo la condicion la función se cree que el tileya estaba pintado de antes
        //    if (!_shouldRecalculate)
        //    {
        //        ColorAttackTile();
        //    }
        //}

        #endregion
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
        //A pesar de que esta función está vacía, no se puede quitar por que hay que hacer override y que no haga nada.

        #region DEPRECATED

        //myCurrentObjective = null;
        //myCurrentObjectiveTile = null;

        ////Si no ha sido alertado compruebo si hay players al alcance que van a hacer que se despierte y se mueva
        //if (!haveIBeenAlerted)
        //{
        //    //Comprobar las unidades que hay en mi rango de acción
        //    unitsInRange = LM.TM.GetAllUnitsInRangeWithoutPathfinding(rangeOfAction, GetComponent<UnitBase>());

        //    for (int i = 0; i < unitsInRange.Count; i++)
        //    {
        //        if (unitsInRange[i].GetComponent<PlayerUnit>())
        //        {
        //            keepSearching = true;
        //            currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
        //            break;
        //        }
        //    }
        //}

        ////Si ha sido alertado compruebo simplemente hacia donde se va a mover
        //else
        //{
        //    //Determinamos el enemigo más cercano.
        //    //currentUnitsAvailableToAttack = LM.TM.OnlyCheckClosestPathToPlayer();
        //    currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(GetComponent<EnemyUnit>());
        //    //Debug.Log("Line 435 " + currentUnitsAvailableToAttack.Count);

        //    keepSearching = true;
        //}

        //if (keepSearching)
        //{
        //    if (currentUnitsAvailableToAttack.Count == 1)
        //    {
        //        myCurrentObjective = currentUnitsAvailableToAttack[0];
        //        myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
        //    }

        //    //Si hay varios enemigos a la misma distancia, se queda con el que tenga más unidades adyacentes
        //    else if (currentUnitsAvailableToAttack.Count > 1)
        //    {
        //        //Ordeno la lista de posibles objetivos según el número de unidades dyacentes
        //        currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
        //        {
        //            return (b.myCurrentTile.neighboursOcuppied).CompareTo(a.myCurrentTile.neighboursOcuppied);
        //        });

        //        //Elimino a todos los objetivos de la lista que no tengan el mayor número de enemigos adyacentes
        //        for (int i = currentUnitsAvailableToAttack.Count - 1; i > 0; i--)
        //        {
        //            if (currentUnitsAvailableToAttack[0].myCurrentTile.neighboursOcuppied > currentUnitsAvailableToAttack[i].myCurrentTile.neighboursOcuppied)
        //            {
        //                currentUnitsAvailableToAttack.RemoveAt(i);
        //            }
        //        }

        //        //Si sigue habiendo varios enemigos los ordeno segun la vida
        //        if (currentUnitsAvailableToAttack.Count > 1)
        //        {

        //            //Ordeno la lista de posibles objetivos de menor a mayor vida actual
        //            currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
        //            {
        //                return (a.currentHealth).CompareTo(b.currentHealth);

        //            });
        //        }

        //        myCurrentObjective = currentUnitsAvailableToAttack[0];
        //        myCurrentObjectiveTile = myCurrentObjective.myCurrentTile;
        //    }
        //}

        //keepSearching = false;

        #endregion
    }


    #endregion

    public override void FinishMyActions()
    {
        base.FinishMyActions();

        attackCountThisTurn = 0;
        coneUsed = false;
        normalAttackUsed = false;
        areaUsed = false;

        currentCooldownSoulSkill--;
    }

  

}
