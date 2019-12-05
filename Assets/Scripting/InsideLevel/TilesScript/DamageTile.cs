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

    #region INIT
    private void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
    }
    #endregion

    void OnTriggerStay(Collider unitToDoDamage)
    {
        if (LM.currentLevelState == LevelManager.LevelState.ProcessingEnemiesActions && !damageDone)
        {
            //Por si acaso pilla el tile en vez de a la unidad
            if (unitToDoDamage.GetComponent<UnitBase>())
            {
                unitToDoDamage.GetComponent<UnitBase>().ReceiveDamage(damageToDo, null);
                damageDone = true;
                Debug.Log("DAMAGE DONE");
            }
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
