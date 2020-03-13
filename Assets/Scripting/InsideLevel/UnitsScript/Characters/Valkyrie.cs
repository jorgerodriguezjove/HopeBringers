using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Valkyrie : PlayerUnit
{
    public IndividualTiles previousTile;

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS

    //bool para la activa 1
    public bool parryOn;

    [Header("Pasivas")]
    //PASIVAS

    //bool para la pasiva 1
    public bool itsForHonorTime;



    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;


        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
            currentHealth += FindObjectOfType<Monk>().healerBonus;

            if (FindObjectOfType<Monk>().debuffMark2)
            {
                if (!unitToAttack.isStunned)
                {
                    unitToAttack.isStunned = true;
                    unitToAttack.turnStunned = 1;
                }

            }
            else if (FindObjectOfType<Monk>().healerMark2)
            {
                BuffbonusStateDamage = 1;

            }
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

    protected override void DoDamage(UnitBase unitToDealDamage)
    {

        //Añado este if para el count de honor del samurai
        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
       || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
       || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
       || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
       )
        {
            LM.honorCount++;
        }
        base.DoDamage(unitToDealDamage);
    }
}
