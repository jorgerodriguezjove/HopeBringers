using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rogue : PlayerUnit
{
    #region VARIABLES

    //[Header("STATS DE CLASE")]

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS
    public bool checkersAttack;
    public int unitsCanJump;
    //Esto se hace para que al empezar nuevo turno se reseteen los saltos. Hay que poner el mismo número que la variable de arriba
    public int fUnitsCanJump;

    //Lista de posibles unidades a las que atacar
    [HideInInspector]
    public List<UnitBase> unitsAttacked;

    
    public bool extraTurnAttackAfterKill;
    //int para saber uantos turnos extra tiene
    public int extraTurnCount;
    //Esto se hace para que al empezar nuevo turno se reseteen los turnos. Hay que poner el mismo número que la variable de arriba
    public int fextraTurnCount;

    [Header("Pasivas")]
    //PASIVAS

    //Comprobar si tiene la habilidad comprada
    public bool afterKillBonus;
     //El bonus de ataque que se añade tras matar a un enemigo
    public int bonusAttackAfterKill;
    //Máximo de veces que se puede acumular
    public int maxbonusAttackAfterKill;
    //Pasiva mejorada (supongo que no tiene limitante de maxBonus)
    public bool afterKillBonus2;

    //Partícula que se instancia al bufarse el daño
    public GameObject bonusAttackParticle;

    //Comprobar si tiene la habilidad comprada
    public bool smokeBomb;
    public GameObject smokeBombPref;

    public GameObject smokeBombShadow;

    [HideInInspector]
    public List<GameObject> bombsSpawned;

    FacingDirection previousFacingDirection;

    //Pasuva mejorada
    public bool smokeBomb2;
   

    #endregion

    public void SetSpecificStats(bool _multiJumpAttack1, int _multiJumpAttack2,
                                 bool _extraTurnAfterKill1, int _extraTurnAfterKill2,
                                 bool _buffDamage1, bool _buffDamage2,
                                 bool _smokeBomb1, bool _smokeBomb2)
    {
        activeSkillInfo = AppRogueUpgrades.initialActiveText;
        pasiveSkillInfo = AppRogueUpgrades.initialPasiveText;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "genericActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "genericPasive");

        #region Actives

        checkersAttack = _multiJumpAttack1;

        if (_multiJumpAttack2 > 0)
        {
            fUnitsCanJump = _multiJumpAttack2;
            unitsCanJump = _multiJumpAttack2;
            activeSkillInfo = AppRogueUpgrades.multiJumpAttack2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.multiJumpAttack2);

        }

        else if (checkersAttack)
        {
            activeSkillInfo = AppRogueUpgrades.multiJumpAttack1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.multiJumpAttack1);
        }

        extraTurnAttackAfterKill = _extraTurnAfterKill1;
        if (_extraTurnAfterKill2 > 0)
        {
            extraTurnCount = _extraTurnAfterKill2;
            fextraTurnCount = _extraTurnAfterKill2;
            activeSkillInfo = AppRogueUpgrades.extraTurnAfterKill2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.extraTurnAfterKill2);
        }

        else if (extraTurnAttackAfterKill)
        {
            activeSkillInfo = AppRogueUpgrades.extraTurnAfterKill1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.extraTurnAfterKill1);
        }

        #endregion

        #region Pasives

        afterKillBonus = _buffDamage1;
        afterKillBonus2 = _buffDamage2;

        if (afterKillBonus2)
        {
            pasiveSkillInfo = AppRogueUpgrades.buffDamageKill2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.buffDamageKill2);
        }

        else if (afterKillBonus)
        {
            pasiveSkillInfo = AppRogueUpgrades.buffDamageKill1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.buffDamageKill1);
        }

        smokeBomb = _smokeBomb1;
        smokeBomb2 = _smokeBomb2;

        if (smokeBomb2)
        {
            pasiveSkillInfo = AppRogueUpgrades.smokeBomb2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.smokeBomb2);
        }

        else if (smokeBomb)
        {
            pasiveSkillInfo = AppRogueUpgrades.smokeBomb1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppRogueUpgrades.smokeBomb2);
        }

        #endregion
    }

    public override void CheckWhatToDoWithSpecialsTokens()
    {

        if (extraTurnAttackAfterKill)
        {
            myPanelPortrait.GetComponent<Portraits>().specialToken.SetActive(true);
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.enabled = true;
            //Cambiar el número si va a tener más de un turno
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = extraTurnCount.ToString() ;
        }
        else if (checkersAttack)
        {
            unitsCanJump = fUnitsCanJump;
            myPanelPortrait.GetComponent<Portraits>().specialToken.SetActive(true);
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.enabled = true;
            //Cambiar el número si va a tener más de un turno
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = unitsCanJump.ToString();           
        }

        if (afterKillBonus)
        {
            myPanelPortrait.GetComponent<Portraits>().ninjaBuffDamage.enabled = true;
            myPanelPortrait.GetComponent<Portraits>().ninjaBuffDamage.text = baseDamage.ToString();
        }
    }
    public override void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
    {
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();

        for (int i = 0; i < myCurrentTile.neighbours.Count; i++)
        {
            if (!myCurrentTile.neighbours[i].isEmpty && !myCurrentTile.neighbours[i].isObstacle)
            {
                currentTilesInRangeForAttack.Add(myCurrentTile.neighbours[i]);
            }
        }

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

        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }
    }


    public void CheckPasiveUpgrades(UnitBase _unitToAttack)
    {

        if (afterKillBonus)
        {
            if (_unitToAttack.isDead)
            {
                if (afterKillBonus2)
                {
                    baseDamage += bonusAttackAfterKill;
                    StartCoroutine("ParticleWait");
                }
                else if (maxbonusAttackAfterKill > 0)
                {
                    baseDamage += bonusAttackAfterKill;
                    StartCoroutine("ParticleWait");
                    maxbonusAttackAfterKill--;
                }
            }
        }
        else if (smokeBomb)
        {
            if (_unitToAttack.isDead)
            {
                if (smokeBomb2)
                {
                    TM.surroundingTiles.Clear();
                    TM.GetSurroundingTiles(myCurrentTile, 1, true, false);
                    GameObject smokeBombRef1 = Instantiate(smokeBombPref, myCurrentTile.transform.position, myCurrentTile.transform.rotation);
                    //Hago daño a las unidades adyacentes
                    for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                    {
                        if (TM.surroundingTiles[i] != null)
                        {
                            GameObject smokeBombRef = Instantiate(smokeBombPref, TM.surroundingTiles[i].transform.position, TM.surroundingTiles[i].transform.rotation);
                        }
                    }
                }
                else
                {
                    GameObject smokeBombRef = Instantiate(smokeBombPref, myCurrentTile.transform.position, myCurrentTile.transform.rotation);
                }
            }
        }


    }

    IEnumerator ParticleWait()
    {
        yield return new WaitForSeconds(2);
        Instantiate(bonusAttackParticle, transform.position, bonusAttackParticle.transform.rotation);

    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;
        CalculateDamage(unitToAttack);
        CheckIfUnitHasMarks(unitToAttack);

        if (checkersAttack)
        {
            unitsCanJump--;

            //Importante esta llamada sea la primera
            CalculateAttackLogic(unitToAttack, true);

            //Quito el color del tile
            myCurrentTile.ColorDeselect();
            transform.DOJump(currentTileVectorToMove, 1, 1, 1);

            //Cambio la rotación
            NewRotationAfterJump(unitToAttack.myCurrentTile);
            unitsAttacked.Add(unitToAttack);
            //Hago daño
            DoDamage(unitToAttack);

            CheckPasiveUpgrades(unitToAttack);

            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = unitsCanJump.ToString();

                
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            if (unitsCanJump >= 1)
            {
                hasAttacked = false;                
                CheckUnitsAndTilesInRangeToAttack(true);

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

            //Importante esta llamada sea la primera
            CalculateAttackLogic(unitToAttack, true);

            //Quito el color del tile
            myCurrentTile.ColorDeselect();
            transform.DOJump(currentTileVectorToMove, 1, 1, 1);

            //Cambio la rotación
            NewRotationAfterJump(unitToAttack.myCurrentTile);

            //Hago daño
            DoDamage(unitToAttack);

            CheckPasiveUpgrades(unitToAttack);

            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            if (unitToAttack.isDead && extraTurnCount > 0)
            {
                extraTurnCount--;
                hasAttacked = false;
                hasMoved = false;               
                UIM.RefreshTokens();
                LM.DeSelectUnit();
               

                myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = "0";

                //Lo hago aquí para que cuando se seleccione nuevamente ya esté bien calculado.
                LM.tilesAvailableForMovement = LM.TM.OptimizedCheckAvailableTilesForMovement(movementUds, this, false);
                for (int i = 0; i < LM.tilesAvailableForMovement.Count; i++)
                {
                    LM.tilesAvailableForMovement[i].ColorMovement();
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
            //Importante esta llamada sea la primera
            CalculateAttackLogic(unitToAttack, true);

            //Quito el color del tile
            myCurrentTile.ColorDeselect();
            transform.DOJump(currentTileVectorToMove, 1, 1, 1);

            //Cambio la rotación
            NewRotationAfterJump(unitToAttack.myCurrentTile);

            //Hago daño
            DoDamage(unitToAttack);

            CheckPasiveUpgrades(unitToAttack);

            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
    }

    private void CalculateAttackLogic(UnitBase unitToAttack, bool _shouldUpdateInfoAfterMovement)
    {
        if (unitToAttack.myCurrentTile.tileX == myCurrentTile.tileX)
        {
            //Arriba
            if (unitToAttack.myCurrentTile.tileZ > myCurrentTile.tileZ)
            {
                //Muevo al pícaro
                currentTileVectorToMove = myCurrentTile.tilesInLineUp[1].transform.position;  //new Vector3(myCurrentTile.tilesInLineUp[1].tileX, myCurrentTile.tilesInLineUp[1].height, myCurrentTile.tilesInLineUp[1].tileZ);

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineUp[1]);

                if (_shouldUpdateInfoAfterMovement)
                {
                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineUp[1]);
                }  
            }
            //Abajo
            else
            {
                //Muevo al pícaro
                currentTileVectorToMove = myCurrentTile.tilesInLineDown[1].transform.position; //new Vector3(myCurrentTile.tilesInLineDown[1].tileX, myCurrentTile.tilesInLineDown[1].height, myCurrentTile.tilesInLineDown[1].tileZ);

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineDown[1]);

                if (_shouldUpdateInfoAfterMovement)
                {
                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineDown[1]);
                }
            }
        }
        //Izquierda o derecha
        else
        {
            //Derecha
            if (unitToAttack.myCurrentTile.tileX > myCurrentTile.tileX)
            {
                //Muevo al pícaro
                currentTileVectorToMove = myCurrentTile.tilesInLineRight[1].transform.position; //new Vector3(myCurrentTile.tilesInLineRight[1].tileX, myCurrentTile.tilesInLineRight[1].height, myCurrentTile.tilesInLineRight[1].tileZ);

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineRight[1]);

                if (_shouldUpdateInfoAfterMovement)
                {
                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineRight[1]);
                }
            }
            //Izquierda
            else
            {
                //Muevo al pícaro
                currentTileVectorToMove = myCurrentTile.tilesInLineLeft[1].transform.position; //new Vector3(myCurrentTile.tilesInLineLeft[1].tileX, myCurrentTile.tilesInLineLeft[1].height, myCurrentTile.tilesInLineLeft[1].tileZ);

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineLeft[1]);

                if (_shouldUpdateInfoAfterMovement)
                {
                    //Actualizo los tiles
                    UpdateInformationAfterMovement(myCurrentTile.tilesInLineLeft[1]);
                }      
            }
        }
    }

    //Función genérica que comprueba la nueva dirección a la que debe mirar el pícaro tras saltar.
    public void NewRotationAfterJump(IndividualTiles tileWithEnemyAttacked)
    {
        Debug.Log("1");
        if (tileWithEnemyAttacked.tileX == myCurrentTile.tileX)
        {
            Debug.Log("2");
            //Arriba
            if (tileWithEnemyAttacked.tileZ < myCurrentTile.tileZ)
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
            if (tileWithEnemyAttacked.tileX < myCurrentTile.tileX)
            {
                Debug.Log("izq");

                unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.West;
            }
            //Izquierda
            else
            {
                Debug.Log("der");
                unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                currentFacingDirection = FacingDirection.East;
            }
        }
    }

    //Override al calculo de daño porque tiene que mostrar el daño tras el cambio de posición
    public override void CalculateDamage(UnitBase unitToDealDamage)
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

    //Función que se encarga de realizar el calculo e daño como tal. Simplemente es para no repetir el mismo código todo el rato
    private void CalculteDamageLogic(UnitBase unitToDealDamage, IndividualTiles tileLineToCheck, FacingDirection directionForBackAttack)
    {
        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > tileLineToCheck.height)
        {
            damageWithMultipliersApplied -= penalizatorDamageLessHeight;
            unitToDealDamage.downToUpDamageIcon.SetActive(true);
		}

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < tileLineToCheck.height)
        {
            damageWithMultipliersApplied += bonusDamageMoreHeight;
            unitToDealDamage.upToDownDamageIcon.SetActive(true);
		}

        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == directionForBackAttack)
        {
            //Ataque por la espalda
            damageWithMultipliersApplied += bonusDamageBackAttack;
            unitToDealDamage.backStabIcon.SetActive(true);
		}
    }

    //La función es exactamente igual que la original salvo que no calcula el daño, ya que el rogue lo calcula antes de saltar
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


    
    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        tilesInEnemyHover.Clear();
        CalculateAttackLogic(_unitToAttack, false);
        SetShadowRotation(_unitToAttack);

        SetShadowRotation(this);
        shaderHover.SetActive(true);
        shaderHover.transform.position = currentTileVectorToMove;

        
        if (smokeBomb)
        {
            
            if (_unitToAttack.myCurrentTile.tileX == tilesInEnemyHover[0].tileX)
            {
                if (_unitToAttack.myCurrentTile.tileZ < tilesInEnemyHover[0].tileZ)
                {
                    previousFacingDirection = FacingDirection.South;
                    shaderHover.transform.DORotate(new Vector3(0, 180, 0), 0);
                }
                else
                {
                    previousFacingDirection = FacingDirection.North;
                    shaderHover.transform.DORotate(new Vector3(0, 0, 0), 0);


                }
            }
            else
            {
                if (_unitToAttack.myCurrentTile.tileX < tilesInEnemyHover[0].tileX)
                {
                    previousFacingDirection = FacingDirection.West;
                    shaderHover.transform.DORotate(new Vector3(0, -90, 0), 0);

                }
                else
                {
                    previousFacingDirection = FacingDirection.East;
                    shaderHover.transform.DORotate(new Vector3(0, 90, 0), 0);

                }
            }

            CalculateDamagePreviousAttack(_unitToAttack, this, tilesInEnemyHover[0], previousFacingDirection);

            if (_unitToAttack.currentHealth - damageWithMultipliersApplied <= 0)
            {
                if (smokeBomb2)
                {
                    TM.surroundingTiles.Clear();

                    TM.GetSurroundingTiles(tilesInEnemyHover[0], 1, true, false);

                     Vector3 spawnBombShadow = new Vector3(shaderHover.transform.position.x, shaderHover.transform.position.y + 2, shaderHover.transform.position.z);
                    GameObject smokeBombShadowRef1 = Instantiate(smokeBombShadow, spawnBombShadow, shaderHover.transform.rotation);
                    bombsSpawned.Add(smokeBombShadowRef1);

                    //Hago daño a las unidades adyacentes
                    for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                    {
                        if (TM.surroundingTiles[i] != null)
                        {
                           
                            Vector3 spawnBombShadow2 = new Vector3(TM.surroundingTiles[i].transform.position.x, TM.surroundingTiles[i].transform.position.y + 2, TM.surroundingTiles[i].transform.position.z);

                            if (TM.surroundingTiles[i].unitOnTile != null)
                            {
                                GameObject smokeBombShadowRef2 = Instantiate(smokeBombShadow, spawnBombShadow2, TM.surroundingTiles[i].transform.rotation);
                                bombsSpawned.Add(smokeBombShadowRef2);

                            }
                            else
                            {
                                GameObject smokeBombShadowRef = Instantiate(smokeBombShadow, TM.surroundingTiles[i].transform.position, TM.surroundingTiles[i].transform.rotation);
                                bombsSpawned.Add(smokeBombShadowRef);

                            }
                           

                        }
                    }

                }
                else
                {
                    Vector3 spawnBombShadow = new Vector3(shaderHover.transform.position.x, shaderHover.transform.position.y + 2, shaderHover.transform.position.z);
                    //Enseñar sombra bomba de humo
                    GameObject smokeBombShadowRef = Instantiate(smokeBombShadow, spawnBombShadow, shaderHover.transform.rotation);
                    bombsSpawned.Add(smokeBombShadowRef);


                }
            }
        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorAttack();
        }
       


    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        if (smokeBomb)
        {
            for (int i = 0; i < bombsSpawned.Count; i++)
            {
                Destroy(bombsSpawned[i].gameObject);
            }

            bombsSpawned.Clear();
        }
       
    }

    public override void SetShadowRotation(UnitBase unitToSet)
    {
        if (unitToSet.currentFacingDirection == FacingDirection.North)
        {
            unitToSet.shaderHover.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);

        }

        else if (unitToSet.currentFacingDirection == FacingDirection.South)
        {
            unitToSet.shaderHover.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);

        }

        else if (unitToSet.currentFacingDirection == FacingDirection.East)
        {
            unitToSet.shaderHover.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);

        }

        else if (unitToSet.currentFacingDirection == FacingDirection.West)
        {
            unitToSet.shaderHover.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);

        }
    }
}
