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
        base.OnTriggerEnter(unitOnTile);

        if (unitOnTile.GetComponent<Druid>().tileSustitute2)
        {
            if (unitOnTile.GetComponent<Druid>())
            {
                unitOnTile.GetComponent<Druid>().healedLife += unitOnTile.GetComponent<Druid>().bonusOnTile;

            }
            else
            {

                unitOnTile.GetComponent<UnitBase>().BuffbonusStateDamage = 1;
            }

        }
    }
    public override void OnTriggerStay(Collider unitOnTile)
    {
        
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = unitOnTile.gameObject;
            hasUnit = true;
        }
    }

    public override void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<Druid>().tileSustitute2)
        {
            if (unitOnTile.GetComponent<Druid>())
            {
                unitOnTile.GetComponent<Druid>().healedLife -= unitOnTile.GetComponent<Druid>().bonusOnTile;

            }
            else
            {

                unitOnTile.GetComponent<UnitBase>().BuffbonusStateDamage = 0;
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

        //Estas líneas las añado para comprobar si el samurai tiene la mejora de la pasiva 1
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
          

           
            
        }
    }

    public override void Update()
    {
        if (LM.currentLevelState == LevelManager.LevelState.PlayerPhase)
        {
            damageDone = false;
        }
    }


    #region INTERACTION

    private void OnMouseEnter()
    {
        LM.UIM.ShowTileInfo(tileInfo, tileImage);
    }

    private void OnMouseExit()
    {
        LM.UIM.HideTileInfo();
    }

    #endregion
}
