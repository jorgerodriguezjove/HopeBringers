using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rogue : PlayerUnit
{
    #region VARIABLES

    //[Header("STATS DE CLASE")]

    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        if (unitToAttack.myCurrentTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (unitToAttack.myCurrentTile.tileZ > myCurrentTile.tileZ)
            {
                //Muevo al pícaro
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineUp[1].tileX, myCurrentTile.tilesInLineUp[1].height + 1, myCurrentTile.tilesInLineUp[1].tileZ);
                transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                //Cambio la rotación
                NewRotationAfterJump(unitToAttack.myCurrentTile);

                //Actualizo los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineUp[1];
                myCurrentTile.unitOnTile = this;
            }
            //Abajo
            else
            {
                //Muevo al pícaro
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineDown[1].tileX, myCurrentTile.tilesInLineDown[1].height + 1, myCurrentTile.tilesInLineDown[1].tileZ);
                transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                //Cambio la rotación
                NewRotationAfterJump(unitToAttack.myCurrentTile);

                //Actualizo los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineDown[1];
                myCurrentTile.unitOnTile = this;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (unitToAttack.myCurrentTile.tileX > myCurrentTile.tileX)
            {
                //Muevo al pícaro
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineRight[1].tileX, myCurrentTile.tilesInLineRight[1].height + 1, myCurrentTile.tilesInLineRight[1].tileZ);
                transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                //Cambio la rotación
                NewRotationAfterJump(unitToAttack.myCurrentTile);

                //Actualizo los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineRight[1];
                myCurrentTile.unitOnTile = this;
            }
            //Izquierda
            else
            {
                //Muevo al pícaro
                currentTileVectorToMove = new Vector3(myCurrentTile.tilesInLineLeft[1].tileX, myCurrentTile.tilesInLineLeft[1].height + 1, myCurrentTile.tilesInLineLeft[1].tileZ);
                transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                //Cambio la rotación
                NewRotationAfterJump(unitToAttack.myCurrentTile);

                //Actualizo los tiles
                myCurrentTile.unitOnTile = null;
                myCurrentTile = myCurrentTile.tilesInLineLeft[1];
                myCurrentTile.unitOnTile = this;
            }
        }
               
        //Hago daño
        DoDamage(unitToAttack);

        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }

    //Función genérica que comprueba la nueva dirección a la que debe mirar el pícaro tras saltar.
    public void NewRotationAfterJump(IndividualTiles tileWithEnemyAttacked)
    {
        if (tileWithEnemyAttacked.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (tileWithEnemyAttacked.tileZ > myCurrentTile.tileZ)
            {
                transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
            //Abajo
            else
            {
                transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileWithEnemyAttacked.tileX > myCurrentTile.tileX)
            {
                transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
            //Izquierda
            else
            {
                transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
        }
    }

}
