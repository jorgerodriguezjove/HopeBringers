using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class Berserker : PlayerUnit
{
    #region VARIABLES
    [Header("STATS DE CLASE")]
    //Indica si el berserker está en Rage
    [HideInInspector]
    public bool isInRage;

    public GameObject isInRageIcon;

    //Al llegar a 0, el rage se quita
    [HideInInspector]
    public int turnsLeftToRageOff;
    [SerializeField]
    private int maxNumberOfTurnsInRage;

    [SerializeField]
    public Material rageMaterial;    
    private Material finitMaterial;

    [SerializeField]
    private GameObject particleMultpleAttack;

    [SerializeField]
    private GameObject particleCircularAttac;

    [Header("MEJORAS DE PERSONAJE")]

    //[Header("Activas")]
    //ACTIVAS
    public bool circularAttack;

    public FacingDirection previousDirection;

    //Esta variable tiene que cambiar en la mejora 2 de este ataque
    public int timesCircularAttackRepeats;

    public bool areaAttack;
    //Esta variable tiene que cambiar en la mejora 2 de este ataque
    public int bonusDamageAreaAttack;

    [Header("Pasivas")]
    //PASIVAS
    [SerializeField]
    //Este es el int que hay que cambiar para que el rage haga más daño
    private int rageDamagePlus;

    [SerializeField]
    private bool rageFear;
    [SerializeField]
    //Este es el int que hay que cambiar para que el el berserker meta más turnos de miedo
    private int fearTurnBonus;
    #endregion

    

    public void SetSpecificStats(bool _areaAttack, int _areaAttack2,
                                 bool _circularAttack1, int _circularAttack2,
                                 int _rageDamagePlus1, int _rageDamagePlus2, 
                                 bool _rageFear, int _fearTurnBonus)
    {

        //IMPORTANTE REVISAR QUE ESTAN BIEN LOS TEXTOS (NO ESTOY SEGURO DE HABER CORRESPONDIDO CADA MEJORA CON SU TEXTO BIEN)

        activeSkillInfo = AppBerserkUpgrades.BerserkDataBaseActive;
        pasiveSkillInfo = AppBerserkUpgrades.BerserkDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "BerserkDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "BerserkDataBasePasive");

        #region Actives

        areaAttack = _areaAttack;
        bonusDamageAreaAttack = _areaAttack2;
        
        circularAttack = _circularAttack1;
        timesCircularAttackRepeats = _circularAttack2;

        //CHECK
        if (_areaAttack2 > 0)
        {
            bonusDamageAreaAttack = 2;

            activeSkillInfo = AppBerserkUpgrades.areaAttack2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.areaAttack1);
        }

        else if (areaAttack)
        {
            activeSkillInfo = AppBerserkUpgrades.areaAttack1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.areaAttack1);
        }

        if (_circularAttack2 > 1)
        {
            //Aqui no hace falta timesCircularAttackRepeats, porque ya esta arriba.

            activeSkillInfo = AppBerserkUpgrades.circularAttack2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.circularAttack1);
        }

        else if (circularAttack)
        {
            timesCircularAttackRepeats = 1;

            activeSkillInfo = AppBerserkUpgrades.circularAttack1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.circularAttack1);
        }

        #endregion

        #region Pasives

        rageDamagePlus = _rageDamagePlus2;

        if (rageDamagePlus < _rageDamagePlus1)
        {
            rageDamagePlus = _rageDamagePlus1;
        }

        rageFear = _rageFear;
        fearTurnBonus = _fearTurnBonus;

        if (rageDamagePlus > 1)
        {
            pasiveSkillInfo = AppBerserkUpgrades.rageDamage2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.rageDamage1);
        }

        else if (rageFear)
        {
            pasiveSkillInfo = AppBerserkUpgrades.rageDamage1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.rageDamage1);
        }

        if (fearTurnBonus > 1)
        {
            //Aqui no hace falta tocar fearTurnBonus, porque ya se iguala a _fearTurnBonus

            pasiveSkillInfo = AppBerserkUpgrades.fearRage2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.fearRage1);
        }

        else if (fearTurnBonus > 0)
        {
            fearTurnBonus = 1;

            pasiveSkillInfo = AppBerserkUpgrades.fearRage1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppBerserkUpgrades.fearRage1);
        }


        #endregion
    }

    public override void CheckWhatToDoWithSpecialsTokens()
    {
        //Base del rage
        myPanelPortrait.GetComponent<Portraits>().specialToken2.SetActive(true);
        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.enabled = true;

        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.text = turnsLeftToRageOff.ToString();

        myPanelPortrait.GetComponent<Portraits>().specialSkillImage2.sprite = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "BerserkDataBasePasive");
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

        if (areaAttack)
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

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        CheckIfUnitHasMarks(unitToAttack);
        HideAttackEffect(unitToAttack);

        if (circularAttack)
        {
            particleCircularAttac.SetActive(true);
            previousDirection = currentFacingDirection;
            //Animación de ataque 
            //HAY QUE HACER UNA PARA EL ATAQUE GIRATORIO
            myAnimator.SetTrigger("Attack");
            for (int i = 0; i < timesCircularAttackRepeats; i++)
            {
                if (myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    CreateAttackCommand(myCurrentTile.tilesInLineUp[0].unitOnTile);

                    currentFacingDirection = FacingDirection.North;
                    DoDamage(myCurrentTile.tilesInLineUp[0].unitOnTile);
                }
                
                if (myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    CreateAttackCommand(myCurrentTile.tilesInLineDown[0].unitOnTile);

                    currentFacingDirection = FacingDirection.South;
                    DoDamage(myCurrentTile.tilesInLineDown[0].unitOnTile);
                }

               
                if (myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    CreateAttackCommand(myCurrentTile.tilesInLineRight[0].unitOnTile);

                    currentFacingDirection = FacingDirection.East;
                    DoDamage(myCurrentTile.tilesInLineRight[0].unitOnTile);
                }

               
                if (myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    CreateAttackCommand(myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    currentFacingDirection = FacingDirection.West;
                    DoDamage(myCurrentTile.tilesInLineLeft[0].unitOnTile);
                }

               
            }

            currentFacingDirection = previousDirection;
             //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else if (areaAttack)
        {

           

            if (currentFacingDirection == FacingDirection.North)
            {
                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);                   
                }

                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);                  
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {

                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    CreateAttackCommand(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);

                    Instantiate(particleMultpleAttack, currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile.transform);

                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                }
            }

            Instantiate(particleMultpleAttack, unitToAttack.transform);

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            CreateAttackCommand(unitToAttack);

            //Hago daño
            DoDamage(unitToAttack);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            CreateAttackCommand(unitToAttack);

            Instantiate(attackParticle, unitToAttack.transform);

            //Hago daño
            DoDamage(unitToAttack);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorDesAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ResetColor();
            }
        }

        tilesInEnemyHover.Clear();
    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        
            if (rageFear)
            {
                ApplyBuffOrDebuffDamage(unitToDealDamage, -1, fearTurnBonus);
            }

            CalculateDamage(unitToDealDamage);
            //Una vez aplicados los multiplicadores efectuo el daño.
            unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);

            //Añado este if para el count de honor del samurai
            if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
           || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
           || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
           || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
           )
            {
                LM.honorCount++;
            }
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Independientemente de si esta en rage o no, si recibe daño se vuelve a activar el rage

        //Activo el rage
        isInRage = true;
        isInRageIcon.SetActive(true);
        
        //La primera vez que entra en rage inicializo los turnos que puede estar en rage.
        turnsLeftToRageOff = maxNumberOfTurnsInRage - 1;
        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.enabled = true;
        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.text = turnsLeftToRageOff.ToString();
        turnsLeftToRageOff = maxNumberOfTurnsInRage;
        //Cambiar material
        RageColor();


        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public override void CalculateDamage(UnitBase unitToDealDamage)
    {
        //Reseteo la variable de daño a realizar
        damageWithMultipliersApplied = baseDamage;

        //Si estoy en desventaja de altura hago menos daño
        if (unitToDealDamage.myCurrentTile.height > myCurrentTile.height)
        {
            damageWithMultipliersApplied -= penalizatorDamageLessHeight;
            healthBar.SetActive(true);
            downToUpDamageIcon.SetActive(true);
        }

        //Si estoy en ventaja de altura hago más daño
        else if (unitToDealDamage.myCurrentTile.height < myCurrentTile.height)
        {
            damageWithMultipliersApplied += bonusDamageMoreHeight;
            healthBar.SetActive(true);
            upToDownDamageIcon.SetActive(true);
        }

        //Si le ataco por la espalda hago más daño
        if (unitToDealDamage.currentFacingDirection == currentFacingDirection)
        {
            if (unitToDealDamage.GetComponent<EnDuelist>()
               && unitToDealDamage.GetComponent<EnDuelist>().hasTier2
               && hasAttacked)
            {

                if (currentFacingDirection == FacingDirection.North)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 180, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.South;
                }

                else if (currentFacingDirection == FacingDirection.South)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 0, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.North;
                }

                else if (currentFacingDirection == FacingDirection.East)
                {

                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, -90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.West;
                }

                else if (currentFacingDirection == FacingDirection.West)
                {
                    unitToDealDamage.unitModel.transform.DORotate(new Vector3(0, 90, 0), timeDurationRotation);
                    unitToDealDamage.currentFacingDirection = FacingDirection.East;
                }

            }
            else
            {
                //Añado este if para que, cada vez que ataque un jugador y si le va a realizar daño por la espalda, el count del honor se resetea
                if (hasAttacked)
                {
                    LM.honorCount = 0;
                }
                //Ataque por la espalda
                damageWithMultipliersApplied += bonusDamageBackAttack;
                healthBar.SetActive(true);
                backStabIcon.SetActive(true);
            }
        }

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null && samuraiUpgraded.itsForHonorTime2)
        {
            damageWithMultipliersApplied += LM.honorCount;
        }

        if (isInRage)
        {          
            //Añado el daño de rage.
            damageWithMultipliersApplied += rageDamagePlus;
        }

        if (areaAttack)
        {
            //Añado el daño de rage.
            damageWithMultipliersApplied += bonusDamageAreaAttack;
        }

            damageWithMultipliersApplied += buffbonusStateDamage;       
    }

    public void RageChecker()
    {
        if (isInRage)
        {
            turnsLeftToRageOff--;

            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.enabled = true;
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = turnsLeftToRageOff.ToString();
          

            if (turnsLeftToRageOff <= 0)
            {
                isInRage = false;
                isInRageIcon.SetActive(false);
                turnsLeftToRageOff = maxNumberOfTurnsInRage;
                myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.enabled = false;
                
                RageColor();
            }
        }
    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {
        tilesInEnemyHover.Clear();

        if (areaAttack)
        {
            if (currentFacingDirection == FacingDirection.North)
            {
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0]);

                }

                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0]);
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0]);


                }

                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0]);

                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0]);

                }

                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0]);

                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {

                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0]);

                }

                if (currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0] != null)
                {
                    tilesInEnemyHover.Add(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0]);

                }
            }
        }

        else if (circularAttack)
        {
            previousDirection = currentFacingDirection;
            if (myCurrentTile.tilesInLineUp[0].unitOnTile != null)
            {

                tilesInEnemyHover.Add(myCurrentTile.tilesInLineUp[0]);
                currentFacingDirection = FacingDirection.North;
                CalculateDamage(myCurrentTile.tilesInLineUp[0].unitOnTile);
                myCurrentTile.tilesInLineUp[0].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);



            }

            if (myCurrentTile.tilesInLineDown[0].unitOnTile != null)
            {
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineDown[0]);
                currentFacingDirection = FacingDirection.South;
                CalculateDamage(myCurrentTile.tilesInLineDown[0].unitOnTile);
                myCurrentTile.tilesInLineDown[0].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);

            }


            if (myCurrentTile.tilesInLineRight[0].unitOnTile != null)
            {
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineRight[0]);
                currentFacingDirection = FacingDirection.East;
                CalculateDamage(myCurrentTile.tilesInLineRight[0].unitOnTile);
                myCurrentTile.tilesInLineRight[0].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);

            }


            if (myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
            {
                tilesInEnemyHover.Add(myCurrentTile.tilesInLineLeft[0]);
                currentFacingDirection = FacingDirection.West;
                CalculateDamage(myCurrentTile.tilesInLineLeft[0].unitOnTile);
                myCurrentTile.tilesInLineLeft[0].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);

            }
            currentFacingDirection = previousDirection;
        }

        tilesInEnemyHover.Add(_unitToAttack.myCurrentTile);

        
            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                tilesInEnemyHover[i].ColorAttack();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    if (rageFear)
                    {
                        SetBuffDebuffIcon(-1, _unitToAttack, true);
                    }

                    if (circularAttack)
                    {
                        if (timesCircularAttackRepeats >= 2)
                        {
                            tilesInEnemyHover[i].unitOnTile.timesRepeatNumber.enabled = true;
                            tilesInEnemyHover[i].unitOnTile.timesRepeatNumber.text = ("X" + timesCircularAttackRepeats.ToString());
                        }

                    }
                    else
                    {

                    tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied + bonusDamageAreaAttack);
                    }
                }
            }
        
       

    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {


        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorDesAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                if (rageFear)
                {
                    SetBuffDebuffIcon(0, _unitToAttack, true);

                }

                if (circularAttack)
                {
                    if (timesCircularAttackRepeats >= 2)
                    {
                        tilesInEnemyHover[i].unitOnTile.timesRepeatNumber.enabled = false;

                    }

                }
                tilesInEnemyHover[i].unitOnTile.ResetColor();
                tilesInEnemyHover[i].unitOnTile.DisableCanvasHover();
            }
        }


        tilesInEnemyHover.Clear();
    }

    public override void UndoAttack(AttackCommand lastAttack)
    {
        base.UndoAttack(lastAttack);

        isInRage = lastAttack.isInRage;
        turnsLeftToRageOff = lastAttack.rageTurnsLeft;

        if (!isInRage)
        {
            ResetColor();
        }

        Debug.Log("woooooowooo");

        //Quitar efectos de rage visuales si se le quita con el undo
    }

    #region COLORS
    public virtual void RageColor()
    {
        if (!isDead)
        {
            if (isInRage)
            {
                finitMaterial = initMaterial;
                initMaterial = rageMaterial;
                unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;
            }
            else
            {
                initMaterial = finitMaterial;
                unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = initMaterial;

            }
        }

    }

    #endregion
}
