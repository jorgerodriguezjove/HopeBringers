using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : PlayerUnit
{
    #region VARIABLES

    [Header("MEJORAS DE PERSONAJE")]

    [SerializeField]
    public  int healedLife;
   
    public int buffHeal;

    [Header("Activas")]
    //ACTIVAS

    //La activa uno depende de cambiar el int healedLife;

    //bool mejora de la activa 1
    public bool individualHealer2;

    //int que indica cuantas unidades de movimiento se mejora a la unidad
    public int movementUpgrade;

    //bool activa 2
    public bool areaHealer;

    //bool mejora activa 2
    public bool areaHealer2;

    [Header("Pasivas")]
    //PASIVAS

    //bool pasiva 1
    public bool tileTransformer;

    [HideInInspector]
    public List<GameObject> tilesSpawned;

    //bool mejora de la pasiva 1
    public bool tileTransformer2;

    //bool pasiva 2
    public bool tileSustitute;

    //bool mejora de la pasiva 2
    public bool tileSustitute2;

    //int que añade bonus al druida si está en un tile de curación
    public int bonusOnTile;

    public GameObject healerTilePref;

    public GameObject shadowHealerTilePref;

    [SerializeField]
    GameObject areaHealParticle;

    [SerializeField]
    GameObject healParticle;


    #endregion

    public void SetSpecificStats(int _heal1, bool _heal2,
                                 bool _areaHeal1, bool _areaHeal2,
                                 bool _tile1, bool _tile2,
                                 bool _tileMovement1, bool _tileMovement2)
    {

        //IMPORTANTE REVISAR QUE ESTAN BIEN LOS TEXTOS (NO ESTOY SEGURO DE HABER CORRESPONDIDO CADA MEJORA CON SU TEXTO BIEN)

        activeSkillInfo = AppDruidUpgrades.DruidDataBaseActive;
        pasiveSkillInfo = AppDruidUpgrades.DruidDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "DruidDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "DruidDataBasePasive");

        #region Actives

        healedLife = _heal1;
        individualHealer2 = _heal2;

        areaHealer = _areaHeal1;
        areaHealer2 = _areaHeal2;

        if (areaHealer2)
        {
            activeSkillInfo = AppDruidUpgrades.areaHeal2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.areaHeal1);
        }

        else if (areaHealer)
        {
            activeSkillInfo = AppDruidUpgrades.areaHeal1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.areaHeal1);
        }

        if (individualHealer2)
        {
            healedLife = _heal1 + 1;
            movementUpgrade = 1;

            activeSkillInfo = AppDruidUpgrades.heal2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.heal1);
        }

        //CHECK
        else if (healedLife > 1)
        {
            activeSkillInfo = AppDruidUpgrades.heal1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.heal1);
        }

        else
        {
            healedLife = 1;
        }

        #endregion

        #region Pasives

        tileTransformer = _tile1;
        tileTransformer2 = _tile2;

        tileSustitute = _tileMovement1;
        tileSustitute2 = _tileMovement2;


        if (tileSustitute2)
        {
            bonusOnTile = 1;

            pasiveSkillInfo = AppDruidUpgrades.tileMovement2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.tileMovement1);
        }

        else if (tileSustitute)
        {
            pasiveSkillInfo = AppDruidUpgrades.tileMovement1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.tileMovement1);
        }

        if (tileTransformer2)
        {
            pasiveSkillInfo = AppDruidUpgrades.tile2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.tile1);
        }

        else if (tileTransformer)
        {
            pasiveSkillInfo = AppDruidUpgrades.tile1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppDruidUpgrades.tile1);
        }


        #endregion
    }


    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        CheckIfUnitHasMarks(unitToAttack);

        if (areaHealer)
        {
            //Hay que cambiar
            Instantiate(areaHealParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

            if (unitToAttack.GetComponent<PlayerUnit>())
            {
                currentHealth -= 1;
                UIM.RefreshTokens();
                UIM.RefreshHealth();
                unitToAttack.RefreshHealth(false);
                RefreshHealth(false);

                //COMPROBAR QUE NO DE ERROR EN OTRAS COSAS
                TM.surroundingTiles.Clear();

                TM.GetSurroundingTiles(unitToAttack.myCurrentTile, 1, true, false);
                //Hago daño a las unidades adyacentes
                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    if (TM.surroundingTiles[i].unitOnTile != null)
                    {
                        //UNDO
                        CreateAttackCommand(TM.surroundingTiles[i].unitOnTile);

                        if (areaHealer2)
                        {
                            TM.surroundingTiles[i].unitOnTile.isStunned = false;
                            TM.surroundingTiles[i].unitOnTile.turnStunned = 0;
                            ApplyBuffOrDebuffDamage(TM.surroundingTiles[i].unitOnTile, 0, 0);

                            TM.surroundingTiles[i].unitOnTile.buffbonusStateDamage = 0;

                        }
                        if (tileTransformer)
                        {
                           
                            Instantiate(healerTilePref, TM.surroundingTiles[i].unitOnTile.transform.position, TM.surroundingTiles[i].unitOnTile.transform.rotation);
                        }

                        TM.surroundingTiles[i].unitOnTile.currentHealth += healedLife;
                        TM.surroundingTiles[i].unitOnTile.RefreshHealth(false);
                    }
                }

                if (tilesSpawned.Count > 0)
                {
                    for (int i = 0; i < tilesSpawned.Count; i++)
                    {
                        Destroy(tilesSpawned[i].gameObject);
                    }

                    tilesSpawned.Clear();
                }
            }

            else
            {
                //UNDO
                CreateAttackCommand(unitToAttack);

                //Hago daño
                DoDamage(unitToAttack);

                if (currentHealth < maxHealth)
                {
                    currentHealth += healedLife;
                    UIM.RefreshTokens();
                    UIM.RefreshHealth();
                }

                //Hay que cambiar
                SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);
            }
        }

        else
        {
            //UNDO
            CreateAttackCommand(unitToAttack);

            if (unitToAttack.GetComponent<PlayerUnit>())
            {
                //Hay que cambiar
                Instantiate(healParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

                //Hay que cambiar
                SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);
                if (individualHealer2)
                {
                    ApplyBuffOrDebuffMovement(unitToAttack, fMovementUds + movementUpgrade, 3);
                    
                }

                else if (tileTransformer)
                {
                    Instantiate(healerTilePref, unitToAttack.transform.position, unitToAttack.transform.rotation);
                }

                if (tilesSpawned.Count > 0)
                {
                    for (int i = 0; i < tilesSpawned.Count; i++)
                    {
                        Destroy(tilesSpawned[i].gameObject);
                    }

                    tilesSpawned.Clear();
                }

                if (unitToAttack.currentHealth < unitToAttack.maxHealth) 
                {
                    unitToAttack.currentHealth += healedLife;
                    currentHealth -= 1;
                    unitToAttack.RefreshHealth(false);
                    RefreshHealth(false);
                }
                
                UIM.RefreshTokens();
                UIM.RefreshHealth();
            }

            else
            {
                //Hay que cambiar
                Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

                //Hago daño
                DoDamage(unitToAttack);

                if (currentHealth < maxHealth)
                {
                    currentHealth += healedLife;
                    UIM.RefreshTokens();
                    UIM.RefreshHealth();                    
                }

                //Hay que cambiar
                SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);
            }
        }

        HideAttackEffect(unitToAttack);
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

    #region CHECKS
    //AL igual que con el Mago, se hace override a esta función para que pueda atravesar unidades al atacar.
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



        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }


    }
    #endregion

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {

        if (areaHealer)
        {
            if (_unitToAttack.GetComponent<PlayerUnit>())
            {
                TM.surroundingTiles.Clear();

                TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, 1, true, false);

                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    if (TM.surroundingTiles[i] != null)
                    {
                        tilesInEnemyHover.Add(TM.surroundingTiles[i]);

                    }
                }

                for (int i = 0; i < tilesInEnemyHover.Count; i++)
                {
                    tilesInEnemyHover[i].ColorHeal();

                    if (tileTransformer)
                    {
                        GameObject shadowTile = Instantiate(shadowHealerTilePref, tilesInEnemyHover[i].transform.position, tilesInEnemyHover[i].transform.rotation);
                        tilesSpawned.Add(shadowTile);
                    }

                    if (tilesInEnemyHover[i].unitOnTile != null)
                    {
                        tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeHealed();
                    }
                }
            }
            else
            {
                CalculateDamage(_unitToAttack);
                _unitToAttack.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                _unitToAttack.myCurrentTile.ColorAttack();
               
            }
        }
        else
        {

            if (_unitToAttack != null && _unitToAttack.GetComponent<PlayerUnit>())
            {
                if (tileTransformer)
                {
                    GameObject shadowTile = Instantiate(shadowHealerTilePref, _unitToAttack.transform.position, _unitToAttack.transform.rotation);
                    tilesSpawned.Add(shadowTile);
                }

                if (individualHealer2)
                {
                    SetMovementIcon(movementUpgrade,_unitToAttack,true);
                }

                if (_unitToAttack.currentHealth < _unitToAttack.maxHealth)
                {
                    previsualizeAttackIcon.SetActive(true);
                    canvasHover.SetActive(true);
                    canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("-1");
                    _unitToAttack.canvasHover.SetActive(true);
                    _unitToAttack.canvasHover.GetComponent<CanvasHover>().damageNumber.SetText("+" + healedLife);
                    _unitToAttack.canvasHover.GetComponent<CanvasHover>().damageNumber.color = new Color32(0, 255, 50, 255);
                    _unitToAttack.ColorAvailableToBeHealed();
                    _unitToAttack.myCurrentTile.ColorHeal();
                    tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                }
              
                    
               
            }
            else if (_unitToAttack != null)
            {
                CalculateDamage(_unitToAttack);
                _unitToAttack.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
                _unitToAttack.myCurrentTile.ColorAttack();

            }

        }

    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        if (tileTransformer)
        {
            if (tilesSpawned.Count > 0)
            {
                for (int i = 0; i < tilesSpawned.Count; i++)
                {
                    Destroy(tilesSpawned[i].gameObject);
                }

                tilesSpawned.Clear();
            }
        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorBorderRed(); 

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ResetColor();
                tilesInEnemyHover[i].ColorDesAttack();
            }
        }

        tilesInEnemyHover.Clear();

        _unitToAttack.canvasHover.SetActive(false);
        _unitToAttack.ResetColor();
        _unitToAttack.myCurrentTile.ColorBorderRed();
    }

    public override void UndoAttack(AttackCommand lastAttack)
    {
        base.UndoAttack(lastAttack);

        //tiles instanciado y sustituido
    }
}
