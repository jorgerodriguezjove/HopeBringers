using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Samurai : PlayerUnit
{
    #region VARIABLES
    
    [SerializeField]
    private int samuraiFrontAttack;

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

    //bool para la pasiva 2
    public bool buffLonelyArea;
    //bool que indica si tiene aliados en un área de 3x3 o no
    public bool isLonelyLikeMe;
    //int que añade daño si el samurai no tiene aliados en un área de 3x3
    public int lonelyAreaDamage;

    public GameObject isLonelyIcon;

    public GameObject lonelyBox;

    #endregion

    public override void CheckWhatToDoWithSpecialToken()
    {
        if (buffLonelyArea)
        { 
            lonelyBox.SetActive(true);
        }
        CheckIfIsLonely();
        myPanelPortrait.GetComponent<Portraits>().specialToken.SetActive(true);
        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.enabled = true;
        //Cambiar el número si va a tener más de un turno
        myPanelPortrait.GetComponent<Portraits>().specialSkillTurnsLeft.text = LM.honorCount.ToString();              
    }

    public override void CheckUnitsAndTilesInRangeToAttack()
    {
        CheckIfIsLonely();

        currentUnitsAvailableToAttack.Clear();

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
                        myCurrentTile.tilesInLineUp[i].unitOnTile.previsualizeAttackIcon.SetActive(true);
                        myCurrentTile.tilesInLineUp[i].unitOnTile.notAttackX.SetActive(true);

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

        if (currentUnitsAvailableToAttack.Count > 0)
        {


            if (currentFacingDirection == FacingDirection.North && currentUnitsAvailableToAttack[0].currentFacingDirection == FacingDirection.South
              || currentFacingDirection == FacingDirection.South && currentUnitsAvailableToAttack[0].currentFacingDirection == FacingDirection.North
              || currentFacingDirection == FacingDirection.East && currentUnitsAvailableToAttack[0].currentFacingDirection == FacingDirection.West
              || currentFacingDirection == FacingDirection.West && currentUnitsAvailableToAttack[0].currentFacingDirection == FacingDirection.East
              )
            {
                backStabIcon.SetActive(true);
            }
        }

        //Marco las unidades disponibles para atacar de color rojo
        for (int i = 0; i < currentUnitsAvailableToAttack.Count; i++)
        {
            CalculateDamage(currentUnitsAvailableToAttack[i]);
            currentUnitsAvailableToAttack[i].ColorAvailableToBeAttackedAndNumberDamage(damageWithMultipliersApplied);
           
        }


    }

    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;


        if (unitToAttack.isMarked)
        {
            unitToAttack.QuitMarks();
            currentHealth += FindObjectOfType<Monk>().healerBonus * unitToAttack.numberOfMarks;
            unitToAttack.numberOfMarks = 0;

            if (FindObjectOfType<Monk>().debuffMark2)
            {
                if (!unitToAttack.isStunned)
                {
                    StunUnit(unitToAttack, 1);
                }
            }

            else if (FindObjectOfType<Monk>().healerMark2)
            {
                ApplyBuffOrDebuffdamage(this, 1, 3);
            }

            UIM.RefreshTokens();
        }

        if (parryOn)
        {
            unitToParry = unitToAttack;
            parryIcon.SetActive(true);
            //Animación de preparar el parry            
            myAnimator.SetTrigger("Attack");

        }
        else if (doubleAttack)
        {
            for (int i = 0; i < timesDoubleAttackRepeats; i++)
            {
                //Animación de ataque                
                myAnimator.SetTrigger("Attack");

                //Hago daño
                DoDamage(unitToAttack);
            }
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Hago daño
            DoDamage(unitToAttack);

            //Meter sonido Samurai
            //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);

        }
        
    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
      
        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
            || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
            || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
            || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
            )
        {
            if (itsForHonorTime)
            {
                LM.honorCount++;              
            }

            CalculateDamage(unitToDealDamage);

            Debug.Log(damageWithMultipliersApplied);

            //Añado el daño de ataque frontal
            damageWithMultipliersApplied += samuraiFrontAttack;
            backStabIcon.SetActive(false);



        }
        else
        {
            CalculateDamage(unitToDealDamage);
            //Una vez aplicados los multiplicadores efectuo el daño.
           
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
            else{
                damageWithMultipliersApplied += LM.honorCount;
            }
          
        }
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);

        Instantiate(attackParticle, unitModel.transform.position, unitModel.transform.rotation);
    }

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        if (parryOn)
        {
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
            //Hago daño a las unidades adyacentes(3x3)
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
        if(parryOn)
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
       


    }

    public override void HideAttackEffect(UnitBase _unitToAttack)
    {
        if (parryOn)
        {
            parryIcon.SetActive(false);

        }
        else
        {
            _unitToAttack.timesRepeatNumber.enabled = false;

        }
    }
}
