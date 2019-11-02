using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnBalista : EnemyUnit
{
    //Bool que indica si la balista ha preparado el ataque para atacar en el siguiente turno.
    [SerializeField]
    private bool isAttackPrepared = false;

    [SerializeField]
    private bool isMovingToHisRight;

    public override void SearchingObjectivesToAttack()
    {
        if (isAttackPrepared)
        {
            //Disparas
            myCurrentEnemyState = enemyState.Attacking;
        }

        else
        {
            //Buscas
            CheckCharactersInLine();
            if (currentUnitsAvailableToAttack.Count > 0)
            {
                //Resto uno para mover a la unidad al tile anterior al que está ocupado por el personaje.
                furthestAvailableUnitDistance -= 1;

                myCurrentEnemyState = enemyState.Attacking;
            }

            else
            {
                myCurrentEnemyState = enemyState.Moving;
            }
        }
    }

    public override void MoveUnit()
    {
        //En función de a donde este mirando su derecha o su izquierda cambia
        if (currentFacingDirection == FacingDirection.North)
        {
            if (isMovingToHisRight)
            {
                //FALTA AÑADIR QUE SE GIRE AL CHOCAR CON BORDES, OBSTÁCULOS, VACÍO Y UNIDAD


                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height + 1, myCurrentTile.tilesInLineRight[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineRight[0];
                myCurrentTile.unitOnTile = this;
            }

            if (!isMovingToHisRight)
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height + 1, myCurrentTile.tilesInLineLeft[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineLeft[0];
                myCurrentTile.unitOnTile = this;
            }

        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            if (isMovingToHisRight)
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[0].tileX, myCurrentTile.tilesInLineLeft[0].height + 1, myCurrentTile.tilesInLineLeft[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineLeft[0];
                myCurrentTile.unitOnTile = this;
            }

            else
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[0].tileX, myCurrentTile.tilesInLineRight[0].height + 1, myCurrentTile.tilesInLineRight[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineRight[0];
                myCurrentTile.unitOnTile = this;
            }
        }

        else if (currentFacingDirection == FacingDirection.East)
        {
            if (isMovingToHisRight)
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height + 1, myCurrentTile.tilesInLineDown[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineDown[0];
                myCurrentTile.unitOnTile = this;
            }

            else
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height + 1, myCurrentTile.tilesInLineUp[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineUp[0];
                myCurrentTile.unitOnTile = this;
            }
        }

        else if (currentFacingDirection == FacingDirection.West)
        {
            if (isMovingToHisRight)
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[0].tileX, myCurrentTile.tilesInLineUp[0].height + 1, myCurrentTile.tilesInLineUp[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineUp[0];
                myCurrentTile.unitOnTile = this;   
            }

            else
            {
                //Muevo a la balista
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[0].tileX, myCurrentTile.tilesInLineDown[0].height + 1, myCurrentTile.tilesInLineDown[0].tileZ);
                transform.DOMove(currentTileVectorToMove, timeMovementAnimation);

                //Actualizo las variables de los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineDown[0];
                myCurrentTile.unitOnTile = this;
            }
        }

        myCurrentEnemyState = enemyState.Searching;
    }

    public override void Attack()
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
                        myCurrentTile.tilesInLineUp[i].unitOnTile.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied));
                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                for (int i = 0; i < myCurrentTile.tilesInLineDown.Count; i++)
                {
                    if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                    {
                        myCurrentTile.tilesInLineDown[i].unitOnTile.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied));
                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                for (int i = 0; i < myCurrentTile.tilesInLineRight.Count; i++)
                {
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                    {
                        myCurrentTile.tilesInLineRight[i].unitOnTile.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied));
                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                for (int i = 0; i < myCurrentTile.tilesInLineLeft.Count; i++)
                {
                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                    {
                        myCurrentTile.tilesInLineLeft[i].unitOnTile.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied));
                    }
                }
            }

            isAttackPrepared = false;
            myCurrentEnemyState = enemyState.Searching;
        }

        else
        {
            //Prepara ataque
            isAttackPrepared = true;

            //Colorear los tiles visualmente

            myCurrentEnemyState = enemyState.Ended;
        }
    }

    public override void FinishMyActions()
    {
        base.FinishMyActions();
    }
}
