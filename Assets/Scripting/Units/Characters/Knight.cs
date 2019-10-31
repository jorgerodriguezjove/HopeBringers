using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerUnit
{
    #region VARIABLES

    //STATS EXCLUSIVOS DE LA UNIDAD

    [SerializeField]
    int tilesToPush;

    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        Debug.Log("ataco");

        base.Attack(unitToAttack);

        if (currentFacingDirection == FacingDirection.North)
        {
            unitToAttack.MoveByPush(tilesToPush, myCurrentTile.tilesInLineUp);
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            unitToAttack.MoveByPush(tilesToPush, myCurrentTile.tilesInLineDown);
        }

        else if(currentFacingDirection == FacingDirection.East)
        {
            unitToAttack.MoveByPush(tilesToPush, myCurrentTile.tilesInLineRight);
        }

        else if(currentFacingDirection == FacingDirection.West)
        {
            unitToAttack.MoveByPush(tilesToPush, myCurrentTile.tilesInLineLeft);
        }
    }
}
