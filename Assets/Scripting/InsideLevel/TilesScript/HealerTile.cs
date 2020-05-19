using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerTile : DamageTile
{
    public int lifeToHeal;
 
    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        LM.damageTilesInBoard.Add(this);
    }
    #endregion

    public override void OnTriggerEnter(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<Druid>() != null && unitOnTile.GetComponent<Druid>().tileSustitute2)
        {
            if (unitOnTile.GetComponent<Druid>())
            {
                unitOnTile.GetComponent<Druid>().healedLife += unitOnTile.GetComponent<Druid>().bonusOnTile;
            }

            else
            {
                unitOnTile.GetComponent<UnitBase>().buffbonusStateDamage = 1;
            }
        }
         
    }
    public override void OnTriggerStay(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<HealerTile>() != null && unitOnTile.GetComponent<HealerTile>().unitToDoDamage != null)
        {
            Destroy(this.gameObject);
        } 
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = unitOnTile.gameObject;
            hasUnit = true;
        }

     
    }

    public override void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<Druid>())
        {
            if (unitOnTile.GetComponent<Druid>().tileSustitute2)
            {
                unitOnTile.GetComponent<Druid>().healedLife -= unitOnTile.GetComponent<Druid>().bonusOnTile;
            }
        }

        else
        {
            //AQUI HAY QUE USAR LA VARIABLE GUARDAD EN EL LEVEL MANAGER
            if (FindObjectOfType<Druid>().tileSustitute2)
            {
                //No poner a 0. Restar al BuffbonusStateDamage el bonus ganado por entrar (por si entra con un bufo/debufo de antes que no lo pierda)
                unitOnTile.GetComponent<UnitBase>().buffbonusStateDamage = 0;
            } 
        }

        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = null;
            hasUnit = false;
        }
    }

    public override void CheckHasToDoDamage()
    {

        //Estas líneas las añado para comprobar si el druida tiene la mejora de la pasiva 1
        Druid druidUpgraded = FindObjectOfType<Druid>();
  
        if (hasUnit && !damageDone)
        {
            if (druidUpgraded.tileTransformer2)
            {
                if (unitToDoDamage.GetComponent<EnemyUnit>())
                {
                    unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);

                }
                else
                {
                    unitToDoDamage.GetComponent<UnitBase>().currentHealth += lifeToHeal;

                    if (unitToDoDamage.GetComponent<UnitBase>().currentHealth > unitToDoDamage.GetComponent<UnitBase>().maxHealth)
                    {
                        unitToDoDamage.GetComponent<UnitBase>().currentHealth = unitToDoDamage.GetComponent<UnitBase>().maxHealth;
                    }
                }
            }
            else
            {
                unitToDoDamage.GetComponent<UnitBase>().currentHealth += lifeToHeal;

                if (unitToDoDamage.GetComponent<UnitBase>().currentHealth > unitToDoDamage.GetComponent<UnitBase>().maxHealth)
                {
                    unitToDoDamage.GetComponent<UnitBase>().currentHealth = unitToDoDamage.GetComponent<UnitBase>().maxHealth;
                }

            }



            damageDone = true;
        }
    }
}
