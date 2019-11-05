using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnCharger : EnemyUnit
{
    public override void SearchingObjectivesToAttack()
    {
        //Aggro de unidades hacer cuando tengamos la pasiva del caballero
        //

        //Busca enemigos en sus lineas
        CheckCharactersInLine();

        Debug.Log(currentUnitsAvailableToAttack.Count);

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

    public override void Attack()
    {
        //Arriba o abajo
        if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (currentUnitsAvailableToAttack[0].myCurrentTile.tileZ > myCurrentTile.tileZ)
            {
                //Muevo al charger
                if (furthestAvailableUnitDistance > 0)
                {
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[furthestAvailableUnitDistance].tileX, myCurrentTile.tilesInLineUp[furthestAvailableUnitDistance].height + 1, myCurrentTile.tilesInLineUp[furthestAvailableUnitDistance].tileZ);
                    transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                    //Actualizo las variables de los tiles
                    myCurrentTile.unitOnTile = null;
                    myCurrentTile = myCurrentTile.tilesInLineUp[furthestAvailableUnitDistance];
                    myCurrentTile.unitOnTile = this;
                }

                //Roto al charger
                transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;

                //Hago daño a la unidad
                DoDamage(currentUnitsAvailableToAttack[0]);
            }
            //Abajo
            else
            {
                if (furthestAvailableUnitDistance > 0)
                {
                    //Muevo al charger
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[furthestAvailableUnitDistance].tileX, myCurrentTile.tilesInLineDown[furthestAvailableUnitDistance].height + 1, myCurrentTile.tilesInLineDown[furthestAvailableUnitDistance].tileZ);
                    transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                    //Actualizo las variables de los tiles
                    myCurrentTile.unitOnTile = null;
                    myCurrentTile = myCurrentTile.tilesInLineDown[furthestAvailableUnitDistance];
                    myCurrentTile.unitOnTile = this;
                }

                //Roto al charger
                transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;

                //Hago daño a la unidad
                DoDamage(currentUnitsAvailableToAttack[0]);
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (currentUnitsAvailableToAttack[0].myCurrentTile.tileX > myCurrentTile.tileX)
            {
                if (furthestAvailableUnitDistance > 0)
                {
                    //Muevo al charger
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[furthestAvailableUnitDistance].tileX, myCurrentTile.tilesInLineRight[furthestAvailableUnitDistance].height + 1, myCurrentTile.tilesInLineRight[furthestAvailableUnitDistance].tileZ);
                    transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                    //Actualizo las variables de los tiles
                    myCurrentTile.unitOnTile = null;
                    myCurrentTile = myCurrentTile.tilesInLineRight[furthestAvailableUnitDistance];
                    myCurrentTile.unitOnTile = this;
                }

                //Roto al charger
                transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;

                //Hago daño a la unidad
                DoDamage(currentUnitsAvailableToAttack[0]);
            }
            //Izquierda
            else
            {
                if (furthestAvailableUnitDistance > 0)
                {
                    //Muevo al charger
                    currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[furthestAvailableUnitDistance].tileX, myCurrentTile.tilesInLineLeft[furthestAvailableUnitDistance].height + 1, myCurrentTile.tilesInLineLeft[furthestAvailableUnitDistance].tileZ);
                    transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                    //Actualizo las variables de los tiles
                    myCurrentTile.unitOnTile = null;
                    myCurrentTile = myCurrentTile.tilesInLineLeft[furthestAvailableUnitDistance];
                    myCurrentTile.unitOnTile = this;
                }
                  
                //Roto al charger
                transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;

                //Hago daño a la unidad
                DoDamage(currentUnitsAvailableToAttack[0]);
            }
        }

        myCurrentEnemyState = enemyState.Ended;
    }

    public override void FinishMyActions()
    {
        base.FinishMyActions();
    }
}
