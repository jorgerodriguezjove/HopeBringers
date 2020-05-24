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

    //bool para la mejora de la activa 1
    public bool canChooseEnemy;

    //bool para la activa 2
    public bool armorMode;

    //bool para la mejora de la activa 2
    public bool armorMode2;

    //int que indica cuanto añade de armadura
    public int numberOfArmorAdded;

    //Escudos usados al hacer hover para indicar que se va a subir la armadura
    public GameObject armorShield;
    public GameObject armorShield2;

    [Header("Pasivas")]
    //PASIVAS

    //bool para la pasiva 1
    public bool changePositions;

    //bool para la mejora de la pasiva 1
    public bool changePositions2;

    //El número de vida la unidad al que deja la valquiria cambiarse por dicha unidad
    public int numberCanChange;

    //Estas son las partículas que salen cuando le queda poca vida a la unidad
    public GameObject valkHalo;
    public GameObject unitHalo;

    //Estas son las partículas que salen cuando la valquiria está seleccionada
    public GameObject valkHalo2;
    public GameObject unitHalo2;

    [SerializeField]
    private GameObject attack1, attack2, attack3;
    
    [SerializeField]
    public List<PlayerUnit> myHaloUnits = new List<PlayerUnit>();

    [SerializeField]
    public List<GameObject> myHaloInstancies = new List<GameObject>();

    public void SetSpecificStats(int _range1, bool _range2,
                                bool _armor1, bool _armor2,
                                bool _change1, bool _change2,
                                int _height1, int _height2)
    {

        //IMPORTANTE REVISAR QUE ESTAN BIEN LOS TEXTOS (NO ESTOY SEGURO DE HABER CORRESPONDIDO CADA MEJORA CON SU TEXTO BIEN)

        activeSkillInfo = AppValkyrieUpgrades.ValkyrieDataBaseActive;
        pasiveSkillInfo = AppValkyrieUpgrades.ValkyrieDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "ValkyrieDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "ValkyrieDataBasePasive");

        #region Actives

        if (_range1 > attackRange)
        {
            attackRange = _range1;
        }
        canChooseEnemy = _range2;

        armorMode = _armor1;
        armorMode2 = _armor2;

        if (armorMode2)
        {
            numberOfArmorAdded = 2;

            activeSkillInfo = AppValkyrieUpgrades.armorChange2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.armorChange1);
        }

        else if (armorMode)
        {
            numberOfArmorAdded = 1;

            activeSkillInfo = AppValkyrieUpgrades.armorChange1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.armorChange1);
        }

        if (canChooseEnemy)
        {
            activeSkillInfo = AppValkyrieUpgrades.moreRange2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.moreRange1);
        }

        //CHECK
        else if (_range1 > attackRange)
        {
            activeSkillInfo = AppValkyrieUpgrades.moreRange1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.moreRange1);
        }

        #endregion

        #region Pasives

        changePositions = _change1;
        changePositions2 = _change2;

        if (_height2 > maxHeightDifferenceToMove)
        {
            maxHeightDifferenceToMove = _height2;
            pasiveSkillInfo = AppValkyrieUpgrades.height2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.height1);
        }

        else if (_height1 > maxHeightDifferenceToMove)
        {
            maxHeightDifferenceToMove = _height1;
            pasiveSkillInfo = AppValkyrieUpgrades.height1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.height1);
        }

        if (changePositions2)
        {
            numberCanChange = 2; 

            pasiveSkillInfo = AppValkyrieUpgrades.sustitution2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.sustitution1);
        }

        else if (changePositions)
        {
            numberCanChange = 1;

            pasiveSkillInfo = AppValkyrieUpgrades.sustitution1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppValkyrieUpgrades.sustitution1);
        }
        #endregion
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();

        CheckValkyrieHalo();
    }

    public override void Attack(UnitBase unitToAttack)
    {
        CheckIfUnitHasMarks(unitToAttack);

        List<IndividualTiles> tilesInFront = new List<IndividualTiles>();

        tilesInFront = myCurrentTile.GetTilesInFrontOfTheCharacter(currentFacingDirection,3);

        //Particulas
        for (int i = 0; i < tilesInFront.Count; i++)
        {
            if (tilesInFront[i].unitOnTile != null && tilesInFront[i].unitOnTile == unitToAttack)
            {
                if (i == 1)
                {
                    attack1.SetActive(true);
                }
                
                else if (i == 2)
                {
                    attack2.SetActive(true);
                }

                else if (i == 3)
                {
                    attack3.SetActive(true);
                }
            }
        }

        if (canChooseEnemy)
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                if (currentUnitsAvailableToAttack[i] == unitToAttack)
                {
                    break;
                }
                else if (currentUnitsAvailableToAttack[i] != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[i]);

                    DoDamage(currentUnitsAvailableToAttack[i]);
                }

            }


            //Hago daño antes de que se produzca el intercambio
            DoDamage(unitToAttack);

            HideAttackEffect(unitToAttack);
            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);

            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);

            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else if (armorMode)
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            if (unitToAttack.GetComponent<PlayerUnit>())
            {
                if (armorMode2)
                {
                    currentArmor += numberOfArmorAdded;
                    if (currentArmor > currentHealth)
                    {
                        currentArmor = currentHealth;
                    }
                }

                unitToAttack.currentArmor += numberOfArmorAdded;
                if (unitToAttack.currentArmor > unitToAttack.currentHealth)
                {
                    unitToAttack.currentArmor = unitToAttack.currentHealth;
                }

                unitToAttack.RefreshHealth(false);
                RefreshHealth(false);
            }

            else
            {
                //Hago daño
                DoDamage(unitToAttack);
            }

            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);

            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);

            HideAttackEffect(unitToAttack);
            UIM.RefreshHealth();
            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else
        {
            //UNDO
            CreateAttackCommand(unitToAttack);

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Quito el color del tile
            myCurrentTile.ColorDeselect();

            if (unitToAttack.GetComponent<PlayerUnit>())
            {

            }

            else
            {
                //Hago daño
                DoDamage(unitToAttack);
            }

            HideAttackEffect(unitToAttack);
            //Intercambio
            previousTile = unitToAttack.myCurrentTile;

            currentTileVectorToMove = unitToAttack.myCurrentTile.transform.position;
            transform.DOMove(currentTileVectorToMove, 1);

            currentTileVectorToMove = myCurrentTile.transform.position;
            unitToAttack.transform.DOMove(currentTileVectorToMove, 1);

            unitToAttack.UpdateInformationAfterMovement(myCurrentTile);
            UpdateInformationAfterMovement(previousTile);
            unitToAttack.UpdateInformationAfterMovement(unitToAttack.myCurrentTile);

            //Hay que cambiarlo
            SoundManager.Instance.PlaySound(AppSounds.ROGUE_ATTACK);
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
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
        previousTileHeight = 0;

        if (currentFacingDirection == FacingDirection.North)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineUp[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineUp[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    //Si no hay obstáculo marco el tile para indicar el rango
                    if (!myCurrentTile.tilesInLineUp[i].isEmpty && !myCurrentTile.tilesInLineUp[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                        if (!canChooseEnemy)
                        {
                            break;
                        }

                    }

                    if (myCurrentTile.tilesInLineUp[i].isEmpty)
                    {
                        break;
                    }
                }
            }
        }

        if (currentFacingDirection == FacingDirection.South)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineDown[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineDown[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineDown[i].isEmpty && !myCurrentTile.tilesInLineDown[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);

                        if (!canChooseEnemy)
                        {
                            break;
                        }
                    }

                    if (myCurrentTile.tilesInLineDown[i].isEmpty)
                    {
                        break;
                    }
                }
            }
        }

        if (currentFacingDirection == FacingDirection.East)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineRight[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineRight[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineRight[i].isEmpty && !myCurrentTile.tilesInLineRight[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[i]);
                    }
                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);

                        if (!canChooseEnemy)
                        {
                            break;
                        }
                    }

                    if (myCurrentTile.tilesInLineRight[i].isEmpty)
                    {
                        break;
                    }
                }
            }
        }

        if (currentFacingDirection == FacingDirection.West)
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
                //Guardo la altura mas alta en esta linea de tiles
                if (myCurrentTile.tilesInLineLeft[i].height > previousTileHeight)
                {
                    previousTileHeight = myCurrentTile.tilesInLineLeft[i].height;
                }

                //Compruebo que la diferencia de altura con mi tile y con el tile anterior es correcto.
                if (Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack
                    || Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - previousTileHeight) <= maxHeightDifferenceToAttack)
                {
                    if (!myCurrentTile.tilesInLineLeft[i].isEmpty && !myCurrentTile.tilesInLineLeft[i].isObstacle)
                    {
                        currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[i]);
                    }

                    else
                    {
                        break;
                    }

                    //Si hay una unidad la guardo en posibles objetivos
                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);

                        if (!canChooseEnemy)
                        {
                            break;
                        }
                    }

                    if (myCurrentTile.tilesInLineLeft[i].isEmpty)
                    {
                        break;
                    }
                }
            }

        }

        if (_shouldPaintEnemiesAndShowHealthbar)
        {
            //Marco las unidades disponibles para atacar de color rojo
            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                CalculateDamage(currentUnitsAvailableToAttack[i]);
                currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                currentUnitsAvailableToAttack[i].HealthBarOn_Off(true);
                currentUnitsAvailableToAttack[i].myCurrentTile.ColorInteriorRed();
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

    public void ChangePosition(PlayerUnit unitToMove)
    {
        CreateAttackCommand(unitToMove);

        IndividualTiles valkyriePreviousTile = unitToMove.myCurrentTile;
        unitToMove.MoveToTilePushed(myCurrentTile);
        unitToMove.UpdateInformationAfterMovement(myCurrentTile);
        this.MoveToTilePushed(valkyriePreviousTile);
        UpdateInformationAfterMovement(valkyriePreviousTile);
        hasMoved = true;
        LM.UnitHasFinishedMovementAndRotation();
        UIM.RefreshTokens();

    }

    public void ChangePositionIconFeedback(bool has2Show, UnitBase otherUnit)
    {
        if (has2Show)
        {
            if (otherUnit != null)
            {
                otherUnit.changePositionIcon.SetActive(true);
            }
            changePositionIcon.SetActive(true);
        }
        else
        {
            if (otherUnit != null)
            {
                otherUnit.changePositionIcon.SetActive(false);
            }
            changePositionIcon.SetActive(false);
        }

    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        base.ShowAttackEffect(_unitToAttack);

        if ((_unitToAttack.GetComponent<PlayerUnit>()) && !currentUnitsAvailableToAttack.Contains((_unitToAttack)))
        {
            if (LM.selectedCharacter == this && !hasMoved && changePositions)
            {
                if (_unitToAttack.currentHealth <= numberCanChange)
                {
                    ChangePositionIconFeedback(true, _unitToAttack);
                    Cursor.SetCursor(LM.UIM.movementCursor, Vector2.zero, CursorMode.Auto);
                }
            }

        }
        else
        {

            //Al final dijimos que lo de las sombras liaba más que ayudaba

            //shaderHover.SetActive(true);
            //Vector3 vector2Spawn = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            //shaderHover.transform.position = vector2Spawn;

            //_unitToAttack.shaderHover.SetActive(true);
            //Vector3 vector2SpawnEnemy = new Vector3(_unitToAttack.transform.position.x, transform.position.y + 2.5f, _unitToAttack.transform.position.z);
            //_unitToAttack.shaderHover.transform.position = vector2SpawnEnemy;

            ChangePositionIconFeedback(true, _unitToAttack);          

            if (canChooseEnemy)
            {
                tilesInEnemyHover.Clear();

                for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[i].myCurrentTile);

                    if (currentUnitsAvailableToAttack[i] == _unitToAttack)
                    {
                        break;
                    }
                }

            }
            else if (armorMode)
            {
                if (_unitToAttack.GetComponent<PlayerUnit>())
                {
                    if (armorMode2)
                    {
                        if (currentArmor >= currentHealth)
                        {
                        }
                        else
                        {
                            canvasHover.SetActive(true);
                            armorShield2.SetActive(true);

                            if (currentArmor + numberOfArmorAdded >= currentHealth)
                            {
                                canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + (currentHealth - currentArmor).ToString());

                            }

                            else
                            {
                                canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + numberOfArmorAdded.ToString());
                            }
                            canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32(0, 255, 50, 255);
                        }
                    }

                    if (_unitToAttack.currentArmor >= _unitToAttack.currentHealth)
                    {
                    }
                    else
                    {
                        _unitToAttack.canvasHover.SetActive(true);
                        armorShield.SetActive(true);
                        Vector3 vector2Spawn = new Vector3(_unitToAttack.transform.position.x, _unitToAttack.transform.position.y + 1.5f, _unitToAttack.transform.position.z);
                        armorShield.transform.position = vector2Spawn;

                        if (_unitToAttack.currentArmor + numberOfArmorAdded >= _unitToAttack.currentHealth)
                        {
                            _unitToAttack.canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + (_unitToAttack.currentHealth - _unitToAttack.currentArmor).ToString());
                        }
                        else
                        {
                            _unitToAttack.canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + numberOfArmorAdded.ToString());
                        }
                        _unitToAttack.canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32(0, 255, 50, 255);
                    }
                }
                else if (!_unitToAttack.GetComponent<PlayerUnit>())
                {
                    tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                }

            }
            else if (!_unitToAttack.GetComponent<PlayerUnit>())
            {
                tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
            }

            if (tilesInEnemyHover.Count > 0)
            {
                for (int i = 0; i < tilesInEnemyHover.Count; i++)
                {
                    tilesInEnemyHover[i].ColorAttack();

                    if (tilesInEnemyHover[i].unitOnTile != null)
                    {
                        CalculateDamage(tilesInEnemyHover[i].unitOnTile);

                        tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                    }
                }
            }
        }
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        base.HideAttackEffect(_unitToAttack);

        if ((_unitToAttack.GetComponent<PlayerUnit>()) && !currentUnitsAvailableToAttack.Contains((_unitToAttack)))
        {

            ChangePositionIconFeedback(false, _unitToAttack);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);


        }
        else
        {

            if (armorMode)
            {
                if (_unitToAttack.GetComponent<PlayerUnit>())
                {
                    if (armorMode2)
                    {
                        canvasHover.SetActive(false);
                        armorShield2.SetActive(false);
                    }
                    _unitToAttack.canvasHover.SetActive(false);
                    armorShield.SetActive(false);
                }
            }


        }

        sombraHoverUnit.SetActive(false);
        _unitToAttack.sombraHoverUnit.SetActive(false);
        ChangePositionIconFeedback(false, _unitToAttack);

        if (tilesInEnemyHover.Count > 0)
        {
            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                tilesInEnemyHover[i].ColorBorderRed();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    tilesInEnemyHover[i].unitOnTile.ResetColor();
                    tilesInEnemyHover[i].unitOnTile.DisableCanvasHover();
                }
            }
        }

    }


    public void CheckValkyrieHalo()
    {
        if (LM.selectedCharacter == this)
        {
            valkHalo.SetActive(false);

            if (myHaloInstancies.Count > 0)
            {
                for (int i = 0; i < myHaloInstancies.Count; i++)
                {
                    if (myHaloInstancies[i] != null)
                    {
                        Destroy(myHaloInstancies[i]);
                    }   
                }

                myHaloInstancies.Clear();
            }
            
            if (myHaloUnits.Count>0)
            {
               valkHalo2.SetActive(true);

                for (int i = 0; i < myHaloUnits.Count; i++)
                {
                     if(myHaloUnits[i] != null && myHaloUnits[i].currentHealth <= numberCanChange && unitHalo2 != null)
                     {
                        GameObject unitHaloRef = Instantiate(unitHalo2, myHaloUnits[i].transform.position, unitHalo2.transform.rotation);
                        unitHaloRef.SetActive(true);
                        myHaloInstancies.Add(unitHaloRef);
                     }                   
                }
            }  
        }

        else
        {
            
            valkHalo2.SetActive(false);

            if (myHaloInstancies.Count > 0)
            {
                for (int i = 0; i < myHaloInstancies.Count; i++)
                {
                    Destroy(myHaloInstancies[i]);
                }
                myHaloInstancies.Clear();
            }

            if (myHaloUnits.Count > 0)
            {
                valkHalo.SetActive(true);
                for (int i = 0; i < myHaloUnits.Count; i++)
                {
                    if (myHaloUnits[i].currentHealth <= numberCanChange)
                    {
                        GameObject unitHaloRef = Instantiate(unitHalo, myHaloUnits[i].transform.position, unitHalo.transform.rotation);
                        unitHaloRef.SetActive(true);
                        myHaloInstancies.Add(unitHaloRef);
                    }
                }
            }
    }
    
}


    public override void UndoAttack(AttackCommand lastAttack, bool _isThisUnitTheAttacker)
    {
        base.UndoAttack(lastAttack, _isThisUnitTheAttacker);

        CheckUnitsAndTilesInRangeToAttack(false);
        //Estas líneas las añado para comprobar si hay samurai y si hay que actualizar el honor
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null)
        {
            samuraiUpgraded.RefreshHonorOnPortrait();
        }
        UIM.CheckActionsAvaliable();
    }

}


