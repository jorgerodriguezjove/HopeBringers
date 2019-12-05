using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTile : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    private bool damageDone;

    [SerializeField]
    private int damageToDo;


    [Header("REFERENCIAS")]

    private LevelManager LM;


    #endregion

    #region Init
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
    }
    #endregion



    void OnTriggerStay(Collider unitToDoDamage)
    {
        


        if (LM.currentLevelState == LevelManager.LevelState.ProcessingEnemiesActions && !damageDone)
        {
           
            unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);
            damageDone = true;
            

        }
    }

    void Update()
    {
        if (LM.currentLevelState == LevelManager.LevelState.PlayerPhase)
        {
           
                damageDone = false;
            


        }

    }
}
