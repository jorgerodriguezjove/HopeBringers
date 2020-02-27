using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rogue : PlayerUnit
{
    #region VARIABLES

    //[Header("STATS DE CLASE")]

    [Header("MEJORAS DE PERSONAJE")]

    public bool checkersAttack;
    public int unitsCanJump;
    //Lista de posibles unidades a las que atacar
    [HideInInspector]
    public List<UnitBase> unitsAttacked;
    public bool extraTurnAttackAfterKill;



    #endregion

    public void SetSpecificStats(bool _multiJumpAttack1, bool _extraTurnAfterKill1)
    {
        checkersAttack = _multiJumpAttack1;
        extraTurnAttackAfterKill = _extraTurnAfterKill1;
    }

    public override void CheckUnitsAndTilesInRangeToAttack()
    {
        currentUnitsAvailableToAttack.Clear();

        //Arriba
        if (myCurrentTile.tilesInLineUp.Count > 1)
        {
            if (attackRange <= myCurrentTile.tilesInLineUp.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineUp.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null &&
                   (myCurrentTile.tilesInLineUp[i + 1] != null &&
                    myCurrentTile.tilesInLineUp[i + 1].unitOnTile == null &&
                   !myCurrentTile.tilesInLineUp[i + 1].isEmpty &&
                   !myCurrentTile.tilesInLineUp[i + 1].isObstacle) &&
                   Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                   Mathf.Abs(myCurrentTile.tilesInLineUp[i + 1].height - myCurrentTile.tilesInLineUp[i].height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    break;
                }
            }
        }

        //Abajo

        if (myCurrentTile.tilesInLineDown.Count > 1)
        {
            if (attackRange <= myCurrentTile.tilesInLineDown.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineDown.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null &&
                   (myCurrentTile.tilesInLineDown[i + 1] != null &&
                    myCurrentTile.tilesInLineDown[i + 1].unitOnTile == null &&
                   !myCurrentTile.tilesInLineDown[i + 1].isEmpty &&
                   !myCurrentTile.tilesInLineDown[i + 1].isObstacle) &&
                    Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                    Mathf.Abs(myCurrentTile.tilesInLineDown[i + 1].height - myCurrentTile.tilesInLineDown[i].height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    break;
                }
            }
        }

        //Derecha

        if (myCurrentTile.tilesInLineRight.Count > 1)
        {
            if (attackRange <= myCurrentTile.tilesInLineRight.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineRight.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null &&
                   (myCurrentTile.tilesInLineRight[i + 1] != null &&
                    myCurrentTile.tilesInLineRight[i + 1].unitOnTile == null &&
                   !myCurrentTile.tilesInLineRight[i + 1].isEmpty &&
                   !myCurrentTile.tilesInLineRight[i + 1].isObstacle) &&
                    Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                    Mathf.Abs(myCurrentTile.tilesInLineRight[i + 1].height - myCurrentTile.tilesInLineRight[i].height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    break;
                }
            }
        }

        //Izquierda

        if (myCurrentTile.tilesInLineLeft.Count > 1)
        {
            if (attackRange <= myCurrentTile.tilesInLineLeft.Count)
            {
                rangeVSTilesInLineLimitant = attackRange;
            }
            else
            {
                rangeVSTilesInLineLimitant = myCurrentTile.tilesInLineLeft.Count;
            }

            for (int i = 0; i < rangeVSTilesInLineLimitant; i++)
            {
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null &&
                   (myCurrentTile.tilesInLineLeft[i + 1] != null &&
                    myCurrentTile.tilesInLineLeft[i + 1].unitOnTile == null &&
                   !myCurrentTile.tilesInLineLeft[i + 1].isEmpty &&
                   !myCurrentTile.tilesInLineLeft[i + 1].isObstacle) &&
                    Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack &&
                    Mathf.Abs(myCurrentTile.tilesInLineLeft[i + 1].height - myCurrentTile.tilesInLineLeft[i].height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    break;
                }
            }
        }

        //Feedback de ataque
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttacked(this);
        }
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;
        CalculateDamage(unitToAttack);

        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
            currentHealth += 1;           
            UIM.RefreshTokens();

        }
      
        if (checkersAttack)
        {
            unitsCanJump--;

            if (unitToAttack.myCurrentTile.tileX == myCurrentTile.tileX)
            {
                //Arriba
                if (unitToAttack.myCurrentTile.tileZ > myCurrentTile.tileZ)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[1].transform.position;  //new Vector3(myCurrentTile.tilesInLineUp[1].tileX, myCurrentTile.tilesInLineUp[1].height, myCurrentTile.tilesInLineUp[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[1]);
                }
                //Abajo
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[1].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[1].tileX, myCurrentTile.tilesInLineDown[1].height, myCurrentTile.tilesInLineDown[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[1]);
                }
            }
            //Izquierda o derecha
            else
            {
                //Derecha
                if (unitToAttack.myCurrentTile.tileX > myCurrentTile.tileX)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[1].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[1].tileX, myCurrentTile.tilesInLineRight[1].height, myCurrentTile.tilesInLineRight[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[1]);
                }
                //Izquierda
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[1].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[1].tileX, myCurrentTile.tilesInLineLeft[1].height, myCurrentTile.tilesInLineLeft[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[1]);
                }
            }

            unitsAttacked.Add(unitToAttack);
            //Hago daño
            DoDamage(unitToAttack);
            
                
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            if (unitsCanJump >= 1)
            {

                hasAttacked = false;                
                CheckUnitsAndTilesInRangeToAttack();

               

                hasMoved = true;
                LM.DeSelectUnit();
                UIM.RefreshTokens();
                LM.SelectUnit(0, this);

                for (int i = 0; i < unitsAttacked.Count; i++)
                {
                    currentUnitsAvailableToAttack.Remove(unitsAttacked[i]);

                }

            }
            else
            {
                //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
                base.Attack(unitToAttack);
            }

        }
        else if (extraTurnAttackAfterKill)
        {

            if (unitToAttack.myCurrentTile.tileX == myCurrentTile.tileX)
            {
                //Arriba
                if (unitToAttack.myCurrentTile.tileZ > myCurrentTile.tileZ)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[1].transform.position;  //new Vector3(myCurrentTile.tilesInLineUp[1].tileX, myCurrentTile.tilesInLineUp[1].height, myCurrentTile.tilesInLineUp[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[1]);
                }
                //Abajo
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[1].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[1].tileX, myCurrentTile.tilesInLineDown[1].height, myCurrentTile.tilesInLineDown[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[1]);
                }
            }
            //Izquierda o derecha
            else
            {
                //Derecha
                if (unitToAttack.myCurrentTile.tileX > myCurrentTile.tileX)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[1].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[1].tileX, myCurrentTile.tilesInLineRight[1].height, myCurrentTile.tilesInLineRight[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[1]);
                }
                //Izquierda
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[1].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[1].tileX, myCurrentTile.tilesInLineLeft[1].height, myCurrentTile.tilesInLineLeft[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[1]);
                }
            }

            //Hago daño
            DoDamage(unitToAttack);
            
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            if (unitToAttack.isDead && !hasUsedExtraTurn)
            {
                hasAttacked = false;
                hasMoved = false;               
                UIM.RefreshTokens();
                LM.DeSelectUnit();
                hasUsedExtraTurn = true;

                //Lo hago aquí para que cuando se seleccione nuevamente ya esté bien calculado.
                LM.tilesAvailableForMovement = LM.TM.OptimizedCheckAvailableTilesForMovement(movementUds, this);
                for (int i = 0; i < LM.tilesAvailableForMovement.Count; i++)
                {
                    LM.tilesAvailableForMovement[i].ColorSelect();
                }

                LM.SelectUnit(movementUds, this);

            }
            else
            {
                //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
                base.Attack(unitToAttack);
                

            }


        }
        else
        {

            if (unitToAttack.myCurrentTile.tileX == myCurrentTile.tileX)
            {
                //Arriba
                if (unitToAttack.myCurrentTile.tileZ > myCurrentTile.tileZ)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[1].transform.position;  //new Vector3(myCurrentTile.tilesInLineUp[1].tileX, myCurrentTile.tilesInLineUp[1].height, myCurrentTile.tilesInLineUp[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[1]);
                }
                //Abajo
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[1].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[1].tileX, myCurrentTile.tilesInLineDown[1].height, myCurrentTile.tilesInLineDown[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[1]);
                }
            }
            //Izquierda o derecha
            else
            {
                //Derecha
                if (unitToAttack.myCurrentTile.tileX > myCurrentTile.tileX)
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[1].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[1].tileX, myCurrentTile.tilesInLineRight[1].height, myCurrentTile.tilesInLineRight[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[1]);
                }
                //Izquierda
                else
                {
                    //Quito el color del tile
                    myCurrentTile.ColorDeselect();

                    //Muevo al pícaro
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[1].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[1].tileX, myCurrentTile.tilesInLineLeft[1].height, myCurrentTile.tilesInLineLeft[1].tileZ);
                    transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Cambio la rotación
                    NewRotationAfterJump(unitToAttack.myCurrentTile);

                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[1]);
                }
            }

            //Hago daño
            DoDamage(unitToAttack);

            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
        

       
    }

    //Función genérica que comprueba la nueva dirección a la que debe mirar el pícaro tras saltar.
    public void NewRotationAfterJump(IndividualTiles tileWithEnemyAttacked)
    {
        if (tileWithEnemyAttacked.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (tileWithEnemyAttacked.tileZ > myCurrentTile.tileZ)
            {
                unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.South;
            }
            //Abajo
            else
            {
                unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.North;
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (tileWithEnemyAttacked.tileX > myCurrentTile.tileX)
            {
                unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
            //Izquierda
            else
            {
                unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
        }
    }

    //Override al calculo de daño porque tiene que mostrar el daño tras el cambio de posición
    protected override void CalculateDamage(UnitBase unitToDealDamage)
    {
        //Reseteo la variable de daño a realizar
        damageWithMultipliersApplied = baseDamage;

        if (unitToDealDamage.myCurrentTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (unitToDealDamage.myCurrentTile.tileZ > myCurrentTile.tileZ)
            {
                CalculteDamageLogic(unitToDealDamage, myCurrentTile.tilesInLineUp[1], FacingDirection.South );
            }
            //Abajo
            else
            {
                CalculteDamageLogic(unitToDealDamage, myCurrentTile.tilesInLineDown[1], FacingDirection.North);

            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (unitToDealDamage.myCurrentTile.tileX > myCurrentTile.tileX)
            {
                CalculteDamageLogic(unitToDealDamage, myCurrentTile.tilesInLineRight[1], FacingDirection.West);
            }
            //Izquierda
            else
            {
                CalculteDamageLogic(unitToDealDamage, myCurrentTile.tilesInLineLeft[1], FacingDirection.East);

            }
        }
    }

    //Función que se encarga de realizar el calculod e daño como tal. Simplemente es para no repetir el mismo código todo el rato
    private void CalculteDamageLogic(UnitBase unitToDealDamage, IndividualTiles tileLineToCheck, FacingDirection directionForBackAttack)
    {
        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > tileLineToCheck.height)
        {
            damageWithMultipliersApplied -= penalizatorDamageLessHeight;
			downToUpDamageIcon.SetActive(true);
		}

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < tileLineToCheck.height)
        {
            damageWithMultipliersApplied += bonusDamageMoreHeight;
			upToDownDamageIcon.SetActive(true);
		}

        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == directionForBackAttack)
        {
            //Ataque por la espalda
            damageWithMultipliersApplied += bonusDamageBackAttack;
			backStabIcon.SetActive(true);
		}
    }

    //La función es exactamente igual que la original salvo que no calcula el daño, ya que el rogue lo calcula antes de saltar
    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);

        //Si ataco por la espalda instancio la partícula de ataque crítico
        if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
        {
            Instantiate(criticAttackParticle, unitModel.transform.position, unitModel.transform.rotation);
        }

        //Si no, instancio la partícula normal
        else
        {
            Instantiate(attackParticle, unitModel.transform.position, unitModel.transform.rotation);
        }
    }
}
