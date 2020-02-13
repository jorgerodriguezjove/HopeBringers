using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monk : PlayerUnit
{
    public override void Attack(UnitBase unitToAttack)
    {
        hasAttacked = true;

        //Animación de ataque
        myAnimator.SetTrigger("Attack");

        unitToAttack.isMarked = true;

        //Hago daño
        DoDamage(unitToAttack);

        //Meter sonido Samurai
        //SoundManager.Instance.PlaySound(AppSounds.KNIGHT_ATTACK);


        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }
}
