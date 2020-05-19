using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTile : DamageTile
{
    public Rogue myRogueReference;

    public bool isNinjaUpgraded;

    //int para saber cuando hay que destruir este tile. Tiene que ser uno más de lo que se quiere porque se actualiza al empezar la fase del jugador
    public int tileCounter;

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
                unitToDoDamage = unitOnTile.gameObject;
                unitToDoDamage.GetComponent<UnitBase>().isHidden = true;
                hasUnit = true;
            }
        }

        else if(unitOnTile.GetComponent<PlayerUnit>())
        {
            unitToDoDamage = unitOnTile.gameObject;
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

    private void OnDestroy()
    {
        if (myRogueReference != null)
        {
            myRogueReference.realBombsSpawned.Remove(gameObject);
        }
       
        else
        {
            myRogueReference = FindObjectOfType<Rogue>();

            if (myRogueReference != null)
            {
                myRogueReference.realBombsSpawned.Remove(gameObject);
            }
        }
    }
}
