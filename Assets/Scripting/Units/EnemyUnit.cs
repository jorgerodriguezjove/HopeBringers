using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitBase
{
    #region VARIABLES

    //REFERENCIAS
    //Ahora mismo se setea desde el inspector
    public GameObject LevelManagerRef;
    private LevelManager LM;

    #endregion

    #region INIT

    private void Awake()
    {
        //Referencia al LM y me incluyo en la lista de enemiogos
        LM = LevelManagerRef.GetComponent<LevelManager>();
        LM.enemiesOnTheBoard.Add(this);
    }

    private void Start()
    {
        myCurrentTile.unitOnTile = this;
    }

    #endregion

    //Al clickar en una unidad aviso al LM
    private void OnMouseDown()
    {
        Debug.Log("clickao");
        LM.SelectUnitToAttack(GetComponent<UnitBase>());
    }
}
