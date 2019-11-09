using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : PlayerUnit
{
    #region VARIABLES

    [Header("SPECIAL VARIABLES FOR CHARACTER")]

    [SerializeField]
    protected GameObject chargingParticle;

    #endregion

    //En función de donde este mirando el personaje paso una lista de tiles diferente.
    public override void Attack(UnitBase unitToAttack)
    {
        Instantiate(chargingParticle, gameObject.transform.position, chargingParticle.transform.rotation);

        Instantiate(attackParticle, unitToAttack.transform.position, unitToAttack.transform.rotation);

        //Hago daño
        DoDamage(unitToAttack);

        SoundManager.Instance.PlaySound(AppSounds.MAGE_ATTACK);

        //La base tiene que ir al final para que el bool de hasAttacked se active después del efecto.
        base.Attack(unitToAttack);
    }

    //Override especial del mago para que no instancie la partícula de ataque
    protected override void DoDamage(UnitBase unitToDealDamage)
    {
        CalculateDamage(unitToDealDamage);
        //Una vez aplicados los multiplicadores efectuo el daño.
        unitToDealDamage.ReceiveDamage(Mathf.RoundToInt(damageWithMultipliersApplied), this);
    }
}
