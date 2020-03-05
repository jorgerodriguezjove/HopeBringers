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

            }
            else
            {
                Debug.Log(unitToDoDamage);
                unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);

                damageDone = true;
                Debug.Log("DAMAGE DONE");
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

}
