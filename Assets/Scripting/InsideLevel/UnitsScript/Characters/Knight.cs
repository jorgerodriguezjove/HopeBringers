using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerUnit
{
    #region VARIABLES

    [Header("STATS DE CLASE")]

    [SerializeField]
    public int tilesToPush;

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS
    //Empuje en línea
    public bool pushFarther;
    //Empuje en línea mejorado (superempuje)
    public bool pushFarther2;

    //Empuje ancho
    public bool pushWider;
    //Empuje ancho mejorado (stunea a los enemigos atacados)
    public bool pushWider2;

    [Header("Pasivas")]
    //PASIVAS

    //Bloqueo individual (por los lados recibe menos daño)
    public bool lateralBlock;
    //Daño reducido para el knight por los lados
    public int damageLateralBlocked;
    //Bloqueo individual (por la espalda recibe menos daño)
    public bool backBlock;
    //Daño reducido para el knight por la espalda
    public int damageBackBlocked;

    //Los personajes a mi lado reciben menos daño si les atacan por delante
    public bool isBlockingNeighbours;
    //Daño reducido para los personajes atacados
    public int shieldDef;

    //Los personajes a mi lado reciben menos daño si les atacan por delante
    public bool isBlockingNeighboursFull;



    #endregion

    public void SetSpecificStats(bool _pushFarther1, bool _pushFarther2, 
                                 bool _pushWider1, bool _pushWider2,
                                 bool _individualBlock1, bool _individualBlock2, 
                                 bool _neighbourBlock1, bool _neighbourBlock2)
    {
        #region Actives
        //Lo primero que hago es poner el texto genérico, de tal forma que si no hay mejoras se quedará este texto.
        activeSkillInfo = AppKnightUpgrades.initialActiveText;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "genericActive");

        pushFarther = _pushFarther1;
        pushFarther2 = _pushFarther2;

        pushWider = _pushWider1;
        pushWider2 = _pushWider2;

        //Hay que hacer que la mejora mas avanzada se compruebe primero y que compruebe su versión anterior con else if.
        //Esto es por que siempre que compre la segunda mejora, la primera va a estar en true siempre.
        if (pushFarther2)
        {
            tilesToPush = 3;
            activeSkillInfo = AppKnightUpgrades.pushFurther2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.pushFurther2);
        }

        else if (pushFarther)
        {
            tilesToPush = 2;
            activeSkillInfo = AppKnightUpgrades.pushFurther1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.pushFurther1);
        }

        if (pushWider2)
        {
            activeSkillInfo = AppKnightUpgrades.pushWider2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.pushWider2);
        }

        else if (pushWider)
        {
            activeSkillInfo = AppKnightUpgrades.pushWider1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.pushWider1);
        }
        #endregion

        #region Pasives

        //Default
        pasiveSkillInfo = AppKnightUpgrades.initialPasiveText;
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "genericPasive");

        lateralBlock = _individualBlock1;
        backBlock = _individualBlock2;

        isBlockingNeighbours = _neighbourBlock1;
        isBlockingNeighboursFull = _neighbourBlock2;

        if (backBlock)
        {
            pasiveSkillInfo = AppKnightUpgrades.individualBlock2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.individualBlock2);
        }

        else if (lateralBlock)
        {
            pasiveSkillInfo = AppKnightUpgrades.individualBlock1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.individualBlock1);
        }

        if (isBlockingNeighboursFull)
        {
            pasiveSkillInfo = AppKnightUpgrades.neighbourBlock2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.individualBlock2);
        }

        else if (isBlockingNeighbours)
        {
            pasiveSkillInfo = AppKnightUpgrades.neighbourBlock1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppKnightUpgrades.individualBlock1);
        }
        #endregion
    }


    public override void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
    {
        currentUnitsAvailableToAttack.Clear();
        currentTilesInRangeForAttack.Clear();

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
                if (!myCurrentTile.tilesInLineUp[i].isEmpty && !myCurrentTile.tilesInLineUp[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[i]);
                }

                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
                    break;
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
                if (!myCurrentTile.tilesInLineDown[i].isEmpty && !myCurrentTile.tilesInLineDown[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[i]);
                }

                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
                    break;
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
                if (!myCurrentTile.tilesInLineRight[i].isEmpty && !myCurrentTile.tilesInLineRight[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[i]);
                }

                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                    break;
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
                if (!myCurrentTile.tilesInLineLeft[i].isEmpty && !myCurrentTile.tilesInLineLeft[i].isObstacle && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[i]);
                }

                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    //Almaceno la primera unidad en la lista de posibles unidades
                    currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                    break;
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

        if (pushWider)
        {
            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineUp[0].tilesInLineRight[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[0].tilesInLineRight[0]);
                }

                if (myCurrentTile.tilesInLineUp[0].tilesInLineLeft[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineUp[0].tilesInLineLeft[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (myCurrentTile.tilesInLineDown[0].tilesInLineLeft[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[0].tilesInLineLeft[0]);
                }

                if (myCurrentTile.tilesInLineDown[0].tilesInLineRight[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineDown[0].tilesInLineRight[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (myCurrentTile.tilesInLineRight[0].tilesInLineUp[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[0].tilesInLineUp[0]);
                }

                if (myCurrentTile.tilesInLineRight[0].tilesInLineDown[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineRight[0].tilesInLineDown[0]);

                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineLeft[0].tilesInLineUp[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[0].tilesInLineUp[0]);
                }

                if (myCurrentTile.tilesInLineLeft[0].tilesInLineDown[0] != null)
                {
                    currentTilesInRangeForAttack.Add(myCurrentTile.tilesInLineLeft[0].tilesInLineDown[0]);
                }

            }
        }
        
        for (int i = 0; i < currentTilesInRangeForAttack.Count; i++)
        {
            currentTilesInRangeForAttack[i].ColorBorderRed();
        }
    }

    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        CheckIfUnitHasMarks(unitToAttack);

        //Este primer if  lo pongo de momento para seguir la misma estructura que con los otros personajes y por si hay que cambiar algo específico como la animación, el sonido...
        if (pushFarther)
        {
            if (pushFarther2)
            {
                //Hay que cambiarlo/quitarlo después si se quiere aumentar más
                myAnimator.SetTrigger("Attack");
           
                if (currentFacingDirection == FacingDirection.North)
                {
                    if (myCurrentTile.tilesInLineUp[tilesToPush].unitOnTile == null
                        && myCurrentTile.tilesInLineUp[tilesToPush] != null
                        && !myCurrentTile.tilesInLineUp[tilesToPush].isEmpty
                        && !myCurrentTile.tilesInLineUp[tilesToPush].isObstacle)
                    {
                        //UNDO
                        CreateAttackCommand(unitToAttack);

                        unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineUp[tilesToPush]);
                    }
                    
                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {
                        if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                        {
                            if (myCurrentTile.tilesInLineUp[i].unitOnTile != unitToAttack)
                            {
                                //UNDO
                                CreateAttackCommand(myCurrentTile.tilesInLineUp[i].unitOnTile);
                            }

                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineUp[i].unitOnTile);
                        }
                    }
                }

                else if (currentFacingDirection == FacingDirection.South)
                {

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {
                        if (myCurrentTile.tilesInLineDown[tilesToPush].unitOnTile == null
                       && myCurrentTile.tilesInLineDown[tilesToPush] != null
                       && !myCurrentTile.tilesInLineDown[tilesToPush].isEmpty
                       && !myCurrentTile.tilesInLineDown[tilesToPush].isObstacle)
                        {
                            //UNDO
                            CreateAttackCommand(unitToAttack);

                            unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineDown[tilesToPush]);
                        }

                        if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                        {
                            if (myCurrentTile.tilesInLineDown[i].unitOnTile != unitToAttack)
                            {
                                //UNDO
                                CreateAttackCommand(myCurrentTile.tilesInLineDown[i].unitOnTile);
                            }

                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineDown[i].unitOnTile);
                        }
                    }
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    if (myCurrentTile.tilesInLineRight[tilesToPush].unitOnTile == null
                        && myCurrentTile.tilesInLineRight[tilesToPush] != null
                        && !myCurrentTile.tilesInLineRight[tilesToPush].isEmpty
                        && !myCurrentTile.tilesInLineRight[tilesToPush].isObstacle)
                    {
                        //UNDO
                        CreateAttackCommand(unitToAttack);

                        unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineRight[tilesToPush]);
                    }

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {
                        if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                        {
                            if (myCurrentTile.tilesInLineRight[i].unitOnTile != unitToAttack)
                            {
                                //UNDO
                                CreateAttackCommand(myCurrentTile.tilesInLineRight[i].unitOnTile);
                            }

                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        }
                    }
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    if (myCurrentTile.tilesInLineLeft[tilesToPush].unitOnTile == null
                       && myCurrentTile.tilesInLineLeft[tilesToPush] != null
                       && !myCurrentTile.tilesInLineLeft[tilesToPush].isEmpty
                       && !myCurrentTile.tilesInLineLeft[tilesToPush].isObstacle)
                    {
                        //UNDO
                        CreateAttackCommand(unitToAttack);

                        unitToAttack.MoveToTilePushed(myCurrentTile.tilesInLineLeft[tilesToPush]);
                    }

                    for (int i = 0; i - 1 < tilesToPush; i++)
                    {

                        if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                        {
                            if (myCurrentTile.tilesInLineLeft[i].unitOnTile != unitToAttack)
                            {
                                //UNDO
                                CreateAttackCommand(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                            }

                            //Hago daño
                            DoDamage(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        }
                    }
                }
              
                SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
            }

            else
            {               
                //Animación de ataque
                myAnimator.SetTrigger("Attack");

                //UNDO
                CreateAttackCommand(unitToAttack);

                //Hago daño
                DoDamage(unitToAttack);

                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.East)
                {
                    unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
                }

                SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
            }
        }

        else if (pushWider)
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
            if (pushWider2)
            {
                StunUnit(unitToAttack);
            }

            //Hago daño
            DoDamage(unitToAttack);


            if (currentFacingDirection == FacingDirection.North)
            {
                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {

                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineUp, damageMadeByPush, damageMadeByFall);

                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    }

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineUp, damageMadeByPush, damageMadeByFall);

                    }
                }

                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineDown, damageMadeByPush, damageMadeByFall);


                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineDown, damageMadeByPush, damageMadeByFall);
                    }
                }

                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                //UNDO
                CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineRight, damageMadeByPush, damageMadeByFall);

                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineRight, damageMadeByPush, damageMadeByFall);

                    }
                }
                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);

            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall);

                       
                    }
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    //UNDO
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    //Este bool es para la segunda mejora (voy stunneando antes de hacerles daño)
                    if (pushWider2)
                    {
                        StunUnit(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    }
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.ResetColor();
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.SetActive(false);

                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.ExecutePush(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall);
                    }
                }

                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
            }

            SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
        }

        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            //Hago daño
            DoDamage(unitToAttack);

            if (currentFacingDirection == FacingDirection.North)
            {
                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall);
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                unitToAttack.ExecutePush(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall);
            }

            SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);
        }

        HideAttackEffect(unitToAttack);
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
            else if (lateralBlock)
            {
                //Este if sirve para comprobar en que dirección atacan los enemigos y en que dirección está el knight
                if ((currentFacingDirection == FacingDirection.North || currentFacingDirection == FacingDirection.South)
                    && (unitAttacker.currentFacingDirection == FacingDirection.West || unitAttacker.currentFacingDirection == FacingDirection.East)
                    ||
                    (currentFacingDirection == FacingDirection.East || currentFacingDirection == FacingDirection.West)
                    && (unitAttacker.currentFacingDirection == FacingDirection.North || unitAttacker.currentFacingDirection == FacingDirection.South))
                {

                    damageReceived -= damageLateralBlocked;

                    base.ReceiveDamage(damageReceived, unitAttacker);
                }

                else if (backBlock)
                {

                    if (currentFacingDirection == FacingDirection.North && unitAttacker.currentFacingDirection == FacingDirection.North
                || currentFacingDirection == FacingDirection.East && unitAttacker.currentFacingDirection == FacingDirection.East
                || currentFacingDirection == FacingDirection.South && unitAttacker.currentFacingDirection == FacingDirection.South
                || currentFacingDirection == FacingDirection.West && unitAttacker.currentFacingDirection == FacingDirection.West)
                    {

                        damageReceived -= damageBackBlocked;

                        base.ReceiveDamage(damageReceived, unitAttacker);

                    }
                    else
                    {
                        base.ReceiveDamage(damageReceived, unitAttacker);
                    }
                }
                else
                {
                    base.ReceiveDamage(damageReceived, unitAttacker);
                }

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

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        if (pushWider)
        {
            if (currentFacingDirection == FacingDirection.North)
            {
                if (_unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineUp, damageMadeByPush, damageMadeByFall).transform.position;
                    }
                }

                if (_unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineUp, damageMadeByPush, damageMadeByFall).transform.position;

                    }
                }
                tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                SetShadowRotation(_unitToAttack);
                _unitToAttack.shaderHover.SetActive(true);
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall).transform.position;



            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (_unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft[0].tilesInLineDown, damageMadeByPush, damageMadeByFall).transform.position;
                    }
                }

                if (_unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight[0].tilesInLineDown, damageMadeByPush, damageMadeByFall).transform.position;
                    }
                }

                tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                SetShadowRotation(_unitToAttack);
                _unitToAttack.shaderHover.SetActive(true);
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall).transform.position;

               
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (_unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {


                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineRight, damageMadeByPush, damageMadeByFall).transform.position;

                    }
                }

                if (_unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {


                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineRight, damageMadeByPush, damageMadeByFall).transform.position;

                    }
                }

                tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                SetShadowRotation(_unitToAttack);
                _unitToAttack.shaderHover.SetActive(true);
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall).transform.position;


               

            }

            else if (currentFacingDirection == FacingDirection.West)
            {

                if (_unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {


                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall).transform.position;

                    }
                }

                if (_unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {

                    if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0]);
                        SetShadowRotation(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.SetActive(true);
                        currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown[0].tilesInLineLeft, damageMadeByPush, damageMadeByFall).transform.position;

                    }
                }

                tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
                SetShadowRotation(_unitToAttack);
                _unitToAttack.shaderHover.SetActive(true);
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall).transform.position;


                
            }

            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                
                tilesInEnemyHover[i].ColorAttack();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(-1);
                    if (pushWider2)
                    {
                        SetStunIcon(tilesInEnemyHover[i].unitOnTile, true, true);
                        //Descomentar si se quiere cambiar el sitio donde aparece
                      //  tilesInEnemyHover[i].unitOnTile.stunIcon.transform.position = tilesInEnemyHover[i].unitOnTile.shaderHover.transform.position;
                    }
                }
            }



        }

        else if (pushFarther2)
        {
            if (currentFacingDirection == FacingDirection.North)
            {
                if (myCurrentTile.tilesInLineUp[tilesToPush].unitOnTile == null
                    && myCurrentTile.tilesInLineUp[tilesToPush] != null
                    && !myCurrentTile.tilesInLineUp[tilesToPush].isEmpty
                    && !myCurrentTile.tilesInLineUp[tilesToPush].isObstacle)
                {
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = myCurrentTile.tilesInLineUp[tilesToPush].transform.position;
                }

                for (int i = 0; i - 1 < tilesToPush; i++)
                {

                    if (myCurrentTile.tilesInLineUp[i].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(myCurrentTile.tilesInLineUp[i]);
                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {

                for (int i = 0; i - 1 < tilesToPush; i++)
                {
                    if (myCurrentTile.tilesInLineDown[tilesToPush].unitOnTile == null
                   && myCurrentTile.tilesInLineDown[tilesToPush] != null
                   && !myCurrentTile.tilesInLineDown[tilesToPush].isEmpty
                   && !myCurrentTile.tilesInLineDown[tilesToPush].isObstacle)
                    {
                        _unitToAttack.shaderHover.SetActive(true);
                        _unitToAttack.shaderHover.transform.position = myCurrentTile.tilesInLineDown[tilesToPush].transform.position;
                    }

                    if (myCurrentTile.tilesInLineDown[i].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(myCurrentTile.tilesInLineDown[i]);

                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (myCurrentTile.tilesInLineRight[tilesToPush].unitOnTile == null
                    && myCurrentTile.tilesInLineRight[tilesToPush] != null
                    && !myCurrentTile.tilesInLineRight[tilesToPush].isEmpty
                    && !myCurrentTile.tilesInLineRight[tilesToPush].isObstacle)
                {
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = myCurrentTile.tilesInLineRight[tilesToPush].transform.position;
                }

                for (int i = 0; i - 1 < tilesToPush; i++)
                {
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(myCurrentTile.tilesInLineRight[i]);

                    }
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                if (myCurrentTile.tilesInLineLeft[tilesToPush].unitOnTile == null
                   && myCurrentTile.tilesInLineLeft[tilesToPush] != null
                   && !myCurrentTile.tilesInLineLeft[tilesToPush].isEmpty
                   && !myCurrentTile.tilesInLineLeft[tilesToPush].isObstacle)
                {
                    _unitToAttack.shaderHover.SetActive(true);
                    _unitToAttack.shaderHover.transform.position = myCurrentTile.tilesInLineLeft[tilesToPush].transform.position;
                }

                for (int i = 0; i - 1 < tilesToPush; i++)
                {

                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null)
                    {
                        tilesInEnemyHover.Add(myCurrentTile.tilesInLineLeft[i]);

                    }
                }
            }




        }

        else
        {
            tilesInEnemyHover.Clear();

            _unitToAttack.shaderHover.SetActive(true);
            tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
            if (currentFacingDirection == FacingDirection.North)
            {
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall).transform.position;

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall));

                CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineUp, damageMadeByPush, damageMadeByFall).ColorAttack();
                
                
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall).transform.position;

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall));

                CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineDown, damageMadeByPush, damageMadeByFall).ColorAttack();
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall).transform.position;

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall));

                CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineRight, damageMadeByPush, damageMadeByFall).ColorAttack();
            }

            else if (currentFacingDirection == FacingDirection.West)
            {
                _unitToAttack.shaderHover.transform.position = CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall).transform.position;

                //Añado esta línea para luego pintar el tile al que se va a mover
                tilesInEnemyHover.Add(CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall));

                CalculatePushLogic(tilesToPush, myCurrentTile.tilesInLineLeft, damageMadeByPush, damageMadeByFall).ColorAttack();
            }


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

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        shieldBlockAllDamage.SetActive(false);
        _unitToAttack.shaderHover.SetActive(false);

        tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);
        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            CalculateDamage(currentUnitsAvailableToAttack[i]);
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
          
        }

        if (tilesInEnemyHover.Count > 0)
        {
            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    if (pushWider2)
                    {
                        SetStunIcon(tilesInEnemyHover[i].unitOnTile, true, false);

                    }

                    tilesInEnemyHover[i].unitOnTile.hoverImpactIcon.SetActive(false);
                    tilesInEnemyHover[i].ColorDesAttack();
                    tilesInEnemyHover[i].unitOnTile.ResetColor();
                    tilesInEnemyHover[i].unitOnTile.HealthBarOn_Off(false);
                    tilesInEnemyHover[i].ColorBorderRed();
                    tilesInEnemyHover[i].unitOnTile.DisableCanvasHover();
                }
            }
        }

        tilesInEnemyHover.Clear(); 
        
    }
}
