using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : PlayerUnit
{
    #region VARIABLES

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
}
