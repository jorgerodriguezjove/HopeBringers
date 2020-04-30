using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnCharger : EnemyUnit
{
    //Referencia al tile de daño que instancia en tier 2
    [SerializeField]
    public GameObject tileDamageRef;

    //Icono que aparece sobre los tiles donde va a dejar fuego.
    [SerializeField]
    public GameObject iconFireShadow;

    [HideInInspector]
    public List<GameObject> listIconsFire;

    public override void SearchingObjectivesToAttack()
    {
        if (isDead || hasAttacked)
        {
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        //Busca enemigos en sus lineas
        CheckCharactersInLine(false, null);

        //Si coincide que hay varios personajes a la misma distancia, me quedo con el que tiene menos vida
        if (currentUnitsAvailableToAttack.Count > 1)
        {
            //Ordeno la lista de posibles objetivos de menor a mayor vida actual
            currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
            {
                return (a.currentHealth).CompareTo(b.currentHealth);
                
            });
        }
      
        if (currentUnitsAvailableToAttack.Count > 0)
        {
            //Resto uno para mover a la unidad al tile anterior al que está ocupado por el personaje.
            furthestAvailableUnitDistance -= 1;

            myCurrentEnemyState = enemyState.Attacking;
        }

        else
        {
            myCurrentEnemyState = enemyState.Ended;
        }
    }

    //HACER COMO EN LA BALISTA Y OPTIMIZAR EL CHECK PARA QUE GUARDE LOS TILES QUE AFECTA EN UNA LISTA EN VEZ DE ESTAR BUSCANDO EN TILES EN LINEA TODO EL RATO.
    //JORGE RECUERDA MIRAR ESTO----------------------------------------------------------------------------------------------
    public override void Attack()
    {
        //Si no he sido alertado, activo mi estado de alerta.
        if (!haveIBeenAlerted)
        {
            AlertEnemy();
        }

        movementParticle.SetActive(true);

        //Importante este clear no puede ir dentro de SpecialCheckRotation();
        pathToObjective.Clear();
        SpecialCheckRotation(myCurrentTile, true);

        //Seteo la rotación decidida en SpecialCheckRotation();
        unitModel.transform.DORotate(rotationChosenAfterMovement, timeDurationRotation);
        currentFacingDirection = facingDirectionAfterMovement;

        StartCoroutine("MovingUnitAnimation");
    }

    //ESTE ES DIFERENTE AL DEL ENEMY UNIT PORQUE HABRÍA QUE CAMBIAR VARIAS COSAS DE LA LÓGICA PARA QUE FUNCIONASE EL OTRO.
    //La balista y el charger al ser de los primeros y ser bastante distintos voy a intentar no tocarlos mucho.
    new IEnumerator MovingUnitAnimation()
    {
        myCurrentEnemyState = enemyState.Waiting;

        //Animación de movimiento
        for (int j = 0; j < pathToObjective.Count; j++)
        {
            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = pathToObjective[j].transform.position;

            //Muevo y roto a la unidad
            transform.DOMove(currentTileVectorToMove, currentTimeForMovement);

            //Espera entre casillas
            yield return new WaitForSeconds(currentTimeForMovement);

            //Si es tier 2 instancia fuego
            if (myTierLevel == TierLevel.Level2)
            {
                if (j > 0)
                {
                    Instantiate(tileDamageRef, pathToObjective[j - 1].transform.position, tileDamageRef.transform.rotation);
                }
            }
        }

        base.Attack();

        //Actualizo toda la información al terminar de moverme
        hasMoved = true;
        movementParticle.SetActive(false);

        if (furthestAvailableUnitDistance >= 0)
        {
            UpdateInformationAfterMovement(pathToObjective[furthestAvailableUnitDistance]);
        }

        //Hago daño a la unidad
        DoDamage(currentUnitsAvailableToAttack[0]);

        //Push
        if (currentFacingDirection == FacingDirection.North)
        {
            currentUnitsAvailableToAttack[0].ExecutePush(1, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            currentUnitsAvailableToAttack[0].ExecutePush(1, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            currentUnitsAvailableToAttack[0].ExecutePush(1, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            currentUnitsAvailableToAttack[0].ExecutePush(1, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
        }


        myCurrentEnemyState = enemyState.Ended;
    }

    public override void FinishMyActions()
    {
        base.FinishMyActions();
    }

    //Función que pinta o despinta los tiles a los que está atcando la ballesta
    public void FeedbackTilesToAttack(bool shouldColorTiles)
    {
       
        pathToObjective.Clear();    
        myLineRenderer.positionCount = 0;     
     

        if (currentUnitsAvailableToAttack.Count > 0){

            

            //Arriba o abajo
            if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX == myCurrentTile.tileX)
            {
                //Arriba
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tileZ > myCurrentTile.tileZ)
                {
                    for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                    {
                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                        {
                            return;
                        }

                        pathToObjective.Add(myCurrentTile.tilesInLineUp[i]);

                        if (shouldColorTiles)
                        {
                            myLineRenderer.enabled = true;

                            myCurrentTile.tilesInLineUp[i].ColorAttack();
                           
                            shaderHover.transform.DORotate(new Vector3(0, 0, 0), 0);                         
                        }
                        else
                        {
                            myLineRenderer.enabled = false;
                            myCurrentTile.tilesInLineUp[i].ColorDesAttack();
                        }
                    }
                }
                //Abajo
                else
                {
                    for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                    {
                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                        {
                            return;
                        }

                        pathToObjective.Add(myCurrentTile.tilesInLineDown[i]);

                        if (shouldColorTiles)
                        {
                            myLineRenderer.enabled = true;

                            myCurrentTile.tilesInLineDown[i].ColorAttack();
                            shaderHover.transform.DORotate(new Vector3(0, 180, 0), 0);                            
                        }
                        else
                        {
                            myLineRenderer.enabled = false;
                            myCurrentTile.tilesInLineDown[i].ColorDesAttack();
                        }
                    }

                }
            }
            //Izquierda o derecha
            else
            {
                //Derecha
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX > myCurrentTile.tileX)
                {
                    for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                        {
                            return;
                        }

                        pathToObjective.Add(myCurrentTile.tilesInLineRight[i]);

                        if (shouldColorTiles)
                        {
                            myLineRenderer.enabled = true;
                            myCurrentTile.tilesInLineRight[i].ColorAttack();
                            shaderHover.transform.DORotate(new Vector3(0, 90, 0), 0);

                          
                        }
                        else
                        {
                            myLineRenderer.enabled = false;
                            myCurrentTile.tilesInLineRight[i].ColorDesAttack();
                        }
                    }
                }
                //Izquierda
                else
                {
                    for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                    {
                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                        {
                            return;
                        }

                        pathToObjective.Add(myCurrentTile.tilesInLineLeft[i]);

                        if (shouldColorTiles)
                        {
                            myLineRenderer.enabled = true;
                            myCurrentTile.tilesInLineLeft[i].ColorAttack();
                            shaderHover.transform.DORotate(new Vector3(0, -90, 0), 0);

                           


                        }
                        else
                        {
                            myLineRenderer.enabled = false;
                            myCurrentTile.tilesInLineLeft[i].ColorDesAttack();
                        }
                    }
                }


              

            }

           

        }

        else if (currentUnitsAvailableToAttack.Count == 0)
        {
            if (myCurrentTile.tilesInLineUp.Count > 0)
            {                                           
                for (int i = 0; i < myCurrentTile.tilesInLineUp.Count; i++)
                {
                    if (shouldColorTiles)
                    {
                       
                        
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.tilesInLineUp[i - 1].height) > maxHeightDifferenceToAttack 
                            || (myCurrentTile.tilesInLineUp[i].isObstacle)
                            || (myCurrentTile.tilesInLineUp[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineUp[i].ColorAttack();
                    }
                    else
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.tilesInLineUp[i - 1].height) > maxHeightDifferenceToAttack
                              || (myCurrentTile.tilesInLineUp[i].isObstacle)
                              || (myCurrentTile.tilesInLineUp[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineUp[i].ColorDesAttack();
                    }
                }
            }

            if (myCurrentTile.tilesInLineDown.Count > 0)
            {
                for (int i = 0; i < myCurrentTile.tilesInLineDown.Count; i++)
                {
                    if (shouldColorTiles)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.tilesInLineDown[i - 1].height) > maxHeightDifferenceToAttack 
                            || (myCurrentTile.tilesInLineDown[i].isObstacle)
                             || (myCurrentTile.tilesInLineDown[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineDown[i].ColorAttack();
                    }
                    else
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.tilesInLineDown[i - 1].height) > maxHeightDifferenceToAttack
                              || (myCurrentTile.tilesInLineDown[i].isObstacle)
                               || (myCurrentTile.tilesInLineDown[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineDown[i].ColorDesAttack();
                    }
                }

            }

            if (myCurrentTile.tilesInLineRight.Count > 0)
            {

                for (int i = 0; i < myCurrentTile.tilesInLineRight.Count; i++)
                {
                    if (shouldColorTiles)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.tilesInLineRight[i - 1].height) > maxHeightDifferenceToAttack 
                            || (myCurrentTile.tilesInLineRight[i].isObstacle)
                            || (myCurrentTile.tilesInLineRight[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineRight[i].ColorAttack();
                    }
                    else
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.tilesInLineRight[i - 1].height) > maxHeightDifferenceToAttack
                              || (myCurrentTile.tilesInLineRight[i].isObstacle)
                              || (myCurrentTile.tilesInLineRight[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineRight[i].ColorDesAttack();
                    }
                }

            }

            if (myCurrentTile.tilesInLineLeft.Count > 0)
            {

                for (int i = 0; i < myCurrentTile.tilesInLineLeft.Count; i++)
                {
                    if (shouldColorTiles)
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.tilesInLineLeft[i - 1].height) > maxHeightDifferenceToAttack 
                           || (myCurrentTile.tilesInLineLeft[i].isObstacle)
                           || (myCurrentTile.tilesInLineLeft[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineLeft[i].ColorAttack();
                    }
                    else
                    {
                        if (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.tilesInLineLeft[i - 1].height) > maxHeightDifferenceToAttack
                              || (myCurrentTile.tilesInLineLeft[i].isObstacle)
                              || (myCurrentTile.tilesInLineLeft[i].isEmpty))
                        {
                            break;
                        }
                        myCurrentTile.tilesInLineLeft[i].ColorDesAttack();
                    }
                }

            }
        }



    }

    public List<IndividualTiles> tilesBehindObjective = new List<IndividualTiles>();

    //Esta dirección no se aplica al charger a nivel lógico, simplemente me sirve para saber en que dirección esta el objetivo.
    FacingDirection temporalFacingDirectionWhileHover;

    //El bool y el tile es solo para la balista pero comparte la funcion con el charger y tiene que tenerlo
    public override void CheckCharactersInLine(bool _NoUsarEsteBooleano, IndividualTiles _noUsarEsteTile)
    {
        if (!isDead)
        {
            tilesBehindObjective.Clear();
            currentUnitsAvailableToAttack.Clear();

            //Busco objetivos en los tiles de arriba

            //Seteo número de tiles a comprobar en función del rango y del número de tiles disponibles
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
                //Paro de comprobar si hay un obstáculo, un tile vacío o una unidad enemiga.
                if (myCurrentTile.tilesInLineUp[i].isObstacle   ||
                    myCurrentTile.tilesInLineUp[i].isEmpty      ||
                    (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<EnemyUnit>()) ||
                    (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.tilesInLineUp[i - 1].height) > maxHeightDifferenceToMove))
                {
                    break;
                }

                //Si por el contrario encuentro una unidad del jugador a mi altura, la añado a la lista de objetivos (en el resto de direcciones antes compruebo si es la unidad más lejana)
                else if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && myCurrentTile.tilesInLineUp[i].unitOnTile.GetComponent<PlayerUnit>())
                {
                    //Almaceno la primera unidad en la lista de posibles unidades.
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    furthestAvailableUnitDistance = i;

                    if (i+1 < myCurrentTile.tilesInLineUp.Count)
                    {
                        tilesBehindObjective.Add(myCurrentTile.tilesInLineUp[i+1]);
                    }


                    temporalFacingDirectionWhileHover = FacingDirection.North;

                    //Break ya que sólo me interesa la primera unidad de la linea
                    break;
                }
            }

            //Tiles derecha
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
                if (myCurrentTile.tilesInLineRight[i].isObstacle ||
                    myCurrentTile.tilesInLineRight[i].isEmpty ||
                    (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<EnemyUnit>()) ||
                    (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.tilesInLineRight[i - 1].height) > maxHeightDifferenceToMove))
                {
                    break;
                }

                else if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && myCurrentTile.tilesInLineRight[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Compruebo que unidad está más lejos
                    if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                    {
                        currentUnitsAvailableToAttack.Clear();
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        furthestAvailableUnitDistance = i;
                    }

                    //Si tienen la misma distancia almaceno a las dos
                    else if (furthestAvailableUnitDistance == i)
                    {
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    }

                    if (i + 1 < myCurrentTile.tilesInLineRight.Count)
                    {
                        tilesBehindObjective.Add(myCurrentTile.tilesInLineRight[i + 1]);
                    }

                    temporalFacingDirectionWhileHover = FacingDirection.East;

                    //Break ya que sólo me interesa la primera unidad de la linea
                    break;
                }
            }

            //Tiles abajo
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
                if (myCurrentTile.tilesInLineDown[i].isObstacle ||
                    myCurrentTile.tilesInLineDown[i].isEmpty ||
                    (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<EnemyUnit>()) ||
                    (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.tilesInLineDown[i - 1].height) > maxHeightDifferenceToMove))
                {
                    break;
                }

                else if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && myCurrentTile.tilesInLineDown[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Compruebo que unidad está más lejos
                    if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                    {
                        currentUnitsAvailableToAttack.Clear();
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                        furthestAvailableUnitDistance = i;
                    }

                    //Si tienen la misma distancia almaceno a las dos
                    else if (furthestAvailableUnitDistance == i)
                    {
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    }

                    if (i + 1 < myCurrentTile.tilesInLineDown.Count)
                    {
                        tilesBehindObjective.Add(myCurrentTile.tilesInLineDown[i + 1]);
                    }

                    temporalFacingDirectionWhileHover = FacingDirection.South;

                    //Break ya que sólo me interesa la primera unidad de la linea
                    break;
                }
            }

            //Tiles abajo
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
                if (myCurrentTile.tilesInLineLeft[i].isObstacle ||
                   myCurrentTile.tilesInLineLeft[i].isEmpty ||
                   (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<EnemyUnit>()) ||
                   (i > 0 && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.tilesInLineLeft[i - 1].height) > maxHeightDifferenceToMove))
                {
                    break;
                }

                else if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && myCurrentTile.tilesInLineLeft[i].unitOnTile.GetComponent<PlayerUnit>() && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Compruebo que unidad está más lejos
                    if (currentUnitsAvailableToAttack.Count == 0 || furthestAvailableUnitDistance < i)
                    {
                        currentUnitsAvailableToAttack.Clear();
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        furthestAvailableUnitDistance = i;
                    }

                    //Si tienen la misma distancia almaceno a las dos
                    else if (furthestAvailableUnitDistance == i)
                    {
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    }

                    if (i + 1 < myCurrentTile.tilesInLineLeft.Count)
                    {
                        tilesBehindObjective.Add(myCurrentTile.tilesInLineLeft[i + 1]);
                    }

                    temporalFacingDirectionWhileHover = FacingDirection.West;

                    //Break ya que sólo me interesa la primera unidad de la linea
                    break;
                }
            }

            //Si coincide que hay varios personajes a la misma distancia, me quedo con el que tiene menos vida
            if (currentUnitsAvailableToAttack.Count > 1)
            {
                //Ordeno la lista de posibles objetivos de menor a mayor vida actual
                currentUnitsAvailableToAttack.Sort(delegate (UnitBase a, UnitBase b)
                {
                    return (a.currentHealth).CompareTo(b.currentHealth);

                });
            }
        }
    }

    IndividualTiles tileWhereObjectiveShadowWillEnd;

    public void ShowPushResult()
    {
        currentUnitsAvailableToAttack[0].shaderHover.SetActive(true);
        SetShadowRotation(currentUnitsAvailableToAttack[0]);


        if (temporalFacingDirectionWhileHover == FacingDirection.North)
        {
            tileWhereObjectiveShadowWillEnd = CalculatePushLogic(1, pathToObjective[pathToObjective.Count - 1].tilesInLineUp, damageMadeByPush, damageMadeByFall);

            currentUnitsAvailableToAttack[0].shaderHover.transform.position = tileWhereObjectiveShadowWillEnd.transform.position;

            tileWhereObjectiveShadowWillEnd.ColorAttack();
        }

        else if (temporalFacingDirectionWhileHover == FacingDirection.South)
        {
            tileWhereObjectiveShadowWillEnd = CalculatePushLogic(1, pathToObjective[pathToObjective.Count - 1].tilesInLineDown, damageMadeByPush, damageMadeByFall);

             currentUnitsAvailableToAttack[0].shaderHover.transform.position = tileWhereObjectiveShadowWillEnd.transform.position;

             tileWhereObjectiveShadowWillEnd.ColorAttack();
        }

        else if (temporalFacingDirectionWhileHover == FacingDirection.East)
        {
            tileWhereObjectiveShadowWillEnd = CalculatePushLogic(1, pathToObjective[pathToObjective.Count - 1].tilesInLineRight, damageMadeByPush, damageMadeByFall);
            currentUnitsAvailableToAttack[0].shaderHover.transform.position = tileWhereObjectiveShadowWillEnd.transform.position;
            tileWhereObjectiveShadowWillEnd.ColorAttack();
        }

        else if (temporalFacingDirectionWhileHover == FacingDirection.West)
        {
            tileWhereObjectiveShadowWillEnd = CalculatePushLogic(1, pathToObjective[pathToObjective.Count - 1].tilesInLineLeft, damageMadeByPush, damageMadeByFall);
            currentUnitsAvailableToAttack[0].shaderHover.transform.position = tileWhereObjectiveShadowWillEnd.transform.position;
            tileWhereObjectiveShadowWillEnd.ColorAttack();
        }
    }

    public void HideShowResult()
    {
        if (currentUnitsAvailableToAttack.Count >0 )
        {
            currentUnitsAvailableToAttack[0].shaderHover.SetActive(false);
            tileWhereObjectiveShadowWillEnd.ColorDesAttack();
        }
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {
        myLineRenderer.positionCount += (pathToObjective.Count);
        for (int j = 0; j < pathToObjective.Count; j++)
        {
            Vector3 pointPosition = new Vector3(pathToObjective[j].transform.position.x, pathToObjective[j].transform.position.y + 0.5f, pathToObjective[j].transform.position.z);
            if (j < pathToObjective.Count)
            {
                if (j == 0)
                {
                    myLineRenderer.SetPosition(0, myCurrentTile.transform.position);
                }
                else
                {
                    myLineRenderer.SetPosition(j, pointPosition);
                }
                
            }
        }
    }



    public void InstantiateIconsFire()
    {
        for (int i = 0; i < pathToObjective.Count; i++)
        {
            GameObject icon = Instantiate(iconFireShadow);
            icon.transform.position = new Vector3(pathToObjective[i].transform.position.x, pathToObjective[i].transform.position.y + 0.5f, pathToObjective[i].transform.position.z);
            listIconsFire.Add(icon);

        }
    }

    public void RemoveIconsFire()
    {
        for (int i = 0; i < listIconsFire.Count; i++)
        {
            Destroy(listIconsFire[i]);
        }

        listIconsFire.Clear();
    }
}
