using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class Berserker : PlayerUnit
{
    #region VARIABLES

    [Header("STATS DE CLASE")]
    //Indica si el berserker está en Rage
    private bool isInRage;

    public GameObject isInRageIcon;

  



    //Al llegar a 0, el rage se quita
    private int turnsLeftToRageOff;
    [SerializeField]
    private int maxNumberOfTurnsInRage;

    [SerializeField]
    public Material rageMaterial;    
    private Material finitMaterial;

    [Header("MEJORAS DE PERSONAJE")]

    //[Header("Activas")]
    //ACTIVAS
    public bool circularAttack;
    //Esta variable tiene que cambiar en la mejora 2 de este ataque
    public int timeCircularAttackrepeats;

    public bool areaAttack;
    //Esta variable tiene que cambiar en la mejora 2 de este ataque
    public int bonusDamageAreaAttack;


    [Header("Pasivas")]
    //PASIVAS

    [SerializeField]
    //Este es el int que hay que cambiar para que el rage haga más daño
    private int rageDamagePlus;

    private bool rageFear;
    [SerializeField]
    //Este es el int que hay que cambiar para que el el berserker meta más turnos de miedo
    private int fearTurnBonus;


    #endregion

    public void SetSpecificStats(bool _areaAttack, bool _circularAttack1)
    {
        areaAttack = _areaAttack;
        circularAttack = _circularAttack1;

        if (areaAttack)
        {
            bonusDamageAreaAttack = 2;
        }
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;
        tilesInEnemyHover.Clear();

        if (unitToAttack.isMarked)
        {
            unitToAttack.isMarked = false;
            unitToAttack.monkMark.SetActive(false);
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

        if (circularAttack)
        {
            //Animación de ataque 
            //HAY QUE HACER UNA PARA EL ATAQUE GIRATORIO
            myAnimator.SetTrigger("Attack");
            for (int i = 0; i < timeCircularAttackrepeats; i++)
            {
                currentFacingDirection = FacingDirection.North;
                if (myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    DoDamage(myCurrentTile.tilesInLineUp[0].unitOnTile);
                }

                currentFacingDirection = FacingDirection.South;
                if (myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    DoDamage(myCurrentTile.tilesInLineDown[0].unitOnTile);
                }

                currentFacingDirection = FacingDirection.East;
                if (myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {
                    DoDamage(myCurrentTile.tilesInLineRight[0].unitOnTile);
                }

                currentFacingDirection = FacingDirection.West;
                if (myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    DoDamage(myCurrentTile.tilesInLineLeft[0].unitOnTile);
                }

                //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
                base.Attack(unitToAttack);
            }
           
        }
        else if (areaAttack)
        {
            baseDamage = bonusDamageAreaAttack;

            if (currentFacingDirection == FacingDirection.North)
            {
                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {                   
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);                   
                }

                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {                    
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);                  
                }
            }

            else if (currentFacingDirection == FacingDirection.South)
            {
                if (unitToAttack.myCurrentTile.tilesInLineLeft.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineRight.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile != null)
                {                  
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineRight[0].unitOnTile);
                }
            }

            else if (currentFacingDirection == FacingDirection.East)
            {
                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                }
            }

            else if (currentFacingDirection == FacingDirection.West)
            {

                if (unitToAttack.myCurrentTile.tilesInLineUp.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0].unitOnTile);
                }

                if (unitToAttack.myCurrentTile.tilesInLineDown.Count > 0 && currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile != null)
                {
                    DoDamage(currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineDown[0].unitOnTile);
                }
            }

            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Hago daño
            DoDamage(unitToAttack);

            for (int i = 0; i < tilesInEnemyHover.Count; i++)
            {
                tilesInEnemyHover[i].ColorDesAttack();

                if (tilesInEnemyHover[i].unitOnTile != null)
                {
                    tilesInEnemyHover[i].unitOnTile.ResetColor();
                }
            }
            tilesInEnemyHover.Clear();
            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
            

        }
        else
        {
            //Animación de ataque
            myAnimator.SetTrigger("Attack");

            //Hago daño
            DoDamage(unitToAttack);

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
        
    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        if (isInRage)
        {
           
            CalculateDamage(unitToDealDamage);

            Debug.Log(damageWithMultipliersApplied);

            //Añado el daño de rage.
            damageWithMultipliersApplied += rageDamagePlus;

            Debug.Log(damageWithMultipliersApplied);

            //Una vez aplicados los multiplicadores efectuo el daño.
            unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);

            if (rageFear)
            {
                unitToDealDamage.hasFear = true;
                unitToDealDamage.turnsWithFear += fearTurnBonus;

            }


        }
        else
        {
            CalculateDamage(unitToDealDamage);
            //Una vez aplicados los multiplicadores efectuo el daño.
            unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
        }

        //Añado este if para el count de honor del samurai
        if (currentFacingDirection == FacingDirection.North && unitToDealDamage.currentFacingDirection == FacingDirection.South
       || currentFacingDirection == FacingDirection.South && unitToDealDamage.currentFacingDirection == FacingDirection.North
       || currentFacingDirection == FacingDirection.East && unitToDealDamage.currentFacingDirection == FacingDirection.West
       || currentFacingDirection == FacingDirection.West && unitToDealDamage.currentFacingDirection == FacingDirection.East
       )
        {
            LM.honorCount++;
        }
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

    public override void ReceiveDamage(int damageReceived, UnitBase unitAttacker)
    {
        //Independientemente de si esta en rage o no, si recibe daño se vuelve a activar el rage

        //Activo el rage
        isInRage = true;
        isInRageIcon.SetActive(true);
        
        //La primera vez que entra en rage inicializo los turnos que puede estar en rage.
        turnsLeftToRageOff = maxNumberOfTurnsInRage - 1;
        myPanelPortrait.GetComponent<Portraits>().rageTurnsLeft.enabled = true;
        myPanelPortrait.GetComponent<Portraits>().rageTurnsLeft.text = turnsLeftToRageOff.ToString();
        turnsLeftToRageOff = maxNumberOfTurnsInRage;
        //Cambiar material
        RageColor();


        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public void RageChecker()
    {
        if (isInRage)
        {
            turnsLeftToRageOff--;

            myPanelPortrait.GetComponent<Portraits>().rageTurnsLeft.enabled = true;
            myPanelPortrait.GetComponent<Portraits>().rageTurnsLeft.text = turnsLeftToRageOff.ToString();
          

            if (turnsLeftToRageOff <= 0)
            {
                isInRage = false;
                isInRageIcon.SetActive(false);
                turnsLeftToRageOff = maxNumberOfTurnsInRage;
                myPanelPortrait.GetComponent<Portraits>().rageTurnsLeft.enabled = false;
                
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

                if ( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineLeft[0] != null)
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
                if ( currentUnitsAvailableToAttack[0].myCurrentTile.tilesInLineUp[0] != null)
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


        for (int i = 0; i < tilesInEnemyHover.Count; i++)
        {
            tilesInEnemyHover[i].ColorAttack();

            if (tilesInEnemyHover[i].unitOnTile != null)
            {
                tilesInEnemyHover[i].unitOnTile.ColorAvailableToBeAttacked(-1);
            }
        }



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
