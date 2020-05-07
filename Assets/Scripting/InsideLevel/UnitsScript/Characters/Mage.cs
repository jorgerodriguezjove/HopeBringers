using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : PlayerUnit
{
    #region VARIABLES

    [Header("SPECIAL VARIABLES FOR CHARACTER")]

    [SerializeField]
    protected GameObject chargingParticle;

    [SerializeField]
    private GameObject particleAreaAttack;

    [SerializeField]
    private GameObject particleLightning;

    //Prefab del mage decoy
    [SerializeField]
    protected GameObject mageDecoyRefAsset;

    //Lista con decoys que tiene este mago.
    [SerializeField]
    public List<GameObject> myDecoys = new List<GameObject>();

    

    //Número máximo de decoys que se pueden instanciar
    [SerializeField]
    private int maxDecoys;

    
    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS

    public bool areaAttack;
    public bool areaAttack2;
    [HideInInspector]
    public int areaRange = 1;

    public bool lightningChain;
    public int timeElectricityAttackExpands;
    //Este int para que vuelva a su estado principal
    public int fTimeElectricityAttackExpands;

    //Mejora del ataque (no hace daño a aliados y cada vez que hace la cadena, aumenta el daño)
    public bool lightningChain2;

    public int limitantAttackBonus;
    //Las dos siguientes variables las suelo poner en el awake pero se puede poner de forma manual. Hay que mirar como solucionarlo
    //Este int lo añado para que el limite de ataques vuelva a su estado del principio antes de volver a atacar
    public int fLimitantAttackBonus;
    //Este int lo añado para que el ataque del mago vuelva a su estado del principio antes de volver a atacar
    public int fBaseDamage;

    [HideInInspector]
    public List<UnitBase> attackingUnits;
    [HideInInspector]
    public List<UnitBase> nextUnits;
    [HideInInspector]
    public List<UnitBase> unitsFinished;

    [Header("Pasivas")]
    //PASIVAS
    public bool isDecoyBomb;
    //Esta mejora está en los decoys
    public bool isDecoyBomb2;

    public bool mirrorDecoy;
    public bool mirrorDecoy2;

    #endregion

    public void SetSpecificStats(bool _lightningChain1, bool _lightningChain2,
                                 bool _crossAreaAttack1,bool _crossAreaAttack2,
                                 bool _bombDecoy1, bool _bombDecoy2,
                                 bool _mirrorDecoy1, bool _mirrorDecoy2)
    {
        activeSkillInfo = AppMageUpgrades.MageDataBaseActive;
        pasiveSkillInfo = AppMageUpgrades.MageDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "MageDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "MageDataBasePasive");

        #region Actives

        lightningChain = _lightningChain1;
        lightningChain2 = _lightningChain2;
            
        areaAttack = _crossAreaAttack1;
        areaAttack2 = _crossAreaAttack2;
        areaRange = 1;

        fBaseDamage = baseDamage;

        if (lightningChain2)
        {
            timeElectricityAttackExpands = 999;
            limitantAttackBonus = 3;

            activeSkillInfo = AppMageUpgrades.lightningChain2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.lightningChain1);
        }

        else if(lightningChain)
        {
            timeElectricityAttackExpands = 3;
            limitantAttackBonus = 0;

            activeSkillInfo = AppMageUpgrades.lightningChain1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.lightningChain1);
        }

        if (areaAttack2)
        {
            activeSkillInfo = AppMageUpgrades.crossAreaAttack2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.crossAreaAttack1);
        }

        else if (areaAttack)
        {
            activeSkillInfo = AppMageUpgrades.crossAreaAttack1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.crossAreaAttack1);
        }

        //Importante que este después de las mejoras
        fTimeElectricityAttackExpands = timeElectricityAttackExpands;
        fLimitantAttackBonus = limitantAttackBonus;

        #endregion

        #region Pasives

        isDecoyBomb = _bombDecoy1;
        isDecoyBomb2 = _bombDecoy2;

        mirrorDecoy = _mirrorDecoy1;
        mirrorDecoy2 = _mirrorDecoy2;


        if (isDecoyBomb2)
        {
            pasiveSkillInfo = AppMageUpgrades.bombDecoy2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.bombDecoy1);
        }

        else if (isDecoyBomb)
        {
            pasiveSkillInfo = AppMageUpgrades.bombDecoy1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.bombDecoy1);
        }

        if (mirrorDecoy2)
        {
            pasiveSkillInfo = AppMageUpgrades.mirrorDecoy2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.mirrorDecoy1);
        }

        else if (mirrorDecoy)
        {
            
            pasiveSkillInfo = AppMageUpgrades.mirrorDecoy1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppMageUpgrades.mirrorDecoy1);
        }

        #endregion
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        CheckIfUnitHasMarks(unitToAttack);

        if (mirrorDecoy)
        {
            for (int i = 0; i < myDecoys.Count; i++)
            {
                //En el override de esta función el decoy también comprueba si tiene la segunda mejora y ataca de una forma o de la otra
                myDecoys[i].GetComponent<MageDecoy>().CheckUnitsAndTilesInRangeToAttack(true);
                myDecoys[i].GetComponent<MageDecoy>().HideAttackEffect(null);
            }

        }

        Instantiate(chargingParticle, gameObject.transform.position, chargingParticle.transform.rotation);

      

        if (areaAttack)
        {
            Instantiate(particleAreaAttack, unitToAttack.transform.position, unitToAttack.transform.rotation);

            if (areaAttack2)
            {
                //Animación de ataque 
                //HAY QUE HACER UNA PARA EL ATAQUE EN CRUZ O PARTÍCULAS
                //myAnimator.SetTrigger("Attack");

                //UNDO
                CreateAttackCommand(unitToAttack);

                //COMPROBAR QUE NO DE ERROR EN OTRAS COSAS
                TM.surroundingTiles.Clear();

                TM.GetSurroundingTiles(unitToAttack.myCurrentTile, areaRange, true, false);

                //Hago daño
                DoDamage(unitToAttack);

                //Hago daño a las unidades adyacentes
                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    if (TM.surroundingTiles[i].unitOnTile != null)
                    {
                        //UNDO
                        CreateAttackCommand(TM.surroundingTiles[i].unitOnTile);

                        DoDamage(TM.surroundingTiles[i].unitOnTile);
                    }
                }
        
            }

            else
            {
                //Animación de ataque 
                //HAY QUE HACER UNA PARA EL ATAQUE EN CRUZ O PARTÍCULAS
                //myAnimator.SetTrigger("Attack");                              
                //Hago daño a las unidades adyacentes
                for (int i = 0; i < unitToAttack.myCurrentTile.neighbours.Count; ++i)
                {
                    if (unitToAttack.myCurrentTile.neighbours[i].unitOnTile != null)
                    {
                        //UNDO
                        CreateAttackCommand(unitToAttack.myCurrentTile.neighbours[i].unitOnTile);

                        DoDamage(unitToAttack.myCurrentTile.neighbours[i].unitOnTile);
                    }
                }

                //UNDO
                CreateAttackCommand(unitToAttack);

                //Hago daño
                DoDamage(unitToAttack);
            }

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else if (lightningChain)
        {
            //unitsToAttack.Clear();
            //nextUnits.Clear();
            unitsFinished.Clear();

            if (lightningChain2 && unitToAttack.GetComponent<PlayerUnit>())
            {

            }
            else
            {
                //UNDO
                CreateAttackCommand(unitToAttack);

                //Hago daño
                DoDamage(unitToAttack);
            }

            attackingUnits.Add(unitToAttack);

            for (int i = 0; i < attackingUnits.Count; i++)
            {                
                    for (int j = 0; j < attackingUnits[i].myCurrentTile.neighbours.Count; j++)
                    {
                        if (attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile != null &&
                            !unitsFinished.Contains(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile))
                        {
                            //UNDO
                            CreateAttackCommand(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile);

                            Instantiate(particleLightning, unitToAttack.transform.position, unitToAttack.transform.rotation);

                            DoDamage(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile);

                            nextUnits.Add(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile);
                        }
                    }

                unitsFinished.Add(attackingUnits[i]);
                attackingUnits.Remove(attackingUnits[i]);

                if ( attackingUnits.Count == 0)
                {
                    timeElectricityAttackExpands--;
                    if (timeElectricityAttackExpands > 0)
                    {
                        for (int k = 0; k < nextUnits.Count; k++)
                        {
                            attackingUnits.Add(nextUnits[k]);
                            nextUnits.Remove(nextUnits[k]);
                        }

                        return;
                    }
                    else
                    {
                        break;
                    }                    
                }                                                   
            }
           
            limitantAttackBonus = fLimitantAttackBonus;
            baseDamage = fBaseDamage;
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else
        {
            //UNDO
            CreateAttackCommand(unitToAttack);

            Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

            //Hago daño
            DoDamage(unitToAttack);

            SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
    }

    //Override especial del mago para anlar ventajas por altura y daño por la espalda
    public override void CalculateDamage(UnitBase unitToDealDamage)
    {
        //Reseteo la variable de daño a realizar
        damageWithMultipliersApplied = baseDamage;

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null && samuraiUpgraded.itsForHonorTime2)
        {
            damageWithMultipliersApplied += LM.honorCount;

        }

        damageWithMultipliersApplied += buffbonusStateDamage;

        Debug.Log("Daño base: " + baseDamage + " Daño con multiplicadores " + damageWithMultipliersApplied);
    }
    
    //Override especial del mago para que no instancie la partícula de ataque
    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        
            CalculateDamage(unitToDealDamage);
        

            //Añado este if para el count de honor del samurai
        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
        ||  currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
        ||  currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
        ||  currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East)
        {           
                LM.honorCount++;            
        }

        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
    }

    #region MOVEMENT

    IndividualTiles oldTile;

    //El LevelManager avisa a la unidad de que debe moverse.
    //Esta función tiene que ser override para que el mago pueda instanciar decoys.
    public override void MoveToTile(IndividualTiles tileToMove, List<IndividualTiles> pathReceived)
    {
        oldTile = myCurrentTile;

        base.MoveToTile(tileToMove,pathReceived);

        if (tileToMove != oldTile)
        {
            //Compruebo si tengo que instanciar decoy
            CheckDecoy(oldTile);
        }
    }

    public void CheckDecoy(IndividualTiles tileForDecoy)
    {
        if (myDecoys.Count < maxDecoys)
        {
            //Instancio el decoy
            InstantiateDecoy(tileForDecoy);
        }

        else
        {
            //Destruyo al decoy anterior
            GameObject decoyToDestroy = myDecoys[0];
            Destroy(decoyToDestroy);
            LM.charactersOnTheBoard.Remove(decoyToDestroy.GetComponent<PlayerUnit>());
            myDecoys.Remove(decoyToDestroy);

            //Instancio el decoy
            InstantiateDecoy(tileForDecoy);
        }
    }

    public void InstantiateDecoy(IndividualTiles tileForDecoy)
    {
        GameObject decoyToInstantiate = Instantiate(mageDecoyRefAsset, transform.position, transform.rotation);

        //Inicializo mageDecoy. SI VUELVE A SALTAR ERROR POR EL TILE DEL MAGEDECOY NO TOCAR ESTO, HABLAR EL PROBLEMA PARA QUE SOLUCIONARLO ENTRE LOS DOS Y QUE NO VUELVA A SALIR
        decoyToInstantiate.GetComponent<MageDecoy>().myMage = this;
        decoyToInstantiate.GetComponent<MageDecoy>().InitializeHealth();
        decoyToInstantiate.GetComponent<MageDecoy>().myCurrentTile = tileForDecoy;
        decoyToInstantiate.GetComponent<MageDecoy>().myCurrentTile.WarnInmediateNeighbours();
        decoyToInstantiate.GetComponent<MageDecoy>().UpdateInformationAfterMovement(tileForDecoy);

        //Pongo esta referencia para que el mage solo pueda cambiarse con sus decoys y para que pueda comprobar sus booleanos (para las habilidades)
        myDecoys.Add(decoyToInstantiate);
    }

    #endregion

    #region CHECKS
    //Hago override a esta función para que pueda atravesar unidades al atacar.
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


    public override void UndoMove(IndividualTiles tileToMoveBack, FacingDirection rotationToTurnBack, bool shouldResetMovement)
    {
        base.UndoMove(tileToMoveBack, rotationToTurnBack, shouldResetMovement);

        if (shouldResetMovement)
        {
            if (myDecoys.Count > 0)
            {
                Destroy(myDecoys[myDecoys.Count - 1]);
                myDecoys.RemoveAt(myDecoys.Count - 1);
            }
        }
    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        tilesInEnemyHover.Clear();
        tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);

        if (areaAttack)
        {
            if (areaAttack2)
            {
                TM.GetSurroundingTiles(_unitToAttack.myCurrentTile, areaRange, true, false);

                for (int i = 0; i < TM.surroundingTiles.Count; ++i)
                {
                    tilesInEnemyHover.Add(TM.surroundingTiles[i]);
                }

            }
            else
            {
                //Animación de ataque 
                //HAY QUE HACER UNA PARA EL ATAQUE EN CRUZ O PARTÍCULAS
                //myAnimator.SetTrigger("Attack");                              
                //Hago daño a las unidades adyacentes

                for (int i = 0; i < _unitToAttack.myCurrentTile.neighbours.Count; ++i)
                {
                    if (_unitToAttack.myCurrentTile.neighbours[i] != null)
                    {
                        tilesInEnemyHover.Add(_unitToAttack.myCurrentTile.neighbours[i]);
                    }
                }


            }

        }
        else if (lightningChain)
        {
            attackingUnits.Clear();
            attackingUnits.Add(_unitToAttack);
         
            for (int i = 0; i < attackingUnits.Count; i++)
            {
                for (int j = 0; j < attackingUnits[i].myCurrentTile.neighbours.Count; j++)
                {
                    if (attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile != null &&
                        !unitsFinished.Contains(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile))
                    {
                        nextUnits.Add(attackingUnits[i].myCurrentTile.neighbours[j].unitOnTile);
                        tilesInEnemyHover.Add(attackingUnits[j].myCurrentTile.neighbours[j]);

                    }
                }

                unitsFinished.Add(attackingUnits[i]);
                attackingUnits.Remove(attackingUnits[i]);

                if (attackingUnits.Count == 0)
                {
                    timeElectricityAttackExpands--;
                    if (timeElectricityAttackExpands > 0)
                    {
                        for (int k = 0; k < nextUnits.Count; k++)
                        {
                            attackingUnits.Add(nextUnits[k]);
                            nextUnits.Remove(nextUnits[k]);
                        }

                        return;
                    }
                    else
                    {
                        break;
                    }
                }
            }

                   
        }

        limitantAttackBonus = fLimitantAttackBonus;
        timeElectricityAttackExpands = fTimeElectricityAttackExpands;
        attackingUnits.Clear();

        if (mirrorDecoy)
        {
            for (int i = 0; i < myDecoys.Count; i++)
            {
                myDecoys[i].GetComponent<MageDecoy>().CheckUnitsAndTilesToColorAtHover();
            }

        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
            }
        }
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        if (mirrorDecoy)
        {
            for (int i = 0; i < myDecoys.Count; i++)
            {
                myDecoys[i].GetComponent<MageDecoy>().HideAttackEffect(_unitToAttack);
            }

        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorBorderRed();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ResetColor();
                tilesInEnemyHover[i].unitOnTile.DisableCanvasHover();

            }
        }
        tilesInEnemyHover.Clear();
    }

    public override void UndoAttack(AttackCommand lastAttack)
    {
        base.UndoAttack(lastAttack);

        //myDecoys.
        //Hacer la movida de los decoys
    }

}
