using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : PlayerUnit
{
    #region VARIABLES

    //Indica si el berserker está en Rage
    public bool isInRage;

    //Indica si el berserker está en Rage
    public int rageDamageMulti;

    //Al llegar a 0, el rage se quita
    public int turnsToTurnRageOff;
    public int fturnsToTurnRageOff =  3;


    [SerializeField]
    public Material rageMaterial;

    
    private Material finitMaterial;




    //[Header("STATS DE CLASE")]

    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        //Animación de ataque
        myAnimator.SetTrigger("Attack");

        //Hago daño
        DoDamage(unitToAttack);

        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }

    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        if (isInRage)
        {
           
            CalculateDamage(unitToDealDamage);

            //Añado el daño de rage.
            damageWithMultipliersApplied *= rageDamageMulti;

            //Una vez aplicados los multiplicadores efectuo el daño.
            unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);


        }
        else
        {
            CalculateDamage(unitToDealDamage);
            //Una vez aplicados los multiplicadores efectuo el daño.
            unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
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
        if (!isInRage)
        {
            isInRage = true;
            //Cambiar material
            RageColor();

        }
       

        base.ReceiveDamage(damageReceived, unitAttacker);
    }

    public void RageChecker()
    {
        if (isInRage)
        {
            turnsToTurnRageOff--;

            if (turnsToTurnRageOff <= 0)
            {
                isInRage = false;
                turnsToTurnRageOff = fturnsToTurnRageOff;
                RageColor();
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
