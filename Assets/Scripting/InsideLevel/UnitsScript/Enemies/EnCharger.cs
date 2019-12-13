using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnCharger : EnemyUnit
{
    //Referencia al tile de daño que instancia en tier 2
    [SerializeField]
    public GameObject tileDamageRef;

    //Path de tiles a seguir hasta el objetivo
    [SerializeField]
    public List<IndividualTiles> pathToObjective = new List<IndividualTiles>();

    public override void SearchingObjectivesToAttack()
    {
        if (isDead || hasAttacked)
        {
            Debug.Log("dead");
            myCurrentEnemyState = enemyState.Ended;
            return;
        }

        //Aggro de unidades hacer cuando tengamos la pasiva del caballero
        //

        //Busca enemigos en sus lineas
        CheckCharactersInLine();

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
        pathToObjective.Clear();

        //Arriba o abajo
        if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (currentUnitsAvailableToAttack[0].myCurrentTile.tileZ > myCurrentTile.tileZ)
            {
                for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                {
                    pathToObjective.Add(myCurrentTile.tilesInLineUp[i]);
                }

                //Roto al charger
                unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;

                StartCoroutine("MovingUnitAnimation");
            }
            //Abajo
            else
            {
                for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                {
                    pathToObjective.Add(myCurrentTile.tilesInLineDown[i]);
                }

                //Roto al charger

                unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;

                StartCoroutine("MovingUnitAnimation");
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
                    pathToObjective.Add(myCurrentTile.tilesInLineRight[i]);
                }

                //Roto al charger

                unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;

                StartCoroutine("MovingUnitAnimation");
            }
            //Izquierda
            else
            {
                for (int i = 0; i <= furthestAvailableUnitDistance; i++)
                {
                    pathToObjective.Add(myCurrentTile.tilesInLineLeft[i]);
                }

                //Roto al charger

                unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;

                StartCoroutine("MovingUnitAnimation");

            }
        }
    }

    IEnumerator MovingUnitAnimation()
    {
        myCurrentEnemyState = enemyState.Waiting;

        //Animación de movimiento
        for (int j = 0; j < pathToObjective.Count; j++)
        {
            //Calcula el vector al que se tiene que mover.
            currentTileVectorToMove = pathToObjective[j].transform.position;

            //Muevo y roto a la unidad
            transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

            //Espera entre casillas
            yield return new WaitForSeconds(timeMovementAnimation);

            //Si es tier 2 instancia fuego
            if (myTierLevel == TierLevel.Level2)
            {
                if (j > 0)
                {
                    Instantiate(tileDamageRef, pathToObjective[j - 1].transform.position, tileDamageRef.transform.rotation);
                }
            }
        }

        //Espero después de moverme para que no vaya demasiado rápido
        yield return new WaitForSeconds(timeWaitAfterMovement);

        //Actualizo toda la información al terminar de moverme
        hasMoved = true;
        movementParticle.SetActive(false);

        if (furthestAvailableUnitDistance >= 0)
        {
            UpdateInformationAfterMovement(pathToObjective[furthestAvailableUnitDistance]);
        }

        //Hago daño a la unidad
        DoDamage(currentUnitsAvailableToAttack[0]);
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
                        myCurrentTile.tilesInLineUp[i].ColorAttack();
                        //Roto al charger
                        shaderHover.transform.DORotate(new Vector3(0, 0, 0), 0);

                    }
                    else
                    {
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
                        myCurrentTile.tilesInLineDown[i].ColorAttack();
                        shaderHover.transform.DORotate(new Vector3(0, 180, 0), 0);
                    }
                    else
                    {
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
                        myCurrentTile.tilesInLineRight[i].ColorAttack();
                        shaderHover.transform.DORotate(new Vector3(0, 90, 0), 0);
                    }
                    else
                    {
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
                        myCurrentTile.tilesInLineLeft[i].ColorAttack();
                        shaderHover.transform.DORotate(new Vector3(0, -90, 0), 0);
                    }
                    else
                    {
                        myCurrentTile.tilesInLineLeft[i].ColorDesAttack();
                    }
                }
            }
        }
    }

    public override void CheckCharactersInLine()
    {
        if (!isDead)
        {
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

                    //Break ya que sólo me interesa la primera unidad de la linea
                    break;
                }
            }
        }
    }

    //Esta función sirve para que busque los objetivos a atacar pero sin que haga cambios en el turn state del enemigo
    public override void SearchingObjectivesToAttackShowActionPathFinding()
    {    
       
    }
}
