using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerUnit
{
    #region VARIABLES

    [Header("STATS DE CLASE")]

    [SerializeField]
    int tilesToPush;

    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        if (currentFacingDirection == FacingDirection.North)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush);
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush);
        }

        else if(currentFacingDirection == FacingDirection.East)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush);
        }

        else if(currentFacingDirection == FacingDirection.West)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush);
        }

        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }
}
