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

    //bool para pasiva 2
    public bool healerMark;

    //se necesita este bool para comprobar si tiene la mejora de la pasiva 2
    public bool healerMark2;

    //Hay que cambiar este número 
    public int healerBonus;

    //Hay que cambiar este número 
    public int healWithUpgradedMark;

    #endregion

    public void SetSpecificStats(bool _turn1, bool _turn2,
                                bool _suplex1, bool _suplex2,
                                bool _markDebuff1, bool _markDebuff2,
                                bool _markBuff1, bool _markBuff2)
    {

        //IMPORTANTE REVISAR QUE ESTAN BIEN LOS TEXTOS (NO ESTOY SEGURO DE HABER CORRESPONDIDO CADA MEJORA CON SU TEXTO BIEN)

        activeSkillInfo = AppMonkUpgrades.MonkDataBaseActive;
        pasiveSkillInfo = AppMonkUpgrades.MonkDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "MonkDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "MonkDataBasePasive");

        #region Actives

        rotatorTime = _turn1;
        rotatorTime2 = _turn2;

        suplex = _suplex1;
        suplex2 = _suplex2;

        if (rotatorTime2)
        {
            activeSkillInfo = AppMonkUpgrades.turn2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.turn1);
        }

        else if (rotatorTime)
        {
            activeSkillInfo = AppMonkUpgrades.turn1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.turn1);
        }

        if (suplex2)
        {
            activeSkillInfo = AppMonkUpgrades.suplex2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.suplex1);
        }

        else if (suplex)
        {
            activeSkillInfo = AppMonkUpgrades.suplex1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.suplex1);
        }

        #endregion

        #region Pasives

        debuffMark = _markDebuff1;
        debuffMark2 = _markDebuff2;

        healerMark = _markBuff1;
        healerMark2 = _markBuff2;

        healerBonus = 1;

        if (debuffMark2)
        {
            pasiveSkillInfo = AppMonkUpgrades.markDebuff2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.markDebuff1);
        }

        else if (debuffMark)
        {
            pasiveSkillInfo = AppMonkUpgrades.markDebuff1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.markDebuff1);
        }

        if (healerMark2)
        {
            healerBonus = 2;

            pasiveSkillInfo = AppMonkUpgrades.markBuff2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.markBuff1);
        }

        else if (healerMark)
        {
            healerBonus = 2;

            pasiveSkillInfo = AppMonkUpgrades.markBuff1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMonkUpgrades.markBuff1);
        }
        #endregion

        healWithUpgradedMark = healerBonus + 1;
    }

    public void PutQuitMark(UnitBase _unitToMark,UnitBase _unitAttacking ,bool haveToPut, bool showFutureMark)
    {
        //Si pongo la marca actualizo los numeros y el estado del bool
        if (haveToPut)
        {
            _unitToMark.isMarked = true;

            _unitToMark.numberOfMarks += 1;

            if (suplex2)
            {
                if (_unitToMark.numberOfMarks > 2)
                {
                    _unitToMark.isMarked = false;
                    _unitToMark.numberOfMarks = 0;

                    _unitAttacking.currentHealth += healWithUpgradedMark;
                }
            }

            //NO ES >= 1, ESTÁ BIEN >
            else if (_unitToMark.numberOfMarks > 1)
            {
                _unitToMark.isMarked = false;
                _unitToMark.numberOfMarks = 0;

                _unitAttacking.currentHealth += healerBonus;
            }

            _unitAttacking.RefreshHealth(false);
            UIM.RefreshHealth();
        }

        //Independientemente de que actualice la maca o no tengo que enseñarla
        //Esto sirve para el showAction que tiene que mostrar el estado en el que acaba la marca (como si ya se hubiese aplicado el ataque)
        if (showFutureMark)
        {
            if (_unitToMark.isMarked)
            {
                if (_unitToMark.numberOfMarks == 1)
                {
                    if (suplex2)
                    {
                        _unitToMark.monkMark.SetActive(false);

                        if (_unitAttacking.GetComponent<Monk>())
                        {
                            _unitToMark.monkMarkUpgrade.SetActive(true);
                        }
                    }

                    else
                    {
                        _unitToMark.monkMark.SetActive(false);

                        _unitAttacking.canvasHover.SetActive(true);
                        _unitAttacking.canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + healerBonus);
                        _unitAttacking.canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32(0, 255, 50, 255);
                        _unitAttacking.ColorAvailableToBeHealed();
                        _unitAttacking.myCurrentTile.ColorHeal();
                    }
                }

                else if (_unitToMark.numberOfMarks == 2)
                {
                    _unitToMark.monkMark.SetActive(false);

                    _unitToMark.monkMarkUpgrade.SetActive(false);

                    _unitAttacking.canvasHover.SetActive(true);
                    _unitAttacking.canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + healWithUpgradedMark);
                    _unitAttacking.canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32(0, 255, 50, 255);
                    _unitAttacking.ColorAvailableToBeHealed();
                    _unitAttacking.myCurrentTile.ColorHeal();
                }
            }

            else if (_unitToMark.numberOfMarks == 0)
            {
                if (_unitAttacking.GetComponent<Monk>())
                {
                    _unitToMark.monkMark.SetActive(true);                  
                }

                _unitToMark.monkMarkUpgrade.SetActive(false);
            }
        }

        //El estado actual (sirve para el ataque)
        else
        {
            if (_unitToMark.isMarked)
            {
                if (_unitToMark.numberOfMarks == 1)
                {
                    _unitToMark.monkMark.SetActive(true);

                    _unitToMark.monkMarkUpgrade.SetActive(false);
                }

                else if (_unitToMark.numberOfMarks == 2)
                {
                    _unitToMark.monkMark.SetActive(false);

                    _unitToMark.monkMarkUpgrade.SetActive(true);
                }
            }

            else if (_unitToMark.numberOfMarks == 0)
            {
                _unitToMark.monkMark.SetActive(false);

                _unitToMark.monkMarkUpgrade.SetActive(false);
            }
        }
    }

    public override void Attack(UnitBase unitToAttack)
    {
       

        if (rotatorTime){

            unitToAttack.hoverRotateIcon.SetActive(false);

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            //Hago daño. IMPORTANTE tiene que ir despues del CreateAttackCommand pero antes que el giro porque si no le ataca por la espalda.
            DoDamage(unitToAttack);

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
                                //UNDO
                                CreateAttackCommand(TM.surroundingTiles[i].unitOnTile);

                                PutQuitMark(TM.surroundingTiles[i].unitOnTile,this,true, false);
                            }
                        }
                    }
                }
            }

            PutQuitMark(unitToAttack, this, true, false);

            rotatorFeedbackArrow.SetActive(false);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else if (suplex)
        {
            //UNDO
            CreateAttackCommand(unitToAttack);

            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    unitToAttack.UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    unitToAttack.UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {

                if (myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    unitToAttack.UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position;
                    unitToAttack.transform.DOJump(currentTileVectorToMove, 1, 1, 1);

                    //Actualizo los tiles
                    unitToAttack.UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[0]);
                }
            }

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            PutQuitMark(unitToAttack, this, true, false);

            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);

            hasAttacked = true;
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            PutQuitMark(unitToAttack, this, true, false);

            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Monk
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);

            hasAttacked = true;
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        if (unitToAttack.isMarked && debuffMark)
        {
            ApplyBuffOrDebuffDamage(unitToAttack, -1, 99);
        }

        hasAttacked = true;
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

    public override void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
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

                if (!myCurrentTile.tilesInLineUp[0].isEmpty && !myCurrentTile.tilesInLineUp[0].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineUp[0].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[0]);
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

                if (!myCurrentTile.tilesInLineDown[0].isEmpty && !myCurrentTile.tilesInLineDown[0].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineDown[0].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[0]);
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

                if (!myCurrentTile.tilesInLineRight[0].isEmpty && !myCurrentTile.tilesInLineRight[0].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineRight[0].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[0]);
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

                if (!myCurrentTile.tilesInLineLeft[0].isEmpty && !myCurrentTile.tilesInLineLeft[0].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineLeft[0].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[0]);
                }
            }

        }

        if (_shouldPaintEnemiesAndShowHealthbar)
        {
            //Feedback de ataque
            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                CalculateDamage(currentUnitsAvailableToAttack[i]);
                currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);

                currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
            }
        }

        if (LM.selectedCharacter == this || LM.selectedCharacter == null)
        {
            for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
            {
                currentTilesInRangeForAttack[i].ColorBorderRed();
            }
        }
    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        //Este no usa la base porque es el propio monje

        if (rotatorTime)
        {
            if (rotatorTime2)
            {
                if (_unitToAttack.isMarked)
                {
                    TM.surroundingTiles.Clear();

                    TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, 1, true, false);

                    //Marco a las unidades adyacentes si no están marcadas
                    for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                    {
                        if (TM.surroundingTiles[i].unitOnTile != null)
                        {
                            if (TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>()
                                && !TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>().isMarked)
                            {
                                PutQuitMark(TM.surroundingTiles[i].unitOnTile, this, false, true);
                            }
                        }
                    }
                }
            }


            PutQuitMark(_unitToAttack, this, false, true);

            _unitToAttack.hoverRotateIcon.SetActive(true);
        }

        else if (suplex)
        {
            tilesInEnemyHover.Clear();
            //COMPROBAR
            SetShadowRotation(_unitToAttack, _unitToAttack.myCurrentTile, myCurrentTile);

            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineDown[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineDown[0].transform.position;                                       
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineDown[0]);

                    if (!isMovingorRotating)
                    {
                        _unitToAttack.sombraHoverUnit.SetActive(true);
                        _unitToAttack.sombraHoverUnit.transform.position = currentTileVectorToMove;

                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (myCurrentTile.tilesInLineUp[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineUp[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineUp[0]);

                    if (!isMovingorRotating)
                    {
                        _unitToAttack.sombraHoverUnit.SetActive(true);
                        _unitToAttack.sombraHoverUnit.transform.position = currentTileVectorToMove;
                    }   
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {

                if (myCurrentTile.tilesInLineLeft[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineLeft[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineLeft[0]);
                    if (!isMovingorRotating)
                    {
                        _unitToAttack.sombraHoverUnit.SetActive(true);
                        _unitToAttack.sombraHoverUnit.transform.position = currentTileVectorToMove;

                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineRight[0].unitOnTile == null)
                {
                    currentTileVectorToMove = myCurrentTile.tilesInLineRight[0].transform.position;
                    tilesInEnemyHover.Add(myCurrentTile.tilesInLineRight[0]);
                    if (!isMovingorRotating)
                    {
                        _unitToAttack.sombraHoverUnit.SetActive(true);
                        _unitToAttack.sombraHoverUnit.transform.position = currentTileVectorToMove;

                    }
                }
            }

            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                tilesInEnemyHover[i].ColorAttack();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(-1);
                }
            }

            //MARCAS
            PutQuitMark(_unitToAttack, this, false, true);
        }

        else
        {
            PutQuitMark(_unitToAttack, this, false, true);
        }

        CalculateDamage(_unitToAttack);
        _unitToAttack.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
      //No usa la base porque es el propio monje

        if (rotatorTime)
        {
            if (rotatorTime2)
            {
                if (_unitToAttack.isMarked)
                {
                    PutQuitMark(_unitToAttack, this, false, false);

                    //COMPROBAR QUE NO DE ERROR EN OTRAS COSAS
                    TM.surroundingTiles.Clear();

                    TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, 1, true, false);

                    //Marco a las unidades adyacentes si no están marcadas
                    for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                    {
                        if (TM.surroundingTiles[i].unitOnTile != null)
                        {
                            if (TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>()
                                && !TM.surroundingTiles[i].unitOnTile.GetComponent<EnemyUnit>().isMarked)
                            {
                                if (!hasAttacked)
                                {
                                    PutQuitMark(TM.surroundingTiles[i].unitOnTile, this, false, false);
                                }
                            }
                        }
                    }
                }

                _unitToAttack.hoverRotateIcon.SetActive(false);
            }

            else
            {
                if (!hasAttacked)
                {
                    PutQuitMark(_unitToAttack, this, false, false);
                }

                _unitToAttack.hoverRotateIcon.SetActive(false);
            }
        }

        //Monje
        DisableCanvasHover();
        ResetColor();
        myCurrentTile.ColorDeselect();

        //Enemigo
        PutQuitMark(_unitToAttack, this, false, false);
        _unitToAttack.ResetColor();
        _unitToAttack.DisableCanvasHover();
    }
}
