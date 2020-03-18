using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTile : DamageTile
{
    public bool isNinjaUpgraded;

    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        LM.damageTilesInBoard.Add(this);
    }
    #endregion

    public override void OnTriggerStay(Collider unitOnTile)
    {

        if (isNinjaUpgraded)
        {
            if (unitOnTile.GetComponent<EnemyUnit>())
            {
                unitToDoDamage = unitOnTile.gameObject;
                hasUnit = true;
            }
            else if (unitOnTile.GetComponent<PlayerUnit>())
            {
                unitToDoDamage.GetComponent<UnitBase>().isHidden = true;
                hasUnit = true;

            }

        }
        else if(unitOnTile.GetComponent<PlayerUnit>())
            {
            unitToDoDamage.GetComponent<UnitBase>().isHidden = true;
            hasUnit = true;
        
             }
       
    }

    public override void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage.GetComponent<UnitBase>().isHidden = false;
            unitToDoDamage = null;
            hasUnit = false;
        }
    }

    public override void CheckHasToDoDamage()
    {
        if (hasUnit && !damageDone)
        {
            if (unitToDoDamage.GetComponent<Druid>())
            {
                damageDone = true;
            }
            else
            {
                Debug.Log(unitToDoDamage);
                

                damageDone = true;
                Debug.Log("DAMAGE DONE");
            }


        }
    }


}
