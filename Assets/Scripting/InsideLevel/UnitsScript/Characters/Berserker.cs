using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : PlayerUnit
{
    #region VARIABLES

    [Header("STATS DE CLASE")]
    //Indica si el berserker está en Rage
    private bool isInRage;

    //Indica si el berserker está en Rage
    [SerializeField]
    private int rageDamagePlus;

    //Al llegar a 0, el rage se quita
    private int turnsLeftToRageOff;
    [SerializeField]
    private int maxNumberOfTurnsInRage;

    [SerializeField]
    public Material rageMaterial;    
    private Material finitMaterial;

    [Header("MEJORAS DE PERSONAJE")]

    public bool circularAttack;

    public bool doubleAttack;
    public int timesDoubleAttackRepeats;

    #endregion

    public void SetSpecificStats(bool _doubleAttack1, bool _circularAttack1)
    {
        doubleAttack = _doubleAttack1;
        circularAttack = _circularAttack1;
    }

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        if (circularAttack)
        {
            //Animación de ataque 
            //HAY QUE HACER UNA PARA EL ATAQUE GIRATORIO
            myAnimator.SetTrigger("Attack");

            //Hago daño
            for (int i = 0; i < myCurrentTile.neighbours.Count; ++i)
            {
                if (myCurrentTile.neighbours[i].unitOnTile != null)
                {
                    DoDamage(myCurrentTile.neighbours[i].unitOnTile);
                }
            }

            //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
            base.Attack(unitToAttack);
        }
        else if (doubleAttack)
        {                       
            for (int i = 0; i < timesDoubleAttackRepeats; i++)
            {
                //Animación de ataque 
                //HAY QUE HACER UNA PARA EL ATAQUE GIRATORIO
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
        //Independientemente de si esta en rage o no, si recibe daño se vuelve a activar el rage

        //Activo el rage
        isInRage = true;

        //La primera vez que entra en rage inicializo los turnos que puede estar en rage.
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

            if (turnsLeftToRageOff <= 0)
            {
                isInRage = false;
                turnsLeftToRageOff = maxNumberOfTurnsInRage;
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
