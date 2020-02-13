using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Valkyrie : PlayerUnit
{
    
    public IndividualTiles previousTile;

    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;


        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
            currentHealth += 1;
            UIM.RefreshTokens();

        }


        //Animación de ataque
        myAnimator.SetTrigger("Attack");

        //Quito el color del tile
        myCurrentTile.ColorDeselect();

        previousTile = unitToAttack.myCurrentTile;

        currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
        transform.DOMove(currentTileVectorToMove, 1);
        

        currentTileVectorToMove = myCurrentTile.transform.position;
        unitToAttack.transform.DOMove(currentTileVectorToMove, 1);
        
        unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
        UpdateInformationAfterMovement(previousTile);
        unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);





        //Hago daño
        DoDamage(unitToAttack);
             //Hay que cambiarlo
              SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
             //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
             base.Attack(unitToAttack);
        

    }
}
