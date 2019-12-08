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
        hasAttacked = true;

        //Animación de ataque
        myAnimator.SetTrigger("Attack");

        //Hago daño
        DoDamage(unitToAttack);

        if (currentFacingDirection == FacingDirection.North)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
        }

        else if (currentFacingDirection == FacingDirection.South)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
        }

        else if(currentFacingDirection == FacingDirection.East)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
        }

        else if(currentFacingDirection == FacingDirection.West)
        {
            unitToAttack.CalculatePushPosition(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
        }

        SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {

        if (unitAttacker != null)
        {
            if (currentFacingDirection == FacingDirection.North && unitAttacker.currentFacingDirection == FacingDirection.South
          || currentFacingDirection == FacingDirection.East && unitAttacker.currentFacingDirection == FacingDirection.West
          || currentFacingDirection == FacingDirection.South && unitAttacker.currentFacingDirection == FacingDirection.North
          || currentFacingDirection == FacingDirection.West && unitAttacker.currentFacingDirection == FacingDirection.East)
            {
                //No recibe daño
                Debug.Log("bloqueado el ataque");
            }

            else
            {
                base.ReceiveDamage(damageReceived, unitAttacker);
            }
        }

        //Si el atacante es null probablemente es un tile de daño o algo por el estilo
        else
        {
            base.ReceiveDamage(damageReceived, unitAttacker);
        }
    }


}
