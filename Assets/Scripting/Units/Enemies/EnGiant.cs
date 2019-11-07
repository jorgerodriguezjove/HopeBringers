using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnGiant : EnemyUnit
{
    //Guardo la primera unidad en la lista de currentUnitAvailbleToAttack para  no estar llamandola constantemente
    private UnitBase myCurentObjective;
    private IndividualTiles myCurrentObjectiveTile;

    public override void SearchingObjectivesToAttack()
    {
        //Determinamos el enemigo más cercano.
        currentUnitsAvailableToAttack = LM.CheckEnemyPathfinding(range, this);

        //Si no hay enemigos termina su turno
        if (currentUnitsAvailableToAttack.Count == 0)
        {
            myCurrentEnemyState = enemyState.Ended;
        }

        else if (currentUnitsAvailableToAttack.Count == 1)
        {
            myCurentObjective = currentUnitsAvailableToAttack[0];
            myCurrentObjectiveTile = myCurentObjective.myCurrentTile;
            myCurrentEnemyState = enemyState.Attacking;
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
            for (int i = currentUnitsAvailableToAttack.Count-1; i > 0; i--)
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

            myCurentObjective = currentUnitsAvailableToAttack[0];
            myCurrentObjectiveTile = myCurentObjective.myCurrentTile;

            myCurrentEnemyState = enemyState.Attacking;
        }
    }

    public override void Attack()
    {
        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            //Si mi objetivo es adyacente a mi le ataco
            if (myCurrentTile.neighbours[i].unitOnTile != null && myCurrentTile.neighbours[i].unitOnTile == currentUnitsAvailableToAttack[0])
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

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineRight[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineLeft[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
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

                    //Atacar al enemigo
                    DoDamage(currentUnitsAvailableToAttack[0]);

                    //Comprobar si a sus lados hay unidades
                    if (myCurrentObjectiveTile.tilesInLineUp[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }

                    if (myCurrentObjectiveTile.tilesInLineDown[0] != null && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                }

                myCurrentEnemyState = enemyState.Ended;
                break;
            }

            else
            {
               if (!hasMoved)
               {
                    myCurrentEnemyState = enemyState.Moving;
               }
               else
               {
                    myCurrentEnemyState = enemyState.Ended;
               }
            }
        }
    }

    public override void MoveUnit()
    {
        //Arriba o abajo
        if (myCurrentObjectiveTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (myCurrentObjectiveTile.tileZ > myCurrentTile.tileZ)
            {
                if (!myCurrentTile.tilesInLineUp[0].isEmpty && !myCurrentTile.tilesInLineUp[0].isObstacle && myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                {
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height + 1, myCurrentTile.tilesInLineUp[0].tileZ);
                    MovementLogic(myCurrentTile.tilesInLineUp);
                    RotateLogic(FacingDirection.North);
                }
                else
                {
                    myCurrentEnemyState = enemyState.Ended;
                }
            }
            //Abajo
            else
            {
                if (!myCurrentTile.tilesInLineDown[0].isEmpty && !myCurrentTile.tilesInLineDown[0].isObstacle && myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                {
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height + 1, myCurrentTile.tilesInLineDown[0].tileZ);
                    MovementLogic(myCurrentTile.tilesInLineDown);
                    RotateLogic(FacingDirection.South);
                }

                else
                {
                    myCurrentEnemyState = enemyState.Ended;
                }
            }
        }
        //Izquierda o derecha
        else if (myCurrentObjectiveTile.tileZ == myCurrentTile.tileZ)
        {
            //Derecha
            if (myCurrentObjectiveTile.tileX > myCurrentTile.tileX)
            {
                if (!myCurrentTile.tilesInLineRight[0].isEmpty && !myCurrentTile.tilesInLineRight[0].isObstacle && myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                {
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height + 1, myCurrentTile.tilesInLineRight[0].tileZ);
                    MovementLogic(myCurrentTile.tilesInLineRight);
                    RotateLogic(FacingDirection.East);
                }

                else
                {
                    myCurrentEnemyState = enemyState.Ended;
                }
            }
            //Izquierda
            else
            {
                if (!myCurrentTile.tilesInLineLeft[0].isEmpty && !myCurrentTile.tilesInLineLeft[0].isObstacle && myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                {
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height + 1, myCurrentTile.tilesInLineLeft[0].tileZ);
                    MovementLogic(myCurrentTile.tilesInLineLeft);
                    RotateLogic(FacingDirection.West);
                }

                else
                {
                    myCurrentEnemyState = enemyState.Ended;
                }
            }
        }

        //Diagonales
        else
        {
            //Diag derecha
            if (myCurrentObjectiveTile.tileX > myCurrentTile.tileX)
            {
               //Diag Arriba Derecha
               if(myCurrentObjectiveTile.tileZ > myCurrentTile.tileZ)
               {
                    //Si el tile de arriba esta libre me muevo a él
                    if (!myCurrentTile.tilesInLineUp[0].isEmpty && !myCurrentTile.tilesInLineUp[0].isObstacle && myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height + 1, myCurrentTile.tilesInLineUp[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineUp);
                        RotateLogic(FacingDirection.North);
                    }

                    //Si no compruebo el de la derecha para intentar moverme a él.
                    else if (!myCurrentTile.tilesInLineRight[0].isEmpty && !myCurrentTile.tilesInLineRight[0].isObstacle && myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height + 1, myCurrentTile.tilesInLineRight[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineRight);
                        RotateLogic(FacingDirection.East);
                    }

                    else
                    {
                        myCurrentEnemyState = enemyState.Ended;
                    }
                }

               //Diag Abajo Derecha
               else
               {
                    //Si el tile de abajo esta libre me muevo a él
                    if (!myCurrentTile.tilesInLineDown[0].isEmpty && !myCurrentTile.tilesInLineDown[0].isObstacle && myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height + 1, myCurrentTile.tilesInLineDown[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineDown);
                        RotateLogic(FacingDirection.South);
                    }

                    //Si el tile de arriba esta libre me muevo a él
                    else if (!myCurrentTile.tilesInLineRight[0].isEmpty && !myCurrentTile.tilesInLineRight[0].isObstacle && myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height + 1, myCurrentTile.tilesInLineRight[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineRight);
                        RotateLogic(FacingDirection.East);
                    }

                    else
                    {
                        myCurrentEnemyState = enemyState.Ended;
                    }
                }
            }
            else
            {
                //Diag Arriba Izquierda
                if (myCurrentObjectiveTile.tileZ > myCurrentTile.tileZ)
                {
                    //Si el tile de arriba esta libre me muevo a él
                    if (!myCurrentTile.tilesInLineUp[0].isEmpty && !myCurrentTile.tilesInLineUp[0].isObstacle && myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height + 1, myCurrentTile.tilesInLineUp[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineUp);
                        RotateLogic(FacingDirection.North);
                    }

                    //Si el tile de arriba esta libre me muevo a él
                    else if (!myCurrentTile.tilesInLineLeft[0].isEmpty && !myCurrentTile.tilesInLineLeft[0].isObstacle && myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height + 1, myCurrentTile.tilesInLineLeft[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineLeft);
                        RotateLogic(FacingDirection.West);
                    }

                    else
                    {
                        myCurrentEnemyState = enemyState.Ended;
                    }
                }

                //Diag Abajo Izquierda
                else
                {
                    //Si el tile de abajo esta libre me muevo a él
                    if (!myCurrentTile.tilesInLineDown[0].isEmpty && !myCurrentTile.tilesInLineDown[0].isObstacle && myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height + 1, myCurrentTile.tilesInLineDown[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineDown);
                        RotateLogic(FacingDirection.South);
                    }

                    //Si el tile de arriba esta libre me muevo a él
                    else if (!myCurrentTile.tilesInLineLeft[0].isEmpty && !myCurrentTile.tilesInLineLeft[0].isObstacle && myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                    {
                        currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height + 1, myCurrentTile.tilesInLineLeft[0].tileZ);
                        MovementLogic(myCurrentTile.tilesInLineLeft);
                        RotateLogic(FacingDirection.West);
                    }

                    else
                    {
                        myCurrentEnemyState = enemyState.Ended;
                    }
                }
            }
        }


        //Comprueba la dirección en la que se encuentra el objetivo.
        //Si se encuentra justo en el mismo eje (movimiento torre), el gigante avanza en esa dirección.
        //Si se encuentra un bloqueo se queda en el sitio intentando avanzar contra el bloqueo.

        //Sin embargo si el objetivo se encuentra en diágonal (por ejemplo arriba a la derecha)
        //El gigante tiene que decidir una de las dos (DISEÑO REGLAS DE PATHFINDING)
        //Una vez decidida avanza en esta dirección hasta que no pueda más y si sigue estando en diagonal, avanza en la que había descartado antes.
        
        //Buscar de nuevo si puedo pegarle

        myCurrentEnemyState = enemyState.Searching;
    }

    //Lógica actual del movimiento. Básicamente es el encargado de mover al modelo y setear las cosas
    private void MovementLogic(List<IndividualTiles> ListWithNewTile)
    {
        //Muevo al gigante
        transform.DOMove(currentTileVectorToMove, timeMovementAnimation);
        
        //Actualizo las variables de los tiles
        myCurrentTile.unitOnTile = null;
        myCurrentTile = ListWithNewTile[0];
        myCurrentTile.unitOnTile = this;

        //Aviso de que se ha movido
        hasMoved = true;
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

}
