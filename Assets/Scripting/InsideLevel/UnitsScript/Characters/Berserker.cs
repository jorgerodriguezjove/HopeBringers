using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : PlayerUnit
{
    #region VARIABLES

    //Indica si el berserker está en Rage
    public bool isInRage;

    //Al llegar a 0, el rage se quita
    public int turnsToTurnRageOff;

    
    public int fturnsToTurnRageOff =  3;

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

            }
        }
    }

    #region COLORS

    
    public virtual void RageColor()
    {
        if (!isDead)
        {
            unitMaterialModel.GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
        }

    }

    #endregion
}
