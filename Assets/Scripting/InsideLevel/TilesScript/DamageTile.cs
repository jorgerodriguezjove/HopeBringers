using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTile : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    public bool damageDone;

    [SerializeField]
    private bool hasUnit;

    [SerializeField]
    private GameObject unitToDoDamage;

    [SerializeField]
    private int damageToDo;

    [Header("REFERENCIAS")]
    private LevelManager LM;


    #endregion

    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        LM.damageTilesInBoard.Add(this);
    }
    #endregion

    void OnTriggerStay(Collider unitOnTile)
    {
        
            //Por si acaso pilla el tile en vez de a la unidad
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = unitOnTile.gameObject;
             hasUnit = true;
        }

      
    }

    void OnTriggerExit(Collider unitOnTile)
    {
        if (unitOnTile.GetComponent<UnitBase>())
        {
            unitToDoDamage = null;
            hasUnit = false;
        }
    }



        public void CheckHasToDoDamage()
    {
        if(hasUnit && !damageDone){
            unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);
            
            damageDone = true;
            Debug.Log("DAMAGE DONE");
            
        }
        

    }
}
