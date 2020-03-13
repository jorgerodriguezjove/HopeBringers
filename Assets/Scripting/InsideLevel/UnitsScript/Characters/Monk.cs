using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Monk : PlayerUnit
{
    #region VARIABLES

    //[Header("STATS DE CLASE")]

    [Header("MEJORAS DE PERSONAJE")]


    [Header("Activas")]
    
    //bool para la activa 1
    public bool rotatorTime;

    //bool para la mejora de la activa 1
    public bool rotatorTime2;


    //bool para la activa 2
    public bool suplex;


    [Header("Pasivas")]
    //PASIVAS

     //bool para la pasiva 1
    public bool debuffMark;

    //bool para la mejora de la pasiva 1
    public bool debuffMark2;

    //no hace falta bool de pasiva porque con cambiar este int ya cambia en los personajes
    public int healerBonus;

    //se necesita este bool para comprobar si tiene la mejora de la pasiva 2
    public bool healerMark2;


    #endregion


    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        if (rotatorTime){

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            unitToAttack.isMarked = true;

            //PREGUNTAR SI LA ROTACIÓN TIENE QUE IR ANTES O DESPÚES DE HACER DAÑO

        
         if (unitToAttack.currentFacingDirection == FacingDirection.North)
         {
                unitToAttack.unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                unitToAttack.currentFacingDirection = FacingDirection.South;
         }

         else if (unitToAttack.currentFacingDirection == FacingDirection.South)
         {
                unitToAttack.unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                unitToAttack.currentFacingDirection = FacingDirection.North;
         }

         else if (unitToAttack.currentFacingDirection == FacingDirection.East)
         {

                unitToAttack.unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                unitToAttack.currentFacingDirection = FacingDirection.West;
         }

         else if (unitToAttack.currentFacingDirection == FacingDirection.West)
         {
                unitToAttack.unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                unitToAttack.currentFacingDirection = FacingDirection.East;
         }

            if (rotatorTime2)
            {
                if (unitToAttack.isMarked)
                {
                    unitToAttack.isMarked = false;
                    currentHealth += healerBonus;

                    //COMPROBAR QUE NO DE ERROR EN OTRAS COSAS
                    TM.surroundingTiles.Clear();

                    TM.GetSurroundingTiles(unitToAttack.myCurrentTile, 1, true, false);


                    //Marco a las unidades adyacentes si no están marcadas
                    for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                    {
                        if (TM.surroundingTiles[i].unitOnTile != null)
                        {
                            if(TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>()
                                && !TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>().isMarked)
                            {

                                TM.surroundingTiles[i].unitOnTile.isMarked = true;
                            }
                        }
                    }
                }

            }
            
            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
        else if (suplex)
        {

            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(unitToAttack.myCurrentTile.tilesInLineDown[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(unitToAttack.myCurrentTile.tilesInLineUp[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {

                if (myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(unitToAttack.myCurrentTile.tilesInLineLeft[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(unitToAttack.myCurrentTile.tilesInLineRight[0]);
                }
            }

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            unitToAttack.isMarked = true;


            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            unitToAttack.isMarked = true;

            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }

      
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
