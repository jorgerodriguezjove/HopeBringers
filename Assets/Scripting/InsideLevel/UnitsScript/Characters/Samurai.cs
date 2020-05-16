using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Samurai : PlayerUnit
{
    #region VARIABLES
    
    [SerializeField]
    private int samuraiFrontAttack;


    //Lista de unidades que no puede atacar. La añado para que el samurai pueda poner su icono al no poder atacarles por la espalda
    [HideInInspector]
    public List<UnitBase> UnitsNonAttack;

    [Header("MEJORAS DE PERSONAJE")]

    [Header("Activas")]
    //ACTIVAS

    //bool para la activa 1
    public bool parryOn;

    public GameObject parryIcon;

    //bool para la mejora de la activa 1
    public bool parryOn2;

    public UnitBase unitToParry;

    //bool para la activa 2
    public bool doubleAttack;

    //int que  indica el número de veces que el samurai ataca
    public int timesDoubleAttackRepeats;


    [Header("Pasivas")]
    //PASIVAS

    //bool para la pasiva 1
    public bool itsForHonorTime;

    //bool para la mejora de la pasiva 1
    public bool itsForHonorTime2;

    //Esta variable la uso unicamente para el undo porque en commands no puedo acceder a lm.honor
    [HideInInspector]
    public int currentHonor;

    //bool para la pasiva 2
    public bool buffLonelyArea;
    //bool que indica si tiene aliados en un área de 3x3 o no
    public bool isLonelyLikeMe;
    //int que añade daño si el samurai no tiene aliados en un área de 3x3
    public int lonelyAreaDamage;

    public GameObject isLonelyIcon;

    public GameObject lonelyBox;


    [SerializeField]
    private GameObject particleParryAttack;

    [SerializeField]
    private GameObject particleMultipleAttack;

    #endregion

    public void SetSpecificStats(bool _parry1, bool _parry2,
                                bool _multiAttack1, int _multiAttack2,
                                bool _honor1, bool _honor2,
                                bool _loneWolf1, bool _loneWolf2)
    {

        //IMPORTANTE REVISAR QUE ESTAN BIEN LOS TEXTOS (NO ESTOY SEGURO DE HABER CORRESPONDIDO CADA MEJORA CON SU TEXTO BIEN)

        activeSkillInfo = AppSamuraiUpgrades.SamuraiDataBaseActive;
        pasiveSkillInfo = AppSamuraiUpgrades.SamuraiDataBasePasive;

        activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "SamuraiDataBaseActive");
        pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + "SamuraiDataBasePasive");

        #region Actives

        parryOn = _parry1;
        parryOn2 = _parry2;

        doubleAttack = _multiAttack1;
        timesDoubleAttackRepeats = _multiAttack2;

        if (parryOn2)
        {
            activeSkillInfo = AppSamuraiUpgrades.parry2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.parry1);
        }

        else if (parryOn)
        {
            activeSkillInfo = AppSamuraiUpgrades.parry1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.parry1);
        }

        //TENER CUIDADO CON ESTA, DEPENDE DE CUANTOS ATAQUES TENGA LA SEGUNDA MEJORA
        if (timesDoubleAttackRepeats > 2)
        {
            //Aqui no hace falta timesDoubleAttackRepeats, porque ya esta arriba.

            activeSkillInfo = AppSamuraiUpgrades.multiAttack2Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.multiAttack1);
        }

        else if (doubleAttack)
        {
            timesDoubleAttackRepeats = 2;

            activeSkillInfo = AppSamuraiUpgrades.multiAttack1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.multiAttack1);
        }

        #endregion

        #region Pasives

        itsForHonorTime = _honor1;
        itsForHonorTime2 = _honor2;

        buffLonelyArea = _loneWolf1;
        isLonelyLikeMe = _loneWolf2;

        lonelyAreaDamage = 2;

        if (_honor2)
        {
            pasiveSkillInfo = AppSamuraiUpgrades.honor2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.honor1);
        }

        else if (_honor1)
        {
            pasiveSkillInfo = AppSamuraiUpgrades.honor1Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.honor1);
        }

        if (_loneWolf2)
        {
            pasiveSkillInfo = AppSamuraiUpgrades.loneWolf2Text;
            pasiveTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.loneWolf1);
        }

        else if (_loneWolf1)
        {
            pasiveSkillInfo = AppSamuraiUpgrades.loneWolf1Text;
            activeTooltipIcon = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.loneWolf1);
        }


        #endregion
    }

    public override void CheckWhatToDoWithSpecialsTokens()
    {
        CheckIfIsLonely();

        if (itsForHonorTime)
        {
            myPanelPortrait.GetComponent<Portraits>().specialToken2.SetActive(true);
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.enabled = true;

            //Cambiar el número si va a tener más de un turno
            myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.text = LM.honorCount.ToString();

            myPanelPortrait.GetComponent<Portraits>().specialSkillImage2.sprite = Resources.Load<Sprite>(AppPaths.PATH_RESOURCE_GENERIC_ICONS + AppSamuraiUpgrades.honor1);
        }


        if (unitToParry != null)
        {
            unitToParry = null;
            parryIcon.SetActive(false);
        }
    }

    public override void CheckUnitsAndTilesInRangeToAttack(bool _shouldPaintEnemiesAndShowHealthbar)
    {
        CheckIfIsLonely();

        currentUnitsAvailableToAttack.Clear();
        UnitsNonAttack.Clear();

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
                if (myCurrentTile.tilesInLineUp[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineUp[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    if (myCurrentTile.tilesInLineUp[i].unitOnTile.currentFacingDirection == FacingDirection.North)
                    {
                        UnitsNonAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);

                        break;
                    }
                    else
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineUp[i].unitOnTile);
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
                if (myCurrentTile.tilesInLineDown[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineDown[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    if (myCurrentTile.tilesInLineDown[i].unitOnTile.currentFacingDirection == FacingDirection.South)
                    {
                        UnitsNonAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);

                        break;
                    }
                    else
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineDown[i].unitOnTile);
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
                if (myCurrentTile.tilesInLineRight[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineRight[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    if (myCurrentTile.tilesInLineRight[i].unitOnTile.currentFacingDirection == FacingDirection.East)
                    {
                        UnitsNonAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
                        break;
                    }
                    else
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineRight[i].unitOnTile);
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
                if (myCurrentTile.tilesInLineLeft[i].unitOnTile != null && Mathf.Abs(myCurrentTile.tilesInLineLeft[i].height - myCurrentTile.height) <= maxHeightDifferenceToAttack)
                {
                    if (myCurrentTile.tilesInLineLeft[i].unitOnTile.currentFacingDirection == FacingDirection.West)
                    {
                        UnitsNonAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
                        break;
                    }
                    else
                    {
                        //Almaceno la primera unidad en la lista de posibles unidades
                        currentUnitsAvailableToAttack.Add(myCurrentTile.tilesInLineLeft[i].unitOnTile);
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
            }
        }
    }

    public override void Attack(UnitBase unitToAttack)
    {
        

        CheckIfUnitHasMarks(unitToAttack);
        unitToAttack.timesRepeatNumber.enabled = false; 
        
        
        if (parryOn)
        {
            //UNDO
            CreateAttackCommand(unitToAttack);

            unitToParry = unitToAttack;
            parryIcon.SetActive(true);
            LM.DeSelectUnit();
            //Animación de preparar el parry            
           // myAnimator.SetTrigger("Attack");

        }

        else if (doubleAttack)
        {
            for (int i = 0; i < timesDoubleAttackRepeats; i++)
            {
                //Animación de ataque                
                myAnimator.SetTrigger("Attack");

                particleMultipleAttack.SetActive(true);

                //He hecho que la partícula sea hija del unitModel y asi, vaya acorde a estas
                //if (currentFacingDirection == FacingDirection.North)
                //{

                //    particleMultipleAttack.transform.DORotate(new Vector3(0, 0, 180),0);

                //}
                //else if (currentFacingDirection == FacingDirection.South)
                //{

                //    particleMultipleAttack.transform.DORotate(new Vector3(0, 0, 0), 0);
                //}
                //else if (currentFacingDirection == FacingDirection.East)
                //{
                //    particleMultipleAttack.transform.DORotate(new Vector3(0, 0, 90), 0);

                //}
                //else if (currentFacingDirection == FacingDirection.West) 
                //{
                //    particleMultipleAttack.transform.DORotate(new Vector3(0, 0, 270), 0);
                //}
        
                

                //UNDO
                CreateAttackCommand(unitToAttack);

                //Hago daño
                DoDamage(unitToAttack);
            }

            if (itsForHonorTime)
            {
                //Cambiar el número si va a tener más de un turno
                myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.text = LM.honorCount.ToString();
            }

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }

        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //UNDO
            CreateAttackCommand(unitToAttack);

            Instantiate(attackParticle, unitToAttack.transform);

            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Samurai
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);

            if (itsForHonorTime)
            {
                //Cambiar el número si va a tener más de un turno
                myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft2.text = LM.honorCount.ToString();
            }

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }

        hasAttacked = true;

    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        CalculateDamage(unitToDealDamage);
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
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

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
        Samurai samuraiUpgraded = FindObjectOfType<Samurai>();

        if (samuraiUpgraded != null && samuraiUpgraded.itsForHonorTime2)
        {
            
            damageWithMultipliersApplied += LM.honorCount;

        }

        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
          || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
          || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
          || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
          )
        {
            if (itsForHonorTime)
            {
                currentHonor++;
                LM.honorCount++;
            }

            //Añado el daño de ataque frontal
            damageWithMultipliersApplied += samuraiFrontAttack;
            backStabIcon.SetActive(false);
        }

        CheckIfIsLonely();
        if (isLonelyLikeMe)
        {
            //Añado el daño de area solitaria
            damageWithMultipliersApplied += lonelyAreaDamage;

        }
        if (itsForHonorTime)
        {
            if (itsForHonorTime2)
            {
                //Este espacio lo dejo para que el multiplicador no se sume dos veces, ya que al ser la mejora de la pasiva el multiplicador se suma en la función calculateDamage para todas las unidades.
            }
            else
            {
                damageWithMultipliersApplied += LM.honorCount;
            }
        }

        damageWithMultipliersApplied += buffbonusStateDamage;

        Debug.Log("Daño base: " + baseDamage + " Daño con multiplicadores " + damageWithMultipliersApplied);
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        if (parryOn)
        {
            particleParryAttack.SetActive(true);

            if (parryOn2)
            {
                if (unitAttacker == unitToParry)
                {
                    damageReceived = 0;
                    DoDamage(unitToParry);
                    UIM.RefreshHealth();
                    unitToParry = null;
                    parryIcon.SetActive(false);
                }
                else if (unitToParry != null)
                {
                    if (( unitAttacker.currentFacingDirection == FacingDirection.North || unitAttacker.currentFacingDirection == FacingDirection.South
                        && currentFacingDirection == FacingDirection.West || currentFacingDirection == FacingDirection.East)
                        ||
                        (unitAttacker.currentFacingDirection == FacingDirection.West || unitAttacker.currentFacingDirection == FacingDirection.East
                        && currentFacingDirection == FacingDirection.North || currentFacingDirection == FacingDirection.South))
                    {

                        damageReceived = 0;
                        DoDamage(unitToParry);
                        UIM.RefreshHealth();
                        unitToParry = null;
                        parryIcon.SetActive(false);

                    }
                }
            }

            else if(unitAttacker == unitToParry)
            {
                damageReceived = 0;
                DoDamage(unitToParry);
                UIM.RefreshHealth();
                unitToParry = null;
                parryIcon.SetActive(false);
            }
        }

        else
        {
            base.ReceiveDamage(damageReceived, unitAttacker);
        }
    }

    public void CheckIfIsLonely()
    {
        if (buffLonelyArea)
        {
            TM.GetSurroundingTiles(myCurrentTile, 1, true, false);
            
            for (int i = 0; i < myCurrentTile.surroundingNeighbours.Count; ++i)
            {
                if (myCurrentTile.surroundingNeighbours[i].unitOnTile != null)
                {
                    if (myCurrentTile.surroundingNeighbours[i].unitOnTile.GetComponent<PlayerUnit>())
                    {
                        isLonelyIcon.SetActive(false);
                        isLonelyLikeMe = false;
                        break;
                    }
                    else
                    {
                        isLonelyIcon.SetActive(true);
                        isLonelyLikeMe = true;
                    }
                }
                else
                {
                    isLonelyIcon.SetActive(true);
                    isLonelyLikeMe = true;

                }
            }
            if (isLonelyLikeMe)
            {
                isLonelyIcon.SetActive(true);
            }
        }
    }

    public override void ShowAttackEffect(UnitBase _unitToAttack)
    {  
        if (currentUnitsAvailableToAttack.Count > 0)
        {
             if (parryOn)
             {
                 parryIcon.SetActive(true);
             }

             else
             {
                 if (doubleAttack)
                 {
                     _unitToAttack.timesRepeatNumber.enabled = true;
                     _unitToAttack.timesRepeatNumber.text = ("X" + timesDoubleAttackRepeats.ToString());

                 }
             }

             //Marco las unidades disponibles para atacar de color rojo
             for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                CalculateDamage(currentUnitsAvailableToAttack[i]);
                currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
            }
        }

         if (UnitsNonAttack.Count > 0)
         {
             //Marco las unidades disponibles para atacar de color rojo
             for (int i = 0; i < UnitsNonAttack.Count; i++)
             {
                 UnitsNonAttack[i].notAttackX.SetActive(true);
             }
         }
    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        _unitToAttack.timesRepeatNumber.enabled = false;

        if (currentUnitsAvailableToAttack.Count > 0)
        {
            if (parryOn)
            {
                if (unitToParry == null)
                {
                    parryIcon.SetActive(false);
                }
            }

            //Marco las unidades disponibles para atacar de color rojo
            for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
            {
                currentUnitsAvailableToAttack[i].ResetColor();
                currentUnitsAvailableToAttack[i].DisableCanvasHover();
            }
        }

        if (UnitsNonAttack.Count > 0)
        {
            //Marco las unidades disponibles para atacar de color rojo
            for (int i = 0; i < UnitsNonAttack.Count; i++)
            {
                UnitsNonAttack[i].notAttackX.SetActive(false);
            }
        }
    }

     
    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        CheckIfIsLonely();
        if (buffLonelyArea)
        {
            lonelyBox.SetActive(true);
        }
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();

        if (buffLonelyArea && !LM.selectedCharacter == this)
        {
            lonelyBox.SetActive(false);
        }
    }

    public override void UndoAttack(AttackCommand lastAttack, bool _isThisUnitTheAttacker)
    {
        base.UndoAttack(lastAttack, _isThisUnitTheAttacker);

        unitToParry = lastAttack.unitToParry;

        CheckIfIsLonely();

        //Falta Honor
        currentHonor = lastAttack.honor;
        LM.honorCount = currentHonor;
    }

}
