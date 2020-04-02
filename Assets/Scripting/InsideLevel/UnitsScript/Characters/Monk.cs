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

    //Flecha que sale encima de la unidad a la que va a girar
    public GameObject rotatorFeedbackArrow;

    //bool para la mejora de la activa 1
    public bool rotatorTime2;


    //bool para la activa 2
    public bool suplex;

    public bool suplex2;


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
            unitToAttack.monkMark.SetActive(true);
            unitToAttack.numberOfMarks = 1;

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
                    unitToAttack.monkMark.SetActive(false);
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
                                TM.surroundingTiles[i].unitOnTile.monkMark.SetActive(true);
                            }
                        }
                    }
                }

            }
            
            //Hago daño
            DoDamage(unitToAttack);
            rotatorFeedbackArrow.SetActive(false);

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
            unitToAttack.monkMark.SetActive(true);

            if (suplex2 && unitToAttack.numberOfMarks == 1)
            {

                unitToAttack.numberOfMarks = 2;

            }
            else
            {
                unitToAttack.numberOfMarks = 1;

            }


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
            unitToAttack.monkMark.SetActive(true);
            unitToAttack.numberOfMarks = 1;

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

    public override void CheckUnitsAndTilesInRangeToAttack()
    {
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();

        //Arriba
        if (currentFacingDirection == FacingDirection.North)
        {
            
            if (myCurrentTile.tilesInLineUp.Count > 0)
            {
                if (attackRange <= myCurrentTile.tilesInLineUp.Count)
                {
                    rangeVSTilesInLineLimitant = attackRange;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
                }

                if (suplex)
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null &&
                           (myCurrentTile.tilesInLineDown.Count > 0 &&
                            myCurrentTile.tilesInLineDown[i] != null &&
                            myCurrentTile.tilesInLineDown[i].unitOnTile == null &&
                           !myCurrentTile.tilesInLineDown[i].isEmpty &&
                           !myCurrentTile.tilesInLineDown[i].isObstacle) &&
                           Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                           Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                            break;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null &&
                           Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                            break;
                        }
                    }
                }


            }
        }


        //Abajo
        if (currentFacingDirection == FacingDirection.South)
        {
            if (myCurrentTile.tilesInLineDown.Count > 0)
            {
                if (attackRange <= myCurrentTile.tilesInLineDown.Count)
                {
                    rangeVSTilesInLineLimitant = attackRange;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
                }

                if (suplex)
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null &&
                           (myCurrentTile.tilesInLineUp.Count > 0 &&
                           myCurrentTile.tilesInLineUp[i] != null &&
                            myCurrentTile.tilesInLineUp[i].unitOnTile == null &&
                           !myCurrentTile.tilesInLineUp[i].isEmpty &&
                           !myCurrentTile.tilesInLineUp[i].isObstacle) &&
                            Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                            Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                            break;
                        }
                    }
                }

                else

                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null &&
                            Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                            break;
                        }
                    }
                }


            }
        }

        //Derecha
        if (currentFacingDirection == FacingDirection.East)
        {
            if (myCurrentTile.tilesInLineRight.Count > 0)
            {
                if (attackRange <= myCurrentTile.tilesInLineRight.Count)
                {
                    rangeVSTilesInLineLimitant = attackRange;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
                }

                if (suplex)
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null &&
                           (myCurrentTile.tilesInLineLeft.Count > 0 &&
                           myCurrentTile.tilesInLineLeft[i] != null &&
                            myCurrentTile.tilesInLineLeft[i].unitOnTile == null &&
                           !myCurrentTile.tilesInLineLeft[i].isEmpty &&
                           !myCurrentTile.tilesInLineLeft[i].isObstacle) &&
                            Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                            Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null &&
                            Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                            break;
                        }
                    }
                }


            }
        }
        //Izquierda

        if (currentFacingDirection == FacingDirection.West)
        {
            if (myCurrentTile.tilesInLineLeft.Count > 0)
            {
                if (attackRange <= myCurrentTile.tilesInLineLeft.Count)
                {
                    rangeVSTilesInLineLimitant = attackRange;
                }
                else
                {
                    rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
                }

                if (suplex)
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null &&
                           (myCurrentTile.tilesInLineRight.Count > 0 &&
                           myCurrentTile.tilesInLineRight[i] != null &&
                            myCurrentTile.tilesInLineRight[i].unitOnTile == null &&
                           !myCurrentTile.tilesInLineRight[i].isEmpty &&
                           !myCurrentTile.tilesInLineRight[i].isObstacle) &&
                            Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                            Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                            break;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
                    {
                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null &&
                            Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                        {
                            //Almaceno la primera unidad en la lista de posibles unidades
                            currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                            break;
                        }
                    }
                }
            }

        }
        //Feedback de ataque
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            CalculateDamage(currentUnitsAvailableToAttack[i]);
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttacked(damageWithMultipliersApplied);

            currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
        }

        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }
    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        if (rotatorTime)
        {
            rotatorFeedbackArrow.SetActive(true);
             Vector3 spawnRotatorArrow = new Vector3(_unitToAttack.transform.position.x, _unitToAttack.transform.position.y + 1, _unitToAttack.transform.position.z);
            rotatorFeedbackArrow.transform.position = spawnRotatorArrow;

        }
        else if (suplex)
        {
            tilesInEnemyHover.Clear();

            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position;                                       
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineDown[0]);
                   
                   
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineUp[0]);
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = currentTileVectorToMove;
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {

                if (myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineLeft[0]);
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = currentTileVectorToMove;
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineRight[0]);
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = currentTileVectorToMove;
                }
            }

            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                tilesInEnemyHover[i].ColorAttack();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttacked(-1);
                }
            }
        }
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        if (rotatorTime)
        {
            rotatorFeedbackArrow.SetActive(false);


        }
        else if (suplex)
        {

            _unitToAttack.shaderHover.SetActive(false);
        }
    }
}
